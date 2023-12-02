using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day02 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		var total = 0;
		for (var i = 0; i < input.Lines.Length; i++)
		{
			var inputLineSpan = input.Lines[i].AsSpan();

			var lookupStartIndex = inputLineSpan.IndexOf(':') + 2; // offset by 2 due to whitespace following the colon
			inputLineSpan = inputLineSpan.Slice(lookupStartIndex);

			if (ValidateGame(inputLineSpan))
			{
				total += i + 1;
			}
		}

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static bool ValidateGame(ReadOnlySpan<char> span)
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
			while (char.IsDigit(c))
			{
				currentNumber = currentNumber * 10 + (c - '0');
				c = span[++i];
			}

			i++; // skip the whitespace

			switch (span[i])
			{
				case 'r':
					redCount += currentNumber;
					if (redCount > 12)
					{
						return false;
					}

					i += 3; // skip the 'red'
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

	public override object SolvePart2(Input input)
	{
		var total = 0;
		foreach (var inputLine in input.Lines)
		{
			var inputLineSpan = inputLine.AsSpan();

			var lookupStartIndex = inputLineSpan.IndexOf(':') + 2; // offset by 2 due to whitespace following the colon
			inputLineSpan = inputLineSpan.Slice(lookupStartIndex);

			total += GetGamePower(ref inputLineSpan);
		}

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static int GetGamePower(ref ReadOnlySpan<char> span)
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
			while (char.IsDigit(c))
			{
				currentNumber = currentNumber * 10 + (c - '0');
				c = span[++i];
			}

			i++; // skip the whitespace

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