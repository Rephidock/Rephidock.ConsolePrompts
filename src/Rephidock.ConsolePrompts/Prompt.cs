using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;


namespace Rephidock.ConsolePrompts;


/// <summary>
/// A class to create instances of <see cref="Prompt{T}"/>.
/// </summary>
public static class Prompt {

	#region //// Generic Creation

	/// <summary>
	/// Creates a prompt for any value.
	/// A parser is required.
	/// </summary>
	/// <returns>A new <see cref="Prompt{T}"/></returns>
	public static Prompt<T> For<T>(string? textPrompt, Func<string, IFormatProvider?, T> parser) {
		return new Prompt<T>().SetPrompt(textPrompt).SetParser(parser);
	}

	/// <summary>
	/// Creates a prompt for a parsable value.
	/// </summary>
	/// <returns>A new <see cref="Prompt{T}"/></returns>
	public static Prompt<T> For<T>(string? textPrompt = null) where T : IParsable<T> {
		return new Prompt<T>().SetPrompt(textPrompt).SetParser(T.Parse);
	}

	/// <summary>
	/// Creates a prompt for a numeric value.
	/// </summary>
	/// <returns>A new <see cref="Prompt{T}"/></returns>
	public static Prompt<T> For<T>(string? textPrompt, bool allowInfinite, bool allowNan)
	where T : struct, INumber<T>
	{

		Prompt<T> ret = For<T>(textPrompt);

		if (!allowInfinite) ret.DisallowInfinities();
		if (!allowNan) ret.DisallowNaN();

		return ret;
	}

	/// <summary>Creates a prompt for a string.</summary>
	/// <returns>A new <see cref="Prompt{string}"/></returns>
	public static Prompt<string> ForString(string? textPrompt = null, bool trim = true) {

		// Define parsers
		static string PassthroughParser(string input, IFormatProvider? _) => input;
		static string TrimParser(string input, IFormatProvider? _) => input.Trim();

		// Choose the parser
		Func<string, IFormatProvider?, string> chosenParser = trim ? TrimParser : PassthroughParser;

		// Create Prompt
		return For<string>(textPrompt, chosenParser).SetParserFormat(null);
	}

	/// <summary>
	/// Creates a prompt for a boolean.
	/// Supports one character input (y/n).
	/// </summary>
	/// <returns>A new <see cref="Prompt{bool}"/></returns>
	public static Prompt<bool> ForBool(string? textPrompt = null, bool defaultValue = false) {

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

				if (input[0] == 'Y' || input[0] == 'T' || input[0] == '1') return true;
				if (input[0] == 'N' || input[0] == 'F' || input[0] == '0') return false;
			}

			// Throw if all other fails
			throw new FormatException();
		}

		return For<bool>(textPrompt, BoolParser).SetParserFormat(null);
	}

	#endregion

}


/// <summary>
/// A prompt (query) for a value to be shown to the user.
/// </summary>
public sealed class Prompt<T> {

	#region //// Text Prompt

	/// <summary>Text prompt to be tweaked and displayed.</summary>
	string? textPrompt = null;

	/// <summary>
	/// Sets the text prompt to be displayed on query.
	/// If text prompt is empty, whitespace or <see langword="null"/>,
	/// <see cref="PromptStyler.NullPromptDisplay"/> will be displayed.
	/// </summary>
	/// <returns>this</returns>
	public Prompt<T> SetPrompt(string? textPrompt) {

		if (string.IsNullOrWhiteSpace(textPrompt)) {
			this.textPrompt = null;
		} else {
			this.textPrompt = textPrompt;
		}

		return this;
	}

	/// <summary>
	/// <para>
	/// Removes text prompt from the current <seealso cref="Prompt"/>.
	/// Equivalent of setting text prompt to whitespace or <see langword="null"/>.
	/// </para>
	/// See also: <seealso cref="PromptStyler.NullPromptDisplay"/>.
	/// </summary>
	/// <returns>this</returns>
	public Prompt<T> RemovePrompt() => SetPrompt(null);

	#endregion

	#region //// Hints

	readonly List<PromptHint> hints = new();

	/// <summary>
	/// <para>
	/// Adds a hint to be displayed with the prompt.
	/// Only hints with sufficient hint level will be displayed.
	/// </para>
	/// See also: <seealso cref="PromptStyler.HintLevel"/>
	/// </summary>
	/// <returns>this</returns>
	/// <exception cref="ArgumentException">minRequiredLevel is <see cref="PromptHintLevel.None"/></exception>
	public Prompt<T> AddHint(PromptHint hint) {

		if (hint.Level == PromptHintLevel.None) {
			throw new ArgumentException(
					$"Hint level {PromptHintLevel.None} is reserved to disable all hints. " +
					$"Please use {PromptHintLevel.Minimal} or higher.",
					nameof(hint)
				);
		}

		hints.Add(hint);
		return this;
	}

	/// <inheritdoc cref="AddHint(PromptHint)"/>
	public Prompt<T> AddHint(string hint, PromptHintLevel minRequiredLevel) {
		return AddHint(new PromptHint { Text = hint, Level = minRequiredLevel });
	}

	#endregion

	#region //// Parser

	IFormatProvider? formatProvider = DefaultFormatProvider;
	Func<string, IFormatProvider?, T>? ThrowingParser;

	/// <summary>
	/// Sets the parser used by the textPrompt.
	/// If provided input cannot be parsed, the parser should
	/// throw an exception. See <see cref="AddValidator(Action{T})"/> 
	/// for list of exceptions caught during user input.
	/// </summary>
	/// <exception cref="ArgumentNullException">throwingParser is null</exception>
	/// <returns>this</returns>
	public Prompt<T> SetParser(Func<string, IFormatProvider?, T> throwingParser) {

		ArgumentNullException.ThrowIfNull(throwingParser, nameof(throwingParser));

		ThrowingParser = throwingParser;
		return this;
	}

	/// <inheritdoc cref="SetParser(Func{string, IFormatProvider?, T})"/>
	public Prompt<T> SetParser(Func<string, IFormatProvider?, T> throwingParser, IFormatProvider? formatProvider) {
		SetParser(throwingParser);
		SetParserFormat(formatProvider);
		return this;
	}

	/// <summary>Sets <see cref="IFormatProvider"/> to be used by the parser.</summary>
	/// <returns>this</returns>
	public Prompt<T> SetParserFormat(IFormatProvider? formatProvider) {
		this.formatProvider = formatProvider;
		return this;
	}

	/// <summary>
	/// Default format used by the parser.
	/// Is <see cref="CultureInfo.InvariantCulture"/>
	/// </summary>
	public static IFormatProvider DefaultFormatProvider => CultureInfo.InvariantCulture;

	#endregion

	#region //// Validator

	Action<T>? ThrowingValidator;

	/// <summary>
	/// Adds a throwing validator for user input.
	/// On invalid input the provided validator should throw
	///	one of following exceptions:
	///	<see cref="FormatException"/>,
	///	<see cref="OverflowException"/>,
	///	<see cref="ArgumentException"/>,
	///	<see cref="ArgumentOutOfRangeException"/>,
	///	<see cref="PathTooLongException"/>,
	///	<see cref="NotSupportedException"/>,
	///	<see cref="NotImplementedException"/>.
	///	Those exceptions will be caught and the user will be
	///	prompted to input something else.
	/// </summary>
	/// <returns>this</returns>
	[SuppressMessage("Style", "IDE0054:Use compound assignment", Justification = "Explicitly show a new delegate is created")]
	public Prompt<T> AddValidator(Action<T> throwingValidator) {

		if (throwingValidator is null) return this;

		if (ThrowingValidator is null) {
			ThrowingValidator = throwingValidator;
		} else {
			ThrowingValidator = ThrowingValidator + throwingValidator;
		}

		return this;
	}

	#endregion

	/// <summary>
	/// Displays the text prompt and asks the user to input a value.
	///	If input is not valid the user will be prompted again.
	/// </summary>
	/// <returns>Value given by the user.</returns>
	/// <exception cref="InvalidOperationException">No parser is given</exception>
	public T Display() {

		// Guards
		if (ThrowingParser is null) {
			throw new InvalidOperationException("Cannot query a value without a parser");
		}

		do {

			// Stylize prompt text and add hints
			string styledPromptText = PromptStyler.MakePromptDisplayString(textPrompt, hints);

			try {

				// Read line
				Console.Write(styledPromptText);
				string input = Console.ReadLine() ?? "";

				// Parse and validate
				T value = ThrowingParser(input, formatProvider);
				if (ThrowingValidator is not null) ThrowingValidator(value);

				// Return
				return value;

			} catch (Exception ex) {

				if (
					ex is FormatException
					|| ex is OverflowException
					|| ex is ArgumentException
					|| ex is ArgumentOutOfRangeException
					|| ex is PathTooLongException
					|| ex is NotSupportedException
					|| ex is NotImplementedException
				) {
					Console.WriteLine($"Invalid input: {ex.Message}");
				} else {
					throw;
				}

			}

		} while (true);

	}

}
