using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day11 : HappyPuzzleBase
{
	// ReSharper disable once CognitiveComplexity
	public override object SolvePart1(Input input)
	{
		var width = input.Lines[0].Length + 1;

		scoped Span<int> columnContainsGalaxyData = stackalloc int[width];

		scoped Span<Part1GalaxyInfo> galaxyInfoData = stackalloc Part1GalaxyInfo[500];
		var galaxyInfoDataSize = 0;

		// Read the input and store the galaxy data in galaxyInfo objects
		// yOffset is applied on-the-fly while hinting data for xOffset is also collected
		var yOffset = 0;
		for (var y = 0; y < input.Lines.Length; y++)
		{
			var inputLineSpan = input.Lines[y].AsSpan();
			var foundGalaxyInRow = false;
			for (var x = 0; x < inputLineSpan.Length; x++)
			{
				var inputChar = inputLineSpan[x];
				if (inputChar == '#')
				{
					foundGalaxyInRow = true;
					columnContainsGalaxyData[x] = 1;
					galaxyInfoData[galaxyInfoDataSize++] = new Part1GalaxyInfo(x, y + yOffset);
				}
			}

			if (!foundGalaxyInRow)
			{
				yOffset++;
			}
		}

		// Calculate xOffset for each x value of the grid
		var xOffset = 0;
		for (var i = 0; i < columnContainsGalaxyData.Length; i++)
		{
			ref var columnContainsGalaxy = ref columnContainsGalaxyData[i];
			if (columnContainsGalaxy == 0)
			{
				xOffset++;
			}

			columnContainsGalaxy = xOffset;
		}

		// Slice the galaxyInfoData to only contain the actual data
		galaxyInfoData = galaxyInfoData.Slice(0, galaxyInfoDataSize);

		// Apply the xOffset to the galaxyInfoData
		foreach (ref var galaxyInfo in galaxyInfoData)
		{
			galaxyInfo = new Part1GalaxyInfo(galaxyInfo.X + columnContainsGalaxyData[galaxyInfo.X], galaxyInfo.Y);
		}

		return Part1_CalculateAndSumDistance(galaxyInfoData);
	}

	private static int Part1_CalculateAndSumDistance(scoped Span<Part1GalaxyInfo> galaxyData)
	{
		var distanceSum = 0;

		for (var i = 0; i < galaxyData.Length - 1; i++)
		{
			var referenceGalaxy = galaxyData[i];

			for (var j = i + 1; j < galaxyData.Length; j++)
			{
				var galaxy2 = galaxyData[j];

				distanceSum += Math.Abs(galaxy2.X - referenceGalaxy.X) + Math.Abs(galaxy2.Y - referenceGalaxy.Y);
			}
		}

		return distanceSum;
	}

	private readonly struct Part1GalaxyInfo
	{
		public readonly int X;
		public readonly int Y;

		public Part1GalaxyInfo(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	// ReSharper disable once CognitiveComplexity
	public override object SolvePart2(Input input)
	{
		var width = input.Lines[0].Length;

		scoped Span<long> columnContainsGalaxyData = stackalloc long[width];

		scoped Span<Part2GalaxyInfo> galaxyInfoData = stackalloc Part2GalaxyInfo[500];
		var galaxyInfoDataSize = 0;

		// Read the input and store the galaxy data in galaxyInfo objects
		// yOffset is applied on-the-fly while hinting data for xOffset is also collected
		var yOffset = 0L;
		for (var y = 0; y < input.Lines.Length; y++)
		{
			var inputLineSpan = input.Lines[y].AsSpan();
			var foundGalaxyInRow = false;
			for (var x = 0; x < inputLineSpan.Length; x++)
			{
				var inputChar = inputLineSpan[x];
				if (inputChar == '#')
				{
					foundGalaxyInRow = true;
					columnContainsGalaxyData[x] = 1;
					galaxyInfoData[galaxyInfoDataSize++] = new Part2GalaxyInfo(x, y + yOffset);
				}
			}

			if (!foundGalaxyInRow)
			{
				yOffset += 999_999;
			}
		}

		// Calculate xOffset for each x value of the grid
		var xOffset = 0L;
		for (var i = 0; i < columnContainsGalaxyData.Length; i++)
		{
			ref var columnContainsGalaxy = ref columnContainsGalaxyData[i];
			if (columnContainsGalaxy == 0)
			{
				xOffset += 999_999;
			}

			columnContainsGalaxy = xOffset;
		}

		// Slice the galaxyInfoData to only contain the actual data
		galaxyInfoData = galaxyInfoData.Slice(0, galaxyInfoDataSize);

		// Apply the xOffset to the galaxyInfoData
		foreach (ref var galaxyInfo in galaxyInfoData)
		{
			galaxyInfo = new Part2GalaxyInfo(galaxyInfo.X + columnContainsGalaxyData[(int) galaxyInfo.X], galaxyInfo.Y);
		}

		return Part2_CalculateAndSumDistance(galaxyInfoData);
	}

	private static long Part2_CalculateAndSumDistance(scoped Span<Part2GalaxyInfo> galaxyData)
	{
		var distanceSum = 0L;

		for (var i = 0; i < galaxyData.Length - 1; i++)
		{
			var referenceGalaxy = galaxyData[i];

			for (var j = i + 1; j < galaxyData.Length; j++)
			{
				var galaxy2 = galaxyData[j];

				distanceSum += Math.Abs(galaxy2.X - referenceGalaxy.X) + Math.Abs(galaxy2.Y - referenceGalaxy.Y);
			}
		}

		return distanceSum;
	}

	private readonly struct Part2GalaxyInfo
	{
		public readonly long X;
		public readonly long Y;

		public Part2GalaxyInfo(long x, long y)
		{
			X = x;
			Y = y;
		}
	}
}