using System;

namespace OpenClassic.Server.Networking
{
    public abstract class AbstractPacketWriter : IPacketWriter
    {
        private const int ThreeByteOffset = 3;
        private const int TwoByteOffset = 2;

        private readonly uint[] BitMasks =
        {
            0, 0x1, 0x3, 0x7,
            0xf, 0x1f, 0x3f, 0x7f,
            0xff, 0x1ff, 0x3ff, 0x7ff,
            0xfff, 0x1fff, 0x3fff, 0x7fff,
            0xffff, 0x1ffff, 0x3ffff, 0x7ffff,
            0xfffff, 0x1fffff, 0x3fffff, 0x7fffff,
            0xffffff, 0x1ffffff, 0x3ffffff, 0x7ffffff,
            0xfffffff, 0x1fffffff, 0x3fffffff, 0x7fffffff,
            0xffffffff
        };

        public void CreatePacket(ISession session, int id)
        {
            ThrowIfPacketAlreadyStarted(session);
            FlushIfApproachingCapacity(session);

            var buffer = session.Buffer;
            var packetStart = buffer.WriterIndex;

            session.CurrentPacketId = id;
            session.CurrentPacketStartIndex = packetStart;
            session.CurrentPacketBitfieldPosition = 0;
        }

        public void FormatPacket(ISession session)
        {
            ThrowIfPacketNotStarted(session);
            CheckAndAdvanceBitPositionToStartOfNextByte(session);

            var buffer = session.Buffer;
            // ReSharper disable once PossibleInvalidOperationException
            var packetId = session.CurrentPacketId.Value;
            var dataLen = session.CurrentPacketBitfieldPosition / 8;
            var dataPlusOpcodeLen = dataLen + 1;

            buffer.SetWriterIndex(session.CurrentPacketStartIndex);

            if (dataLen >= 160)
            {
                buffer.WriteByte(160 + (dataPlusOpcodeLen / 256));
                buffer.WriteByte(dataPlusOpcodeLen & 0xff);
                buffer.WriteByte(packetId);

                var newWriterIndex = session.CurrentPacketStartIndex + ThreeByteOffset + dataLen;
                buffer.SetWriterIndex(newWriterIndex);
            }
            else
            {
                if (dataLen > 0)
                {
                    var lastByteIndex = session.CurrentPacketStartIndex + TwoByteOffset + dataLen;
                    var lastByte = buffer.GetByte(lastByteIndex);

                    buffer.WriteByte(dataPlusOpcodeLen);
                    buffer.WriteByte(lastByte);
                    buffer.WriteByte(packetId);

                    buffer.SetWriterIndex(lastByteIndex);
                }
                else
                {
                    var newWriterIndex = buffer.WriterIndex + TwoByteOffset;

                    buffer.WriteByte(dataPlusOpcodeLen);
                    buffer.WriteByte(packetId);
                    buffer.SetWriterIndex(newWriterIndex);
                }
            }

            session.CurrentPacketId = null;
        }

        public void WriteBits(ISession session, int value, int numBits)
        {
            ThrowIfPacketNotStarted(session);

            var buffer = session.Buffer;
            var bytePos = buffer.WriterIndex + ThreeByteOffset + (session.CurrentPacketBitfieldPosition >> 3);
            var bitOffset = 8 - (session.CurrentPacketBitfieldPosition & 7);
            session.CurrentPacketBitfieldPosition += numBits;

            for (; numBits > bitOffset; bitOffset = 8)
            {
                var curByte = buffer.GetByte(bytePos);
                curByte &= (byte)~BitMasks[bitOffset];
                curByte |= (byte)((value >> (numBits - bitOffset)) & BitMasks[bitOffset]);

                buffer.SetByte(bytePos++, curByte);

                numBits -= bitOffset;
            }
            if (numBits == bitOffset)
            {
                var curByte = buffer.GetByte(bytePos);
                curByte &= (byte)~BitMasks[bitOffset];
                curByte |= (byte)(value & BitMasks[bitOffset]);

                buffer.SetByte(bytePos, curByte);
            }
            else
            {
                var curByte = buffer.GetByte(bytePos);
                curByte &= (byte)~(BitMasks[numBits] << (bitOffset - numBits));
                curByte |= (byte)((value & BitMasks[numBits]) << (bitOffset - numBits));

                buffer.SetByte(bytePos, curByte);
            }
        }

        public void WriteByte(ISession session, int value)
        {
            ThrowIfPacketNotStarted(session);
            CheckAndAdvanceBitPositionToStartOfNextByte(session);

            var index = GetWriteStartIndex(session);
            session.Buffer.SetByte(index, value);
            IncrementBitsWritte(session, 8);
        }

        public void WriteBytes(ISession session, byte[] src)
        {
            ThrowIfPacketNotStarted(session);
            CheckAndAdvanceBitPositionToStartOfNextByte(session);

            var index = GetWriteStartIndex(session);
            session.Buffer.SetBytes(index, src);
            IncrementBitsWritte(session, src.Length * 8);
        }

        public void WriteBytes(ISession session, byte[] src, int srcIndex, int length)
        {
            ThrowIfPacketNotStarted(session);
            CheckAndAdvanceBitPositionToStartOfNextByte(session);

            var index = GetWriteStartIndex(session);
            session.Buffer.SetBytes(index, src, srcIndex, length);
            IncrementBitsWritte(session, length * 8);
        }

        public void WriteShort(ISession session, int value)
        {
            ThrowIfPacketNotStarted(session);
            CheckAndAdvanceBitPositionToStartOfNextByte(session);

            var index = GetWriteStartIndex(session);
            session.Buffer.SetShort(index, value);
            IncrementBitsWritte(session, 16);
        }

        public void WriteInt(ISession session, int value)
        {
            ThrowIfPacketNotStarted(session);
            CheckAndAdvanceBitPositionToStartOfNextByte(session);

            var index = GetWriteStartIndex(session);
            session.Buffer.SetInt(index, value);
            IncrementBitsWritte(session, 32);
        }

        public void WriteLong(ISession session, long value)
        {
            ThrowIfPacketNotStarted(session);
            CheckAndAdvanceBitPositionToStartOfNextByte(session);

            var index = GetWriteStartIndex(session);
            session.Buffer.SetLong(index, value);
            IncrementBitsWritte(session, 64);
        }

        #region Protected methods

        protected void FlushIfApproachingCapacity(ISession session)
        {
            if (session.Buffer.WriterIndex > (session.MaxPacketLength * 4) / 5)
            {
                session.WriteAndFlushSessionBuffer();
            }
        }

        protected void CheckAndAdvanceBitPositionToStartOfNextByte(ISession session)
        {
            if ((session.CurrentPacketBitfieldPosition & 7) == 0)
            {
                return;
            }

            session.CurrentPacketBitfieldPosition = 8 * ((session.CurrentPacketBitfieldPosition + 7) / 8);
        }

        protected void ThrowIfPacketAlreadyStarted(ISession session)
        {
            if (session.CurrentPacketId != null)
            {
                throw new InvalidOperationException(
                    "Can't start creating a new packet - there is already a packet in progress.");
            }
        }

        protected void ThrowIfPacketNotStarted(ISession session)
        {
            if (session.CurrentPacketId == null)
            {
                throw new InvalidOperationException("Can't write to or format packet - there is no packet in progress.");
            }
        }

        protected int GetWriteStartIndex(ISession session)
        {
            return session.CurrentPacketStartIndex + ThreeByteOffset + (session.CurrentPacketBitfieldPosition / 8);
        }

        protected void IncrementBitsWritte(ISession session, int bitsWritten)
        {
            session.CurrentPacketBitfieldPosition += bitsWritten;
        }

        #endregion
    }
}
