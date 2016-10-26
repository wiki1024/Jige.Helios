using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jige.Helios.Demo
{
    class Program
    {
        
        static void Main(string[] args)
        {
            using (var cts = new CancellationTokenSource())
            {
                var host = "127.0.0.1";
                var port = 8007;

                var client = new EmailClient(host, port);

               var task= client.Init();
                task.Wait();
                while (true)
                {
                    var input = Console.ReadLine();
                    if (input == "yes")
                    {
                        Parallel.For(0, 1200, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (i) =>
                        {
                            //if (i > 115)
                            //{
                            //    await Task.Delay(2000);
                            //}
                            var payload = new EmailPayload
                            {
                                Type = "Email",
                                From = ".net",
                                To = "java",
                                Subject = "dnotnetty_" + i.ToString(),
                                HtmlBody = "hehe"
                            };


                            await client.Send(payload);
                        });
                    }
                }
                



            


                cts.Token.WaitHandle.WaitOne();
            }
        }

       
    }
}
