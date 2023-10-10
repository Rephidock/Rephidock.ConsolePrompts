

namespace Rephidock.ConsolePrompts;


public enum PromptHintLevel {
	None,
	Minimal,
	Standard,
	Verbose
}


public readonly record struct PromptHint {

	/// <summary>
	/// Text of the hint.
	/// </summary>
	public required string Text { get; init; }

	/// <summary>
	/// Minimum hint level required to display the hint.
	/// </summary>
	public required PromptHintLevel Level { get; init; }

}
