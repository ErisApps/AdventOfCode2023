using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace AdventOfCode2023.Benchmarks.Standalone.Puzzles;

[MemoryDiagnoser(true)]
[CategoriesColumn, AllStatisticsColumn, BaselineColumn, MinColumn, Q1Column, MeanColumn, Q3Column, MaxColumn, MedianColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Day09Benchmark
{
	private readonly Input _input;

	public Day09Benchmark()
	{
		_input = Helpers.GetInput("Day09.txt");
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART1)]
	public int Part1()
	{
		scoped Span<int> numbersBuffer = stackalloc int[50];

		var total = 0;
		for (var i = 0; i < _input.Lines.Length; i++)
		{
			var inputLineSpan = _input.Lines[i].AsSpan();
			ParseLine(ref inputLineSpan, numbersBuffer, out var numbersBufferSize);

			Part1_ReduceAndExtrapolate(numbersBuffer.Slice(0, numbersBufferSize + 1));

			total += numbersBuffer[numbersBufferSize];
		}

		return total;
	}

	private void Part1_ReduceAndExtrapolate(Span<int> slice)
	{
		scoped Span<int> reducedSlice = stackalloc int[slice.Length - 1];

		var continueReducing = false;
		for (var i = 0; i < slice.Length - 2; i++)
		{
			var diff = slice[i + 1] - slice[i];
			reducedSlice[i] = diff;

			continueReducing |= diff != 0;
		}

		if (continueReducing)
		{
			Part1_ReduceAndExtrapolate(reducedSlice);
		}

		slice[^1] = reducedSlice[^1] + slice[^2];
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART2)]
	public int Part2()
	{
		scoped Span<int> numbersBuffer = stackalloc int[50];

		var total = 0;
		for (var i = 0; i < _input.Lines.Length; i++)
		{
			var inputLineSpan = _input.Lines[i].AsSpan();
			ParseLine(ref inputLineSpan, numbersBuffer.Slice(1), out var numbersBufferSize);

			Part2_ReduceAndExtrapolate(numbersBuffer.Slice(0, numbersBufferSize + 1));

			total += numbersBuffer[0];
		}

		return total;
	}

	private void Part2_ReduceAndExtrapolate(Span<int> slice)
	{
		scoped Span<int> reducedSlice = stackalloc int[slice.Length - 1];

		var continueReducing = false;
		for (var i = slice.Length - 2; i >= 1; i--)
		{
			var diff = slice[i + 1] - slice[i];
			reducedSlice[i] = diff;

			continueReducing |= diff != 0;
		}

		if (continueReducing)
		{
			Part2_ReduceAndExtrapolate(reducedSlice);
		}

		slice[0] = slice[1] - reducedSlice[0];
	}

	// ReSharper disable once CognitiveComplexity
	private static void ParseLine(scoped ref ReadOnlySpan<char> inputLine, scoped Span<int> numbersBuffer, out int numbersBufferSize)
	{
		numbersBufferSize = 0;

		for (var i = 0; i < inputLine.Length; i++)
		{
			var c = inputLine[i];
			var shouldNegate = false;
			if (c == '-')
			{
				shouldNegate = true;
				c = inputLine[++i];
			}

			var number = c - '0';
			++i;
			for (; i < inputLine.Length; i++)
			{
				c = inputLine[i];
				if (c == ' ')
				{
					break;
				}

				number = number * 10 + c - '0';
			}

			numbersBuffer[numbersBufferSize++] = shouldNegate ? -number : number;
		}
	}
}