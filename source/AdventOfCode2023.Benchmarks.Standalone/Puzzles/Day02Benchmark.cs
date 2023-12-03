using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace AdventOfCode2023.Benchmarks.Standalone.Puzzles;

[MemoryDiagnoser(true)]
[CategoriesColumn, AllStatisticsColumn, BaselineColumn, MinColumn, Q1Column, MeanColumn, Q3Column, MaxColumn, MedianColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Day02Benchmark
{
	private readonly Input _input;

	public Day02Benchmark()
	{
		_input = Helpers.GetInput("Day02.txt");
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART1)]
	public int Part1()
	{
		var total = 0;
		for (var i = 0; i < _input.Lines.Length; i++)
		{
			var inputLineSpan = _input.Lines[i].AsSpan();

			var lookupStartIndex = inputLineSpan.IndexOf(':') + 2; // offset by 2 due to whitespace following the colon
			inputLineSpan = inputLineSpan.Slice(lookupStartIndex);

			if (Part1_ValidateGame(inputLineSpan))
			{
				total += i + 1;
			}
		}

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static bool Part1_ValidateGame(ReadOnlySpan<char> span)
	{
		var redCount = 0;
		var greenCount = 0;
		var blueCount = 0;

		var i = 0;
		do
		{
			switch (span[i])
			{
				case ';':
					redCount = 0;
					greenCount = 0;
					blueCount = 0;

					i += 2; // skip the semi-colon and whitespace
					break;
				case ':':
					i += 2; // skip the colon and whitespace
					break;
			}

			var c = span[i];
			var currentNumber = c - '0';

			c = span[++i];
			while (char.IsAsciiDigit(c))
			{
				currentNumber = currentNumber * 10 + (c - '0');
				c = span[++i];
			}

			i++; // skip the comma/semi-colon and whitespace

			switch (span[i])
			{
				case 'r':
					redCount += currentNumber;
					if (redCount > 12)
					{
						return false;
					}

					i += 3; // skip the 'red' and whitespace
					break;
				case 'g':
					greenCount += currentNumber;
					if (greenCount > 13)
					{
						return false;
					}

					i += 5;
					break;
				case 'b':
					blueCount += currentNumber;
					if (blueCount > 14)
					{
						return false;
					}

					i += 4;
					break;
			}
		} while (i < span.Length);

		return true;
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART2)]
	public int Part2()
	{
		var total = 0;
		foreach (var inputLine in _input.Lines)
		{
			var inputLineSpan = inputLine.AsSpan();

			var lookupStartIndex = inputLineSpan.IndexOf(':') + 2; // offset by 2 due to whitespace following the colon
			inputLineSpan = inputLineSpan.Slice(lookupStartIndex);

			total += Part2_GetGamePower(ref inputLineSpan);
		}

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static int Part2_GetGamePower(ref ReadOnlySpan<char> span)
	{
		var maxRedCount = 0;
		var maxGreenCount = 0;
		var maxBlueCount = 0;

		var i = 0;
		do
		{
			var c = span[i];
			var currentNumber = c - '0';

			c = span[++i];
			while (char.IsAsciiDigit(c))
			{
				currentNumber = currentNumber * 10 + (c - '0');
				c = span[++i];
			}

			i++; // skip the comma/semi-colon and whitespace

			switch (span[i])
			{
				case 'r':
					if (maxRedCount < currentNumber)
					{
						maxRedCount = currentNumber;
					}

					i += 5; // skip the 'red', comma/semi-colon and whitespace
					break;
				case 'g':
					if (maxGreenCount < currentNumber)
					{
						maxGreenCount = currentNumber;
					}

					i += 7;
					break;
				case 'b':
					if (maxBlueCount < currentNumber)
					{
						maxBlueCount = currentNumber;
					}

					i += 6;
					break;
			}
		} while (i < span.Length);

		return maxRedCount * maxGreenCount * maxBlueCount;
	}
}