using AdventOfCode2023.Common;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2023.Benchmarks;

[MemoryDiagnoser(true)]
[CategoriesColumn, AllStatisticsColumn, BaselineColumn, MinColumn, Q1Column, MeanColumn, Q3Column, MaxColumn, MedianColumn]
public class HappyPuzzleBaseBenchmark<TPuzzle> where TPuzzle : HappyPuzzleBase, new()
{
	private readonly TPuzzle _sub;
	private readonly Input _input;

	public HappyPuzzleBaseBenchmark()
	{
		_sub = new TPuzzle();
		_input = Helpers.GetInput(_sub.AssetName);
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART1)]
	public void SolvePart1() => _sub.SolvePart1(_input);

	[Benchmark]
	[BenchmarkCategory(Constants.PART2)]
	public void SolvePart2() => _sub.SolvePart2(_input);
}