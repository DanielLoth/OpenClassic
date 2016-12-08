using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenClassic.Server.Networking
{
    public class GameMessageDecoder : ByteToMessageDecoder
    {
        public override bool IsSharable => false;

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            Debug.Assert(context != null);
            Debug.Assert(input != null);
            Debug.Assert(output != null);

            while (true)
            {
                input.MarkReaderIndex();
                var firstByteInPacketIndex = input.ReaderIndex;

                if (input.ReadableBytes < 2)
                {
                    break;
                }

                var payloadLengthIncludingOpcode = (int)input.ReadByte();
                var isTwoByteLen = false;
                if (payloadLengthIncludingOpcode >= 160)
                {
                    payloadLengthIncludingOpcode = (payloadLengthIncludingOpcode - 160) * 256 + input.ReadByte();
                    isTwoByteLen = true;
                }

                var inboundDataReadyForProcessing = input.ReadableBytes >= payloadLengthIncludingOpcode;
                if (!inboundDataReadyForProcessing)
                {
                    input.ResetReaderIndex();
                    break;
                }

                var payloadLengthExcludingOpcode = payloadLengthIncludingOpcode - 1;
                if (payloadLengthExcludingOpcode == 0)
                {
                    // Read a 1-byte slice, which is the opcode.
                    var packet = input.ReadSlice(1);

                    // Update the reader index to 1, which means that there are zero
                    // readable bytes in the slice.
                    packet.SetReaderIndex(1);
                    packet.MarkReaderIndex();

                    // Finally, add it to the output. Calling Retain() ensures that
                    // the underlying IByteBuffer, if freed by DotNetty, doesn't
                    // result in this slice being released prematurely.
                    output.Add(packet.Retain());
                }
                else if (!isTwoByteLen)
                {
                    var lastPayloadByte = input.ReadByte();
                    var opcode = input.ReadByte();
                    var payloadLenExclOpcodeAndLastByte = payloadLengthExcludingOpcode - 1;

                    var packet = input.Allocator.Buffer(payloadLengthIncludingOpcode);
                    packet.WriteByte(opcode);
                    packet.SetReaderIndex(1);
                    packet.MarkReaderIndex();

                    input.ReadBytes(packet, payloadLenExclOpcodeAndLastByte);

                    packet.WriteByte(lastPayloadByte);

                    output.Add(packet);
                }
                else
                {
                    var opcode = input.ReadByte();

                    var packet = input.Allocator.Buffer(payloadLengthIncludingOpcode);
                    packet.WriteByte(opcode);
                    packet.SetReaderIndex(1);
                    packet.MarkReaderIndex();

                    input.ReadBytes(packet, payloadLengthExcludingOpcode);

                    output.Add(packet);
                }
            }
        }
    }
}
