

namespace Rephidock.ConsolePrompts;


/// <summary>A listing of default hint types.</summary>
public static class PromptHintKeys {

	/// <summary>Hint type denoting simple text. Payload: given text.</summary>
	public const string BasicText = "text";

	/// <summary>Hint type denoting requested type. Payload: the required type.</summary>
	public const string TypeHint = "type";

	/// <summary>Hint type denoting a y/n prompt. Payload: default value.</summary>
	public const string Boolean = "bool";

}
