﻿using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using PerformanceTests.Consts;
using PerformanceTests.Helpers;

namespace PerformanceTests.Tests
{
    public class HashTest
    {
        private BenchmarkTestInstance _benchmarkTestInstance = default!;

        [GlobalSetup]
        public void Setup()
        {
            _benchmarkTestInstance = new BenchmarkTestInstance("Files/Hash");
        }

        [Params(Subfolders.Small, Subfolders.Big)] 
        public string Subfolder = default!;

        [Benchmark]
        public async Task<string[]> HashOld()
        {
            return await _benchmarkTestInstance.TestOldWay<string[]>(Subfolder);
        }

        [Benchmark]
        public async Task<string[]> HashNew()
        {
            return await _benchmarkTestInstance.TestNewWay<string[]>(Subfolder);
        }
    }
}