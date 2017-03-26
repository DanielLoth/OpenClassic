using System;
using System.Diagnostics;

namespace OpenClassic.Server.Extensions
{
    public static class ArraySegmentExtensions
    {
        public static void MoveDataToStart(this ArraySegment<byte> segment, int srcOffset)
        {
#if DEBUG
            // Preconditions
            Debug.Assert(segment.Array != null);
            Debug.Assert(segment.Count > 0);
            Debug.Assert(segment.Offset >= 0);

            // The source offset is within the range of the virtual array.
            Debug.Assert(srcOffset >= 0 && srcOffset < segment.Count);
#endif

            var countAfterOffset = segment.Count - srcOffset;

            MoveDataToStart(segment, srcOffset, countAfterOffset);
        }

        public static void MoveDataToStart(this ArraySegment<byte> segment, int srcOffset, int bytesToMove)
        {
#if DEBUG
            // Preconditions
            Debug.Assert(segment.Array != null);
            Debug.Assert(segment.Count > 0);
            Debug.Assert(segment.Offset >= 0);

            // The source offset is within the range of the virtual array.
            Debug.Assert(srcOffset >= 0 && srcOffset < segment.Count);

            // We must move at least one byte
            Debug.Assert(bytesToMove > 0 && bytesToMove < segment.Count);
#endif

            var buffer = segment.Array;
            var actualStartIndex = segment.Offset;
            var actualSrcOffset = actualStartIndex + srcOffset;

            // Do the copy
            Buffer.BlockCopy(buffer, actualSrcOffset, buffer, actualStartIndex, bytesToMove);
        }
    }
}
