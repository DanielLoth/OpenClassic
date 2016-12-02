using DotNetty.Buffers;
using System.Diagnostics;

namespace OpenClassic.Server.Networking
{
    public class GamePooledByteBufferAllocator : IByteBufferAllocator
    {
        readonly IByteBufferAllocator DelegateAllocator = PooledByteBufferAllocator.Default;

        const int BufferFrontPadding = 4;
        const int DefaultInitialCapacity = 256; // This is the same as DotNetty's (at least at 3 Dec 2016).
        const int InitialCapacityPlusPadding = BufferFrontPadding + DefaultInitialCapacity;

        public static readonly IByteBufferAllocator Default = new GamePooledByteBufferAllocator();

        public IByteBuffer Buffer() => Buffer(DefaultInitialCapacity, int.MaxValue);

        public IByteBuffer Buffer(int initialCapacity) => Buffer(initialCapacity, int.MaxValue);

        public IByteBuffer Buffer(int initialCapacity, int maxCapacity)
        {
            Debug.Assert(initialCapacity >= 0);
            Debug.Assert(maxCapacity >= 0);

            if (initialCapacity == 0 && maxCapacity == 0)
            {
                return DelegateAllocator.Buffer(0, 0); // Will returned a reusable EmptyBuffer.
            }

            var initialCapacityPlusPadding = initialCapacity + BufferFrontPadding;
            var maxCapacityPlusPadding = maxCapacity + BufferFrontPadding;

            var buffer = DelegateAllocator.Buffer(initialCapacityPlusPadding, maxCapacityPlusPadding);
            var writerIndex = buffer.WriterIndex;
            var writerIndexAfterPadding = writerIndex + BufferFrontPadding;

            buffer.SetWriterIndex(writerIndexAfterPadding);

            return buffer;
        }

        public CompositeByteBuffer CompositeBuffer() => DelegateAllocator.CompositeBuffer();

        public CompositeByteBuffer CompositeBuffer(int maxComponents) => DelegateAllocator.CompositeBuffer(maxComponents);
    }
}
