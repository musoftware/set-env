using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace set_env
{
    [Serializable]
    public class ProfileStruct
    {

        [JsonProperty("user_vars")]
        public Dictionary<string, string> UserVars { get; set; }


        [JsonProperty("machine_vars")]
        public Dictionary<string, string> MachineVars { get; set; }

    }
}
