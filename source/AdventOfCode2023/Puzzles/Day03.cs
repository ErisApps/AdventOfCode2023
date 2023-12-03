using System.Runtime.CompilerServices;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day03 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		var total = 0;

		scoped Span<int> lookBehindSymbolsBuffer = stackalloc int[12];
		var lookBehindSymbolsBufferSize = 0;

		scoped Span<int> currentLineSymbolsBuffer = stackalloc int[12];
		var currentLineSymbolsBufferSize = 0;

		scoped Span<int> lookAheadSymbolsBuffer = stackalloc int[12];
		var lookAheadSymbolsBufferSize = 0;

		scoped Span<PartNumberDescriptor> partNumbersBuffer = stackalloc PartNumberDescriptor[16];
		var partNumbersBufferSize = 0;

		scoped Span<PartNumberDescriptor> partNumbersUpcomingBuffer = stackalloc PartNumberDescriptor[16];
		var partNumbersUpcomingBufferSize = 0;

		var inputLineSpan = input.Lines[0].AsSpan();
		Part1_ParseLine(ref inputLineSpan, currentLineSymbolsBuffer, out currentLineSymbolsBufferSize, partNumbersBuffer, out partNumbersBufferSize);

		for (var i = 1; i < input.Lines.Length; i++)
		{
			inputLineSpan = input.Lines[i].AsSpan();
			Part1_ParseLine(ref inputLineSpan, lookAheadSymbolsBuffer, out lookAheadSymbolsBufferSize, partNumbersUpcomingBuffer, out partNumbersUpcomingBufferSize);

			var slicedCurrentSymbolsBuffer = currentLineSymbolsBuffer.Slice(0, currentLineSymbolsBufferSize);
			var slicedLookAheadSymbolsBuffer = lookAheadSymbolsBuffer.Slice(0, lookAheadSymbolsBufferSize);

			ValidatePartNumbersAndSum(
				ref total,
				partNumbersBuffer.Slice(0, partNumbersBufferSize),
				slicedCurrentSymbolsBuffer,
				lookBehindSymbolsBuffer.Slice(0, lookBehindSymbolsBufferSize),
				slicedLookAheadSymbolsBuffer);

			partNumbersUpcomingBuffer.Slice(0, partNumbersUpcomingBufferSize).CopyTo(partNumbersBuffer);
			partNumbersBufferSize = partNumbersUpcomingBufferSize;

			slicedCurrentSymbolsBuffer.CopyTo(lookBehindSymbolsBuffer);
			lookBehindSymbolsBufferSize = currentLineSymbolsBufferSize;

			slicedLookAheadSymbolsBuffer.CopyTo(currentLineSymbolsBuffer);
			currentLineSymbolsBufferSize = lookAheadSymbolsBufferSize;
		}

		ValidatePartNumbersAndSum(
			ref total,
			partNumbersBuffer.Slice(0, partNumbersBufferSize),
			currentLineSymbolsBuffer.Slice(0, currentLineSymbolsBufferSize),
			lookBehindSymbolsBuffer.Slice(0, lookBehindSymbolsBufferSize),
			lookAheadSymbolsBuffer.Slice(0, 0)); // empty buffer

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static void Part1_ParseLine(
		ref ReadOnlySpan<char> line,
		scoped Span<int> lookAheadSymbolsBuffer,
		out int lookAheadSymbolsBufferSize,
		scoped Span<PartNumberDescriptor> partNumbersBuffer,
		out int partNumbersBufferSize)
	{
		lookAheadSymbolsBufferSize = 0;
		partNumbersBufferSize = 0;

		var lineIndex = 0;
		while (lineIndex < line.Length)
		{
			var c = line[lineIndex];
			// Check if ., if so, skip
			if (c == '.')
			{
				lineIndex++;
				continue;
			}

			// Check if not a digit, if so, this is a symbol and we can skip further processing
			if (!char.IsAsciiDigit(c))
			{
				lookAheadSymbolsBuffer[lookAheadSymbolsBufferSize++] = lineIndex;
				lineIndex++;
				continue;
			}

			// if we reached this part, it means that this is a number, so we need to parse it
			var partNumberStartIndex = lineIndex++;
			var partNumber = c - '0';

			for (; lineIndex < line.Length; lineIndex++)
			{
				c = line[lineIndex];
				if (!char.IsAsciiDigit(c))
				{
					break;
				}

				partNumber = partNumber * 10 + (c - '0');
			}

			// start and end index is off by 1 in both directions to reduce the amount of calculations needed later on
			partNumbersBuffer[partNumbersBufferSize++] = new PartNumberDescriptor(partNumber, partNumberStartIndex - 1, lineIndex);
		}
	}

	// ReSharper disable once CognitiveComplexity
	private static void ValidatePartNumbersAndSum(
		ref int total,
		Span<PartNumberDescriptor> partNumbersBuffer,
		Span<int> currentLineSymbolsBuffer,
		Span<int> lookBehindSymbolsBuffer,
		Span<int> lookAheadSymbolsBuffer)
	{
		foreach (var partNumberDescriptor in partNumbersBuffer)
		{
			ValidatePartNumberAndSum(ref total, partNumberDescriptor, currentLineSymbolsBuffer, lookBehindSymbolsBuffer, lookAheadSymbolsBuffer);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	// ReSharper disable once CognitiveComplexity
	private static void ValidatePartNumberAndSum(
		ref int total,
		PartNumberDescriptor partNumberDescriptor,
		Span<int> currentLineSymbolsBuffer,
		Span<int> lookBehindSymbolsBuffer,
		Span<int> lookAheadSymbolsBuffer)
	{
		var minStartingIndex = partNumberDescriptor.PartNumberStartIndex;
		var maxEndingIndex = partNumberDescriptor.PartNumberEndIndex;

		foreach (var symbolIndex in currentLineSymbolsBuffer)
		{
			if (symbolIndex >= minStartingIndex && symbolIndex <= maxEndingIndex)
			{
				total += partNumberDescriptor.PartNumber;
				return;
			}
		}

		foreach (var symbolIndex in lookBehindSymbolsBuffer)
		{
			if (symbolIndex >= minStartingIndex && symbolIndex <= maxEndingIndex)
			{
				total += partNumberDescriptor.PartNumber;
				return;
			}
		}

		foreach (var symbolIndex in lookAheadSymbolsBuffer)
		{
			if (symbolIndex >= minStartingIndex && symbolIndex <= maxEndingIndex)
			{
				total += partNumberDescriptor.PartNumber;
				return;
			}
		}
	}

	public override object SolvePart2(Input input)
	{
		var total = 0;

		scoped Span<int> symbolsBuffer = stackalloc int[12];
		var symbolsBufferSize = 0;

		scoped Span<int> symbolsUpcomingBuffer = stackalloc int[12];
		var symbolsUpcomingBufferSize = 0;

		scoped Span<PartNumberDescriptor> lookBehindPartNumbersBuffer = stackalloc PartNumberDescriptor[16];
		var lookBehindPartNumbersBufferSize = 0;

		scoped Span<PartNumberDescriptor> currentPartNumbersBuffer = stackalloc PartNumberDescriptor[16];
		var currentPartNumbersBufferSize = 0;

		scoped Span<PartNumberDescriptor> lookAheadPartNumbersBuffer = stackalloc PartNumberDescriptor[16];
		var lookAheadPartNumbersBufferSize = 0;

		var inputLineSpan = input.Lines[0].AsSpan();
		Part2_ParseLine(ref inputLineSpan, symbolsBuffer, out symbolsBufferSize, currentPartNumbersBuffer, out currentPartNumbersBufferSize);

		for (var i = 1; i < input.Lines.Length; i++)
		{
			inputLineSpan = input.Lines[i].AsSpan();
			Part2_ParseLine(ref inputLineSpan, symbolsUpcomingBuffer, out symbolsUpcomingBufferSize, lookAheadPartNumbersBuffer, out lookAheadPartNumbersBufferSize);

			var slicedCurrentPartNumbersBuffer = currentPartNumbersBuffer.Slice(0, currentPartNumbersBufferSize);
			var slicedLookAheadSymbolsBuffer = lookAheadPartNumbersBuffer.Slice(0, lookAheadPartNumbersBufferSize);

			ValidateGearsAndSum(
				ref total,
				symbolsBuffer.Slice(0, symbolsBufferSize),
				slicedCurrentPartNumbersBuffer,
				lookBehindPartNumbersBuffer.Slice(0, lookBehindPartNumbersBufferSize),
				slicedLookAheadSymbolsBuffer);

			symbolsUpcomingBuffer.Slice(0, symbolsUpcomingBufferSize).CopyTo(symbolsBuffer);
			symbolsBufferSize = symbolsUpcomingBufferSize;

			slicedCurrentPartNumbersBuffer.CopyTo(lookBehindPartNumbersBuffer);
			lookBehindPartNumbersBufferSize = currentPartNumbersBufferSize;

			slicedLookAheadSymbolsBuffer.CopyTo(currentPartNumbersBuffer);
			currentPartNumbersBufferSize = lookAheadPartNumbersBufferSize;
		}

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static void Part2_ParseLine(
		ref ReadOnlySpan<char> line,
		scoped Span<int> lookAheadSymbolsBuffer,
		out int lookAheadSymbolsBufferSize,
		scoped Span<PartNumberDescriptor> partNumbersBuffer,
		out int partNumbersBufferSize)
	{
		lookAheadSymbolsBufferSize = 0;
		partNumbersBufferSize = 0;

		var lineIndex = 0;
		while (lineIndex < line.Length)
		{
			var c = line[lineIndex];
			// Check if ., if so, skip
			switch (c)
			{
				case '.':
					lineIndex++;
					continue;
				case '*':
					lookAheadSymbolsBuffer[lookAheadSymbolsBufferSize++] = lineIndex;
					lineIndex++;
					continue;
			}

			// Check if not a digit, if so, this is a symbol and we can skip further processing
			if (!char.IsAsciiDigit(c))
			{
				lineIndex++;
				continue;
			}

			// if we reached this part, it means that this is a number, so we need to parse it
			var partNumberStartIndex = lineIndex++;
			var partNumber = c - '0';

			for (; lineIndex < line.Length; lineIndex++)
			{
				c = line[lineIndex];
				if (!char.IsAsciiDigit(c))
				{
					break;
				}

				partNumber = partNumber * 10 + (c - '0');
			}

			// start and end index is off by 1 in both directions to reduce the amount of calculations needed later on
			partNumbersBuffer[partNumbersBufferSize++] = new PartNumberDescriptor(partNumber, partNumberStartIndex - 1, lineIndex);
		}
	}

	private static void ValidateGearsAndSum(
		ref int total,
		scoped Span<int> symbolsBuffer,
		scoped Span<PartNumberDescriptor> partNumbersBuffer,
		scoped Span<PartNumberDescriptor> lookBehindPartNumbersBuffer,
		scoped Span<PartNumberDescriptor> lookAheadPartNumbersBuffer)
	{
		foreach (var gearIndex in symbolsBuffer)
		{
			var adjacentPartNumbersCount = 0;
			var gearRatio = 1;

			ValidateHorizontalAdjacentGearByPartNumberBuffer(
				ref adjacentPartNumbersCount,
				ref gearRatio,
				gearIndex,
				ref partNumbersBuffer);

			if (adjacentPartNumbersCount > 1)
			{
				total += gearRatio;
				continue;
			}

			ValidateGearByPartNumberBuffer(
				ref adjacentPartNumbersCount,
				ref gearRatio,
				gearIndex,
				ref lookBehindPartNumbersBuffer);

			if (adjacentPartNumbersCount > 1)
			{
				total += gearRatio;
				continue;
			}

			ValidateGearByPartNumberBuffer(
				ref adjacentPartNumbersCount,
				ref gearRatio,
				gearIndex,
				ref lookAheadPartNumbersBuffer);

			if (adjacentPartNumbersCount > 1)
			{
				total += gearRatio;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ValidateHorizontalAdjacentGearByPartNumberBuffer(
		ref int adjacentPartNumbersCount,
		ref int gearRatio,
		int gearIndex,
		scoped ref Span<PartNumberDescriptor> partNumberBuffer)
	{
		for (var i = 0; i < partNumberBuffer.Length; i++)
		{
			var partNumberDescriptor = partNumberBuffer[i];

			if (gearIndex == partNumberDescriptor.PartNumberEndIndex)
			{
				adjacentPartNumbersCount++;
				gearRatio *= partNumberDescriptor.PartNumber;
			}

			if (gearIndex == partNumberDescriptor.PartNumberStartIndex)
			{
				adjacentPartNumbersCount++;
				gearRatio *= partNumberDescriptor.PartNumber;

				partNumberBuffer = partNumberBuffer.Slice(i);

				break;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ValidateGearByPartNumberBuffer(
		ref int adjacentPartNumbersCount,
		ref int gearRatio,
		int gearIndex,
		scoped ref Span<PartNumberDescriptor> partNumberBuffer)
	{
		for (var i = 0; i < partNumberBuffer.Length; i++)
		{
			var partNumberDescriptor = partNumberBuffer[i];
			if (gearIndex < partNumberDescriptor.PartNumberStartIndex)
			{
				partNumberBuffer = partNumberBuffer.Slice(i);

				break;
			}

			if (gearIndex >= partNumberDescriptor.PartNumberStartIndex && gearIndex <= partNumberDescriptor.PartNumberEndIndex)
			{
				adjacentPartNumbersCount++;
				gearRatio *= partNumberDescriptor.PartNumber;
			}
		}
	}

	// Shared across both puzzle solutions

	private readonly record struct PartNumberDescriptor(int PartNumber, int PartNumberStartIndex, int PartNumberEndIndex)
	{
		public readonly int PartNumber = PartNumber;
		public readonly int PartNumberStartIndex = PartNumberStartIndex;
		public readonly int PartNumberEndIndex = PartNumberEndIndex;
	}
}