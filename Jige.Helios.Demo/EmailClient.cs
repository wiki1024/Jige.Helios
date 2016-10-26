using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Jige.Helios.Demo
{
    public class EmailClient
    {
        public static AttributeKey<TaskCompletionSource<bool>> Auth_Task_Key = AttributeKey<TaskCompletionSource<bool>>.ValueOf("Auth_task");
        private string host;
        private int port;
        private volatile Task<bool> _authTask;
        public Task<IChannel> ConnecTask { get; private set; }
        public Task<bool> AuthTask
        {
            get { return _authTask; }
            set { this._authTask = value; }
        }

        private ConcurrentQueue<EmailPayload> bufferQueue = new ConcurrentQueue<EmailPayload>();

        public EmailClient(String host, int port)
        {
            this.host = host;
            this.port = port;
            AuthTask = new Task<bool>((() => false));
            AuthTask.Start();
           
        }

        /// <summary>
        /// make sure this method run before instance is accesible to normal use
        /// </summary>
        public async Task Init()
        {
            var group = new MultithreadEventLoopGroup(2);

            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;


                    pipeline.AddLast(new LineBasedFrameDecoder(1024));
                    pipeline.AddLast(new LineEncoder(Encoding.UTF8, "\r\n")
                        , new StringDecoder(Encoding.UTF8)
                        , new JsonEncoder()
                        , new EmailClientHandler());
                }));

            var bootstrapChannelTask = bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(host), port));
            ConnecTask = bootstrapChannelTask;
            var connection = await bootstrapChannelTask;
            var tcs = new TaskCompletionSource<bool>();
            connection.GetAttribute(Auth_Task_Key).Set(tcs);
            var client = this;
           
            
            var chain=tcs.Task.ContinueWith(async authT =>
                {
                    if (authT.IsCompleted && authT.Result)
                    {
                        await Task.Delay(1000);
                        client.AuthTask = tcs.Task;//no need to wait on the auth 
                        await client.FlushQueue();
                    }
                    else
                    {
                        Debug.WriteLine("login fails");
                    }
                });
            await connection.WriteAndFlushAsync(new Auth {Type = "Auth", Password = "password", Username = "admin"});
        }

        public async Task<bool> IsReady()
        {
            var channel = await ConnecTask;
            if (channel.Active)
            {
                var auth = await AuthTask;
                if (auth)
                {
                    return true;
                }
                else
                {
                    Debug.WriteLine("default false");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task FlushQueue()
        {
            EmailPayload payload;
            while (bufferQueue.TryDequeue(out payload))
            {
                if (await IsReady())
                {
                    var channel = await ConnecTask;
                    channel.WriteAndFlushAsync(payload);
                }
                else
                {
                    bufferQueue.Enqueue(payload);
                    break;
                }
            }
        }

        public async Task Send(EmailPayload email)
        {
            if (await IsReady())
            {
                await (await ConnecTask).WriteAndFlushAsync(email);
               
            }
            else
            {
                Debug.WriteLine("queued");
                email.Subject = email.Subject + "_queued";
                bufferQueue.Enqueue(email);
            }
        }
    }
}
