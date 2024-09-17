using System.Diagnostics;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day17 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		var length = input.Lines.Length;
		var width = input.Lines[0].Length;

		scoped Span<int> crucibleHeatLoss = stackalloc int[length * width];
		var crucibleHeatLossSize = 0;

		ParseMap(input, crucibleHeatLoss, ref crucibleHeatLossSize);

		return Part1_SearchLowestResistancePath(crucibleHeatLoss, length, width);
	}

	private static int Part1_SearchLowestResistancePath(Span<int> crucibleHeatLoss, int length, int width)
	{
		var priorityQueue = new PriorityQueue<Crucible, int>();

		priorityQueue.Enqueue(new Crucible(0, Direction.Right, 0), 0);
		priorityQueue.Enqueue(new Crucible(0, Direction.Down, 0), 0);

		var seenSet = new HashSet<Crucible>();
		while (priorityQueue.TryDequeue(out var crucible, out var heatLoss))
		{
			// Console.WriteLine("processing crucible at position {0} with heat loss {1}", crucible.position, heatLoss);
			if (crucible.position == crucibleHeatLoss.Length - 1)
			{
				return heatLoss;
			}

			foreach (var nextCrucible in Part1_ValidCrucibleMoves(crucible, length, width))
			{
				if (seenSet.Add(nextCrucible))
				{
					// Console.WriteLine("Moving {0:G} from position {1} to position {2} with heat loss {3}", nextCrucible.direction, crucible.position, nextCrucible.position, heatLoss + crucibleHeatLoss[nextCrucible.position]);
					priorityQueue.Enqueue(nextCrucible, heatLoss + crucibleHeatLoss[nextCrucible.position]);
				}
				// Console.WriteLine("Skipping {0:G} from position {1} to position {2} with heat loss {3}", nextCrucible.direction, crucible.position, nextCrucible.position, heatLoss + crucibleHeatLoss[nextCrucible.position]);
			}
		}

		throw new UnreachableException();
	}

	// ReSharper disable once CognitiveComplexity
	private static IEnumerable<Crucible> Part1_ValidCrucibleMoves(Crucible crucible, int length, int width)
	{
		var x = crucible.position % width;
		var y = crucible.position / width;

		foreach (var nextCrucibleDirection in Part1_GetMovesForCurrentCrucible(crucible))
		{
			switch (nextCrucibleDirection.Direction)
			{
				case Direction.Up when y > 0:
					yield return new Crucible(crucible.position - width, nextCrucibleDirection.Direction, nextCrucibleDirection.steps);
					break;
				case Direction.Right when x < width - 1:
					yield return new Crucible(crucible.position + 1, nextCrucibleDirection.Direction, nextCrucibleDirection.steps);
					break;
				case Direction.Down when y < length - 1:
					yield return new Crucible(crucible.position + width, nextCrucibleDirection.Direction, nextCrucibleDirection.steps);
					break;
				case Direction.Left when x > 0:
					yield return new Crucible(crucible.position - 1, nextCrucibleDirection.Direction, nextCrucibleDirection.steps);
					break;
			}
		}
	}

	private static IEnumerable<(Direction Direction, int steps)> Part1_GetMovesForCurrentCrucible(Crucible crucible)
	{
		if (crucible.stepsInSameDirection < 3)
		{
			yield return (crucible.direction, crucible.stepsInSameDirection + 1);
		}

		if (crucible.direction is Direction.Up or Direction.Down)
		{
			yield return (Direction.Left, 1);
			yield return (Direction.Right, 1);
		}
		else
		{
			yield return (Direction.Up, 1);
			yield return (Direction.Down, 1);
		}
	}

	public override object SolvePart2(Input input)
	{
		var length = input.Lines.Length;
		var width = input.Lines[0].Length;

		scoped Span<int> crucibleHeatLoss = stackalloc int[length * width];
		var crucibleHeatLossSize = 0;

		ParseMap(input, crucibleHeatLoss, ref crucibleHeatLossSize);

		return Part2_SearchLowestResistancePath(crucibleHeatLoss, length, width);
	}

	// ReSharper disable once CognitiveComplexity
	private static int Part2_SearchLowestResistancePath(Span<int> crucibleHeatLoss, int length, int width)
	{
		var priorityQueue = new PriorityQueue<Crucible, int>();

		priorityQueue.Enqueue(new Crucible(0, Direction.Right, 0), 0);
		priorityQueue.Enqueue(new Crucible(0, Direction.Down, 0), 0);

		var seenSet = new HashSet<Crucible>();
		while (priorityQueue.TryDequeue(out var crucible, out var heatLoss))
		{
			//Console.WriteLine("processing crucible at position {0} with heat loss {1}", crucible.position, heatLoss);
			if (crucible.position == crucibleHeatLoss.Length - 1 && crucible.stepsInSameDirection >= 4)
			{
				return heatLoss;
			}

			foreach (var nextCrucible in Part2_ValidCrucibleMoves(crucible, length, width))
			{
				if (seenSet.Add(nextCrucible))
				{
					//Console.WriteLine("Moving {0:G} from position {1} to position {2} with heat loss {3}", nextCrucible.direction, crucible.position, nextCrucible.position, heatLoss + crucibleHeatLoss[nextCrucible.position]);
					priorityQueue.Enqueue(nextCrucible, heatLoss + crucibleHeatLoss[nextCrucible.position]);
				}
			}
		}

		throw new UnreachableException();
	}

	// ReSharper disable once CognitiveComplexity
	private static IEnumerable<Crucible> Part2_ValidCrucibleMoves(Crucible crucible, int length, int width)
	{
		var x = crucible.position % width;
		var y = crucible.position / width;

		foreach (var nextCrucibleDirection in Part2_GetMovesForCurrentCrucible(crucible))
		{
			switch (nextCrucibleDirection.Direction)
			{
				case Direction.Up when y > 0:
					yield return new Crucible(crucible.position - width, nextCrucibleDirection.Direction, nextCrucibleDirection.steps);
					break;
				case Direction.Right when x < width - 1:
					yield return new Crucible(crucible.position + 1, nextCrucibleDirection.Direction, nextCrucibleDirection.steps);
					break;
				case Direction.Down when y < length - 1:
					yield return new Crucible(crucible.position + width, nextCrucibleDirection.Direction, nextCrucibleDirection.steps);
					break;
				case Direction.Left when x > 0:
					yield return new Crucible(crucible.position - 1, nextCrucibleDirection.Direction, nextCrucibleDirection.steps);
					break;
			}
		}
	}

	private static IEnumerable<(Direction Direction, int steps)> Part2_GetMovesForCurrentCrucible(Crucible crucible)
	{
		if (crucible.stepsInSameDirection < 10)
		{
			yield return (crucible.direction, crucible.stepsInSameDirection + 1);
		}

		if (crucible.stepsInSameDirection < 4)
		{
			yield break;
		}

		if (crucible.direction is Direction.Up or Direction.Down)
		{
			yield return (Direction.Left, 1);
			yield return (Direction.Right, 1);
		}
		else
		{
			yield return (Direction.Up, 1);
			yield return (Direction.Down, 1);
		}
	}

	private static void ParseMap(Input input, Span<int> crucibleHeatLoss, ref int crucibleHeatLossSize)
	{
		for (var y = 0; y < input.Lines.Length; y++)
		{
			var inputLineSpan = input.Lines[y].AsSpan();
			for (var x = 0; x < inputLineSpan.Length; x++)
			{
				crucibleHeatLoss[crucibleHeatLossSize++] = inputLineSpan[x] - '0';
			}
		}
	}

	private enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}

	private readonly record struct Crucible(int position, Direction direction, int stepsInSameDirection);
}