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
    public class Perf_StreamReader : Perf_StreamBase
    {
        private static readonly char[] s_readData;
        private static readonly MemoryStream s_stream;
        private static readonly StreamReader s_reader;

        static Perf_StreamReader()
        {
            s_readData = new char[s_length.Max() * DefaultStreamBufferSize];
            s_stream = new MemoryStream(Encoding.ASCII.GetBytes(s_readData));
            s_reader = new StreamReader(s_stream, new UTF8Encoding(false, true), true, s_readData.Length);
        }

        [Benchmark]
        [MemberData(nameof(Perf_StreamBase.LengthMemberData))]
        public void ReadCharArray(int readLength)
        {
            char[] charArray = new char[s_length.Max()];
            PerformanceLoop(readLength, () =>
                {
                    s_reader.Read(charArray, 0, readLength);
                }
            );
        }

        [Benchmark]
        [MemberData(nameof(Perf_StreamBase.LengthMemberData))]
        public void ReadCharSpan(int readLength)
        {
            char[] charArray = new char[s_length.Max()];
            Span<char> charSpan = new Span<char>(charArray);

            int innerIterations = MemoryStreamSize / readLength;
            int outerIteration = TotalCount / innerIterations;
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < outerIteration; i++)
                    {
                        for (int j = 0; j < innerIterations; j++)
                        {
                            s_reader.Read(charSpan);
                        }
                    }
                }
            }
        }
    }
}
