using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day07 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		const int handLength = 5;

		scoped Span<byte> alignedRawHand = stackalloc byte[8];
		scoped Span<char> editableHandSpan = stackalloc char[5];

		const int estimatedHandsBufferBucketSize = 250;

		scoped Span<Hand> highCardHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var highCardHandsBufferSize = 0;

		scoped Span<Hand> singlePairHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var singlePairHandsBufferSize = 0;

		scoped Span<Hand> twoPairHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var twoPairHandsBufferSize = 0;

		scoped Span<Hand> threeOfAKindHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var threeOfAKindHandsBufferSize = 0;

		scoped Span<Hand> fullHouseHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var fullHouseHandsBufferSize = 0;

		scoped Span<Hand> fourOfAKindHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var fourOfAKindHandsBufferSize = 0;

		scoped Span<Hand> fiveOfAKindHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var fiveOfAKindHandsBufferSize = 0;

		for (var i = 0; i < input.Lines.Length; i++)
		{
			var inputLine = input.Lines[i].AsSpan();
			var handSlice = inputLine.Slice(0, handLength);

			handSlice.CopyTo(editableHandSpan);
			var handType = Part1_DetermineHandType(ref editableHandSpan);

			for (var j = 0; j < handLength; j++)
			{
				alignedRawHand[handLength - j] = (byte) editableHandSpan[j];
			}
			var handPower = MemoryMarshal.Read<long>(alignedRawHand);

			var bidSpan = ParseBid(inputLine.Slice(6));

			switch (handType)
			{
				case HandType.HighCard:
					InsertHandIntoBuffer(ref highCardHandsBuffer, ref highCardHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.OnePair:
					InsertHandIntoBuffer(ref singlePairHandsBuffer, ref singlePairHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.TwoPairs:
					InsertHandIntoBuffer(ref twoPairHandsBuffer, ref twoPairHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.ThreeOfAKind:
					InsertHandIntoBuffer(ref threeOfAKindHandsBuffer, ref threeOfAKindHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.FullHouse:
					InsertHandIntoBuffer(ref fullHouseHandsBuffer, ref fullHouseHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.FourOfAKind:
					InsertHandIntoBuffer(ref fourOfAKindHandsBuffer, ref fourOfAKindHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.FiveOfAKind:
					InsertHandIntoBuffer(ref fiveOfAKindHandsBuffer, ref fiveOfAKindHandsBufferSize, handPower, bidSpan);
					break;
				default:
					throw new UnreachableException();
			}
		}

		var total = 0;
		var offset = 1;

		Comparison<Hand> handComparison = (x, y) => Math.Sign(x.HandPower - y.HandPower);

		CombineHandBids(ref total, ref offset, ref highCardHandsBuffer, ref highCardHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref singlePairHandsBuffer, ref singlePairHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref twoPairHandsBuffer, ref twoPairHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref threeOfAKindHandsBuffer, ref threeOfAKindHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref fullHouseHandsBuffer, ref fullHouseHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref fourOfAKindHandsBuffer, ref fourOfAKindHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref fiveOfAKindHandsBuffer, ref fiveOfAKindHandsBufferSize, ref handComparison);

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static HandType Part1_DetermineHandType(ref Span<char> editableHandSpan)
	{
		// 13 card types + max 5 different cards
		// first section is used to count the card of each type and the later section is used for an insertion sort
		scoped Span<short> cardCounts = stackalloc short[18];

		for (var i = 0; i < 5; i++)
		{
			ref var card = ref editableHandSpan[i];

			var cardIndex = card switch
			{
				'T' => 8,
				'J' => 9,
				'Q' => 10,
				'K' => 11,
				'A' => 12,
				_ => card - '0' - 2
			};

			card = (char) cardIndex;

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

		return cardCounts[13] switch
		{
			5 => HandType.FiveOfAKind,
			4 => HandType.FourOfAKind,
			3 => cardCounts[14] == 2 ? HandType.FullHouse : HandType.ThreeOfAKind,
			2 => cardCounts[14] == 2 ? HandType.TwoPairs : HandType.OnePair,
			_ => HandType.HighCard
		};
	}

	public override object SolvePart2(Input input)
	{
		const int handLength = 5;

		scoped Span<byte> alignedRawHand = stackalloc byte[8];
		scoped Span<char> editableHandSpan = stackalloc char[5];

		const int estimatedHandsBufferBucketSize = 250;

		scoped Span<Hand> highCardHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var highCardHandsBufferSize = 0;

		scoped Span<Hand> singlePairHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var singlePairHandsBufferSize = 0;

		scoped Span<Hand> twoPairHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var twoPairHandsBufferSize = 0;

		scoped Span<Hand> threeOfAKindHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var threeOfAKindHandsBufferSize = 0;

		scoped Span<Hand> fullHouseHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var fullHouseHandsBufferSize = 0;

		scoped Span<Hand> fourOfAKindHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var fourOfAKindHandsBufferSize = 0;

		scoped Span<Hand> fiveOfAKindHandsBuffer = stackalloc Hand[estimatedHandsBufferBucketSize];
		var fiveOfAKindHandsBufferSize = 0;

		for (var i = 0; i < input.Lines.Length; i++)
		{
			var inputLine = input.Lines[i].AsSpan();
			var handSlice = inputLine.Slice(0, handLength);

			handSlice.CopyTo(editableHandSpan);
			var handType = Part2_DetermineHandType(ref editableHandSpan);

			for (var j = 0; j < handLength; j++)
			{
				alignedRawHand[handLength - j] = (byte) editableHandSpan[j];
			}
			var handPower = MemoryMarshal.Read<long>(alignedRawHand);

			var bidSpan = ParseBid(inputLine.Slice(6));

			switch (handType)
			{
				case HandType.HighCard:
					InsertHandIntoBuffer(ref highCardHandsBuffer, ref highCardHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.OnePair:
					InsertHandIntoBuffer(ref singlePairHandsBuffer, ref singlePairHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.TwoPairs:
					InsertHandIntoBuffer(ref twoPairHandsBuffer, ref twoPairHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.ThreeOfAKind:
					InsertHandIntoBuffer(ref threeOfAKindHandsBuffer, ref threeOfAKindHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.FullHouse:
					InsertHandIntoBuffer(ref fullHouseHandsBuffer, ref fullHouseHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.FourOfAKind:
					InsertHandIntoBuffer(ref fourOfAKindHandsBuffer, ref fourOfAKindHandsBufferSize, handPower, bidSpan);
					break;
				case HandType.FiveOfAKind:
					InsertHandIntoBuffer(ref fiveOfAKindHandsBuffer, ref fiveOfAKindHandsBufferSize, handPower, bidSpan);
					break;
				default:
					throw new UnreachableException();
			}
		}

		var total = 0;
		var offset = 1;

		Comparison<Hand> handComparison = (x, y) => Math.Sign(x.HandPower - y.HandPower);

		CombineHandBids(ref total, ref offset, ref highCardHandsBuffer, ref highCardHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref singlePairHandsBuffer, ref singlePairHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref twoPairHandsBuffer, ref twoPairHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref threeOfAKindHandsBuffer, ref threeOfAKindHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref fullHouseHandsBuffer, ref fullHouseHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref fourOfAKindHandsBuffer, ref fourOfAKindHandsBufferSize, ref handComparison);
		CombineHandBids(ref total, ref offset, ref fiveOfAKindHandsBuffer, ref fiveOfAKindHandsBufferSize, ref handComparison);

		return total;
	}

	// ReSharper disable once CognitiveComplexity
	private static HandType Part2_DetermineHandType(ref Span<char> editableHandSpan)
	{
		// 13 card types + max 5 different cards
		// first section is used to count the card of each type and the later section is used for an insertion sort
		scoped Span<short> cardCounts = stackalloc short[18];

		for (var i = 0; i < 5; i++)
		{
			ref var card = ref editableHandSpan[i];

			var cardIndex = card switch
			{
				'T' => 8,
				'J' => 12,
				'Q' => 9,
				'K' => 10,
				'A' => 11,
				_ => card - '0' - 2
			};

			card = (char) cardIndex;

			cardCounts[cardIndex]++;
		}

		var secondSectionSize = 13; // Used for insertion sort
		var cardCountsFound = 0; // Used as optimization for insertion sort

		// Iterate over first section
		for (var i = 0; i < 12 && cardCountsFound < 5 ; i++)
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
		cardCounts[13] += cardCounts[12];

		return cardCounts[13] switch
		{
			5 => HandType.FiveOfAKind,
			4 => HandType.FourOfAKind,
			3 => cardCounts[14] == 2 ? HandType.FullHouse : HandType.ThreeOfAKind,
			2 => cardCounts[14] == 2 ? HandType.TwoPairs : HandType.OnePair,
			_ => HandType.HighCard
		};
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
	private static void CombineHandBids(ref int total, ref int offset, ref Span<Hand> handsBuffer, ref int handsBufferSize, ref Comparison<Hand> handComparison)
	{
		handsBuffer.Slice(0, handsBufferSize).Sort(handComparison);

		for (var i = 0; i < handsBufferSize; i++)
		{
			total += handsBuffer[i].Bid * (i + offset);
		}
		offset += handsBufferSize;
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