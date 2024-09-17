using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day12 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		scoped Span<char> patternBuffer = stackalloc char[35];

		scoped Span<int> rulesBuffer = stackalloc int[10];

		var total = 0;
		for (var i = 0; i < input.Lines.Length; i++)
		{
			var inputLineSpan = input.Lines[i].AsSpan();

			Part1_ParseLine(ref inputLineSpan, patternBuffer, out var patternBufferSize, rulesBuffer, out var rulesBufferSize);

			total += Count(patternBuffer.Slice(0, patternBufferSize), 0, rulesBuffer.Slice(0, rulesBufferSize), 0);

			_cache.Clear();
		}

		return total;
	}

	private static void Part1_ParseLine(
		ref ReadOnlySpan<char> inputLineSpan,
		scoped Span<char> pattenBuffer, out int patternBufferSize,
		scoped Span<int> rulesBuffer, out int rulesBufferSize)
	{
		var i = 0;
		do
		{
			// Advance until we hit the space after the pattern
		} while (inputLineSpan[i++] != ' ');

		patternBufferSize = i - 1;

		var patternSlice = inputLineSpan.Slice(0, patternBufferSize);
		patternSlice.CopyTo(pattenBuffer);

		rulesBufferSize = 0;
		do
		{
			var c = inputLineSpan[i++];
			var currentNumber = c - '0';

			for (; i < inputLineSpan.Length; i++)
			{
				c = inputLineSpan[i];
				if (c == ',')
				{
					break;
				}

				currentNumber = currentNumber * 10 + (c - '0');
			}

			rulesBuffer[rulesBufferSize++] = currentNumber;
			i++;
		} while (i < inputLineSpan.Length);
	}

	/*static int Run(int mult)
	{
		var tot = 0;
		foreach (var line in )
		{
			var temp = line.Split();
			string a = temp[0];
			var c = Array.ConvertAll(temp[1].Split(','), int.Parse);
			_cache.Clear();
			if (mult > 1)
			{
				a = string.Concat(Enumerable.Repeat(a + "?", 5)).TrimEnd('?');
			}

			tot += Count(ref a, 0, c, 0);
		}
		return tot;
	}*/

	private static readonly Dictionary<(int, int), int> _cache = new();

	// ReSharper disable once CognitiveComplexity
	// ReSharper disable once CyclomaticComplexity
	private static int Count(scoped Span<char> patternBuffer, int patternSymbolPosition, scoped Span<int> rulesBuffer, int rulePosition)
	{
		// Skip dots
		// TODO: Check if safe-guarding against out-of-bounds is necessary
		while (patternSymbolPosition < patternBuffer.Length && patternBuffer[patternSymbolPosition] == '.')
		{
			patternSymbolPosition++;
		}

		/*if (_cache.ContainsKey((patternSymbolPosition, rulePosition)))
		{
			return _cache[(patternSymbolPosition, rulePosition)];
		}*/

		if (rulePosition == rulesBuffer.Length - 1)
		{
			for (var j = patternSymbolPosition; j < patternBuffer.Length; j++)
			{
				if (patternBuffer[j] != '.' && patternBuffer[j] != '?')
				{
					//_cache[(patternSymbolPosition, rulePosition)] = 0;
					return 0;
				}
			}
			//_cache[(patternSymbolPosition, rulePosition)] = 1;
			return 1;
		}

		if (patternSymbolPosition >= patternBuffer.Length)
		{
			return 0;
		}

		var ruleLength = rulesBuffer[rulePosition];
		var startPosition = patternSymbolPosition;
		var currentPosition = patternSymbolPosition;

		// Scan the current pattern buffer for the current rule
		var found = false;
		while (ruleLength > 0 && currentPosition < patternBuffer.Length && (patternBuffer[currentPosition] == '?' || patternBuffer[currentPosition] == '#'))
		{
			found |= patternBuffer[currentPosition] == '#';
			currentPosition++;
			ruleLength--;
		}

		if ((found || currentPosition >= patternBuffer.Length) && ruleLength > 0)
		{
			return 0;
		}

		var count = 0;
		if (currentPosition == patternBuffer.Length - 1  || patternBuffer[currentPosition] != '#')
		{
			count += Count(patternBuffer, currentPosition + 1, rulesBuffer, rulePosition + 1);
		}

		while (startPosition < currentPosition && patternBuffer[startPosition] == '?' && (patternBuffer[currentPosition] == '#' || patternBuffer[currentPosition] == '?'))
		{
			found = found || patternBuffer[currentPosition] == '#';
			startPosition++;
			currentPosition++;
			if (currentPosition == patternBuffer.Length || patternBuffer[currentPosition] != '#')
			{
				count += Count(patternBuffer, currentPosition + 1, rulesBuffer, rulePosition + 1);
			}
		}

		if (!found && currentPosition < patternBuffer.Length)
		{
			count += Count(patternBuffer, currentPosition + 1, rulesBuffer, rulePosition);
		}

		//_cache[(patternSymbolPosition, rulePosition)] = count;
		return count;
	}

	public override object SolvePart2(Input input)
	{
		throw new NotImplementedException();
	}
}