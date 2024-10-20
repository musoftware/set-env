using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace set_env
{
    public static class PhpVersionManager
    {
        // Update the PhpDir to point to C:\tools instead of C:\tools\php
        private const string PhpDir = @"C:\tools";

        public static void ListPhpVersions()
        {
            // Check if the tools directory exists
            if (Directory.Exists(PhpDir))
            {
                Console.WriteLine("PHP Versions found:");
                // Get directories directly under C:\tools
                var versions = Directory.GetDirectories(PhpDir);
                foreach (var version in versions)
                {
                    // Output the version folder names
                    Console.WriteLine($"* {Path.GetFileName(version)}");
                }
            }
            else
            {
                Console.WriteLine("PHP directory not found.");
            }
        }

        public static void AddPhpVersion(string version)
        {
            if (!version.StartsWith("php"))
            {
                version = "php" + version;
            }
            // Combine the PhpDir with the version to get the full path
            string versionPath = Path.Combine(PhpDir, version);
            if (Directory.Exists(versionPath))
            {
                RemovePhpVersions();
                // Add the PHP version path to the system PATH variable
                EnvironmentHelper.AddPathVariable(versionPath, EnvironmentVariableTarget.Machine);
                Console.WriteLine($"Added PHP version {version} to the PATH.");
            }
            else
            {
                Console.WriteLine($"PHP version {version} not found in {PhpDir}.");
            }
        }

        public static void RemovePhpVersion(string version)
        {
            // Create the expected path for the version
            string versionPath = Path.Combine(PhpDir, version);
            string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

            // Check if the specific PHP version is in the PATH
            if (currentPath.Contains(versionPath))
            {
                // Remove the specific PHP version path from the system PATH variable
                EnvironmentHelper.RemovePathVariable(versionPath, EnvironmentVariableTarget.Machine);
                Console.WriteLine($"Removed PHP version {version} from the PATH.");
            }
            else
            {
                Console.WriteLine($"PHP version {version} not found in the PATH.");
            }
        }

        public static void RemovePhpVersions()
        {
            if (Directory.Exists(PhpDir))
            {
                Console.WriteLine("PHP Versions found:");
                // Get directories directly under C:\tools
                var versions = Directory.GetDirectories(PhpDir);
                foreach (var version in versions)
                {
                    RemovePhpVersion(Path.GetFileName(version));
                }
            }
            else
            {
                Console.WriteLine("PHP directory not found.");
            }
        }
    }
}
