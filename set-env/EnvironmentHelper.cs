using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace set_env
{
    public class EnvironmentHelper
    {
        public static Dictionary<string, string> CreateDictionary(IDictionary vars)
        {
            var dict = new Dictionary<string, string>();
            foreach (DictionaryEntry entry in vars)
            {
                dict[entry.Key.ToString()] = entry.Value?.ToString();
            }
            return dict;
        }

        public static void DeleteEnvironmentVariables(EnvironmentVariableTarget target)
        {
            var vars = Environment.GetEnvironmentVariables(target);
            foreach (var item in CreateDictionary(vars))
            {
                Environment.SetEnvironmentVariable(item.Key, null, target);
            }
        }

        public static void SetEnvironmentVariables(Dictionary<string, string> vars, EnvironmentVariableTarget target)
        {
            foreach (var item in vars)
            {
                Environment.SetEnvironmentVariable(item.Key, item.Value, target);
            }
        }

        public static void AddPathVariable(string path, EnvironmentVariableTarget target)
        {
            var currentPath = Environment.GetEnvironmentVariable("PATH", target);
            if (!currentPath.Contains(path))
            {
                var newPath = currentPath + ";" + path;
                Environment.SetEnvironmentVariable("PATH", newPath, target);
                Console.WriteLine($"Added {path} to PATH.");
            }
            else
            {
                Console.WriteLine($"{path} is already in PATH.");
            }
        }

        public static void RemovePathVariable(string path, EnvironmentVariableTarget target)
        {
            var currentPath = Environment.GetEnvironmentVariable("PATH", target);
            if (currentPath.Contains(path))
            {
                var newPath = currentPath.Replace(path + ";", "").Replace(path, "");
                Environment.SetEnvironmentVariable("PATH", newPath, target);
                Console.WriteLine($"Removed {path} from PATH.");
            }
            else
            {
                Console.WriteLine($"{path} is not in PATH.");
            }
        }

    }
}
