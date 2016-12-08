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

                    Debug.Assert(packet.ReadableBytes == 0);

                    // Finally, add it to the output. Calling Retain() ensures that
                    // the underlying IByteBuffer, if freed by DotNetty, doesn't
                    // result in this slice being released prematurely.
                    output.Add(packet.Retain());
                }
                else if (!isTwoByteLen)
                {
                    var packet = input.ReadSlice(payloadLengthIncludingOpcode);

                    var lastPayloadByte = packet.ReadByte();
                    var opcode = packet.ReadByte();

                    // Move bytes along to make room for the last byte at the
                    // end of the packet.
                    for (int rIdx = 2, wIdx = 1; rIdx < payloadLengthExcludingOpcode; rIdx++, wIdx++)
                    {
                        var val = packet.GetByte(rIdx);
                        packet.SetByte(wIdx, val);
                    }

                    // Opcode goes in index 0
                    packet.SetByte(0, opcode);

                    // Last byte goes to the end of the packet.
                    packet.SetByte(packet.Capacity - 1, lastPayloadByte);

                    packet.SetReaderIndex(1);
                    packet.MarkReaderIndex();

                    output.Add(packet.Retain());
                }
                else
                {
                    var packet = input.ReadSlice(payloadLengthIncludingOpcode);

                    packet.SetReaderIndex(1);
                    packet.MarkReaderIndex();

                    Debug.Assert(packet.ReadableBytes == payloadLengthExcludingOpcode);

                    output.Add(packet.Retain());
                }
            }
        }
    }
}
