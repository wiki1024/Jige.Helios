using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jige.Helios.Demo
{
    public  class Auth
    {
        [JsonProperty(Order = -100)]
        public string Type { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
