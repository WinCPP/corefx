// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void TryCopyTo()
        {
            int[] src = { 1, 2, 3 };
            int[] dst = { 99, 100, 101 };

            Span<int> srcSpan = new Span<int>(src);
            bool success = srcSpan.TryCopyTo(dst);
            Assert.True(success);
            Assert.Equal<int>(src, dst);
        }

        [Fact]
        public static void TryCopyToSingle()
        {
            int[] src = { 1 };
            int[] dst = { 99 };

            Span<int> srcSpan = new Span<int>(src);
            bool success = srcSpan.TryCopyTo(dst);
            Assert.True(success);
            Assert.Equal<int>(src, dst);
        }

        [Fact]
        public static void TryCopyToArraySegmentImplicit()
        {
            int[] src = { 1, 2, 3 };
            int[] dst = { 5, 99, 100, 101, 10 };
            var segment = new ArraySegment<int>(dst, 1, 3);

            Span<int> srcSpan = new Span<int>(src);
            bool success = srcSpan.TryCopyTo(segment);
            Assert.True(success);
            Assert.Equal<int>(src, segment);
        }

        [Fact]
        public static void TryCopyToEmpty()
        {
            int[] src = { };
            int[] dst = { 99, 100, 101 };

            Span<int> srcSpan = new Span<int>(src);
            bool success = srcSpan.TryCopyTo(dst);
            Assert.True(success);
            int[] expected = { 99, 100, 101 };
            Assert.Equal<int>(expected, dst);
        }

        [Fact]
        public static void TryCopyToLonger()
        {
            int[] src = { 1, 2, 3 };
            int[] dst = { 99, 100, 101, 102 };

            Span<int> srcSpan = new Span<int>(src);
            bool success = srcSpan.TryCopyTo(dst);
            Assert.True(success);
            int[] expected = { 1, 2, 3, 102 };
            Assert.Equal<int>(expected, dst);
        }

        [Fact]
        public static void TryCopyToShorter()
        {
            int[] src = { 1, 2, 3 };
            int[] dst = { 99, 100 };

            Span<int> srcSpan = new Span<int>(src);
            bool success = srcSpan.TryCopyTo(dst);
            Assert.False(success);
            int[] expected = { 99, 100 };
            Assert.Equal<int>(expected, dst);  // TryCopyTo() checks for sufficient space before doing any copying.
        }

        [Fact]
        public static void CopyToShorter()
        {
            int[] src = { 1, 2, 3 };
            int[] dst = { 99, 100 };

            Span<int> srcSpan = new Span<int>(src);
            AssertThrows<ArgumentException, int>(srcSpan, (_srcSpan) => _srcSpan.CopyTo(dst));
            int[] expected = { 99, 100 };
            Assert.Equal<int>(expected, dst);  // CopyTo() checks for sufficient space before doing any copying.
        }

        [Fact]
        public static void Overlapping1()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97 };

            Span<int> src = new Span<int>(a, 1, 6);
            Span<int> dst = new Span<int>(a, 2, 6);
            src.CopyTo(dst);

            int[] expected = { 90, 91, 91, 92, 93, 94, 95, 96 };
            Assert.Equal<int>(expected, a);
        }

        [Fact]
        public static void Overlapping2()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97 };

            Span<int> src = new Span<int>(a, 2, 6);
            Span<int> dst = new Span<int>(a, 1, 6);
            src.CopyTo(dst);

            int[] expected = { 90, 92, 93, 94, 95, 96, 97, 97 };
            Assert.Equal<int>(expected, a);
        }

        [Fact]
        public static void CopyToArray()
        {
            int[] src = { 1, 2, 3 };
            int[] dst = { 99, 100, 101 };

            src.CopyTo(dst);
            Assert.Equal<int>(src, dst);
        }

        [Fact]
        public static void CopyToSingleArray()
        {
            int[] src = { 1 };
            int[] dst = { 99 };

            src.CopyTo(dst);
            Assert.Equal<int>(src, dst);
        }

        [Fact]
        public static void CopyToEmptyArray()
        {
            int[] src = { };
            int[] dst = { 99, 100, 101 };

            src.CopyTo(dst);
            int[] expected = { 99, 100, 101 };
            Assert.Equal<int>(expected, dst);

            int[] dstEmpty = { };

            src.CopyTo(dst);
            int[] expectedEmpty = { };
            Assert.Equal<int>(expectedEmpty, dstEmpty);
        }

        [Fact]
        public static void CopyToLongerArray()
        {
            int[] src = { 1, 2, 3 };
            int[] dst = { 99, 100, 101, 102 };

            src.CopyTo(dst);
            int[] expected = { 1, 2, 3, 102 };
            Assert.Equal<int>(expected, dst);
        }

        [Fact]
        public static void CopyToShorterArray()
        {
            int[] src = { 1, 2, 3 };
            int[] dst = { 99, 100 };

            AssertThrows<ArgumentException, int>(src, (_src) => _src.CopyTo(dst));
            int[] expected = { 99, 100 };
            Assert.Equal<int>(expected, dst);  // CopyTo() checks for sufficient space before doing any copying.
        }

        [Fact]
        public static void CopyToCovariantArray()
        {
            string[] src = new string[] { "Hello" };
            object[] dst = new object[] { "world" };

            src.CopyTo<object>(dst);
            Assert.Equal("Hello", dst[0]);
        }

        // This test case tests the Span.CopyTo method for large buffers of size 4GB or more. In the fast path,
        // the CopyTo method performs copy in chunks of size 4GB (uint.MaxValue) with final iteration copying
        // the residual chunk of size (bufferSize % 4GB). The inputs sizes to this method, 4GB and 4GB+256B,
        // test the two size selection paths in CoptyTo method - memory size that is multiple of 4GB or,
        // a multiple of 4GB + some more size.
        [Theory]
        [OuterLoop]
        [PlatformSpecific(TestPlatforms.Windows | TestPlatforms.OSX)]
        [InlineData(4L * 1024L * 1024L * 1024L)]
        [InlineData((4L * 1024L * 1024L * 1024L) + 256)]
        public static void CopyToLargeSizeTest(long bufferSize)
        {
            // If this test is run in a 32-bit process, the large allocation will fail.
            if (Unsafe.SizeOf<IntPtr>() != sizeof(long))
            {
                return;
            }

            int GuidCount = (int)(bufferSize / Unsafe.SizeOf<Guid>());
            bool allocatedFirst = false;
            bool allocatedSecond = false;
            IntPtr memBlockFirst = IntPtr.Zero;
            IntPtr memBlockSecond = IntPtr.Zero;

            unsafe
            {
                try
                {
                    allocatedFirst = AllocationHelper.TryAllocNative((IntPtr)bufferSize, out memBlockFirst);
                    allocatedSecond = AllocationHelper.TryAllocNative((IntPtr)bufferSize, out memBlockSecond);

                    if (allocatedFirst && allocatedSecond)
                    {
                        ref Guid memoryFirst = ref Unsafe.AsRef<Guid>(memBlockFirst.ToPointer());
                        var spanFirst = new Span<Guid>(memBlockFirst.ToPointer(), GuidCount);

                        ref Guid memorySecond = ref Unsafe.AsRef<Guid>(memBlockSecond.ToPointer());
                        var spanSecond = new Span<Guid>(memBlockSecond.ToPointer(), GuidCount);

                        Guid theGuid = Guid.Parse("900DBAD9-00DB-AD90-00DB-AD900DBADBAD");
                        for (int count = 0; count < GuidCount; ++count)
                        {
                            Unsafe.Add(ref memoryFirst, count) = theGuid;
                        }

                        spanFirst.CopyTo(spanSecond);

                        for (int count = 0; count < GuidCount; ++count)
                        {
                            var guidfirst = Unsafe.Add(ref memoryFirst, count);
                            var guidSecond = Unsafe.Add(ref memorySecond, count);
                            Assert.Equal(guidfirst, guidSecond);
                        }
                    }
                }
                finally
                {
                    if (allocatedFirst) AllocationHelper.ReleaseNative(ref memBlockFirst);
                    if (allocatedSecond) AllocationHelper.ReleaseNative(ref memBlockSecond);
                }
            }
        }

        private struct TestData
        {
            public int a;
            public int b;
            public long c;
            public char d;
            public TestData(int a = 0, int b = 0, long c = 0, char d = '\0')
            {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
            }
        }

        [Fact]
        [Trait("category", "mperf")]
        public static void PerfTestDriver()
        {
            CopyPerfTest<int>();
            CopyPerfTest<TestData>();
        }

        private static void CopyPerfTest<T>()
        {
            int iterationCount = 20;
            var timeSpent = new long[iterationCount];

            CopyPerfTestWrapperBackward<T>(20000000, iterationCount, timeSpent);
            Console.WriteLine("------------- Backward Type: " + typeof(T).ToString() + " -------------");
            foreach (var item in timeSpent)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("-------------");

            CopyPerfTestWrapperForward<T>(20000000, iterationCount, timeSpent);
            Console.WriteLine("------------- Forward Type: " + typeof(T).ToString() + " -------------");
            foreach (var item in timeSpent)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("-------------");
        }

        private static void CopyPerfTestWrapperBackward<T>(int size, int iterationCount, long[] timeSpent)
        {
            Stopwatch stopWatch = new Stopwatch();

            int elementCount = size;
            int spanCount = elementCount * 4;

            var theArray = new T[spanCount];
            var srcSpan = new Span<T>(theArray, 0, elementCount * 2);

            for (int iteration = 0; iteration < iterationCount; ++iteration)
            {
                var destSpan = new Span<T>(theArray, elementCount + iteration, elementCount * 2);

                GC.Collect();

                stopWatch.Reset();
                stopWatch.Start();
                srcSpan.CopyTo(destSpan);
                stopWatch.Stop();
                timeSpent[iteration] = stopWatch.ElapsedMilliseconds;
            }
        }

        private static void CopyPerfTestWrapperForward<T>(int size, int iterationCount, long[] timeSpent)
        {
            Stopwatch stopWatch = new Stopwatch();

            int elementCount = size;
            int spanCount = elementCount * 4;

            var theArray = new T[spanCount];
            var destSpan = new Span<T>(theArray, 0, elementCount * 2);

            for (int iteration = 0; iteration < iterationCount; ++iteration)
            {
                var srcSpan = new Span<T>(theArray, elementCount + iteration, elementCount * 2);

                GC.Collect();

                stopWatch.Reset();
                stopWatch.Start();
                srcSpan.CopyTo(destSpan);
                stopWatch.Stop();
                timeSpent[iteration] = stopWatch.ElapsedMilliseconds;
            }
        }
    }
}
