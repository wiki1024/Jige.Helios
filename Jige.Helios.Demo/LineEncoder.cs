using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Jige.Helios.Demo
{
    public class LineEncoder:MessageToMessageEncoder<string>
    {
        private Encoding encoding;
        private byte[] lineSeparator;
        public LineEncoder(Encoding encoding,string lineSeparator)
        {
            this.encoding = encoding;
            this.lineSeparator = this.encoding.GetBytes(lineSeparator);
        }

        protected override void Encode(IChannelHandlerContext context, string message, List<object> output)
        {
            IByteBuffer buffer = ByteBufferUtil.EncodeString(context.Allocator, message, encoding);
            buffer.WriteBytes(lineSeparator);
            output.Add(buffer);
        }
    }
}