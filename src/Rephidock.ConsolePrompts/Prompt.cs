using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;


namespace Rephidock.ConsolePrompts;


/// <summary>
/// <para>
/// A class for creating instances of <see cref="Prompt{T}"/>
/// for <see cref="Console"/> quickly.
/// </para>
/// <para>
/// For advanced use or for multiple prompts <see cref="Prompter"/>
/// is recommended.
/// </para>
/// </summary>
public static class Prompt {

	#region //// Shortcuts

	/// <inheritdoc cref="Prompter.PromptFor{T}(string?, Func{string, IFormatProvider?, T})"/>
	public static Prompt<T> For<T>(string? textPrompt, Func<string, IFormatProvider?, T> parser) {
		return new Prompter().PromptFor<T>(textPrompt, parser);
	}

	/// <inheritdoc cref="Prompter.PromptFor{T}(string?)"/>
	public static Prompt<T> For<T>(string? textPrompt = null) where T : IParsable<T> {
		return new Prompter().PromptFor<T>(textPrompt);
	}

	/// <inheritdoc cref="Prompter.PromptFor{T}(string?, bool)"/>
	public static Prompt<T> For<T>(string? textPrompt, bool forceFinite) where T : struct, INumber<T> {
		return new Prompter().PromptFor<T>(textPrompt, forceFinite);
	}

	/// <inheritdoc cref="Prompter.PromptForString(string?, bool)"/>
	public static Prompt<string> ForString(string? textPrompt = null, bool trim = true) {
		return new Prompter().PromptForString(textPrompt, trim);
	}

	/// <inheritdoc cref="Prompter.PromptForBool(string?, bool)"/>
	public static Prompt<bool> ForBool(string? textPrompt = null, bool defaultValue = false) {
		return new Prompter().PromptForBool(textPrompt, defaultValue);
	}

	#endregion

}


/// <summary>
/// A prompt (query) for a value to be shown to the user.
/// </summary>
/// <remarks>
/// Each prompt has dedicated text query, hints, parser and validators;
/// everything else is read from the parent <see cref="Prompter"/>.
/// </remarks>
public sealed class Prompt<T> {

	#region //// Parent + Constructor

	internal Prompt(Prompter prompter) {
		this.prompter = prompter;
	}

	// Prompter reference
	readonly Prompter prompter;

	#endregion

	#region //// Text Prompt

	/// <summary>Text prompt to be displayed.</summary>
	string? textPrompt = null;

	/// <summary>
	/// Sets the text prompt to be displayed on query.
	/// </summary>
	/// <remarks>
	/// If text prompt is empty, whitespace or <see langword="null"/>,
	/// <see cref="Prompter.NullPromptFormat"/> will be used or its no-hints equivalent.
	/// </remarks>
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
	/// Removes text prompt from the current <see cref="Prompt"/>.
	/// Equivalent of setting text prompt to whitespace or <see langword="null"/>.
	/// </para>
	/// <para>
	/// See also: <see cref="SetPrompt"/>.
	/// </para>
	/// </summary>
	/// <returns>this</returns>
	public Prompt<T> RemovePrompt() => SetPrompt(null);

	#endregion

	#region //// Hints

	private readonly List<PromptHint> hints = new();

	/// <summary>Adds a hint to be displayed with the prompt.</summary>
	/// <returns>this</returns>
	public Prompt<T> AddHint(PromptHint hint) {
		hints.Add(hint);
		return this;
	}

	/// <summary>Adds a type hint to be displayed with the prompt.</summary>
	/// <remarks>Added hint has type of <see cref="PromptHintTypes.TypeHint"/></remarks>
	/// <returns>this</returns>
	public Prompt<T> AddTypeHint() {
		return AddHint(new PromptHint(PromptHintTypes.TypeHint));
	}

	/// <summary>Adds a text hint to be displayed with the prompt.</summary>
	/// <remarks>Added hint has type of <see cref="PromptHintTypes.BasicText"/></remarks>
	/// <returns>this</returns>
	public Prompt<T> AddAdditionalText(string hintText) {
		return AddHint(new PromptHint(PromptHintTypes.BasicText, hintText));
	}

	/// <summary>
	/// Removes the last added hint.
	/// Does nothing if no hints were previously added.
	/// </summary>
	/// <returns>this</returns>
	public Prompt<T> RemoveLastHint() {
		
		if (hints.Count > 0) {
			hints.RemoveAt(hints.Count - 1);
		}

		return this;
	}

	/// <summary>Removes all added hints.</summary>
	/// <returns>this</returns>
	public Prompt<T> RemoveAllHints() {
		hints.Clear();
		return this;
	}

	/// <summary>Removes all added hints which match a predicate</summary>
	/// <returns>this</returns>
	public Prompt<T> RemoveHintsMatching(Predicate<PromptHint> predicate) {
		hints.RemoveAll(predicate);
		return this;
	}

	#endregion

	#region //// Parser and Validator

	Func<string, IFormatProvider?, T>? ThrowingParser;

	/// <summary>
	/// <para>
	/// Sets the parser used by this instance of <see cref="Prompt{T}"/>.
	/// </para>
	/// <para>
	/// If provided input cannot be parsed, the parser should throw
	/// a common exception, like <see cref="FormatException"/> or <see cref="ArgumentException"/>
	/// for it to be caught during the querying.
	/// See <see cref="AddValidator(Action{T})"/> for a full list of exceptions caught.
	/// </para>
	/// <para>
	/// When the exception is caught the user will be prompted to input a value again.
	/// </para>
	/// </summary>
	/// <exception cref="ArgumentNullException"><paramref name="throwingParser"/> is <see langword="null"/></exception>
	/// <returns>this</returns>
	public Prompt<T> SetParser(Func<string, IFormatProvider?, T> throwingParser) {

		ArgumentNullException.ThrowIfNull(throwingParser, nameof(throwingParser));

		ThrowingParser = throwingParser;
		return this;
	}

	Action<T> ThrowingValidator = (T _) => { };

	/// <summary>
	/// <para>
	/// Adds a throwing validator for user input.
	/// </para>
	/// <para>
	/// On invalid input the provided validator should throw a <see cref="PromptInputException"/>
	/// or one of other exceptions caught (see below).
	/// </para>
	/// <para>
	/// When the exception is caught the user will be prompted to input a value again.
	/// </para>
	/// <para>
	/// The following exceptions are also caught:
	///	<see cref="FormatException"/>,
	///	<see cref="ArgumentException"/>,
	///	<see cref="ArgumentOutOfRangeException"/>,
	///	<see cref="OverflowException"/>,
	///	<see cref="PathTooLongException"/>,
	///	<see cref="NotSupportedException"/>,
	///	<see cref="NotImplementedException"/>.
	///	</para>
	/// </summary>
	/// <returns>this</returns>
	public Prompt<T> AddValidator(Action<T> throwingValidator) {
		ThrowingValidator += throwingValidator;
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
			string styledPromptText = prompter.FormatPromptDisplay(textPrompt, hints);

			try {

				// Write text prompt and read line
				prompter.OutputStream.Write(styledPromptText);
				string input = prompter.InputStream.ReadLine() ?? "";

				// Parse and validate
				return ParseAndValidate(input);

			} catch (Exception ex) {

				if (
					ex is PromptInputException
					|| ex is FormatException
					|| ex is OverflowException
					|| ex is ArgumentException
					|| ex is ArgumentOutOfRangeException
					|| ex is PathTooLongException
					|| ex is NotSupportedException
					|| ex is NotImplementedException
				) {
					prompter.OutputStream.WriteLine(TO_BE_REMOVED_PromptStyler.MakeInvalidInputString(ex));
				} else {
					throw;
				}

			}

		} while (true);

	}

	/// <summary>
	/// Parses given input and validates it using given throwing parser and validators.
	/// <b>Will</b> throw if invalid input is given.
	/// </summary>
	/// <remarks>
	/// Primarily made for <see cref="Display()"/> and for testing.
	/// </remarks>
	/// <returns>Parsed input</returns>
	public T ParseAndValidate(string input) {

		// Guards
		if (ThrowingParser is null) {
			throw new InvalidOperationException("Cannot parse input without a parser.");
		}

		// Parse and validate
		T value = ThrowingParser(input, prompter.FormatProvider);
		ThrowingValidator(value);

		// Return
		return value;

	}

}
