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

                if (payloadLengthIncludingOpcode == 1)
                {
                    // Read a 1-byte slice, which is the opcode.
                    var opcode = input.ReadByte();
                    var newPacket = Unpooled.Buffer(1);

                    newPacket.WriteByte(opcode);

                    // Update the reader index to 1, which means that there are zero
                    // readable bytes in the slice.
                    newPacket.SetReaderIndex(1);
                    newPacket.MarkReaderIndex();

                    Debug.Assert(newPacket.ReadableBytes == 0);

                    output.Add(newPacket);
                }
                else if (!isTwoByteLen)
                {
                    var remainingBytes = payloadLengthIncludingOpcode;

                    var lastPayloadByte = input.ReadByte();
                    var opcode = input.ReadByte();
                    remainingBytes -= 2;

                    var payload = new byte[payloadLengthIncludingOpcode];
                    payload[0] = opcode;
                    input.ReadBytes(payload, 1, remainingBytes);
                    payload[payload.Length - 1] = lastPayloadByte;

                    var newPacket = Unpooled.WrappedBuffer(payload);
                    newPacket.SetReaderIndex(1);
                    newPacket.MarkReaderIndex();

                    output.Add(newPacket);
                }
                else
                {
                    var remainingBytes = payloadLengthIncludingOpcode;

                    var opcode = input.ReadByte();
                    remainingBytes--;

                    var payload = new byte[payloadLengthIncludingOpcode];
                    payload[0] = opcode;
                    input.ReadBytes(payload, 1, remainingBytes);

                    var newPacket = Unpooled.WrappedBuffer(payload);
                    newPacket.SetReaderIndex(1);
                    newPacket.MarkReaderIndex();

                    output.Add(newPacket);
                }
            }
        }
    }
}
