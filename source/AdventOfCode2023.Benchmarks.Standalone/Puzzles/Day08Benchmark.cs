using System.Diagnostics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace AdventOfCode2023.Benchmarks.Standalone.Puzzles;

[MemoryDiagnoser(true)]
[CategoriesColumn, AllStatisticsColumn, BaselineColumn, MinColumn, Q1Column, MeanColumn, Q3Column, MaxColumn, MedianColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Day08Benchmark
{
	private readonly Input _input;

	public Day08Benchmark()
	{
		_input = Helpers.GetInput("Day08.txt");
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART1)]
	public int Part1()
	{
		scoped Span<Part1Node> networkNodesBuffer = stackalloc Part1Node[26 * 26 * 26];

		for (var i = 2; i < _input.Lines.Length; i++)
		{
			var nodeSpan = _input.Lines[i].AsSpan();

			var idSpan = nodeSpan.Slice(0, 3);
			var id = ParseBase26(idSpan);

			var nextLeftIdSpan = nodeSpan.Slice(7, 3);
			var nextLeftId = ParseBase26(nextLeftIdSpan);

			var nextRightIdSpan = nodeSpan.Slice(12, 3);
			var nextRightId = ParseBase26(nextRightIdSpan);

			networkNodesBuffer[id] = new Part1Node(nextLeftId, nextRightId);
		}

		var instructionSet = _input.Lines[0].AsSpan();

		// 0 (base26-decoded version of AAA) is the id of the first node in the network
		// 17575 (base26-decoded version of ZZZ) is the id of the last node in the network
		return Part1_CalculateStepsTillTargetNode(ref networkNodesBuffer, instructionSet, 0, 17575);
	}

	private static int Part1_CalculateStepsTillTargetNode(ref Span<Part1Node> networkNodesBuffer, ReadOnlySpan<char> instructionSet, int startNodeId, int targetNodeId)
	{
		var stepCounter = 0;

		var currentNode = networkNodesBuffer[startNodeId];
		for (var i = 0; i < instructionSet.Length; i++)
		{
			++stepCounter;
			var nextNodeId = instructionSet[i] switch
			{
				'L' => currentNode.NextLeft,
				'R' => currentNode.NextRight,
				_ => throw new UnreachableException()
			};

			if (nextNodeId == targetNodeId)
			{
				return stepCounter;
			}

			currentNode = networkNodesBuffer[nextNodeId];

			// Wrap around, seemingly cheaper than wrapping the for-loop in a while(true) loop
			if (i == instructionSet.Length - 1)
			{
				i = -1;
			}
		}

		throw new UnreachableException();
	}

	private readonly struct Part1Node
	{
		public readonly int NextLeft;
		public readonly int NextRight;

		public Part1Node(int nextLeft, int nextRight)
		{
			NextLeft = nextLeft;
			NextRight = nextRight;
		}
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART2)]
	public long Part2()
	{
		scoped Span<int> routeStartDescriptorsBuffer = stackalloc int[10];
		var routeDescriptorsBufferSize = 0;

		scoped Span<Part2Node> networkNodesBuffer = stackalloc Part2Node[26 * 26 * 26];

		for (var i = 2; i < _input.Lines.Length; i++)
		{
			var nodeSpan = _input.Lines[i].AsSpan();

			var idSpan = nodeSpan.Slice(0, 3);
			var id = ParseBase26(idSpan);

			var nextLeftIdSpan = nodeSpan.Slice(7, 3);
			var nextLeftId = ParseBase26(nextLeftIdSpan);

			var nextRightIdSpan = nodeSpan.Slice(12, 3);
			var nextRightId = ParseBase26(nextRightIdSpan);

			var endingChar = idSpan[2];

			networkNodesBuffer[id] = new Part2Node(nextLeftId, nextRightId, endingChar == 'Z');

			if (endingChar == 'A')
			{
				routeStartDescriptorsBuffer[routeDescriptorsBufferSize++] = id;
			}
		}

		var instructionSet = _input.Lines[0].AsSpan();

		scoped Span<int> routeStepCountsBuffer = stackalloc int[routeDescriptorsBufferSize];
		for (var i = 0; i < routeDescriptorsBufferSize; i++)
		{
			var startNodeId = routeStartDescriptorsBuffer[i];

			routeStepCountsBuffer[i] = Part2_CalculateStepsTillTargetNodeTillZEndingNode(ref networkNodesBuffer, instructionSet, startNodeId);
		}

		return Part2_LeastCommonMultiple(ref routeStepCountsBuffer);
	}

	private static int Part2_CalculateStepsTillTargetNodeTillZEndingNode(scoped ref Span<Part2Node> networkNodesBuffer, scoped ReadOnlySpan<char> instructionSet, int startNodeId)
	{
		var stepCounter = 0;

		var currentNode = networkNodesBuffer[startNodeId];
		for (var i = 0; i < instructionSet.Length; i++)
		{
			var nextNodeId = instructionSet[i] switch
			{
				'L' => currentNode.NextLeft,
				'R' => currentNode.NextRight,
				_ => throw new UnreachableException()
			};

			currentNode = networkNodesBuffer[nextNodeId];

			if (currentNode.IsEndingNode)
			{
				stepCounter += i + 1;
				return stepCounter;
			}

			// Wrap around, seemingly cheaper than wrapping the for-loop in a while(true) loop
			if (i == instructionSet.Length - 1)
			{
				stepCounter += instructionSet.Length;
				i = -1;
			}
		}

		throw new UnreachableException();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static long Part2_GreatestCommonDivisor(long a, long b)
	{
		while (b != 0)
		{
			var t = b;
			b = a % b;
			a = t;
		}

		return a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static long Part2_LeastCommonMultiple(long a, long b)
	{
		return a * b / Part2_GreatestCommonDivisor(a, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static long Part2_LeastCommonMultiple(scoped ref Span<int> numbers)
	{
		long leastCommonMultiple = numbers[0];
		for (var i = 0; i < numbers.Length; i++)
		{
			leastCommonMultiple = Part2_LeastCommonMultiple(leastCommonMultiple, numbers[i]);
		}

		return leastCommonMultiple;
	}

	private readonly struct Part2Node
	{
		public readonly int NextLeft;
		public readonly int NextRight;

		public readonly bool IsEndingNode;

		public Part2Node(int nextLeft, int nextRight, bool isEndingNode)
		{
			NextLeft = nextLeft;
			NextRight = nextRight;
			IsEndingNode = isEndingNode;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ParseBase26(scoped ReadOnlySpan<char> nodeId)
	{
		return (nodeId[0] - 'A') * 676 + (nodeId[1] - 'A') * 26 + (nodeId[2] - 'A'); // 676 is 26^2
	}
}