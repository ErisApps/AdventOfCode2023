using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day01 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		var intCharSearchValues = SearchValues.Create("123456789");

		var total = 0;
		foreach (var line in input.Lines)
		{
			var lineSpan = line.AsSpan();

			var firstCharIndex = lineSpan.IndexOfAny(intCharSearchValues);
			var lastCharIndex = lineSpan.LastIndexOfAny(intCharSearchValues);

			var calibrationNumber = (lineSpan[firstCharIndex] - '0') * 10 + (lineSpan[lastCharIndex] - '0');

			total += calibrationNumber;
		}

		return total;
	}

	public override object SolvePart2(Input input)
	{
		var total = 0;
		foreach (var inputLine in input.Lines)
		{
			var inputLineSpan = inputLine.AsSpan();
			var first = Part2_FindFirst(ref inputLineSpan);
			var last = Part2_FindLast(ref inputLineSpan);

			total += first * 10 + last;
		}

		return total;
	}

	private static int Part2_FindFirst(ref ReadOnlySpan<char> span)
	{
		for (var i = 0; i < span.Length; i++)
		{
			if (Part2_Find_SharedLogic(span, i, out var findFirst))
			{
				return findFirst.Value;
			}
		}

		return -1;
	}

	private static int Part2_FindLast(ref ReadOnlySpan<char> span)
	{
		for (var i = span.Length - 1; i >= 0; i--)
		{
			if (Part2_Find_SharedLogic(span, i, out var findFirst))
			{
				return findFirst.Value;
			}
		}

		return -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	// ReSharper disable once CognitiveComplexity
	// ReSharper disable once CyclomaticComplexity
	private static bool Part2_Find_SharedLogic(ReadOnlySpan<char> span, int startingIndex, [NotNullWhen(true)] out int? number)
	{
		var c = span[startingIndex];

		if (char.IsAsciiDigit(c))
		{
			{
				number = c - '0';
				return true;
			}
		}

		if (span.Length - startingIndex < 3)
		{
			number = null;
			return false;
		}

		ReadOnlySpan<char> slice;
		switch (c)
		{
			case 'e':
				if (span.Length - startingIndex < 5)
				{
					number = null;
					return false;
				}

				slice = span.Slice(startingIndex + 1, 4);
				if (slice.SequenceEqual("ight"))
				{
					{
						number = 8;
						return true;
					}
				}

				break;
			case 'f':
				if (span.Length - startingIndex < 4)
				{
					number = null;
					return false;
				}

				slice = span.Slice(startingIndex + 1, 3);
				switch (slice)
				{
					case "our":
					{
						number = 4;
						return true;
					}
					case "ive":
					{
						number = 5;
						return true;
					}
				}

				break;
			case 'o':
				slice = span.Slice(startingIndex + 1, 2);
				if (slice.SequenceEqual("ne"))
				{
					number = 1;
					return true;
				}

				break;
			case 'n':
				if (span.Length - startingIndex < 4)
				{
					number = null;
					return false;
				}

				slice = span.Slice(startingIndex + 1, 3);
				if (slice.SequenceEqual("ine"))
				{
					number = 9;
					return true;
				}

				break;
			case 's':
				if (span.Length - startingIndex >= 5)
				{
					slice = span.Slice(startingIndex + 1, 4);
					if (slice.SequenceEqual("even"))
					{
						number = 7;
						return true;
					}
				}

				if (span.Slice(startingIndex + 1, 2).SequenceEqual("ix"))
				{
					number = 6;
					return true;
				}

				break;
			case 't':
				if (span.Length - startingIndex >= 5)
				{
					slice = span.Slice(startingIndex + 1, 4);
					if (slice.SequenceEqual("hree"))
					{
						number = 3;
						return true;
					}
				}

				if (span.Slice(startingIndex + 1, 2).SequenceEqual("wo"))
				{
					number = 2;
					return true;
				}

				break;
		}

		number = null;
		return false;
	}
}