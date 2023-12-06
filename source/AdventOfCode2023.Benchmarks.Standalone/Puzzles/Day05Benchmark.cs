using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace AdventOfCode2023.Benchmarks.Standalone.Puzzles;

[MemoryDiagnoser(true)]
[CategoriesColumn, AllStatisticsColumn, BaselineColumn, MinColumn, Q1Column, MeanColumn, Q3Column, MaxColumn, MedianColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Day05Benchmark
{
	private readonly Input _input;

	public Day05Benchmark()
	{
		_input = Helpers.GetInput("Day05.txt");
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART1)]
	public long Part1()
	{
		var inputLines = _input.Lines.AsSpan();
		var inputLinesIndex = 0;

		var inputLineSpan = inputLines[inputLinesIndex].AsSpan();
		inputLinesIndex += 3;

		scoped Span<long> seedsBuffer = stackalloc long[40];
		Part1_ParseSeeds(ref inputLineSpan, seedsBuffer, out var seedsBufferSize);
		var slicedSeedsBuffer = seedsBuffer.Slice(0, seedsBufferSize);

		scoped Span<TransformationLayerNode> transformationLayersBuffer = stackalloc TransformationLayerNode[50];

		for (; inputLinesIndex < inputLines.Length; inputLinesIndex += 2)
		{
			Part1_ParseTransformationLayers(inputLines, ref inputLinesIndex, transformationLayersBuffer, out var transformationLayersBufferSize);
			Part2_ApplyTransformationLayersToSeeds(slicedSeedsBuffer, transformationLayersBuffer.Slice(0, transformationLayersBufferSize));
		}

		var lowestLocation = long.MaxValue;
		for (var i = 0; i < slicedSeedsBuffer.Length; i++)
		{
			if (slicedSeedsBuffer[i] < lowestLocation)
			{
				lowestLocation = slicedSeedsBuffer[i];
			}
		}

		return lowestLocation;
	}

	private static void Part1_ParseSeeds(ref ReadOnlySpan<char> inputLine, scoped Span<long> seedBuffer, out int seedBufferSize)
	{
		seedBufferSize = 0;

		var lineCharIndex = "seeds: ".Length;
		do
		{
			var c = inputLine[lineCharIndex++];
			long currentNumber = c - '0';

			for (; lineCharIndex < inputLine.Length; lineCharIndex++)
			{
				c = inputLine[lineCharIndex];
				if (!char.IsAsciiDigit(c))
				{
					break;
				}

				currentNumber = currentNumber * 10 + (c - '0');
			}

			seedBuffer[seedBufferSize++] = currentNumber;
			lineCharIndex++;
		} while (lineCharIndex < inputLine.Length);
	}

	// ReSharper disable once CognitiveComplexity
	private static void Part1_ParseTransformationLayers(
		scoped Span<string> inputLines,
		ref int inputLinesIndex,
		scoped Span<TransformationLayerNode> transformationLayersBuffer,
		out int transformationLayersBufferSize)
	{
		transformationLayersBufferSize = 0;

		Span<long> temporalNumberBuffer = stackalloc long[3];

		do
		{
			var temporalNumberBufferSize = 0;

			var inputLineSpan = inputLines[inputLinesIndex].AsSpan();
			if (inputLineSpan.IsEmpty)
			{
				return;
			}

			var lineCharIndex = 0;
			do
			{
				var c = inputLineSpan[lineCharIndex++];
				long currentNumber = c - '0';

				for (; lineCharIndex < inputLineSpan.Length; lineCharIndex++)
				{
					c = inputLineSpan[lineCharIndex];
					if (!char.IsAsciiDigit(c))
					{
						break;
					}

					currentNumber = currentNumber * 10 + (c - '0');
				}

				temporalNumberBuffer[temporalNumberBufferSize++] = currentNumber;
				lineCharIndex++;
			} while (lineCharIndex < inputLineSpan.Length);

			transformationLayersBuffer[transformationLayersBufferSize++] = TransformationLayerNode.CreateFromRaw(temporalNumberBuffer[0], temporalNumberBuffer[1], temporalNumberBuffer[2]);

			inputLinesIndex++;
		} while (inputLinesIndex < inputLines.Length);
	}

	private static void Part2_ApplyTransformationLayersToSeeds(scoped Span<long> seedsBuffer, scoped Span<TransformationLayerNode> transformationLayerBuffer)
	{
		foreach (scoped ref var seed in seedsBuffer)
		{
			foreach (var transformationLayerNode in transformationLayerBuffer)
			{
				if (seed >= transformationLayerNode.SourceRangeStart && seed < transformationLayerNode.SourceRangeEnd)
				{
					seed += transformationLayerNode.Offset;
					break;
				}
			}
		}
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART2)]
	public long Part2()
	{
		var inputLines = _input.Lines.AsSpan();
		var inputLinesIndex = 0;

		var inputLineSpan = inputLines[inputLinesIndex].AsSpan();
		inputLinesIndex += 3;

		scoped Span<SeedRange> seedRangesBuffer = stackalloc SeedRange[200];
		Part2_ParseSeedRanges(ref inputLineSpan, seedRangesBuffer, out var seedRangesBufferSize);

		scoped Span<TransformationLayerNode> transformationLayersBuffer = stackalloc TransformationLayerNode[50];

		for (; inputLinesIndex < inputLines.Length; inputLinesIndex += 2)
		{
			Part2_ParseTransformationLayers(inputLines, ref inputLinesIndex, transformationLayersBuffer, out var transformationLayersBufferSize);
			Part2_ApplyTransformationLayersToSeedRanges(seedRangesBuffer, ref seedRangesBufferSize, transformationLayersBuffer.Slice(0, transformationLayersBufferSize));
		}

		var lowestLocation = long.MaxValue;
		for (var i = 0; i < seedRangesBufferSize; i++)
		{
			if (seedRangesBuffer[i].StartInclusive < lowestLocation)
			{
				lowestLocation = seedRangesBuffer[i].StartInclusive;
			}
		}

		return lowestLocation;
	}


	// ReSharper disable once CognitiveComplexity
	private static void Part2_ParseSeedRanges(
		ref ReadOnlySpan<char> inputLine,
		scoped Span<SeedRange> seedRangeBuffer,
		out int seedRangesBufferSize)
	{
		seedRangesBufferSize = 0;

		var lineCharIndex = "seeds: ".Length;
		do
		{
			var seedRangeStart = ParseLong(ref inputLine, ref lineCharIndex);
			lineCharIndex++;
			var seedRangeLength = ParseLong(ref inputLine, ref lineCharIndex);
			lineCharIndex++;

			// Sorted insert, probably slow here but it's not a big deal
			var seedRangeEntry = SeedRange.CreateFromRaw(seedRangeStart, seedRangeLength);

			var seedRangesBufferIndex = seedRangesBufferSize - 1;
			while (seedRangesBufferIndex >= 0)
			{
				ref var currentIndexSeedRangeEntry = ref seedRangeBuffer[seedRangesBufferIndex];
				if (currentIndexSeedRangeEntry.StartInclusive < seedRangeEntry.StartInclusive)
				{
					break;
				}

				seedRangeBuffer[seedRangesBufferIndex + 1] = currentIndexSeedRangeEntry;
				seedRangesBufferIndex--;
			}

			seedRangeBuffer[seedRangesBufferIndex + 1] = seedRangeEntry;
			++seedRangesBufferSize;
		} while (lineCharIndex < inputLine.Length);

		return;

		static long ParseLong(ref ReadOnlySpan<char> inputLine, ref int lineCharIndex)
		{
			var c = inputLine[lineCharIndex++];
			long currentNumber = c - '0';

			for (; lineCharIndex < inputLine.Length; lineCharIndex++)
			{
				c = inputLine[lineCharIndex];
				if (!char.IsAsciiDigit(c))
				{
					break;
				}

				currentNumber = currentNumber * 10 + (c - '0');
			}

			return currentNumber;
		}
	}

	// ReSharper disable once CognitiveComplexity
	private static void Part2_ParseTransformationLayers(
		scoped Span<string> inputLines,
		ref int inputLinesIndex,
		scoped Span<TransformationLayerNode> transformationLayersBuffer,
		out int transformationLayersBufferSize)
	{
		transformationLayersBufferSize = 0;

		Span<long> temporalNumberBuffer = stackalloc long[3];

		do
		{
			var temporalNumberBufferSize = 0;

			var inputLineSpan = inputLines[inputLinesIndex].AsSpan();
			if (inputLineSpan.IsEmpty)
			{
				return;
			}

			var lineCharIndex = 0;
			do
			{
				var c = inputLineSpan[lineCharIndex++];
				long currentNumber = c - '0';

				for (; lineCharIndex < inputLineSpan.Length; lineCharIndex++)
				{
					c = inputLineSpan[lineCharIndex];
					if (!char.IsAsciiDigit(c))
					{
						break;
					}

					currentNumber = currentNumber * 10 + (c - '0');
				}

				temporalNumberBuffer[temporalNumberBufferSize++] = currentNumber;
				lineCharIndex++;
			} while (lineCharIndex < inputLineSpan.Length);

			// Sorted insertion from here on out, will make it easier to rewrite the buffer later on
			var transformationLayerNode = TransformationLayerNode.CreateFromRaw(temporalNumberBuffer[0], temporalNumberBuffer[1], temporalNumberBuffer[2]);

			var transformationLayersBufferIndex = transformationLayersBufferSize - 1;
			while (transformationLayersBufferIndex >= 0)
			{
				ref var currentIndexTransformationLayerNode = ref transformationLayersBuffer[transformationLayersBufferIndex];
				if (currentIndexTransformationLayerNode.SourceRangeStart <= transformationLayerNode.SourceRangeStart)
				{
					break;
				}

				transformationLayersBuffer[transformationLayersBufferIndex + 1] = currentIndexTransformationLayerNode;
				transformationLayersBufferIndex--;
			}

			transformationLayersBuffer[transformationLayersBufferIndex + 1] = transformationLayerNode;
			++transformationLayersBufferSize;

			inputLinesIndex++;
		} while (inputLinesIndex < inputLines.Length);
	}

	private static void Part2_ApplyTransformationLayersToSeedRanges(
		scoped Span<SeedRange> seedRangesBuffer,
		ref int seedRangesBufferSize,
		scoped Span<TransformationLayerNode> transformerLayersBuffer)
	{
		scoped Span<SeedRange> intermediateSeedRangesBuffer = stackalloc SeedRange[seedRangesBuffer.Length];
		var intermediateSeedRangesBufferSize = 0;

		for (var i = 0; i < seedRangesBufferSize; i++)
		{
			var seedRange = seedRangesBuffer[i];

			Part2_ApplyTransformationLayerToSeedRange(seedRange, transformerLayersBuffer, intermediateSeedRangesBuffer, ref intermediateSeedRangesBufferSize);
		}

		var intermediateSeedsBufferSlice = intermediateSeedRangesBuffer.Slice(0, intermediateSeedRangesBufferSize);
		intermediateSeedsBufferSlice.CopyTo(seedRangesBuffer);

		seedRangesBufferSize = intermediateSeedRangesBufferSize;
	}

	// ReSharper disable once CognitiveComplexity
	private static void Part2_ApplyTransformationLayerToSeedRange(
		SeedRange seedRange,
		scoped Span<TransformationLayerNode> transformationLayersBuffer,
		scoped Span<SeedRange> targetSeedRangeBuffer,
		ref int targetSeedRangeBufferSize)
	{
		for (var j = 0; j < transformationLayersBuffer.Length; j++)
		{
			var transformationLayerNode = transformationLayersBuffer[j];

			// Case 1: No overlap
			// Skip and check next transformation layer node
			if (!(seedRange.StartInclusive < transformationLayerNode.SourceRangeEnd && transformationLayerNode.SourceRangeStart < seedRange.EndExclusive))
			{
				continue;
			}

			// Case 2: Overlap, but seed range is fully contained in transformation layer node range
			// 1-on-1 mapping
			if (transformationLayerNode.SourceRangeStart <= seedRange.StartInclusive && seedRange.EndExclusive <= transformationLayerNode.SourceRangeEnd)
			{
				targetSeedRangeBuffer[targetSeedRangeBufferSize++] = ShiftRange(seedRange, transformationLayerNode.Offset);
				return;
			}

			// Case 3: Overlap, but transformation layer node range is fully contained in seed range
			// Split seed range and re-apply transformation on new subrange
			if (seedRange.StartInclusive <= transformationLayerNode.SourceRangeStart && transformationLayerNode.SourceRangeEnd <= seedRange.EndExclusive)
			{
				targetSeedRangeBuffer[targetSeedRangeBufferSize++] = new SeedRange(seedRange.StartInclusive, transformationLayerNode.SourceRangeStart);
				targetSeedRangeBuffer[targetSeedRangeBufferSize++] = ShiftRange(new SeedRange(transformationLayerNode.SourceRangeStart, transformationLayerNode.SourceRangeEnd), transformationLayerNode.Offset);
				Part2_ApplyTransformationLayerToSeedRange(
					new SeedRange(transformationLayerNode.SourceRangeEnd, seedRange.EndExclusive),
					transformationLayersBuffer,
					targetSeedRangeBuffer,
					ref targetSeedRangeBufferSize);
				return;
			}

			// Case 4: Partial overlap, but seed range starts in middle of transformation layer node range
			// Split seed range and re-apply transformation on new subrange
			if (transformationLayerNode.SourceRangeStart <= seedRange.StartInclusive && transformationLayerNode.SourceRangeEnd <= seedRange.EndExclusive)
			{
				targetSeedRangeBuffer[targetSeedRangeBufferSize++] = new SeedRange(seedRange.StartInclusive, transformationLayerNode.SourceRangeEnd).WithOffset(transformationLayerNode.Offset);
				Part2_ApplyTransformationLayerToSeedRange(
					new SeedRange(transformationLayerNode.SourceRangeEnd, seedRange.EndExclusive),
					transformationLayersBuffer,
					targetSeedRangeBuffer,
					ref targetSeedRangeBufferSize);
				return;
			}

			// Case 5: Partial overlap, but seed range ends in middle of transformation layer node range
			if (seedRange.StartInclusive <= transformationLayerNode.SourceRangeStart && seedRange.EndExclusive <= transformationLayerNode.SourceRangeEnd)
			{
				targetSeedRangeBuffer[targetSeedRangeBufferSize++] = new SeedRange(seedRange.StartInclusive, transformationLayerNode.SourceRangeStart);
				Part2_ApplyTransformationLayerToSeedRange(
					new SeedRange(transformationLayerNode.SourceRangeStart, seedRange.EndExclusive).WithOffset(transformationLayerNode.Offset),
					transformationLayersBuffer,
					targetSeedRangeBuffer,
					ref targetSeedRangeBufferSize);
				return;
			}
		}

		targetSeedRangeBuffer[targetSeedRangeBufferSize++] = seedRange;
		return;

		static SeedRange ShiftRange(SeedRange seedRange, long offset)
		{
			return new SeedRange(seedRange.StartInclusive + offset, seedRange.EndExclusive + offset);
		}
	}

	[DebuggerDisplay("SourceRangeStart = {SourceRangeStart}, SourceRangeEnd = {SourceRangeEnd}, Offset = {Offset}")]
	private readonly struct TransformationLayerNode
	{
		public readonly long SourceRangeStart;
		public readonly long SourceRangeEnd;
		public readonly long Offset;

		public TransformationLayerNode(long sourceRangeStart, long sourceRangeEnd, long offset)
		{
			SourceRangeStart = sourceRangeStart;
			SourceRangeEnd = sourceRangeEnd;
			Offset = offset;
		}

		public static TransformationLayerNode CreateFromRaw(long destinationRangeStart, long sourceRangeStart, long rangeLength)
		{
			return new(sourceRangeStart, sourceRangeStart + rangeLength, destinationRangeStart - sourceRangeStart);
		}
	}

	[DebuggerDisplay("StartInclusive = {StartInclusive}, EndExclusive = {EndExclusive}")]
	private readonly struct SeedRange(long startInclusive, long endExclusive)
	{
		public readonly long StartInclusive = startInclusive;
		public readonly long EndExclusive = endExclusive;

		public SeedRange WithOffset(long offset)
		{
			return new SeedRange(StartInclusive + offset, EndExclusive + offset);
		}

		public static SeedRange CreateFromRaw(long start, long amount)
		{
			return new SeedRange(start, start + amount);
		}
	}
}