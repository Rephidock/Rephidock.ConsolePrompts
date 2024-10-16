

namespace Rephidock.ConsolePrompts;


/// <summary>
/// <para>
/// A hint displayed by <see cref="Prompt{T}"/>.
/// </para>
/// <para>
/// Not styled.
/// When the <see cref="Prompt{T}"/> is displayed, the <see cref="Prompter"/> will format
/// each <see cref="PromptHint"/> instance in accordance with one of
/// <see cref="PromptHintHandlers"/> based on <see cref="PromptHint.Key"/>.
/// </para>
/// <para>
/// See also: <see cref="Prompter.SetHintHandler(string, System.Func{PromptHint, string?})"/>.
/// </para>
/// </summary>
/// <remarks>For a hint with a payload see <see cref="PromptHint{TPayload}"/>.</remarks>
public record PromptHint {

	/// <summary>
	/// <para>
	/// The key (type) of the hint. Used to stylise hints.
	/// </para>
	/// <para>
	/// See also: <see cref="Prompter.SetHintHandler(string, System.Func{PromptHint, string?})"/>
	/// </para>
	/// </summary>
	// Left as a string, not enum, so that consumers
	// can add and handle their own hint keys
	public string Key { get; }
	
	/// <summary>Creates a hint with no additional information.</summary>>
	public PromptHint(string type) {
		Key = type;
	}

}


/// <typeparam name="TPayload">The type of addictional information stored, ideally immutable.</typeparam>
/// <remarks>For a hint without a payload see <see cref="PromptHint"/>.</remarks>
/// <inheritdoc cref="PromptHint"/>
public record PromptHint<TPayload> : PromptHint {

	/// <summary>The payload of the hint.</summary>
	public TPayload Payload { get; }

	/// <summary>Creates a hint with a payload.</summary>
	public PromptHint(string type, TPayload payload) : base(type) {
		Payload = payload;
	}

}
