using DotNetty.Buffers;
using System.Diagnostics;

namespace OpenClassic.Server.Networking
{
    public class GamePooledByteBufferAllocator : IByteBufferAllocator
    {
        readonly IByteBufferAllocator DelegateAllocator = PooledByteBufferAllocator.Default;

        public const int BufferFrontPadding = 4;
        const int DefaultInitialCapacity = 256; // This is the same as DotNetty's (at least at 3 Dec 2016).
        const int PaddingMask = unchecked((int)0xAAAAAAAA);

        public static readonly IByteBufferAllocator Default = new GamePooledByteBufferAllocator();

        public IByteBuffer Buffer() => Buffer(DefaultInitialCapacity, int.MaxValue);

        public IByteBuffer Buffer(int initialCapacity) => Buffer(initialCapacity, int.MaxValue);

        public IByteBuffer Buffer(int initialCapacity, int maxCapacity)
        {
            // If the capacity values are negative then clamp them at zero.
            initialCapacity = initialCapacity >= 0 ? initialCapacity : 0;
            maxCapacity = maxCapacity >= 0 ? maxCapacity : 0;

            Debug.Assert(initialCapacity >= 0);
            Debug.Assert(maxCapacity >= 0);

            if (initialCapacity == 0 && maxCapacity == 0)
            {
                return DelegateAllocator.Buffer(0, 0); // Will returned a reusable EmptyBuffer.
            }

            long maxCapLong = (long) maxCapacity + BufferFrontPadding;
            long initCapLong = (long)initialCapacity + BufferFrontPadding;

            var clampedMaxCapacity = maxCapLong > int.MaxValue ? int.MaxValue : (int) maxCapLong;
            var clampedInitialCapacity = initCapLong > int.MaxValue ? int.MaxValue : (int)initCapLong;

            Debug.Assert(clampedMaxCapacity >= 0 && clampedMaxCapacity <= int.MaxValue);
            Debug.Assert(clampedInitialCapacity >= 0 && clampedInitialCapacity <= int.MaxValue);

            var buffer = DelegateAllocator.Buffer(clampedInitialCapacity, clampedMaxCapacity);
            var writerIndex = buffer.WriterIndex;
            var writerIndexAfterPadding = writerIndex + BufferFrontPadding;

            buffer.SetInt(writerIndex, PaddingMask);

            buffer.SetWriterIndex(writerIndexAfterPadding);
            buffer.MarkWriterIndex();

            buffer.SetReaderIndex(writerIndexAfterPadding);
            buffer.MarkReaderIndex();

            return buffer;
        }

        public CompositeByteBuffer CompositeBuffer() => DelegateAllocator.CompositeBuffer();

        public CompositeByteBuffer CompositeBuffer(int maxComponents) => DelegateAllocator.CompositeBuffer(maxComponents);
    }
}
