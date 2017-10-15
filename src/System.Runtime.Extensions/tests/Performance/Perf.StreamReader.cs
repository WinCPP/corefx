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
        private static readonly byte[] s_readData;

        static Perf_StreamReader()
        {
            string readData = new string('a', s_length.Max() * DefaultStreamBufferSize);
            s_readData = Encoding.ASCII.GetBytes(readData);
        }

        [Benchmark]
        [MemberData(nameof(Perf_StreamBase.LengthMemberData))]
        public void ReadCharArray(int readLength)
        {
            char[] charArray = new char[s_length.Max()];

            int innerIterations = MemoryStreamSize / readLength;
            int outerIteration = TotalCount / innerIterations;            
            using (var stream = new MemoryStream(s_readData))
            {
                using (var reader = new StreamReader(stream, new UTF8Encoding(false, true), true, s_readData.Length))
                {
                    foreach (BenchmarkIteration iteration in Benchmark.Iterations)
                    {
                        using (iteration.StartMeasurement())
                        {
                            for (int i = 0; i < outerIteration; i++)
                            {
                                for (int j = 0; j < innerIterations; j++)
                                {
                                    reader.Read(charArray);
                                }
                            }
                        }
                    }
                }
            }
        }

        [Benchmark]
        [MemberData(nameof(Perf_StreamBase.LengthMemberData))]
        public void ReadCharSpan(int readLength)
        {
            char[] charArray = new char[s_length.Max()];
            Span<char> charSpan = new Span<char>(charArray);

            int innerIterations = MemoryStreamSize / readLength;
            int outerIteration = TotalCount / innerIterations;
            using (var stream = new MemoryStream(s_readData))
            {
                using (var reader = new StreamReader(stream, new UTF8Encoding(false, true), true, s_readData.Length))
                {
                    foreach (BenchmarkIteration iteration in Benchmark.Iterations)
                    {
                        using (iteration.StartMeasurement())
                        {
                            for (int i = 0; i < outerIteration; i++)
                            {
                                for (int j = 0; j < innerIterations; j++)
                                {
                                    reader.Read(charSpan);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
