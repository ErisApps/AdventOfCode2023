// See https://aka.ms/new-console-template for more information

using AdventOfCode2023.Benchmarks;
using AdventOfCode2023.Common;
using BenchmarkDotNet.Running;

var benchmarkCases = HappyPuzzleHelpers
	.DiscoverPuzzles()
	.TakeLast(4)
	.Take(1)
	.Select(x => typeof(HappyPuzzleBaseBenchmark<>).MakeGenericType(x))
	.ToArray();

BenchmarkRunner.Run(benchmarkCases);