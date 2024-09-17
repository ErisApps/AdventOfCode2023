using System.Diagnostics;
using System.Runtime.CompilerServices;
using AdventOfCode2023.Common;

namespace AdventOfCode2023.Puzzles;

public class Day19 : HappyPuzzleBase
{
	public override object SolvePart1(Input input)
	{
		var i = 0;

		scoped Span<Part1WorkflowNode> workflowNodesBuffer = stackalloc Part1WorkflowNode[26 * 26 * 26];

		for (;; i++)
		{
			var inputLineSpan = input.Lines[i].AsSpan();
			if (inputLineSpan.IsEmpty)
			{
				i++;
				break;
			}

			Part1_ParseWorkflowEntry(ref inputLineSpan, out var id, out var workflowNode);
			workflowNodesBuffer[id] = workflowNode;
		}

		var startingNode = workflowNodesBuffer[221]; // Magic id for "in" node

		var total = 0;
		for (; i < input.Lines.Length; i++)
		{
			var inputLineSpan = input.Lines[i].AsSpan();
			var present = Part1_ParsePresent(ref inputLineSpan);

			if (startingNode.Apply(ref workflowNodesBuffer, present))
			{
				total += present.GetTotal();
			}
		}

		return total;
	}

	private static void Part1_ParseWorkflowEntry(ref ReadOnlySpan<char> workflowEntrySpan, out int id, out Part1WorkflowNode workflowNode)
	{
		var i = 0;

		var currentIdSpanStart = 0;
		while (workflowEntrySpan[++i] != '{')
		{
			// Advance to the next character
		}

		id = ParseBase26Id(workflowEntrySpan.Slice(currentIdSpanStart, i));

		// Advance to start of first rule
		++i;

		scoped Span<Part1WorkflowNodeRule> workflowNodeRulesBuffer = stackalloc Part1WorkflowNodeRule[4];
		var workflowNodeRulesBufferSize = 0;

		while (true)
		{
			var operatorCharacter = workflowEntrySpan[i + 1];
			if (operatorCharacter != '>' && operatorCharacter != '<')
			{
				break;
			}

			// Parse the rule definition
			var workflowNodeRuleDefinition = workflowEntrySpan.Slice(i, 2) switch
			{
				"x>" => WorkflowNodeRuleDefinition.XGreaterThan,
				"x<" => WorkflowNodeRuleDefinition.XLessThan,
				"m>" => WorkflowNodeRuleDefinition.MGreaterThan,
				"m<" => WorkflowNodeRuleDefinition.MLessThan,
				"a>" => WorkflowNodeRuleDefinition.AGreaterThan,
				"a<" => WorkflowNodeRuleDefinition.ALessThan,
				"s>" => WorkflowNodeRuleDefinition.SGreaterThan,
				"s<" => WorkflowNodeRuleDefinition.SLessThan,
				_ => throw new UnreachableException()
			};
			i += 2;

			// Parse the operand
			var operand = 0;
			char currentChar;
			while ((currentChar = workflowEntrySpan[i++]) != ':')
			{
				operand = operand * 10 + (currentChar - '0');
			}

			// Parse the target id
			currentIdSpanStart = i;
			while (workflowEntrySpan[i++] != ',')
			{
				// Advance to the next character
			}

			var targetId = ParseBase26RuleTargetId(workflowEntrySpan.Slice(currentIdSpanStart, i - currentIdSpanStart - 1));

			workflowNodeRulesBuffer[workflowNodeRulesBufferSize++] = new Part1WorkflowNodeRule(workflowNodeRuleDefinition, operand, targetId);
		}

		var fallbackId = ParseBase26RuleTargetId(workflowEntrySpan[i..^1]);

		workflowNode = new Part1WorkflowNode(workflowNodeRulesBuffer.Slice(0, workflowNodeRulesBufferSize), fallbackId);
	}

	private static Part1Present Part1_ParsePresent(ref ReadOnlySpan<char> itemSpan)
	{
		var i = 3;

		var x = Part1_ParseNumber(itemSpan, ref i);
		i += 3;
		var m = Part1_ParseNumber(itemSpan, ref i);
		i += 3;
		var a = Part1_ParseNumber(itemSpan, ref i);
		i += 3;
		var s = Part1_ParseNumber(itemSpan, ref i);

		return new Part1Present(x, m, a, s);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Part1_ParseNumber(ReadOnlySpan<char> span, ref int i)
	{
		var result = 0;
		while (char.IsAsciiDigit(span[i]))
		{
			result = result * 10 + (span[i] - '0');
			i++;
		}

		return result;
	}

	private readonly struct Part1WorkflowNode
	{
		// Can't use array in readonly structs when we intend to store them in stackalloc-ed arrays so we're gonna have to store the workflow node rules in separate fields to make the compiler happy,
		// so basically manual loop/array unwinding shenanigans
		private readonly Part1WorkflowNodeRule _rule1;
		private readonly Part1WorkflowNodeRule? _rule2;
		private readonly Part1WorkflowNodeRule? _rule3;
		private readonly int _ruleCount;

		private readonly int _fallbackId;

		public Part1WorkflowNode(Span<Part1WorkflowNodeRule> rules, int fallback)
		{
			_fallbackId = fallback;

			_ruleCount = rules.Length;
			_rule1 = rules[0];

			if (_ruleCount == 1)
			{
				return;
			}

			_rule2 = rules[1];

			if (_ruleCount == 2)
			{
				return;
			}

			_rule3 = rules[2];
		}

		public bool Apply(ref Span<Part1WorkflowNode> workflowNodesBuffer, Part1Present part1Present)
		{
			for (var i = 0; i < _ruleCount; i++)
			{
				if (GetRule(i).Apply(part1Present, out var targetId))
				{
					return HandleRuleResult(ref workflowNodesBuffer, part1Present, targetId);
				}
			}

			return HandleRuleResult(ref workflowNodesBuffer, part1Present, _fallbackId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool HandleRuleResult(ref Span<Part1WorkflowNode> workflowNodesBuffer, Part1Present part1Present, int targetId)
		{
			return targetId switch
			{
				int.MinValue => false,
				int.MaxValue => true,
				_ => workflowNodesBuffer[targetId].Apply(ref workflowNodesBuffer, part1Present)
			};
		}

		private Part1WorkflowNodeRule GetRule(int index)
		{
			return index switch
			{
				0 => _rule1,
				1 => _rule2!.Value,
				2 => _rule3!.Value,
				_ => throw new UnreachableException()
			};
		}
	}

	private readonly struct Part1WorkflowNodeRule
	{
		private readonly WorkflowNodeRuleDefinition _definition;
		private readonly int _operand;
		private readonly int _targetId;

		public Part1WorkflowNodeRule(WorkflowNodeRuleDefinition definition, int operand, int targetId)
		{
			_definition = definition;
			_operand = operand;
			_targetId = targetId;
		}

		public bool Apply(Part1Present part1Present, out int targetId)
		{
			var canHandle = _definition switch
			{
				WorkflowNodeRuleDefinition.XGreaterThan => part1Present.X > _operand,
				WorkflowNodeRuleDefinition.XLessThan => part1Present.X < _operand,
				WorkflowNodeRuleDefinition.MGreaterThan => part1Present.M > _operand,
				WorkflowNodeRuleDefinition.MLessThan => part1Present.M < _operand,
				WorkflowNodeRuleDefinition.AGreaterThan => part1Present.A > _operand,
				WorkflowNodeRuleDefinition.ALessThan => part1Present.A < _operand,
				WorkflowNodeRuleDefinition.SGreaterThan => part1Present.S > _operand,
				WorkflowNodeRuleDefinition.SLessThan => part1Present.S < _operand,
				_ => throw new UnreachableException()
			};

			if (canHandle)
			{
				targetId = _targetId;
				return true;
			}

			targetId = int.MinValue;
			return false;
		}
	}

	private readonly struct Part1Present
	{
		public readonly int X;
		public readonly int M;
		public readonly int A;
		public readonly int S;

		public Part1Present(int x, int m, int a, int s)
		{
			X = x;
			M = m;
			A = a;
			S = s;
		}

		public int GetTotal()
		{
			return X + M + A + S;
		}
	}

	// HyperCube splitting :party:
	public override object SolvePart2(Input input)
	{
		scoped Span<Part2WorkflowNode> workflowNodesBuffer = stackalloc Part2WorkflowNode[26 * 26 * 26];

		for (var i = 0;; i++)
		{
			var inputLineSpan = input.Lines[i].AsSpan();
			if (inputLineSpan.IsEmpty)
			{
				break;
			}

			Part2_ParseWorkflowEntry(ref inputLineSpan, out var id, out var workflowNode);
			workflowNodesBuffer[id] = workflowNode;
		}

		var startingNode = workflowNodesBuffer[221]; // Magic id for "in" node

		var startingHyperCube = new Part2HyperCube(1, 4000, 1, 4000, 1, 4000, 1, 4000);

		ReadOnlySpan<Part2WorkflowNode> readOnlyWorkflowNodesBuffer = workflowNodesBuffer;

		return startingNode.ApplyWorkflowAndCalculateHyperVolume(ref readOnlyWorkflowNodesBuffer, startingHyperCube);
	}

	private static void Part2_ParseWorkflowEntry(scoped ref ReadOnlySpan<char> workflowEntrySpan, out int id, out Part2WorkflowNode workflowNode)
	{
		var i = 0;

		var currentIdSpanStart = 0;
		while (workflowEntrySpan[++i] != '{')
		{
			// Advance to the next character
		}

		id = ParseBase26Id(workflowEntrySpan.Slice(currentIdSpanStart, i));

		// Advance to start of first rule
		++i;

		scoped Span<Part2WorkflowNodeRule> workflowNodeRulesBuffer = stackalloc Part2WorkflowNodeRule[4];
		var workflowNodeRulesBufferSize = 0;

		while (true)
		{
			var operatorCharacter = workflowEntrySpan[i + 1];
			if (operatorCharacter != '>' && operatorCharacter != '<')
			{
				break;
			}

			// Parse the rule definition
			var workflowNodeRuleDefinition = workflowEntrySpan.Slice(i, 2) switch
			{
				"x>" => WorkflowNodeRuleDefinition.XGreaterThan,
				"x<" => WorkflowNodeRuleDefinition.XLessThan,
				"m>" => WorkflowNodeRuleDefinition.MGreaterThan,
				"m<" => WorkflowNodeRuleDefinition.MLessThan,
				"a>" => WorkflowNodeRuleDefinition.AGreaterThan,
				"a<" => WorkflowNodeRuleDefinition.ALessThan,
				"s>" => WorkflowNodeRuleDefinition.SGreaterThan,
				"s<" => WorkflowNodeRuleDefinition.SLessThan,
				_ => throw new UnreachableException()
			};
			i += 2;

			// Parse the operand
			var operand = 0;
			char currentChar;
			while ((currentChar = workflowEntrySpan[i++]) != ':')
			{
				operand = operand * 10 + (currentChar - '0');
			}

			// Parse the target id
			currentIdSpanStart = i;
			while (workflowEntrySpan[i++] != ',')
			{
				// Advance to the next character
			}

			var targetId = ParseBase26RuleTargetId(workflowEntrySpan.Slice(currentIdSpanStart, i - currentIdSpanStart - 1));

			workflowNodeRulesBuffer[workflowNodeRulesBufferSize++] = new Part2WorkflowNodeRule(workflowNodeRuleDefinition, operand, targetId);
		}

		var fallbackId = ParseBase26RuleTargetId(workflowEntrySpan[i..^1]);

		workflowNode = new Part2WorkflowNode(workflowNodeRulesBuffer.Slice(0, workflowNodeRulesBufferSize), fallbackId);
	}

	private readonly struct Part2HyperCube
	{
		public readonly int StartX;
		public readonly int EndX;
		public readonly int StartM;
		public readonly int EndM;
		public readonly int StartA;
		public readonly int EndA;
		public readonly int StartS;
		public readonly int EndS;

		public Part2HyperCube(int startX, int endX, int startM, int endM, int startA, int endA, int startS, int endS)
		{
			StartX = startX;
			EndX = endX;
			StartM = startM;
			EndM = endM;
			StartA = startA;
			EndA = endA;
			StartS = startS;
			EndS = endS;
		}

		public Part2HyperCube GetMatchingRuleSegment(WorkflowNodeRuleDefinition workflowNodeRuleDefinition, int operand)
		{
			return workflowNodeRuleDefinition switch
			{
				WorkflowNodeRuleDefinition.XGreaterThan => new Part2HyperCube(operand + 1, EndX, StartM, EndM, StartA, EndA, StartS, EndS),
				WorkflowNodeRuleDefinition.XLessThan => new Part2HyperCube(StartX, operand - 1, StartM, EndM, StartA, EndA, StartS, EndS),
				WorkflowNodeRuleDefinition.MGreaterThan => new Part2HyperCube(StartX, EndX, operand + 1, EndM, StartA, EndA, StartS, EndS),
				WorkflowNodeRuleDefinition.MLessThan => new Part2HyperCube(StartX, EndX, StartM, operand - 1, StartA, EndA, StartS, EndS),
				WorkflowNodeRuleDefinition.AGreaterThan => new Part2HyperCube(StartX, EndX, StartM, EndM, operand + 1, EndA, StartS, EndS),
				WorkflowNodeRuleDefinition.ALessThan => new Part2HyperCube(StartX, EndX, StartM, EndM, StartA, operand - 1, StartS, EndS),
				WorkflowNodeRuleDefinition.SGreaterThan => new Part2HyperCube(StartX, EndX, StartM, EndM, StartA, EndA, operand + 1, EndS),
				WorkflowNodeRuleDefinition.SLessThan => new Part2HyperCube(StartX, EndX, StartM, EndM, StartA, EndA, StartS, operand - 1),
				_ => throw new UnreachableException()
			};
		}

		public Part2HyperCube GetNonMatchingRuleSegment(WorkflowNodeRuleDefinition workflowNodeRuleDefinition, int operand)
		{
			return workflowNodeRuleDefinition switch
			{
				WorkflowNodeRuleDefinition.XGreaterThan => new Part2HyperCube(StartX, operand, StartM, EndM, StartA, EndA, StartS, EndS),
				WorkflowNodeRuleDefinition.XLessThan => new Part2HyperCube(operand, EndX, StartM, EndM, StartA, EndA, StartS, EndS),
				WorkflowNodeRuleDefinition.MGreaterThan => new Part2HyperCube(StartX, EndX, StartM, operand, StartA, EndA, StartS, EndS),
				WorkflowNodeRuleDefinition.MLessThan => new Part2HyperCube(StartX, EndX, operand, EndM, StartA, EndA, StartS, EndS),
				WorkflowNodeRuleDefinition.AGreaterThan => new Part2HyperCube(StartX, EndX, StartM, EndM, StartA, operand, StartS, EndS),
				WorkflowNodeRuleDefinition.ALessThan => new Part2HyperCube(StartX, EndX, StartM, EndM, operand, EndA, StartS, EndS),
				WorkflowNodeRuleDefinition.SGreaterThan => new Part2HyperCube(StartX, EndX, StartM, EndM, StartA, EndA, StartS, operand),
				WorkflowNodeRuleDefinition.SLessThan => new Part2HyperCube(StartX, EndX, StartM, EndM, StartA, EndA, operand, EndS),
				_ => throw new UnreachableException()
			};
		}

		public long GetHyperVolume()
		{
			return (long) (EndX - StartX + 1) * (EndM - StartM + 1) * (EndA - StartA + 1) * (EndS - StartS + 1);
		}
	}

	private readonly struct Part2WorkflowNode
	{
		// Can't use array in readonly structs when we intend to store them in stackalloc-ed arrays so we're gonna have to store the workflow node rules in separate fields to make the compiler happy,
		// so basically manual loop/array unwinding shenanigans
		private readonly Part2WorkflowNodeRule _rule1;
		private readonly Part2WorkflowNodeRule? _rule2;
		private readonly Part2WorkflowNodeRule? _rule3;
		private readonly int _ruleCount;

		private readonly int _fallbackId;

		public Part2WorkflowNode(Span<Part2WorkflowNodeRule> rules, int fallback)
		{
			_fallbackId = fallback;

			_ruleCount = rules.Length;
			_rule1 = rules[0];

			if (_ruleCount == 1)
			{
				return;
			}

			_rule2 = rules[1];

			if (_ruleCount == 2)
			{
				return;
			}

			_rule3 = rules[2];
		}

		// ReSharper disable once CyclomaticComplexity
		// ReSharper disable once CognitiveComplexity
		public long ApplyWorkflowAndCalculateHyperVolume(ref ReadOnlySpan<Part2WorkflowNode> workflowNodesBuffer, Part2HyperCube hyperCube)
		{
			scoped Span<Part2HyperCube> hyperCubesBuffer = stackalloc Part2HyperCube[8];
			var hyperCubesBufferSize = 0;

			hyperCubesBuffer[hyperCubesBufferSize++] = hyperCube;

			var totalHyperVolume = 0L;

			for (var i = 0; i < hyperCubesBufferSize; i++)
			{
				// Theoretically, we don't need to ref var here as we don't need to use the changed value a second time,
				// but it makes the code slightly easier to debug when we want to calculate to the total "hyper volume" for the input hyperCube parameter
				ref var currentHyperCube = ref hyperCubesBuffer[i];

				for (var j = 0; j < _ruleCount; j++)
				{
					var currentWorkflowNodeRule = GetRule(j);

					// Case 1: Fully contained
					switch (currentWorkflowNodeRule.Definition)
					{
						case WorkflowNodeRuleDefinition.XGreaterThan when currentHyperCube.StartX > currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.XLessThan when currentHyperCube.EndX < currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.MGreaterThan when currentHyperCube.StartM > currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.MLessThan when currentHyperCube.EndM < currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.AGreaterThan when currentHyperCube.StartA > currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.ALessThan when currentHyperCube.EndA < currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.SGreaterThan when currentHyperCube.StartS > currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.SLessThan when currentHyperCube.EndS < currentWorkflowNodeRule.Operand:
							// This hypercube is fully contained, so we can just pass it down to the next workflow node
							totalHyperVolume += HandleRuleResult(ref workflowNodesBuffer, currentHyperCube, currentWorkflowNodeRule.TargetId);

							// Handled so go to next hypercube segment in the "queue", but we can't use break here because that would still trigger the fallback scenario, hence why we use a goto
							// This could be avoided if we were to put the inner loop in a separate method, that way... we could just return from the method instead of using a goto but that makes
							// it a little bit trickier as we need to pass along all the variables that we need to keep track of
							goto postFallback;
					}

					// Case 2: Partially contained, needs splitting
					// We only need to check one of the bounds, since if it's not contained, it's not contained and the other bound was already checked in the previous case
					switch (currentWorkflowNodeRule.Definition)
					{
						case WorkflowNodeRuleDefinition.XGreaterThan when currentHyperCube.EndX > currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.XLessThan when currentHyperCube.StartX < currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.MGreaterThan when currentHyperCube.EndM > currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.MLessThan when currentHyperCube.StartM < currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.AGreaterThan when currentHyperCube.EndA > currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.ALessThan when currentHyperCube.StartA < currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.SGreaterThan when currentHyperCube.EndS > currentWorkflowNodeRule.Operand:
						case WorkflowNodeRuleDefinition.SLessThan when currentHyperCube.StartS < currentWorkflowNodeRule.Operand:
							// Add non matching segment back to buffer so it can be handled by this node
							hyperCubesBuffer[hyperCubesBufferSize++] = currentHyperCube.GetNonMatchingRuleSegment(currentWorkflowNodeRule.Definition, currentWorkflowNodeRule.Operand);

							// Take the matching segment of the hypercube and pass it down to the next workflow node
							currentHyperCube = currentHyperCube.GetMatchingRuleSegment(currentWorkflowNodeRule.Definition, currentWorkflowNodeRule.Operand);
							totalHyperVolume += HandleRuleResult(ref workflowNodesBuffer, currentHyperCube, currentWorkflowNodeRule.TargetId);

							// Handled so go to next hypercube segment in the "queue", but we can't use break here because that would still trigger the fallback scenario, hence why we use a goto
							// This could be avoided if we were to put the inner loop in a separate method, that way... we could just return from the method instead of using a goto but that makes
							// it a little bit trickier as we need to pass along all the variables that we need to keep track of
							goto postFallback;
					}

					// Case 3: Not contained, so NO-OP and continue to next rule in line
				}

				totalHyperVolume += HandleRuleResult(ref workflowNodesBuffer, currentHyperCube, _fallbackId);

				postFallback: ;
			}

			return totalHyperVolume;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long HandleRuleResult(ref ReadOnlySpan<Part2WorkflowNode> workflowNodesBuffer, Part2HyperCube hyperCube, int targetId)
		{
			return targetId switch
			{
				int.MinValue => 0,
				int.MaxValue => hyperCube.GetHyperVolume(),
				_ => workflowNodesBuffer[targetId].ApplyWorkflowAndCalculateHyperVolume(ref workflowNodesBuffer, hyperCube)
			};
		}

		private Part2WorkflowNodeRule GetRule(int index)
		{
			return index switch
			{
				0 => _rule1,
				1 => _rule2!.Value,
				2 => _rule3!.Value,
				_ => throw new UnreachableException()
			};
		}
	}

	private readonly struct Part2WorkflowNodeRule
	{
		public readonly WorkflowNodeRuleDefinition Definition;
		public readonly int Operand;
		public readonly int TargetId;

		public Part2WorkflowNodeRule(WorkflowNodeRuleDefinition definition, int operand, int targetId)
		{
			Definition = definition;
			Operand = operand;
			TargetId = targetId;
		}
	}

	private static int ParseBase26Id(ReadOnlySpan<char> span)
	{
		var result = 0;
		for (var i = 0; i < span.Length; i++)
		{
			result = result * 26 + (span[i] - 'a');
		}

		return result;
	}

	private static int ParseBase26RuleTargetId(ReadOnlySpan<char> idSpan)
	{
		switch (idSpan[0])
		{
			case 'R':
				return int.MinValue;
			case 'A':
				return int.MaxValue;
			default:
				var result = 0;
				for (var i = 0; i < idSpan.Length; i++)
				{
					result = result * 26 + (idSpan[i] - 'a');
				}

				return result;
		}
	}

	private enum WorkflowNodeRuleDefinition
	{
		XGreaterThan,
		XLessThan,
		MGreaterThan,
		MLessThan,
		AGreaterThan,
		ALessThan,
		SGreaterThan,
		SLessThan
	}
}