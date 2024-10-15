using System;
using System.IO;
using System.Globalization;
using System.Numerics;
using System.Collections.Generic;


namespace Rephidock.ConsolePrompts;


/// <summary>
/// A class for creating instances of <see cref="Prompt{T}"/>.
/// Also handles their style.
/// </summary>
public class Prompter {

	/// <summary>
	/// <para>Creates a <see cref="Prompter"/> for the <see cref="Console"/> streams.</para>
	/// <para>Comes with common hint handlers and skips all uknown hints.</para>
	/// </summary>
	public Prompter() : this(Console.Out, Console.In) {
		SetHintHandlers(PromptHintHandlers.GetCommonHandlers());
		UnknownHintHandler = PromptHintHandlers.SkipHintHandler;
	}

	/// <summary>
	/// <para>
	/// Creates a <see cref="Prompter"/> that works on given
	/// input and output streams.
	/// </para>
	/// <para>
	/// Does not take ownership of (does not dispose of) given streams.
	/// </para>
	/// <para>
	/// Does not come with any hint handlers and shows all unknown hints.
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

	#region //// Prompt creation

	/// <summary>
	/// Creates a prompt for any value.
	/// A parser is required.
	/// </summary>
	/// <returns>A new <see cref="Prompt{T}"/></returns>
	public Prompt<T> PromptFor<T>(string? textPrompt, Func<string, IFormatProvider?, T> parser) {
		var prompt = new Prompt<T>(this).SetPrompt(textPrompt).SetParser(parser);
		if (AutoAddTypeHints) prompt.AddTypeHint();
		return prompt;
	}

	/// <summary>
	/// Creates a prompt for a parsable value.
	/// </summary>
	/// <returns>A new <see cref="Prompt{T}"/></returns>
	public Prompt<T> PromptFor<T>(string? textPrompt = null) where T : IParsable<T> {
		return PromptFor(textPrompt, T.Parse);
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
		Prompt<bool> prompt = PromptFor<bool>(textPrompt, BoolParser);

		// Add a y/n hint or replace the type hint
		if (AutoAddTypeHints) {
			prompt.RemoveHintsMatching(hint => hint.HintType == PromptHintTypes.TypeHint);
		}

		prompt.AddHint(
			new PromptHint(
				PromptHintTypes.Boolean,
				defaultValue ? PromptHintTypes.BooleanPayloadDefaultTrue : PromptHintTypes.BooleanPayloadDefaultFalse
			)
		);

		// Return
		return prompt;
	}

	#endregion

	#region //// Format provider and misc. settings

	/// <summary>
	/// Default format used by this class.
	/// Is <see cref="CultureInfo.InvariantCulture"/>.
	/// </summary>
	public static IFormatProvider DefaultFormatProvider => CultureInfo.InvariantCulture;

	/// <summary>
	/// Format provider passed into the parsers for created prompts.
	/// </summary>
	public IFormatProvider FormatProvider { get; set; } = DefaultFormatProvider;

	/// <summary>
	/// Enable or disable automatically adding type hints.
	/// (see <see cref="Prompt{T}.AddTypeHint"/>.
	/// <see langword="false"/> by default.
	/// </summary>
	/// <remarks>
	/// Note that type hints are more technical.
	/// </remarks>
	public bool AutoAddTypeHints { get; set; } = false;

	#endregion

	#region //// Prompt text formatting

	/// <summary>
	/// Format for the text prompts.
	/// </summary>
	/// <remarks>
	/// {0} -- text prompt.
	/// {1} -- hints.
	/// </remarks>
	public string PromptFormat { get; set; } = "{0} ({1}): ";

	/// <summary>
	/// Format for the text prompts if
	/// there are no hints to be displayed.
	/// </summary>
	/// <remarks>
	/// {0} -- text prompt.
	/// </remarks>
	public string PromptFormatNoHints { get; set; } = "{0}: ";

	/// <summary>Text to display when no text prompt is given.</summary>
	/// <remarks>{0} -- hints</remarks>
	public string NullPromptFormat { get; set; } = "[{0}] > ";

	/// <summary>
	/// Text to display when no text prompt is given
	/// if there are no hints to be displayed.
	/// </summary>
	public string NullPromptDisplayNoHints { get; set; } = "> ";

	/// <summary>
	/// The separator between hints used for prompt formatting
	/// </summary>
	public string HintSeparator { get; set; } = ", ";

	/// <summary>Creates a formatted display prompt, taking hints into account.</summary>
	protected internal virtual string FormatPromptDisplay(string? textPrompt, IReadOnlyList<PromptHint> hints) {

		// Check for options
		bool isPromptNull = string.IsNullOrWhiteSpace(textPrompt);
		bool isNoHints = hints.Count == 0;

		// Use the appropriate format
		if (isNoHints) {

			if (isPromptNull) return NullPromptDisplayNoHints;
			return string.Format(PromptFormatNoHints, textPrompt);

		} else {

			string hintsString = string.Join(HintSeparator, FormatHints(hints));

			if (isPromptNull) return string.Format(NullPromptFormat, hintsString);
			return string.Format(PromptFormat, textPrompt, hintsString);

		}

		// [unreachable]
	}

	#endregion

	#region //// Hint formatting

	private readonly Dictionary<string, Func<PromptHint, string?>> hintFormatHandlers = new();

	/// <summary>A hint handler for all hints of undefined hint types.</summary>
	public Func<PromptHint, string?> UnknownHintHandler { get; set; } = PromptHintHandlers.DebugHintHandler;

	/// <summary>
	/// <para>
	/// Sets a hint handler for a specific hint type (see <see cref="PromptHint.HintType"/>).
	/// </para>
	/// <para>
	/// A handler takes in a <see cref="PromptHint"/> and returns a display <see langword="string"/>
	/// for that hint or <see langword="null"/> if the hint should not be visible.
	/// </para>
	/// </summary>
	/// <returns>this</returns>
	public Prompter SetHintHandler(string hintType, Func<PromptHint, string?> handler) {
		hintFormatHandlers[hintType] = handler;
		return this;
	}

	/// <summary>
	/// <para>
	/// Calls <see cref="SetHintHandler(string, Func{PromptHint, string?})"/>
	/// for all handlers in the provided dictionary.
	/// </para>
	/// </summary>
	/// <returns>this</returns>
	public Prompter SetHintHandlers(Dictionary<string, Func<PromptHint, string?>> handlers) {
		foreach (var pair in handlers) SetHintHandler(pair.Key, pair.Value);
		return this;
	}

	/// <summary>
	/// Removes all hint handlers, forcing <see cref="UnknownHintHandler"/>
	/// to be used for every hint, unless new handlers are added.
	/// </summary>
	/// <returns>this</returns>
	public Prompter RemoveAllHintHandlers() {
		hintFormatHandlers.Clear();
		return this;
	}

	/// <summary>Formats each hint in accordance with hint handlers.</summary>
	protected IEnumerable<string> FormatHints(IReadOnlyList<PromptHint> hints) {

		foreach (var hint in hints) {

			// Find handler based on hint type
			var hintHadler = hintFormatHandlers.GetValueOrDefault(hint.HintType, UnknownHintHandler);

			string? hintString = hintHadler(hint);
			if (hintString is not null) yield return hintString;
		}
	
	}

	#endregion

}
