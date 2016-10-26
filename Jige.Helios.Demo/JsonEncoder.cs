using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jige.Helios.Demo
{
    public class JsonEncoder : MessageToMessageEncoder<object>
    {
        private static readonly  JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        protected override void Encode(IChannelHandlerContext context, object message, List<object> output)
        {
            if (message is string)
            {
                output.Add(message);
                return;
            }

            var json = JsonConvert.SerializeObject(message, settings);
            if (json.Length == 0)
            {
                return;
            }

            output.Add(json);

        }
    }
}
