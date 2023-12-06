using System.Runtime.CompilerServices;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles
{
	public class Day06 : HappyPuzzleBase
	{
		public override object SolvePart1(Input input)
		{
			const int prefixLength = 11; // "Distance:  ".Length

			var timeLineSpan = input.Lines[0].AsSpan(prefixLength);
			scoped Span<int> timeBuffer = stackalloc int[6];
			Part1_ParseLine(ref timeLineSpan, timeBuffer, out var timeBufferSize);

			var distanceLineSpan = input.Lines[1].AsSpan(prefixLength);
			scoped Span<int> distanceBuffer = stackalloc int[6];
			Part1_ParseLine(ref distanceLineSpan, distanceBuffer, out _);

			var totalDistanceBetweenRoots = Part1_CalculateDistanceBetweenRoots(timeBuffer[0], distanceBuffer[0] + 1);
			for (var i = 1; i < timeBufferSize; i++)
			{
				totalDistanceBetweenRoots *= Part1_CalculateDistanceBetweenRoots(timeBuffer[i], distanceBuffer[i] + 1);
			}

			return totalDistanceBetweenRoots;
		}

		private static void Part1_ParseLine(scoped ref ReadOnlySpan<char> span, scoped Span<int> numbersBuffer, out int numbersBufferSize)
		{
			numbersBufferSize = 0;

			for (var i = 0; i < span.Length; i++)
			{
				var c = span[i];
				if (c == ' ')
				{
					continue;
				}

				var number = 0;
				for (; i < span.Length; i++)
				{
					c = span[i];
					if (c == ' ')
					{
						break;
					}

					number = number * 10 + c - '0';
				}

				numbersBuffer[numbersBufferSize++] = number;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Part1_CalculateDistanceBetweenRoots(int time, int distanceThreshold)
		{
			var discriminant = time * time - 4 * distanceThreshold;
			var upperBound = (-time - Math.Sqrt(discriminant)) / -2;
			var lowerBound = (-time + Math.Sqrt(discriminant)) / -2;

			return (int) upperBound - (int) Math.Ceiling(lowerBound) + 1;
		}

		public override object SolvePart2(Input input)
		{
			const int prefixLength = 11; // "Distance:  ".Length
			var raceDuration = Part2_ParseNumberIgnoringWhitespace(input.Lines[0].AsSpan(prefixLength));
			var raceDistance = Part2_ParseNumberIgnoringWhitespace(input.Lines[1].AsSpan(prefixLength));

			return Part2_CalculateDistanceBetweenRoots(raceDuration, raceDistance + 1);
		}

		private static long Part2_ParseNumberIgnoringWhitespace(ReadOnlySpan<char> span)
		{
			var number = 0L;
			for (var i = 0; i < span.Length; i++)
			{
				var c = span[i];
				if (c == ' ')
				{
					continue;
				}

				number = number * 10 + c - '0';
			}

			return number;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long Part2_CalculateDistanceBetweenRoots(long time, long distanceThreshold)
		{
			var discriminant = time * time - 4 * distanceThreshold;
			var upperBound = (-time - Math.Sqrt(discriminant)) / -2;
			var lowerBound = (-time + Math.Sqrt(discriminant)) / -2;

			return (long) upperBound - (long) Math.Ceiling(lowerBound) + 1;
		}
	}
}