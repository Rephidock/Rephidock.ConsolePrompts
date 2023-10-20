

namespace Rephidock.ConsolePrompts;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public enum PromptHintLevel {
	None,
	Minimal,
	Standard,
	Verbose
}
#pragma warning restore CS1591


/// <summary>
/// A hint displayed by <see cref="Prompt{T}"/>.
/// </summary>
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
