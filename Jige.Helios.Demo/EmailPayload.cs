using System;
using Newtonsoft.Json;

namespace Jige.Helios.Demo
{
    public class EmailPayload
    {
        public Guid? Uid { get; set; }

        [JsonProperty(Order = -100)]
        public string Type { get; set; }

        public string To { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string HtmlBody { get; set; }
    }
}