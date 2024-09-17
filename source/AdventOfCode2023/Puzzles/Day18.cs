using System.Diagnostics;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day18 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		// We'll use coordinate system where 0,0 is the origin, the x-as becomes positive towards the right and the y-axis becomes positive towards the top

		// + 1 so we have the origin in both start and end
		scoped Span<Part1PolygonPoint> polygonPointBuffer = stackalloc Part1PolygonPoint[input.Lines.Length + 1];

		var previousPoint = polygonPointBuffer[0] = new Part1PolygonPoint(0, 0);

		// Offset by 1 so we can use the first element as a stack pointer
		var slicedPolygonBuffer = polygonPointBuffer.Slice(1);

		// Console.WriteLine("Starting at ({0}, {1})", previousPoint.X, previousPoint.Y);

		var perimeter = 0;

		for (var i = 0; i < input.Lines.Length; i++)
		{
			var inputLineSpan = input.Lines[i].AsSpan();

			var stepCount = inputLineSpan[2] - '0';
			if (inputLineSpan[3] != ' ')
			{
				stepCount = stepCount * 10 + (inputLineSpan[3] - '0');
			}

			perimeter += stepCount;

			previousPoint = inputLineSpan[0] switch
			{
				'U' => new Part1PolygonPoint(previousPoint.X, previousPoint.Y + stepCount),
				'R' => new Part1PolygonPoint(previousPoint.X + stepCount, previousPoint.Y),
				'D' => new Part1PolygonPoint(previousPoint.X, previousPoint.Y - stepCount),
				'L' => new Part1PolygonPoint(previousPoint.X - stepCount, previousPoint.Y),
				_ => throw new UnreachableException()
			};

			// Console.WriteLine("Moved {2} steps towards {3}, ending at ({0}, {1})", previousPoint.X, previousPoint.Y, stepCount, inputLineSpan[0]);

			slicedPolygonBuffer[i] = previousPoint;
		}

		return Part1_ApplyShoelaceFormula(polygonPointBuffer, perimeter);
	}

	private static int Part1_ApplyShoelaceFormula(ReadOnlySpan<Part1PolygonPoint> polygonPointBuffer, int perimeter)
	{
		var total = 0;

		for (var i = polygonPointBuffer.Length - 1; i >= 1; i--)
		{
			var currentPoint = polygonPointBuffer[i];
			var nextPoint = polygonPointBuffer[i - 1];

			total += currentPoint.X * nextPoint.Y - currentPoint.Y * nextPoint.X;
		}

		return (Math.Abs(total) + perimeter) / 2 + 1;
	}

	private readonly struct Part1PolygonPoint
	{
		public readonly int X;
		public readonly int Y;

		public Part1PolygonPoint(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	public override object SolvePart2(Input input)
	{
		// We'll use coordinate system where 0,0 is the origin, the x-as becomes positive towards the right and the y-axis becomes positive towards the top

		// + 1 so we have the origin in both start and end
		scoped Span<Part2PolygonPoint> polygonPointBuffer = stackalloc Part2PolygonPoint[input.Lines.Length + 1];

		var previousPoint = polygonPointBuffer[0] = new Part2PolygonPoint(0, 0);

		// Offset by 1 so we can use the first element as a stack pointer
		var slicedPolygonBuffer = polygonPointBuffer.Slice(1);

		// Console.WriteLine("Starting at ({0}, {1})", previousPoint.X, previousPoint.Y);

		var perimeter = 0L;

		for (var i = 0; i < input.Lines.Length; i++)
		{
			var inputLineSpan = input.Lines[i].AsSpan()[^7..^1];

			long stepCount = Part2_ParseHexDigit(inputLineSpan[0]) * 65536 + Part2_ParseHexDigit(inputLineSpan[1]) * 4096 + Part2_ParseHexDigit(inputLineSpan[2]) * 256 + Part2_ParseHexDigit(inputLineSpan[3]) * 16 + Part2_ParseHexDigit(inputLineSpan[4]);

			perimeter += stepCount;

			previousPoint = inputLineSpan[5] switch
			{
				'3' => new Part2PolygonPoint(previousPoint.X, previousPoint.Y + stepCount),
				'0' => new Part2PolygonPoint(previousPoint.X + stepCount, previousPoint.Y),
				'1' => new Part2PolygonPoint(previousPoint.X, previousPoint.Y - stepCount),
				'2' => new Part2PolygonPoint(previousPoint.X - stepCount, previousPoint.Y),
				_ => throw new UnreachableException()
			};

			// Console.WriteLine("Moved {2} steps towards {3}, ending at ({0}, {1})", previousPoint.X, previousPoint.Y, stepCount, inputLineSpan[0]);

			slicedPolygonBuffer[i] = previousPoint;
		}

		return Part2_ApplyShoelaceFormula(polygonPointBuffer, perimeter);
	}

	private static int Part2_ParseHexDigit(char rawHexValue)
	{
		return rawHexValue switch
		{
			'0' => 0,
			'1' => 1,
			'2' => 2,
			'3' => 3,
			'4' => 4,
			'5' => 5,
			'6' => 6,
			'7' => 7,
			'8' => 8,
			'9' => 9,
			'a' => 10,
			'b' => 11,
			'c' => 12,
			'd' => 13,
			'e' => 14,
			'f' => 15,
			_ => throw new UnreachableException()
		};
	}

	private static long Part2_ApplyShoelaceFormula(ReadOnlySpan<Part2PolygonPoint> polygonPointBuffer, long perimeter)
	{
		var total = 0L;

		for (var i = polygonPointBuffer.Length - 1; i >= 1; i--)
		{
			var currentPoint = polygonPointBuffer[i];
			var nextPoint = polygonPointBuffer[i - 1];

			total += currentPoint.X * nextPoint.Y - currentPoint.Y * nextPoint.X;
		}

		return (Math.Abs(total) + perimeter) / 2 + 1;
	}

	private readonly struct Part2PolygonPoint
	{
		public readonly long X;
		public readonly long Y;

		public Part2PolygonPoint(long x, long y)
		{
			X = x;
			Y = y;
		}
	}
}