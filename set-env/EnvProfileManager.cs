using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace set_env
{
    public class EnvProfileManager
    {
        private string envProfileFolder;

        public EnvProfileManager(string path)
        {
            envProfileFolder = Path.Combine(path, "env");
            if (!Directory.Exists(envProfileFolder))
            {
                Directory.CreateDirectory(envProfileFolder);
            }
        }

        public void ListProfiles()
        {
            Console.WriteLine($"Profiles: ");
            var list = Directory.GetFiles(envProfileFolder, "*.env");
            foreach (var env in list)
            {
                Console.WriteLine($"* {Path.GetFileNameWithoutExtension(env)}");
            }
        }

        public void SaveProfile(string profileName)
        {
            var varsUser = EnvironmentHelper.CreateDictionary(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User));
            var varsMachine = EnvironmentHelper.CreateDictionary(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine));

            ProfileStruct profileStruct = new ProfileStruct
            {
                UserVars = varsUser,
                MachineVars = varsMachine
            };

            File.WriteAllText(Path.Combine(envProfileFolder, profileName) + ".env",
                Newtonsoft.Json.JsonConvert.SerializeObject(profileStruct));
        }

        public void LoadProfile(string profileName)
        {
            string profilePath = Path.Combine(envProfileFolder, profileName) + ".env";

            if (!File.Exists(profilePath))
            {
                Console.WriteLine($"Profile {profileName} does not exist");
                return;
            }

            Console.WriteLine($"Loading profile...");
            string envContent = File.ReadAllText(profilePath);
            ProfileStruct profileStruct = Newtonsoft.Json.JsonConvert.DeserializeObject<ProfileStruct>(envContent);

            Console.WriteLine($"Deleting User Env Vars...");
            EnvironmentHelper.DeleteEnvironmentVariables(EnvironmentVariableTarget.User);
            Console.WriteLine($"Deleted User Env Vars...");

            Console.WriteLine($"Deleting Machine Env Vars...");
            EnvironmentHelper.DeleteEnvironmentVariables(EnvironmentVariableTarget.Machine);
            Console.WriteLine($"Deleted Machine Env Vars...");

            Console.WriteLine($"Setting User Env Vars...");
            EnvironmentHelper.SetEnvironmentVariables(profileStruct.UserVars, EnvironmentVariableTarget.User);
            Console.WriteLine($"Set User Env Vars!");

            Console.WriteLine($"Setting Machine Env Vars...");
            EnvironmentHelper.SetEnvironmentVariables(profileStruct.MachineVars, EnvironmentVariableTarget.Machine);
            Console.WriteLine($"Set Machine Env Vars!");
        }
    }
}
