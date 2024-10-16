using System;
using System.IO;
using System.Numerics;


namespace Rephidock.ConsolePrompts;


/// <summary>
/// A class containing ways to limits user input.
/// Holds extension methods that return the
/// object operated on to allow fluent syntax.
/// </summary>
public static class PromptInputLimiter {

	#region //// String Limits

	/// <summary>Limits input to be of specified exact length.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Provided length is negative</exception>
	public static Prompt<string> OfLength(this Prompt<string> prompt, int length) {

		// Guards
		if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

		// Define and add validator
		void Validator(string value) {
			if (value.Length != length) {
				throw new PromptInputException($"Input must be {length} characters long.");
			}
		}
		
		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(new PromptHint<int>(PromptHintKeys.StringLength, length));

		// Return
		return prompt; 
	}

	/// <summary>
	/// Limits input to be of length within given inclusive bounds.
	/// Set <paramref name="minLength"/> to 0 for no lower bound.
	/// Set <paramref name="maxLength"/> to <see langword="null"/> for no upper bound.
	/// </summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Either of provided length bounds are negative</exception>
	public static Prompt<string> OfLength(this Prompt<string> prompt, int minLength, int? maxLength) {

		// Guards
		if (minLength < 0) throw new ArgumentOutOfRangeException(nameof(minLength));

		if (maxLength.HasValue && maxLength.Value < 0) {
			throw new ArgumentOutOfRangeException(nameof(maxLength));
		}

		// Swap min and max if needed
		if (maxLength.HasValue && minLength > maxLength.Value) {
			(minLength, maxLength) = (maxLength.Value, minLength);
		}

		// Check for easier queries
		if (minLength == 0 && !maxLength.HasValue) {
			return prompt;
		}

		if (maxLength.HasValue && minLength == maxLength) {
			return prompt.OfLength(minLength);
		}

		if (minLength == 1 && !maxLength.HasValue) {
			return prompt.DisallowEmpty();
		}

		// Define and add validator
		void Validator(string value) {
			if (value.Length < minLength || (maxLength.HasValue && value.Length > maxLength)) {
				throw new PromptInputException("Input length is out of range of valid values");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(
			new PromptHint<(int?, int?)>(
				PromptHintKeys.StringLength, 
				(minLength, maxLength)
			)
		);

		// Return
		return prompt;
	}

	/// <summary>Denies empty input.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<string> DisallowEmpty(this Prompt<string> prompt) {

		// Define and add validator
		static void Validator(string value) {
			if (value == "") {
				throw new PromptInputException("Input cannot be empty");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(new PromptHint(PromptHintKeys.StringNotEmpty));

		// Return
		return prompt;
	}

	/// <summary>Denies input that is whitespace only.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<string> DisallowOnlyWhiteSpace(this Prompt<string> prompt) {

		// Define and add validator
		static void Validator(string value) {
			if (string.IsNullOrWhiteSpace(value)) {
				throw new PromptInputException("Input cannot be empty or whitespace");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(new PromptHint(PromptHintKeys.StringNotEmptyOrWhitespace));

		// Return
		return prompt;
	}

	#endregion

	#region //// Path Limits

	/// <summary>Limits input to be a valid filesystem path.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<string> OfPath(this Prompt<string> prompt) {

		// Define and add validator
		static void Validator(string value) {

			if (string.IsNullOrWhiteSpace(value)) throw new PromptInputException("Path is empty.");

			if (value.IndexOfAny(Path.GetInvalidPathChars()) != -1) {
				throw new PromptInputException("Path contains invalid characters.");
			}

			Path.GetFullPath(value); // This call may throw exceptions

		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(TO_BE_REMOVED_PromptStyler.HintStrings.Path, PromptHintLevel.Verbose);

		// Return
		return prompt;
	}

	/// <summary>Limits input to be a path to a file.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<string> OfFilePath(this Prompt<string> prompt, bool mustExist = false) {

		// Define and add validator
		void Validator(string value) {

			// Throw if is not a directory
			if (Directory.Exists(value)) {
				throw new PromptInputException("Given path is for a directory, not a file.");
			}

			// Throw if does not exist
			if (mustExist && !File.Exists(value)) {
				throw new PromptInputException("File does not exist.");
			}

		}

		prompt.OfPath().AddValidator(Validator);

		// Add hint
		prompt.RemoveLastHint(); // remove HintStrings.Path hint
		prompt.AddHint(TO_BE_REMOVED_PromptStyler.HintStrings.FilePath, PromptHintLevel.Verbose);

		if (mustExist) {
			prompt.AddHint(TO_BE_REMOVED_PromptStyler.HintStrings.MustExist, PromptHintLevel.Verbose);
		}

		// Return
		return prompt;
	}

	/// <summary>Limits input to be a path to a directory.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<string> OfDirectoryPath(this Prompt<string> prompt, bool mustExist = false) {

		// Define and add validator
		void Validator(string value) {

			// Throw if is not a directory
			if (File.Exists(value)) {
				throw new PromptInputException("Given path is for a file, not a directory.");
			}

			// Throw if does not exist
			if (mustExist && !Directory.Exists(value)) {
				throw new PromptInputException("Directory does not exist.");
			}

		}

		prompt.OfPath().AddValidator(Validator);

		// Add hint
		prompt.RemoveLastHint(); // remove HintStrings.Path hint
		prompt.AddHint(TO_BE_REMOVED_PromptStyler.HintStrings.DirectoryPath, PromptHintLevel.Verbose);

		if (mustExist) {
			prompt.AddHint(TO_BE_REMOVED_PromptStyler.HintStrings.MustExist, PromptHintLevel.Verbose);
		}

		// Return
		return prompt;
	}

	#endregion

	#region //// Numeric Value Limits

	/// <summary>Limits input to be in numeric range.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> OfRange<T>(this Prompt<T> prompt, T? min, T? max) where T : struct, INumber<T> {
		
		// No limits set
		if (min is null && max is null) {
			return prompt;
		}

		// Swap limits if they are in the wrong order
		if (min is not null && max is not null && min > max) {
			(min, max) = (max, min);
		}

		// Define and add validator
		void Validator(T value) {
			if (min is not null && value < min) throw new PromptInputException("Given value is too small");
			if (max is not null && value > max) throw new PromptInputException("Given value is too large");
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(TO_BE_REMOVED_PromptStyler.MakeRangeHintString(min, max), PromptHintLevel.Standard);

		// Return
		return prompt;
	}

	/// <summary>
	/// Limits input by setting a low numeric bound.
	/// If high bound is also needed use <see cref="OfRange{T}(Prompt{T}, T?, T?)"/>
	/// </summary>
	/// <remarks>
	/// Shorthand for / Equivalent to <c>.OfRange(minBound, null)</c>
	/// </remarks>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> NoLessThan<T>(this Prompt<T> prompt, T minBound) where T : struct, INumber<T> {
		return prompt.OfRange(minBound, null);
	}

	/// <summary>
	/// Limits input by setting a high numeric bound.
	/// If low bound is also needed use <see cref="OfRange{T}(Prompt{T}, T?, T?)"/>
	/// </summary>
	/// <remarks>
	/// Shorthand for / Equivalent to <c>.OfRange(null, maxBound)</c>
	/// </remarks>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> NoGreaterThan<T>(this Prompt<T> prompt, T maxBound) where T : struct, INumber<T> {
		return prompt.OfRange(null, maxBound);
	}

	#endregion

	#region //// Numeric Finiteness limits

	/// <summary>Denies input of infinite values.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> DisallowInfinities<T>(this Prompt<T> prompt) where T : INumber<T> {

		// Define and add validator
		static void Validator(T value) {
			if (T.IsInfinity(value)) {
				throw new PromptInputException("Value is too large or to small to be considered finite");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(TO_BE_REMOVED_PromptStyler.HintStrings.NotInfinite, PromptHintLevel.Verbose);

		// Return
		return prompt;
	}

	/// <summary>Denies input of NaN.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> DisallowNaN<T>(this Prompt<T> prompt) where T : INumber<T> {

		// Define and add validator
		static void Validator(T value) {
			if (T.IsNaN(value)) {
				throw new PromptInputException("Value cannot be NaN");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(TO_BE_REMOVED_PromptStyler.HintStrings.NotNan, PromptHintLevel.Verbose);

		// Return
		return prompt;
	}

	/// <summary>Forces input value to be finite and not NaN.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> ForceFinite<T>(this Prompt<T> prompt) where T : INumber<T> {

		// Define and add validator
		static void Validator(T value) {
			if (!T.IsFinite(value)) {
				throw new PromptInputException("Value must be finite");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(TO_BE_REMOVED_PromptStyler.HintStrings.Finite, PromptHintLevel.Verbose);

		// Return
		return prompt;
	}

	#endregion

	#region //// IEquatable

	/// <summary>Limits input by disallowing a specific value.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> NotEqualTo<T>(this Prompt<T> prompt, T exclusion) where T : IEquatable<T> {
		
		// Define and add validator
		void Validator(T value) {
			if (value.Equals(exclusion)) {
				throw new PromptInputException("Given value is excluded from the pool of valid values");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		string hintText = string.Format(TO_BE_REMOVED_PromptStyler.HintStrings.NotEqualsFormat, exclusion);
		prompt.AddHint(hintText, PromptHintLevel.Standard);

		// Return
		return prompt;
	}

	#endregion

}
