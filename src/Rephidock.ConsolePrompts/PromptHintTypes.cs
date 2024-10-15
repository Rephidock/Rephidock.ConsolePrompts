

namespace Rephidock.ConsolePrompts;


/// <summary>
/// Contains default hint types provided.
/// </summary>
public static class PromptHintTypes {

	/// <summary>Hint type denoting simple text</summary>
	public const string BasicText = "text";

	/// <summary>Hint type denoting requested type</summary>
	public const string TypeHint = "type";

	#region //// Boolean

	/// <summary>Hint type denoting a y/n prompt</summary>
	public const string Boolean = "bool";

	/// <summary>Hint payload denoting a default Y for a y/n prompt</summary>
	public const string BooleanPayloadDefaultTrue = "1";

	/// <summary>Hint payload denoting a default N for a y/n prompt</summary>
	public const string BooleanPayloadDefaultFalse = "0";

	#endregion

}
