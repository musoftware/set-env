using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace set_env
{
    class Program
    {
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }


        static void Main(string[] args)
        {
            string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); ;

            string envProfileFolder = Path.Combine(path, "env");
            if (!Directory.Exists(envProfileFolder))
            {
                Directory.CreateDirectory(envProfileFolder);
            }
 
            if (!IsAdministrator())
            {
                Console.WriteLine("Admin permission is required");
                return;
            }


            if (args.Length == 2)
            {
                if (args[0].ToLower() == "list")
                {
                    var list = Directory.GetFiles(envProfileFolder, "*.env");
                    foreach (var env in list)
                    {
                        Console.WriteLine($"* {Path.GetFileNameWithoutExtension(env)}");
                    }
                } 
                else if (args[0].ToLower() == "save")
                {
                    var varsUser = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User);
                    var varsMachine = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);

                    ProfileStruct @struct = new ProfileStruct()
                    {
                        UserVars = CreateDictionary(varsUser),
                        MachineVars = CreateDictionary(varsMachine),
                    };

                    File.WriteAllText(Path.Combine(envProfileFolder, args[1]),
                        Newtonsoft.Json.JsonConvert.SerializeObject(@struct));
                }
                else if (args[0].ToLower() == "load")
                {
                    if (!File.Exists(Path.Combine(envProfileFolder, args[1])))
                    {
                        Console.WriteLine($"Profile {args[1]} is not exist");
                        return;
                    }

                    Console.WriteLine($"Loading profile...");
                    string EnvPath = File.ReadAllText(Path.Combine(envProfileFolder, args[1]));
                    var profileStruct = Newtonsoft.Json.JsonConvert.DeserializeObject<ProfileStruct>(EnvPath);

                    var varsUser = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User);
                    var varsMachine = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);
                    Console.WriteLine($"Loaded profile");

                    Console.WriteLine($"Deleting User Env Vars...");
                    foreach (var item in CreateDictionary(varsUser))
                        Environment.SetEnvironmentVariable(item.Key, null, EnvironmentVariableTarget.User);
                    Console.WriteLine($"Deleted User Env Vars...");

                    Console.WriteLine($"Deleting Machine Env Vars...");
                    foreach (var item in CreateDictionary(varsMachine))
                        Environment.SetEnvironmentVariable(item.Key, null, EnvironmentVariableTarget.Machine);
                    Console.WriteLine($"Deleted Machine Env Vars...");

                    Console.WriteLine($"Setting User Env Vars...");
                    foreach (var item in profileStruct.UserVars)
                        Environment.SetEnvironmentVariable(item.Key, item.Value, EnvironmentVariableTarget.User);
                    Console.WriteLine($"Setted User Env Vars!");

                    Console.WriteLine($"Setting Machine Env Vars...");
                    foreach (var item in profileStruct.MachineVars)
                        Environment.SetEnvironmentVariable(item.Key, item.Value, EnvironmentVariableTarget.Machine);
                    Console.WriteLine($"Setted Machine Env Vars!");
                }
                else
                {
                    UnavaiableCommands();
                }
            }
            if (args.Length == 0)
            {
                AvailableCommands();
            }
        }

        static void UnavaiableCommands()
        {
            Console.WriteLine("this command is not exist");
            AvailableCommands();
        }

        static void AvailableCommands()
        {
            Console.WriteLine("Available Commands:");
            Console.WriteLine("set-env list");
            Console.WriteLine("set-env load {Profile}");
            Console.WriteLine("set-env save {Profile}");
        }

        private static Dictionary<string, string> CreateDictionary(System.Collections.IDictionary vars)
        {
            var dic = new Dictionary<string, string>();

            foreach (System.Collections.DictionaryEntry item in vars)
            {
                dic.Add(item.Key.ToString(), item.Value.ToString());
            }
            return dic;
        }
    }
}
