using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace AdventOfCode2023.Benchmarks.Standalone.Puzzles;

[MemoryDiagnoser(true)]
[CategoriesColumn, AllStatisticsColumn, BaselineColumn, MinColumn, Q1Column, MeanColumn, Q3Column, MaxColumn, MedianColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class Day07Benchmark
{
	private readonly Input _input;

	public Day07Benchmark()
	{
		_input = Helpers.GetInput("Day07.txt");
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART1)]
	public int Part1()
	{
		const int handLength = 5;

		scoped Span<Hand> handsBuffer = stackalloc Hand[_input.Lines.Length];
		var handsBufferSize = 0;

		for (var i = 0; i < handsBuffer.Length; i++)
		{
			var inputLine = _input.Lines[i].AsSpan();
			var handSlice = inputLine.Slice(0, handLength);

			Part1_ParseHand(ref handSlice, out var handPower);

			var bidSpan = ParseBid(inputLine.Slice(6));

			InsertHandIntoBuffer(ref handsBuffer, ref handsBufferSize, handPower, bidSpan);
		}

		var total = 0;

		Comparison<Hand> handComparison = (x, y) => Math.Sign(x.HandPower - y.HandPower);
		CombineHandBids(ref total, ref handsBuffer, ref handsBufferSize, ref handComparison);

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static void Part1_ParseHand(ref ReadOnlySpan<char> handSpan, out long handPower)
	{
		// 13 card types + max 5 different cards
		// first section is used to count the card of each type and the later section is used for an insertion sort
		scoped Span<short> cardCounts = stackalloc short[18];

		handPower = 0L;
		for (var i = 0; i < 5; i++)
		{
			var card = handSpan[i];

			var cardIndex = card switch
			{
				'T' => 8,
				'J' => 9,
				'Q' => 10,
				'K' => 11,
				'A' => 12,
				_ => card - '0' - 2
			};

			handPower <<= 8;
			handPower += cardIndex;

			cardCounts[cardIndex]++;
		}

		var secondSectionSize = 13; // Used for insertion sort
		var cardCountsFound = 0; // Used as optimization for insertion sort

		// Iterate over first section
		for (var i = 0; i < 13 && cardCountsFound < 5 ; i++)
		{
			var currentCardCount = cardCounts[i];
			if (currentCardCount == 0)
			{
				continue;
			}


			var secondSectionIndex = secondSectionSize - 1;
			while (secondSectionIndex >= 13)
			{
				ref var cardCountInSecondSection = ref cardCounts[secondSectionIndex];
				if (cardCountInSecondSection >= currentCardCount)
				{
					break;
				}

				cardCounts[secondSectionIndex + 1] = cardCountInSecondSection;
				--secondSectionIndex;
			}

			cardCounts[secondSectionIndex + 1] = currentCardCount;
			++secondSectionSize;

			cardCountsFound += currentCardCount;
		}

		var handType = cardCounts[13] switch
		{
			5 => HandType.FiveOfAKind,
			4 => HandType.FourOfAKind,
			3 => cardCounts[14] == 2 ? HandType.FullHouse : HandType.ThreeOfAKind,
			2 => cardCounts[14] == 2 ? HandType.TwoPairs : HandType.OnePair,
			_ => HandType.HighCard
		};

		var handTypePower = (long) handType << 48;
		handPower += handTypePower;
	}

	[Benchmark]
	[BenchmarkCategory(Constants.PART2)]
	public int Part2()
	{
		const int handLength = 5;

		scoped Span<Hand> handsBuffer = stackalloc Hand[_input.Lines.Length];
		var handsBufferSize = 0;

		for (var i = 0; i < handsBuffer.Length; i++)
		{
			var inputLine = _input.Lines[i].AsSpan();
			var handSlice = inputLine.Slice(0, handLength);

			Part2_ParseHand(ref handSlice, out var handPower);

			var bidSpan = ParseBid(inputLine.Slice(6));

			InsertHandIntoBuffer(ref handsBuffer, ref handsBufferSize, handPower, bidSpan);
		}

		var total = 0;

		Comparison<Hand> handComparison = (x, y) => Math.Sign(x.HandPower - y.HandPower);
		CombineHandBids(ref total, ref handsBuffer, ref handsBufferSize, ref handComparison);

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static void Part2_ParseHand(ref ReadOnlySpan<char> handSpan, out long handPower)
	{
		// 13 card types + max 5 different cards
		// first section is used to count the card of each type and the later section is used for an insertion sort
		scoped Span<short> cardCounts = stackalloc short[18];

		handPower = 0L;
		for (var i = 0; i < 5; i++)
		{
			var card = handSpan[i];

			var cardIndex = card switch
			{
				'T' => 9,
				'J' => 0,
				'Q' => 10,
				'K' => 11,
				'A' => 12,
				_ => card - '0' - 1
			};

			handPower <<= 8;
			handPower += cardIndex;

			cardCounts[cardIndex]++;
		}

		var secondSectionSize = 13; // Used for insertion sort
		var cardCountsFound = 0; // Used as optimization for insertion sort

		// Iterate over first section, but skip jokers as they will be added to the highest count later on
		for (var i = 1; i < 13 && cardCountsFound < 5 ; i++)
		{
			var currentCardCount = cardCounts[i];
			if (currentCardCount == 0)
			{
				continue;
			}

			var secondSectionIndex = secondSectionSize - 1;
			while (secondSectionIndex >= 13)
			{
				ref var cardCountInSecondSection = ref cardCounts[secondSectionIndex];
				if (cardCountInSecondSection >= currentCardCount)
				{
					break;
				}

				cardCounts[secondSectionIndex + 1] = cardCountInSecondSection;
				--secondSectionIndex;
			}

			cardCounts[secondSectionIndex + 1] = currentCardCount;
			++secondSectionSize;

			cardCountsFound += currentCardCount;
		}

		// Add jokers to highest card count
		cardCounts[13] += cardCounts[0];

		var handType = cardCounts[13] switch
		{
			5 => HandType.FiveOfAKind,
			4 => HandType.FourOfAKind,
			3 => cardCounts[14] == 2 ? HandType.FullHouse : HandType.ThreeOfAKind,
			2 => cardCounts[14] == 2 ? HandType.TwoPairs : HandType.OnePair,
			_ => HandType.HighCard
		};

		var handTypePower = (long) handType << 48;
		handPower += handTypePower;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ParseBid(ReadOnlySpan<char> span)
	{
		var number = 0;
		for (var i = 0; i < span.Length; i++)
		{
			number = number * 10 + span[i] - '0';
		}

		return number;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void InsertHandIntoBuffer(ref Span<Hand> handsBuffer, ref int handsBufferSize, long handPower, int bid)
	{
		handsBuffer[handsBufferSize++] = new Hand(handPower, bid);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void CombineHandBids(ref int total, ref Span<Hand> handsBuffer, ref int handsBufferSize, ref Comparison<Hand> handComparison)
	{
		handsBuffer.Slice(0, handsBufferSize).Sort(handComparison);

		for (var i = 0; i < handsBufferSize; i++)
		{
			total += handsBuffer[i].Bid * (i + 1);
		}
	}

	private readonly struct Hand
	{
		public readonly long HandPower;
		public readonly int Bid;

		public Hand(long handPower, int bid)
		{
			HandPower = handPower;
			Bid = bid;
		}
	}

	private enum HandType : byte
	{
		HighCard = 0,
		OnePair = 1,
		TwoPairs = 2,
		ThreeOfAKind = 3,
		FullHouse = 4,
		FourOfAKind = 5,
		FiveOfAKind = 6
	}
}