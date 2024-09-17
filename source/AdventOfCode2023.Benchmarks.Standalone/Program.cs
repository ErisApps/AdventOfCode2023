// See https://aka.ms/new-console-template for more information

using AdventOfCode2023.Benchmarks.Standalone.Puzzles;
using BenchmarkDotNet.Running;

//BenchmarkRunner.Run(typeof(Program).Assembly);
BenchmarkRunner.Run<Day11Benchmark>();