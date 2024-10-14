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
public sealed class Prompt<T> {

	#region //// Parent + Constructor

	internal Prompt(Prompter prompter) {
		this.prompter = prompter;
	}

	// Prompter reference
	readonly Prompter prompter;

	#endregion

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
	/// Removes text prompt from the current <see cref="Prompt"/>.
	/// Equivalent of setting text prompt to whitespace or <see langword="null"/>.
	/// </para>
	/// <para>
	/// See also: <see cref="PromptStyler.NullPromptDisplay"/>.
	/// </para>
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
	/// <para>
	/// See also: <see cref="PromptStyler.HintLevel"/>
	/// </para>
	/// </summary>
	/// <returns>this</returns>
	/// <exception cref="ArgumentException">Hint level is <see cref="PromptHintLevel.None"/></exception>
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

	#endregion

	#region //// Parser

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

	#endregion

	#region //// Validator

	Action<T> ThrowingValidator = (T _) => { };

	/// <summary>
	/// <para>
	/// Adds a throwing validator for user input.
	/// </para>
	/// <para>
	/// On invalid input the provided validator should throw a <see cref="PromptInputException"/>
	/// or one of other exceptions caught (see below).
	/// When the exception is caught the user will be prompted to input a value again
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

			// Filter and convert hints to strings
			IEnumerable<string> hintTexts =
				hints
				.HintsTryPrependTypeHint<T>()
				.FilterHints();

			// Stylize prompt text and add hints
			string styledPromptText = PromptStyler.MakePromptDisplayString(textPrompt, hintTexts);

			try {

				// Write text prompt and read line
				Console.Write(styledPromptText);
				string input = Console.ReadLine() ?? "";

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
					Console.WriteLine(PromptStyler.MakeInvalidInputString(ex));
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
