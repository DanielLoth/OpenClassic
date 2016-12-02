using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenClassic.Server.Networking
{
    public class GameMessageDecoder : ByteToMessageDecoder
    {
        public override bool IsSharable => true;

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            Debug.Assert(context != null);
            Debug.Assert(input != null);
            Debug.Assert(output != null);

            input.MarkReaderIndex();

            if (input.ReadableBytes < 2)
            {
                return;
            }

            var payloadLength = (int)input.ReadByte();
            var isTwoByteLen = false;
            if (payloadLength >= 160)
            {
                payloadLength = (payloadLength - 160) * 256 + input.ReadByte();
                isTwoByteLen = true;
            }

            var inboundDataReadyForProcessing = input.ReadableBytes >= payloadLength;
            if (!inboundDataReadyForProcessing)
            {
                input.ResetReaderIndex();
                return;
            }

            var dataLen = payloadLength - 1;
            if (dataLen == 0)
            {
                var opcode = input.ReadByte();
                var emptyBuffer = context.Allocator.Buffer(0, 0);
                var packet = new Packet(opcode, emptyBuffer);

                output.Add(packet);
            }
            else if (!isTwoByteLen)
            {
                var lastPayloadByte = input.ReadByte();
                var opcode = input.ReadByte();
                var newBuffer = context.Allocator.Buffer(payloadLength);

                input.ReadBytes(newBuffer, dataLen - 1);
                newBuffer.WriteByte(lastPayloadByte);

                var packet = new Packet(opcode, newBuffer);

                output.Add(packet);
            }
            else
            {
                var opcode = input.ReadByte();
                var newBuffer = context.Allocator.Buffer(payloadLength);
                input.ReadBytes(newBuffer, dataLen);

                var packet = new Packet(opcode, newBuffer);
                output.Add(packet);
            }
        }
    }
}
