// Licensed to the.NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xunit.Performance;
using System.Linq;
using Xunit;

namespace System.IO.Tests
{
    public class Perf_StreamBase
    {
        protected static int[] s_length = new int[] { 2, 100 };

        private static readonly IEnumerable<object[]> s_memberData;

        protected static IEnumerable<object[]> LengthMemberData() => s_memberData;

        protected const int MemoryStreamSize = 32768;

        protected const int TotalCount = 16777216; // 2^24 - should yield around 300ms runs

        protected const int DefaultStreamBufferSize = 1024; // Same as StreamWriter internal default

        static Perf_StreamBase()
        {
            s_memberData = s_length.Select(value => new object[] { value });
        }

        public void PerformanceLoop(int length, Action method)
        {
            int innerIterations = MemoryStreamSize / length;
            int outerIteration = TotalCount / innerIterations;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < outerIteration; i++)
                    {
                        for (int j = 0; j < innerIterations; j++)
                        {
                            method();
                        }
                    }
                }
            }
        }
    }
}
