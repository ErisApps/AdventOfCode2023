using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day09 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		scoped Span<int> numbersBuffer = stackalloc int[50];

		var total = 0;
		for (var i = 0; i < input.Lines.Length; i++)
		{
			var inputLineSpan = input.Lines[i].AsSpan();
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

	public override object SolvePart2(Input input)
	{
		scoped Span<int> numbersBuffer = stackalloc int[50];

		var total = 0;
		for (var i = 0; i < input.Lines.Length; i++)
		{
			var inputLineSpan = input.Lines[i].AsSpan();
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