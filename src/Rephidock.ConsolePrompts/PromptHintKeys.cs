

namespace Rephidock.ConsolePrompts;


/// <summary>A listing of default hint types.</summary>
public static class PromptHintKeys {

	/// <summary>Hint key denoting simple text. Payload: given text (as string).</summary>
	public const string BasicText = "text";

	/// <summary>Hint key denoting requested type. Payload: the required type (as Type).</summary>
	public const string TypeHint = "type";

	/// <summary>Hint key denoting a y/n prompt. Payload: default value (as bool).</summary>
	public const string Boolean = "bool";


	/// <summary>
	/// Hint key denoting a string length limit.
	/// Payload: exact legnth as int -OR- length range as (int?,int?) 
	/// </summary>
	public const string StringLength = "strLen";

	/// <summary>Hint key denoting a string must not be empty.</summary>
	public const string StringNotEmpty = "strNotEmpty";

	/// <summary>Hint key denoting a string must not be empty or just whitespace.</summary>
	public const string StringNotEmptyOrWhitespace = "strNotEmptyOrWS";


	/// <summary>Hint key denoting the input must be a file system path.</summary>
	public const string Path = "path";

	/// <summary>
	/// Hint key denoting the input must be a file system path to a file.
	/// Payload: wether the file must exist (as bool).
	/// </summary>
	public const string FilePath = "pathFile";

	/// <summary>
	/// Hint key denoting the input must be a file system path to a directory.
	/// Payload: wether the directory must exist (as bool).
	/// </summary>
	public const string DirectoryPath = "pathDirectory";


	/// <summary>
	/// Hint key denoting a numerical range.
	/// Payload: stringified range bounds as (string?,string?).
	/// </summary>
	public const string NumericRange = "range";

	/// <summary>Hint key denoting input must be finite.</summary>
	public const string NumericFinite = "finite";

	/// <summary>Hint key denoting input must not be infinity.</summary>
	public const string NumericNotInfinity = "notInfinite";

	/// <summary>Hint key denoting input must not be NaN.</summary>
	public const string NumericNotNan = "notNaN";


	/// <summary>Hint key denoting a banned value. Payload: the value stringified (as string).</summary>
	public const string NotEqual = "notEqual";

}
