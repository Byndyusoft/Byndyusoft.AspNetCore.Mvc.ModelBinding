// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using PerformanceTests.Tests;

BenchmarkRunner.Run<HashTest>();
BenchmarkRunner.Run<SaveToDiskTest>();
//BenchmarkRunner.Run<UploadToStorageTest>();