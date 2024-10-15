using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Rephidock.ConsolePrompts;


/// <summary>
/// A hint displayed by <see cref="Prompt{T}"/>.
/// Not styled.
/// </summary>
public readonly record struct PromptHint {

	/// <summary>
	/// The type of the hint. Used by the styler.
	/// </summary>
	// Left as a string, not enum, so that consumers
	// can add and handle their own hint types
	public string HintType { get; }

	/// <summary>
	/// The payload of the hint.
	/// Empty if not applicable.
	/// </summary>
	public ReadOnlyCollection<string> HintPayload { get; }

	/// <summary>
	/// Creates a hint with a type and optional payload.
	/// </summary>
	public PromptHint(string type, params string[] payload) {
		HintType = type;
		HintPayload = payload.AsReadOnly();
	}

}
