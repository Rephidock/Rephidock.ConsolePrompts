using System;
using System.IO;
using System.Globalization;
using System.Numerics;


namespace Rephidock.ConsolePrompts;


/// <summary>
/// A class for creating instances of <see cref="Prompt{T}"/>.
/// Also handles their style.
/// </summary>
public class Prompter {

	/// <summary>
	/// Creates a <see cref="Prompter"/> for the <see cref="Console"/> streams.
	/// </summary>
	public Prompter() : this(Console.Out, Console.In) { }

	/// <summary>
	/// <para>
	/// Creates a <see cref="Prompter"/> that works on given
	/// input and output streams.
	/// </para>
	/// <para>
	/// Does not take ownership of (does not dispose of) given streams.
	/// </para>
	/// </summary>
	public Prompter(TextWriter outputStream, TextReader inputStream) {
		OutputStream = outputStream;
		InputStream = inputStream;
	}

	#region //// Streams

	internal readonly TextWriter OutputStream;
	internal readonly TextReader InputStream;

	#endregion

	#region //// Format provider

	/// <summary>
	/// Default format used by this class.
	/// Is <see cref="CultureInfo.InvariantCulture"/>.
	/// </summary>
	public static IFormatProvider DefaultFormatProvider => CultureInfo.InvariantCulture;

	/// <summary>
	/// Format used to create prompts.
	/// </summary>
	public IFormatProvider FormatProvider { get; set; } = DefaultFormatProvider;

	#endregion

	#region //// Prompt creation

	/// <summary>
	/// Creates a prompt for any value.
	/// A parser is required.
	/// </summary>
	/// <returns>A new <see cref="Prompt{T}"/></returns>
	public Prompt<T> PromptFor<T>(string? textPrompt, Func<string, IFormatProvider?, T> parser) {
		return new Prompt<T>(this).SetPrompt(textPrompt).SetParser(parser);
	}

	/// <summary>
	/// Creates a prompt for a parsable value.
	/// </summary>
	/// <returns>A new <see cref="Prompt{T}"/></returns>
	public Prompt<T> PromptFor<T>(string? textPrompt = null) where T : IParsable<T> {
		return new Prompt<T>(this).SetPrompt(textPrompt).SetParser(T.Parse);
	}

	/// <summary>
	/// Creates a prompt for a numeric value.
	/// </summary>
	/// <returns>A new <see cref="Prompt{T}"/></returns>
	public Prompt<T> PromptFor<T>(string? textPrompt, bool forceFinite) where T : struct, INumber<T> {
		Prompt<T> prompt = PromptFor<T>(textPrompt);
		if (forceFinite) prompt.ForceFinite();

		return prompt;
	}

	/// <summary>Creates a prompt for a string.</summary>
	/// <returns>A new <see cref="Prompt{T}"/> where T is <see langword="string"/></returns>
	public Prompt<string> PromptForString(string? textPrompt = null, bool trim = true) {

		// Define parsers
		static string PassthroughParser(string input, IFormatProvider? _) => input;
		static string TrimParser(string input, IFormatProvider? _) => input.Trim();

		// Choose the parser
		Func<string, IFormatProvider?, string> chosenParser = trim ? TrimParser : PassthroughParser;

		// Create Prompt
		return PromptFor<string>(textPrompt, chosenParser);
	}

	/// <summary>
	/// Creates a prompt for a boolean.
	/// Supports one character input (y/n; t/f; 1/0).
	/// </summary>
	/// <returns>A new <see cref="Prompt{T}"/> where T is <see langword="bool"/></returns>
	public Prompt<bool> PromptForBool(string? textPrompt = null, bool defaultValue = false) {

		// Define parser
		bool BoolParser(string input, IFormatProvider? _) {

			// Trim input
			input = input.Trim();

			// Check for default return
			if (input.Length == 0) return defaultValue;

			// Try default parser first
			if (bool.TryParse(input, out bool defaultParserResult)) {
				return defaultParserResult;
			};

			// Check for valid single characters
			if (input.Length == 1) {

				char inputChar = char.ToUpper(input[0]);

				if (inputChar == 'Y' || inputChar == 'T' || inputChar == '1') return true;
				if (inputChar == 'N' || inputChar == 'F' || inputChar == '0') return false;
			}

			// Throw if all other fails
			throw new FormatException();
		}

		// Create prompt
		Prompt<bool> prompt = PromptFor<bool>(textPrompt, BoolParser).SetParserFormat(null);

		// Add y/n hint
		string hintText;
		if (defaultValue) {
			hintText = PromptStyler.HintStrings.BoolDefaultTrue;
		} else {
			hintText = PromptStyler.HintStrings.BoolDefaultFalse;
		}

		prompt.AddHint(hintText, PromptHintLevel.Minimal);

		// Return
		return prompt;
	}

	#endregion

}
