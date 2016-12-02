using DotNetty.Buffers;
using OpenClassic.Server.Networking;
using Xunit;

namespace OpenClassic.Server.Tests.Networking
{
    public class GamePooledByteBufferAllocatorTests
    {
        [Fact]
        public void AllocatesBuffer()
        {
            var buffer = GamePooledByteBufferAllocator.Default.Buffer();

            Assert.NotNull(buffer);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(256)]
        [InlineData(4096)]
        public void AllocatesBufferWithCorrectInitialCapacity(int initialCapacity)
        {
            var buffer = GamePooledByteBufferAllocator.Default.Buffer(initialCapacity);

            var expectedInitialCapacity = initialCapacity + GamePooledByteBufferAllocator.BufferFrontPadding;
            var actualInitialCapacity = buffer.Capacity;

            Assert.NotNull(buffer);
            Assert.Equal(expectedInitialCapacity, actualInitialCapacity);
        }

        [Theory]
        [InlineData(0, 32)]
        [InlineData(0, 256)]
        [InlineData(1, 1)]
        [InlineData(4096, 8192)]
        public void AllocatesBufferWithCorrectMaxCapacity(int initialCapacity, int maxCapacity)
        {
            var buffer = GamePooledByteBufferAllocator.Default.Buffer(initialCapacity, maxCapacity);

            var paddedInitialCapacity = initialCapacity + GamePooledByteBufferAllocator.BufferFrontPadding;
            var paddedMaxCapacity = maxCapacity + GamePooledByteBufferAllocator.BufferFrontPadding;

            Assert.NotNull(buffer);
            Assert.Equal(paddedInitialCapacity, buffer.Capacity);
            Assert.Equal(paddedMaxCapacity, buffer.MaxCapacity);
        }

        [Theory]
        [InlineData(int.MinValue, int.MinValue)]
        [InlineData(-1, -1)]
        public void AllocatesEmptyBufferOnNegativeCapacity(int initialCapacity, int maxCapacity)
        {
            var buffer = GamePooledByteBufferAllocator.Default.Buffer(initialCapacity, maxCapacity);

            Assert.NotNull(buffer);
            Assert.Equal(0, buffer.Capacity);
            Assert.Equal(0, buffer.MaxCapacity);
            Assert.IsType<EmptyByteBuffer>(buffer);
        }

        [Fact]
        public void AllocatesEmptyBufferOnZeroCapacity()
        {
            var buffer = GamePooledByteBufferAllocator.Default.Buffer(0, 0);

            Assert.NotNull(buffer);
            Assert.Equal(0, buffer.Capacity);
            Assert.Equal(0, buffer.MaxCapacity);
            Assert.IsType<EmptyByteBuffer>(buffer);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(32, 32)]
        [InlineData(256, 4096)]
        public void WritesBitMaskInPaddingArea(int initialCapacity, int maxCapacity)
        {
            var buffer = GamePooledByteBufferAllocator.Default.Buffer(initialCapacity, maxCapacity);

            var expectedMask = unchecked((int)0xAAAAAAAA);
            var actualMask = buffer.GetInt(0);

            Assert.NotNull(buffer);
            Assert.Equal(expectedMask, actualMask);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(32, 32)]
        [InlineData(256, 4096)]
        public void SetsReaderIndexToFirstByteAfterPaddingArea(int initialCapacity, int maxCapacity)
        {
            var buffer = GamePooledByteBufferAllocator.Default.Buffer(initialCapacity, maxCapacity);

            var expectedReaderIndex = GamePooledByteBufferAllocator.BufferFrontPadding;
            var actualReaderIndex = buffer.ReaderIndex;

            Assert.NotNull(buffer);
            Assert.Equal(expectedReaderIndex, actualReaderIndex);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(32, 32)]
        [InlineData(256, 4096)]
        public void SetsWriterIndexToFirstByteAfterPaddingArea(int initialCapacity, int maxCapacity)
        {
            var buffer = GamePooledByteBufferAllocator.Default.Buffer(initialCapacity, maxCapacity);

            var expectedWriterIndex = GamePooledByteBufferAllocator.BufferFrontPadding;
            var actualWriterIndex = buffer.WriterIndex;

            Assert.NotNull(buffer);
            Assert.Equal(expectedWriterIndex, actualWriterIndex);
        }

        [Fact]
        public void HasCorrectBytesWrittenCountBeforeAnythingWritten()
        {
            var buffer = GamePooledByteBufferAllocator.Default.Buffer();

            Assert.NotNull(buffer);
            Assert.Equal(0, buffer.ReadableBytes);
        }

        [Fact]
        public void HasCorrectBytesWrittenCountAfterWrite()
        {
            var buffer = GamePooledByteBufferAllocator.Default.Buffer();

            buffer.WriteInt(1337);

            Assert.NotNull(buffer);
            Assert.Equal(4, buffer.ReadableBytes);
        }
    }
}
