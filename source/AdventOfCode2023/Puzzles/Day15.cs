using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day15 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		var total = 0;

		var currentNumber = 0;
		for (var i = 0; i < input.Text.Length; i++)
		{
			if (input.Text[i] == ',')
			{
				total += currentNumber;
				currentNumber = 0;
				continue;
			}

			currentNumber += input.Text[i];
			currentNumber *= 17;
			currentNumber %= 256;
		}

		// Don't forget the last number
		total += currentNumber;

		return total;
	}

	public override object SolvePart2(Input input)
	{
		const int boxCount = 256;
		const int slotsPerBox = 7 + 1; // 7 lenses per box + 1 box header containing the current amount of lenses in the box

		// Encoding is a bit weird, but here it goes... last number describes the amount of lenses in a box and the preceding slotsPerBox - 1 entries are actual lenses or placeholders for lenses
		// A lens will be encoded in an ulong (original idea was an uint, but apparently... the number becomes to big to fit in 28 bits >.>)
		// The first 60 bits are reserved for the decimal value of the base26-encoded label, the subsequent (and last) 4 bits will be used to describe the current focal length.
		// We'll use some bitmasking shenanigans to compare and extract our actual values
		scoped Span<ulong> boxSlots = stackalloc ulong[boxCount * slotsPerBox];

		uint currentLabelBase26 = 0;
		var currentLabelHash = 0;
		for (var i = 0; i < input.Text.Length; i++)
		{
			var c = input.Text[i];
			if (c == '-')
			{
				// Remove lens from box
				RemoveLensFromBox(boxSlots.Slice(currentLabelHash * slotsPerBox, slotsPerBox), currentLabelBase26);

				// Reset for the next entry
				currentLabelBase26 = 0;
				currentLabelHash = 0;
				++i;
				continue;
			}

			if (c == '=')
			{
				// Update or add lens to box
				var focalLength = input.Text[++i] - '0';
				AddOrUpdateLensInBox(boxSlots.Slice(currentLabelHash * slotsPerBox, slotsPerBox), currentLabelBase26, (uint) focalLength);

				// Reset for the next entry
				currentLabelBase26 = 0;
				currentLabelHash = 0;
				++i;
				continue;
			}

			currentLabelBase26 *= 26;
			currentLabelBase26 += (uint) c - 'a';

			currentLabelHash += c;
			currentLabelHash *= 17;
			currentLabelHash %= boxCount;
		}

		var total = 0;
		// Parse final boxSet state
		for (var boxIndex = 0; boxIndex < boxCount; boxIndex++)
		{
			var box = boxSlots.Slice(boxIndex * slotsPerBox, slotsPerBox);
			var boxSize = (int) box[^1];
			if (boxSize == 0)
			{
				continue;
			}

			for (var lensIndex = 0; lensIndex < boxSize; lensIndex++)
			{
				// bitmask isn't needed as we're only interested in the last 4 bits and the first 60 bits will be voided when right-shifting
				// `& 0xF000000000000000` would have been the bitmask
				var focalLength = box[lensIndex] >> 60;
				total += ((boxIndex + 1) * (lensIndex + 1) * (int) focalLength);
			}
		}

		return total;
	}

	private static void RemoveLensFromBox(Span<ulong> boxSlots, ulong label)
	{
		ref var currentLensCount = ref boxSlots[^1];

		var i = 0;
		// Find the index of the lens in the box
		for (; i < (int) currentLensCount; i++)
		{
			// bitmask using the first 60 bits, as per the encoding described above
			if ((boxSlots[i] & 0x0FFFFFFFFFFFFFFF) == label)
			{
				// Remove lens from box
				--currentLensCount;
				break;
			}
		}

		// Move all subsequent lenses one slot to the left
		for (; i < (int) currentLensCount; i++)
		{
			boxSlots[i] = boxSlots[i + 1];
		}

		// Not needed, but clearer for debugging purposes
		boxSlots[(int) currentLensCount] = 0;
	}

	private static void AddOrUpdateLensInBox(Span<ulong> boxSlots, ulong label, ulong focalLength)
	{
		ref var currentLensCount = ref boxSlots[^1];

		// Shift focal length to the left by 60 bits so it's already in the right position
		focalLength <<= 60;

		var i = 0;
		// Find the index of the lens in the box
		for (; i < (int) currentLensCount; i++)
		{
			// bitmask using the first 60 bits, as per the encoding described above
			if ((boxSlots[i] & 0x0FFFFFFFFFFFFFFF) == label)
			{
				// Update focal length
				boxSlots[i] = label | focalLength;
				return;
			}
		}

		// Add lens to box
		boxSlots[(int) currentLensCount++] = label | focalLength;
	}
}