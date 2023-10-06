using System;
using System.IO;
using System.Text;
using System.Numerics;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;


namespace Rephidock.AdeptConsole;


/// <summary>
/// A textPrompt for a value to be shown to the user.
/// </summary>
public sealed class Prompt<T> {

	#region //// Text Prompt

	/// <summary>
	/// Text prompt to be tweaked and displayed,
	/// or <see langword="null"/> for default textPrompt with no tweaks.
	/// </summary>
	string? textPrompt = null;

	/// <summary>
	/// Default display textPrompt.
	/// Does not get tweaked.
	/// </summary>
	public const string DefaultTextPrompt = "> ";

	/// <summary>
	/// Sets the text prompt to be displayed on query.
	/// If the <paramref name="textPrompt"/> is whitespace, empty or <see langword="null"/>,
	/// the <see cref="DefaultTextPrompt"/> is used.
	/// </summary>
	/// <returns>this</returns>
	public Prompt<T> SetPrompt(string? textPrompt) {

		if (IsPromptDefault(textPrompt)) {
			this.textPrompt = null;
		} else {
			this.textPrompt = textPrompt;
		}

		return this;
	}

	/// <summary>
	/// Returns true for default prompts
	/// </summary>
	static bool IsPromptDefault(string? textPrompt) {
		return string.IsNullOrWhiteSpace(textPrompt) || textPrompt == DefaultTextPrompt;
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

	/// <summary>Removes validators from this textPrompt.</summary>
	/// <returns>this</returns>
	public Prompt<T> RemoveValidator() {
		ThrowingValidator = null;
		return this;
	}

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

			// Tweak the textPrompt
			string tweakedPrompt = IsPromptDefault(textPrompt) ? DefaultTextPrompt : $"{textPrompt}: ";

			try {

				// Read line
				Console.Write(tweakedPrompt);
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
