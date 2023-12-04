using System.Numerics;
using System.Runtime.InteropServices;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day04 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		var lines = input.Lines;
		var inputLine = lines[0].AsSpan();

		var startingIndex = inputLine.IndexOf(':') + 2;
		var separatorIndex = startingIndex + inputLine.Slice(startingIndex).IndexOf('|');

		// 2 characters per number + whitespace, replaced by constants in standalone benchmarks
		scoped Span<int> winningNumbersBuffer = stackalloc int[(separatorIndex - startingIndex) / 3];
		scoped Span<int> cardNumbersBuffer = stackalloc int[(inputLine.Length - separatorIndex) / 3];

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
			// // 1 results in 1, 2, results in 2, 3 results in 4, 4 results in 8, etc.
			total += (int) Math.Pow(2, scoringPower - 1);
		}
	}

	public override object SolvePart2(Input input)
	{
		var lines = input.Lines;
		var inputLine = lines[0].AsSpan();

		var startingIndex = inputLine.IndexOf(':') + 2;
		var separatorIndex = startingIndex + inputLine.Slice(startingIndex).IndexOf('|');

		// 2 characters per number + whitespace, replaced by constants in standalone benchmarks
		scoped Span<int> winningNumbersBuffer = stackalloc int[(separatorIndex - startingIndex) / 3];
		scoped Span<int> cardNumbersBuffer = stackalloc int[(inputLine.Length - separatorIndex) / 3];

		scoped Span<int> cardCopiesCountBuffer = stackalloc int[lines.Length];
		cardCopiesCountBuffer.Fill(1);

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

	// ReSharper disable once CognitiveComplexity
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