

namespace Rephidock.ConsolePrompts;


/// <summary>A listing of default hint types.</summary>
public static class PromptHintKeys {

	/// <summary>Hint key denoting simple text. Payload: given text.</summary>
	public const string BasicText = "text";

	/// <summary>Hint key denoting requested type. Payload: the required type.</summary>
	public const string TypeHint = "type";

	/// <summary>Hint key denoting a y/n prompt. Payload: default value.</summary>
	public const string Boolean = "bool";

	/// <summary>
	/// Hint key denoting a string length limit.
	/// Payload: exact legnth as int -OR- length range as (int?,int?) 
	/// </summary>
	public const string StringLength = "strlen";

}
