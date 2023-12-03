using AdventOfCode2023.Puzzles;
using AdventOfCode2023.Tests.Common;

namespace AdventOfCode2023.Tests.Puzzles;

public class Day01Test : HappyPuzzleTestBase<Day01>
{
	protected override string Part1AssetName => "Day01_Part1.txt";
	protected override object Part1ExpectedResult => 142;

	protected override string Part2AssetName => "Day01_Part2.txt";
	protected override object Part2ExpectedResult => 281;
}