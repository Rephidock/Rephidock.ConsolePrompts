

namespace Rephidock.AdeptConsole;


public enum PromptHintLevel {
	None,
	Minimal,
	Standard,
	Verbose
}


internal readonly record struct PromptHint {
	public required string Text { get; init; }
	public required PromptHintLevel Level { get; init; }
}
