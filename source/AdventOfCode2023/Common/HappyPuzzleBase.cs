using JetBrains.Annotations;

namespace AdventOfCode2023.Common;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class HappyPuzzleBase
{
	public virtual string AssetName => GetType().Name.ToLowerInvariant() + ".txt";

	public abstract object SolvePart1(Input input);
	public abstract object SolvePart2(Input input);
}