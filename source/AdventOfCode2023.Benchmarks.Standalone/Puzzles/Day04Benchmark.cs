using System.Numerics;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace AdventOfCode2023.Benchmarks.Standalone.Puzzles;

[MemoryDiagnoser(true)]
[CategoriesColumn, AllStatisticsColumn, BaselineColumn, MinColumn, Q1Column, MeanColumn, Q3Column, MaxColumn, MedianColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Day04Benchmark
{
	private readonly Input _input;

	public Day04Benchmark()
	{
		_input = Helpers.GetInput("Day04.txt");
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART1)]
	public int Part1()
	{
		scoped Span<int> winningNumbersBuffer = stackalloc int[10];
		scoped Span<int> cardNumbersBuffer = stackalloc int[25];

		var lines = _input.Lines;
		var startingIndex = lines[0].IndexOf(':') + 2;

		var total = 0;
		for (var i = 0; i < lines.Length; i++)
		{
			var inputLineSpan = lines[i].AsSpan().Slice(startingIndex);

			ParseLine(ref inputLineSpan, winningNumbersBuffer, cardNumbersBuffer);

			Part1_ValidateAndSum(ref total, winningNumbersBuffer, cardNumbersBuffer);
		}

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static void Part1_ValidateAndSum(ref int total, scoped Span<int> winningNumbersBuffer, scoped Span<int> cardNumbersBuffer)
	{
		var scoringPower = 0;

		for (var i = 0; i < cardNumbersBuffer.Length; i++)
		{
			var cardNumber = cardNumbersBuffer[i];
			for (var j = 0; j < winningNumbersBuffer.Length; j++)
			{
				var winningNumber = winningNumbersBuffer[j];
				if (cardNumber == winningNumber)
				{
					scoringPower++;
					break;
				}
			}
		}

		if (scoringPower > 0)
		{
			// Exponential scoring using f(x) = 2^(x-1)
			// 1 results in 1, 2, results in 2, 3 results in 4, 4 results in 8, etc.
			total += (int) Math.Pow(2, scoringPower - 1);
		}
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART2)]
	public int Part2()
	{
		scoped Span<int> winningNumbersBuffer = stackalloc int[10];
		scoped Span<int> cardNumbersBuffer = stackalloc int[25];

		var lines = _input.Lines;
		scoped Span<int> cardCopiesCountBuffer = stackalloc int[lines.Length];
		cardCopiesCountBuffer.Fill(1);

		var startingIndex = lines[0].IndexOf(':') + 2;

		for (var i = 0; i < lines.Length; i++)
		{
			var inputLineSpan = lines[i].AsSpan().Slice(startingIndex);

			ParseLine(ref inputLineSpan, winningNumbersBuffer, cardNumbersBuffer);

			Part2_ValidateAndScratch(i, winningNumbersBuffer, cardNumbersBuffer, cardCopiesCountBuffer);
		}

		// Vectorized sum
		ReadOnlySpan<Vector<int>> vectors = MemoryMarshal.Cast<int, Vector<int>>(cardCopiesCountBuffer);
		var acc = Vector<int>.Zero;
		for (var i = 0; i < vectors.Length; i++)
		{
			acc += vectors[i];
		}

		var result = Vector.Dot(acc, Vector<int>.One);
		for (var i = vectors.Length * Vector<int>.Count; i < cardCopiesCountBuffer.Length; i++)
		{
			result += cardCopiesCountBuffer[i];
		}

		return result;
	}

	private static void Part2_ValidateAndScratch(int cardIndex, scoped Span<int> winningNumbersBuffer, scoped Span<int> cardNumbersBuffer, scoped Span<int> cardCopiesCountBuffer)
	{
		var currentCardCount = cardCopiesCountBuffer[cardIndex];
		var currentCardCopiesCounterBufferIndex = cardIndex;

		for (var i = 0; i < cardNumbersBuffer.Length; i++)
		{
			var cardNumber = cardNumbersBuffer[i];
			for (var j = 0; j < winningNumbersBuffer.Length; j++)
			{
				var winningNumber = winningNumbersBuffer[j];
				if (cardNumber == winningNumber)
				{
					cardCopiesCountBuffer[++currentCardCopiesCounterBufferIndex] += currentCardCount;
					break;
				}
			}
		}
	}

	private static void ParseLine(ref ReadOnlySpan<char> inputLine, scoped Span<int> winningNumbersBuffer, scoped Span<int> cardNumbersBuffer)
	{
		var currentSpanIndex = 0;
		for (var i = 0; i < inputLine.Length; i += 3)
		{
			var c = inputLine[i];
			if (c == '|')
			{
				inputLine = inputLine.Slice(i + 2);
				break;
			}

			var number = c == ' '
				? inputLine[i + 1] - '0'
				: (c - '0') * 10 + inputLine[i + 1] - '0';

			winningNumbersBuffer[currentSpanIndex++] = number;
		}

		currentSpanIndex = 0;
		for (var i = 0; i < inputLine.Length; i+= 3)
		{
			var c = inputLine[i];
			var number = c == ' '
				? inputLine[i + 1] - '0'
				: (c - '0') * 10 + inputLine[i + 1] - '0';

			cardNumbersBuffer[currentSpanIndex++] = number;
		}
	}
}