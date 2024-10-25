using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            EnvProfileManager profileManager = new EnvProfileManager(path);

            if (args.Length == 1)
            {
                switch (args[0].ToLower())
                {
                    case "list":
                        profileManager.ListProfiles();
                        break;
                    case "list-php":
                        PhpVersionManager.ListPhpVersions();
                        break;
                    default:
                        AvailableCommands();
                        break;
                }
            }
            else if (args.Length == 2)
            {
                if (!IsAdministrator())
                {
                    Console.WriteLine("Admin permission is required. Restarting with elevated permissions...");
                    RestartAsAdministrator(args);
                    return;
                }
                switch (args[0].ToLower())
                {
                    case "save":
                        profileManager.SaveProfile(args[1]);
                        break;
                    case "load":
                        profileManager.LoadProfile(args[1]);
                        break;
                    case "php":
                    case "add-php":
                        PhpVersionManager.AddPhpVersion(args[1]);

                        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                        {
                            // For CMD
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "cmd.exe",
                                Arguments = "/c setx PATH \"%PATH%;C:\\Path\\To\\PHP\"",
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            });

                            // For PowerShell
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "powershell.exe",
                                Arguments = "[System.Environment]::SetEnvironmentVariable('PATH', $env:Path, [System.EnvironmentVariableTarget]::User); $env:PATH = [System.Environment]::GetEnvironmentVariable('PATH', [System.EnvironmentVariableTarget]::User)",
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            });

                            Console.WriteLine("Environment variables refreshed. You may need to restart the console.");
                        }

                        break;
                    default:
                        UnavailableCommands();
                        break;
                }
            }
            else if (args.Length == 1 && args[0].ToLower() == "remove-php")
            {
                PhpVersionManager.RemovePhpVersions();
            }
            else
            {
                AvailableCommands();
            }
        }

        private static void UnavailableCommands()
        {
            Console.WriteLine("Command not available.");
        }

        static void RestartAsAdministrator(string[] args)
        {
            ProcessStartInfo proc = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                Verb = "runas",  // This causes the process to be launched with admin rights
                Arguments = string.Join(" ", args)  // Pass the same command-line arguments
            };

            try
            {
                Process.Start(proc);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to restart the application as an administrator: " + ex.Message);
            }

            Environment.Exit(0); // Close the current instance
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
            Console.WriteLine("set-env php list");
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
