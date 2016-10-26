using System;
using System.Diagnostics;
using System.Threading;

namespace Jige.Helios.Demo
{
    using DotNetty.Transport.Channels;
    public class EmailClientHandler: ChannelHandlerAdapter
    {
        private int totalReceived = 0;

        public override void ChannelRegistered(IChannelHandlerContext ctx)
        {
            base.ChannelRegistered(ctx);
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            base.ChannelActive(ctx);
        }

        public override void ChannelRead(IChannelHandlerContext ctx, object message)
        {
            if (message is string)
            {
                var resJson = message as string;
                var tcs = ctx.Channel.GetAttribute(EmailClient.Auth_Task_Key).Get();
                if (!tcs.Task.IsCompleted)
                {
                    if ("{\"auth\":200}".Equals(resJson))
                    {
                        tcs.SetResult(true);
                    }
                    else if("{\"auth\":-1}".Equals(resJson))
                    {
                        tcs.SetResult(false);
                    }
                }

                Interlocked.Increment(ref totalReceived);

                if (totalReceived > 90)
                {
                    Console.WriteLine("received " + "_ " + totalReceived + resJson + " at " + DateTime.Now);
                }
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception exception)
        {
            Debug.WriteLine(exception.StackTrace);
            ctx.CloseAsync();
        }
    }
}