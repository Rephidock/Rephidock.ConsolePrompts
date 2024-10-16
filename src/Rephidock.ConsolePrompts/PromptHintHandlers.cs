using System;
using System.Collections.Generic;


namespace Rephidock.ConsolePrompts;


/// <summary>A listing of default hint handlers.</summary>
public static class PromptHintHandlers {

	#region //// Generic handlers (no designated hint key)

	/// <summary>
	/// A handler that shows raw contents of the hint.
	/// Not included in <see cref="GetAllHandlers"/>.
	/// </summary>
	public static string? DebugHintHandler(PromptHint hint) => hint.ToString();

	/// <summary>
	/// Hint handler that skips all hints provided.
	/// Not included in <see cref="GetAllHandlers"/>.
	/// </summary>
	public static string? SkipHintHandler(PromptHint _) => null;

	#endregion

	#region //// Simple Handlers

	/// <summary>Hint handler for type <see cref="PromptHintKeys.BasicText"/></summary>
	public static string? BasicTextHintHandler(PromptHint hint) => (hint as PromptHint<string>)?.Payload;

	// TODO

	#endregion

	#region //// Type Handler

	// TODO

	#endregion

	#region //// Collections

	/// <summary>Returns all handlers of this listing.</summary>
	public static Dictionary<string, Func<PromptHint, string?>> GetAllHandlers() {
		return new() {
			{ PromptHintTypes.BasicText, BasicTextHintHandler },
			// TODO
		};
	}

	/// <summary>Returns common handlers of this listing, skipping more technical once.</summary>
	/// <remarks>A handler for <see cref="PromptHintTypes.TypeHint"/> is still included.</remarks>
	public static Dictionary<string, Func<PromptHint, string?>> GetCommonHandlers() {
		return new() {
			{ PromptHintTypes.BasicText, BasicTextHintHandler },
			// TODO
		};
	}

	#endregion

}
