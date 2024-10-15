using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Rephidock.ConsolePrompts;


/// <summary>
/// <para>
/// A hint displayed by <see cref="Prompt{T}"/>.
/// </para>
/// <para>
/// Not styled.
/// When the <see cref="Prompt{T}"/> is displayed, the <see cref="Prompter"/> will format
/// each <see cref="PromptHint"/> instance in accordance with one of
/// <see cref="PromptHintHandlers"/> based on <see cref="PromptHint.HintType"/>.
/// </para>
/// <para>
/// See also: <see cref="Prompter.SetHintHandler(string, System.Func{Rephidock.ConsolePrompts.PromptHint, string?})"/>.
/// </para>
/// </summary>
public readonly record struct PromptHint {

	/// <summary>
	/// <para>
	/// The type of the hint. Used to stylise hints.
	/// </para>
	/// <para>
	/// See also: <see cref="Prompter.SetHintHandler(string, System.Func{PromptHint, string?})"/>
	/// </para>
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
