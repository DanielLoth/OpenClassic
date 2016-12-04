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
                    var opcode = input.ReadByte();

                    //input.SetOpcode(opcode);
                    //input.SetPayloadLength(0);

                    output.Add(input.Retain());
                }
                else if (!isTwoByteLen)
                {
                    var lastPayloadByte = input.ReadByte();
                    var firstByteToMoveLeftIndex = input.ReaderIndex;
                    var opcode = input.ReadByte();

                    var bytesToMoveLeft = payloadLengthExcludingOpcode;

                    var writeIndex = firstByteInPacketIndex;
                    for (int i = 0, readIndex = firstByteToMoveLeftIndex; i < bytesToMoveLeft; i++, readIndex++, writeIndex++)
                    {
                        input.SetByte(writeIndex, input.GetByte(readIndex));
                    }
                    input.SetByte(writeIndex, lastPayloadByte);

                    // Get a slice of the input buffer, and then set the reader
                    // index so that it starts at the second byte (which is the
                    // first byte of the payload after the opcode).
                    var packet = input.Slice(firstByteInPacketIndex, payloadLengthIncludingOpcode);
                    packet.SetReaderIndex(1);
                    packet.MarkReaderIndex();

                    // Make sure Retain() is called on the slice so that the
                    // underlying buffer isn't prematurely recycled by DotNetty.
                    output.Add(packet.Retain());
                }
                else
                {
                    var opcode = input.ReadByte();
                    var newBuffer = context.Allocator.Buffer(payloadLengthIncludingOpcode);
                    input.ReadBytes(newBuffer, payloadLengthExcludingOpcode);

                    var packet = new Packet(opcode, newBuffer);
                    output.Add(input.Retain());
                }
            }
        }
    }
}
