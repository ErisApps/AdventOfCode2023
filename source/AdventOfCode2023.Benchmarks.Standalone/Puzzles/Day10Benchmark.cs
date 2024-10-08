using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace AdventOfCode2023.Benchmarks.Standalone.Puzzles;

[MemoryDiagnoser(true)]
[CategoriesColumn, AllStatisticsColumn, BaselineColumn, MinColumn, Q1Column, MeanColumn, Q3Column, MaxColumn, MedianColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Day10Benchmark
{
	private readonly Input _input;

	public Day10Benchmark()
	{
		_input = Helpers.GetInput("Day10.txt");
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART1)]
	public int Part1()
	{
		var mapData = _input.Text.AsSpan();

		var sideLength = _input.Lines[0].Length;

		FindStartingPosition(ref mapData, sideLength, out var startingNodeIndex, out var startingDirection);

		return Part1_FindLoopLength(ref mapData, sideLength, startingNodeIndex, startingDirection) / 2;
	}

	// ReSharper disable once CognitiveComplexity
	// ReSharper disable once CyclomaticComplexity
	private static int Part1_FindLoopLength(ref ReadOnlySpan<char> mapData, int sideLength, int startingNodeIndex, Direction startingDirection)
	{
		var rawSideLength = sideLength + 1;

		var currentNodeIndex = startingNodeIndex;
		var currentOutgoingDirection = startingDirection;

		for (var currentStepCount = 1;; currentStepCount++)
		{
			switch (currentOutgoingDirection)
			{
				// Check top
				case Direction.Up:
					currentNodeIndex -= rawSideLength;
					var topValue = mapData[currentNodeIndex];

					if (topValue == 'S')
					{
						return currentStepCount;
					}

					currentOutgoingDirection = topValue switch
					{
						'|' => Direction.Up,
						'7' => Direction.Left,
						'F' => Direction.Right,
						_ => throw new UnreachableException()
					};

					break;
				// Check bottom
				case Direction.Down:
					currentNodeIndex += rawSideLength;
					var bottomValue = mapData[currentNodeIndex];

					if (bottomValue == 'S')
					{
						return currentStepCount;
					}

					currentOutgoingDirection = bottomValue switch
					{
						'|' => Direction.Down,
						'J' => Direction.Left,
						'L' => Direction.Right,
						_ => throw new UnreachableException()
					};

					break;
				case Direction.Left:
					--currentNodeIndex;
					var leftValue = mapData[currentNodeIndex];

					if (leftValue == 'S')
					{
						return currentStepCount;
					}

					currentOutgoingDirection = leftValue switch
					{
						'-' => Direction.Left,
						'F' => Direction.Down,
						'L' => Direction.Up,
						_ => throw new UnreachableException()
					};

					break;
				case Direction.Right:
					++currentNodeIndex;
					var rightValue = mapData[currentNodeIndex];

					if (rightValue == 'S')
					{
						return currentStepCount;
					}

					currentOutgoingDirection = rightValue switch
					{
						'-' => Direction.Right,
						'7' => Direction.Down,
						'J' => Direction.Up,
						_ => throw new UnreachableException()
					};

					break;
				default:
					throw new UnreachableException();
			}
		}
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART2)]
	public int Part2()
	{
		var mapData = _input.Text.AsSpan();

		var length = _input.Lines.Length;
		var width = _input.Lines[0].Length;

		FindStartingPosition(ref mapData, width, out var startingNodeIndex, out var startingDirection);

		var rawWidth = width + 1;
		scoped Span<Direction> verticalityBuffer = stackalloc Direction[length * rawWidth];
		Part2_CalculateVerticalityBuffer(mapData, rawWidth, startingNodeIndex, startingDirection, ref verticalityBuffer);

		return Part2_CountEnclosedTiles(ref verticalityBuffer, rawWidth);
	}

	// ReSharper disable once CognitiveComplexity
	// ReSharper disable once CyclomaticComplexity
	private static void Part2_CalculateVerticalityBuffer(
		scoped ReadOnlySpan<char> mapData,
		int rawSideLength,
		int startingNodeIndex,
		Direction startingDirection,
		scoped ref Span<Direction> verticalityBuffer)
	{
		var currentNodeIndex = startingNodeIndex;
		var currentOutgoingDirection = startingDirection;

		verticalityBuffer[currentNodeIndex] = startingDirection;

		while (true)
		{
			switch (currentOutgoingDirection)
			{
				// Check top
				case Direction.Up:
					var topIndex = currentNodeIndex - rawSideLength;
					var topValue = mapData[topIndex];

					if (topValue == 'S')
					{
						return;
					}

					currentOutgoingDirection = topValue switch
					{
						'|' => Direction.Up,
						'7' => Direction.Left,
						'F' => Direction.Right,
						_ => throw new UnreachableException()
					};

					verticalityBuffer[topIndex] = Direction.Up;
					currentNodeIndex = topIndex;
					break;
				// Check bottom
				case Direction.Down:
					var bottomIndex = currentNodeIndex + rawSideLength;
					var bottomValue = mapData[bottomIndex];

					if (bottomValue == 'S')
					{
						return;
					}

					currentOutgoingDirection = bottomValue switch
					{
						'|' => Direction.Down,
						'J' => Direction.Left,
						'L' => Direction.Right,
						_ => throw new UnreachableException()
					};

					verticalityBuffer[bottomIndex] = Direction.Down;
					currentNodeIndex = bottomIndex;
					break;
				case Direction.Left:
					var leftIndex = currentNodeIndex - 1;
					var leftValue = mapData[leftIndex];

					if (leftValue == 'S')
					{
						return;
					}

					currentOutgoingDirection = leftValue switch
					{
						'-' => Direction.Left,
						'F' => Direction.Down,
						'L' => Direction.Up,
						_ => throw new UnreachableException()
					};

					verticalityBuffer[leftIndex] = currentOutgoingDirection == Direction.Up || currentOutgoingDirection == Direction.Down
						? currentOutgoingDirection
						: verticalityBuffer[currentNodeIndex];
					currentNodeIndex = leftIndex;
					break;
				case Direction.Right:
					var rightIndex = currentNodeIndex + 1;
					var rightValue = mapData[rightIndex];

					if (rightValue == 'S')
					{
						return;
					}

					currentOutgoingDirection = rightValue switch
					{
						'-' => Direction.Right,
						'7' => Direction.Down,
						'J' => Direction.Up,
						_ => throw new UnreachableException()
					};

					verticalityBuffer[rightIndex] = currentOutgoingDirection == Direction.Up || currentOutgoingDirection == Direction.Down
						? currentOutgoingDirection
						: verticalityBuffer[currentNodeIndex];
					currentNodeIndex = rightIndex;
					break;
				default:
					throw new UnreachableException();
			}
		}
	}

	// ReSharper disable once CognitiveComplexity
	private static int Part2_CountEnclosedTiles(scoped ref Span<Direction> verticalityBuffer, int rawSideLength)
	{
		var enclosedTiles = 0;

		var currentDirection = Direction.Undefined;
		var directionChangeCount = 0;
		for (var i = 0; i < verticalityBuffer.Length - rawSideLength; i++)
		{
			if (i % rawSideLength == 0)
			{
				currentDirection = Direction.Undefined;
				directionChangeCount = 0;
			}

			var newDirection = verticalityBuffer[i];
			if (newDirection == Direction.Undefined && directionChangeCount % 2 == 1)
			{
				enclosedTiles++;
			}
			else if ((newDirection == Direction.Up || newDirection == Direction.Down) && newDirection != currentDirection)
			{
				currentDirection = newDirection;
				directionChangeCount++;
			}
		}

		return enclosedTiles;
	}

	// ReSharper disable once CognitiveComplexity
	private static void FindStartingPosition(ref ReadOnlySpan<char> mapData, int sideLength, out int startingNodeIndex, out Direction startingDirection)
	{
		startingNodeIndex = mapData.IndexOf('S');

		var rawSideLength = sideLength + 1;

		var terrainRow = startingNodeIndex / rawSideLength;
		var terrainColumn = startingNodeIndex % rawSideLength;

		if (terrainRow > 0)
		{
			var topIndex = startingNodeIndex - rawSideLength;
			var topValue = mapData[topIndex];

			if (topValue is '|' or '7' or 'F')
			{
				startingDirection = Direction.Up;
				return;
			}
		}

		if (terrainRow < sideLength - 1)
		{
			var bottomIndex = startingNodeIndex + rawSideLength;
			var bottomValue = mapData[bottomIndex];

			if (bottomValue is '|' or 'J' or 'L')
			{
				startingDirection = Direction.Down;
				return;
			}
		}

		if (terrainColumn > 0)
		{
			var leftIndex = startingNodeIndex - 1;
			var leftValue = mapData[leftIndex];

			if (leftValue is '-' or 'F' or 'L')
			{
				startingDirection = Direction.Left;
				return;
			}
		}

		if (terrainColumn < sideLength - 1)
		{
			var rightIndex = startingNodeIndex + 1;
			var rightValue = mapData[rightIndex];

			if (rightValue is '-' or '7' or 'J')
			{
				startingDirection = Direction.Right;
				return;
			}
		}

		throw new UnreachableException();
	}

	private enum Direction
	{
		Undefined, // Used for non-visited nodes
		Up,
		Down,
		Left,
		Right
	}
}