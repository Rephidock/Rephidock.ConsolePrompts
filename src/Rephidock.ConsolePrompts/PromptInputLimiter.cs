using System;
using System.IO;
using System.Numerics;


namespace Rephidock.ConsolePrompts;


public static class PromptInputLimiter {

	#region //// String Limits

	/// <summary>Limits input to be of specified exact length.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Provided length is negative</exception>
	public static Prompt<string> OfLength(this Prompt<string> prompt, int length) {

		// Guards
		if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

		// Define and add validator
		void Validator(string value) {
			if (value.Length != length) {
				throw new ArgumentException("Input is not of the correct length");
			}
		}
		
		prompt.AddValidator(Validator);

		// Add hint
		string hintText = string.Format(PromptStyler.HintStrings.LengthFormat, length);
		prompt.AddHint(hintText, PromptHintLevel.Standard);

		// Return
		return prompt; 
	}

	/// <summary>
	/// Limits input to be of length within given bounds.
	/// Set <paramref name="minLength"/> to 0 for no lower bound.
	/// Set <paramref name="maxLength"/> to <see langword="null"/> for no upper bound.
	/// </summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
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

		// Define and add validator
		void Validator(string value) {
			if (value.Length < minLength || (maxLength.HasValue && value.Length > maxLength)) {
				throw new ArgumentException("Input length is out of range of value values");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		string rangeText = PromptStyler.MakeRangeHintString(minLength, maxLength);
		string hintText = string.Format(PromptStyler.HintStrings.LengthFormat, rangeText);
		prompt.AddHint(hintText, PromptHintLevel.Standard);

		// Return
		return prompt;
	}

	/// <summary>Denies empty input.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	public static Prompt<string> DisallowEmpty(this Prompt<string> prompt) {

		// Define and add validator
		static void Validator(string value) {
			if (value == "") {
				throw new ArgumentException("Input cannot be empty");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(PromptStyler.HintStrings.NotEmpty, PromptHintLevel.Verbose);

		// Return
		return prompt;
	}

	/// <summary>Denies input that is whitespace only.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	public static Prompt<string> DisallowOnlyWhiteSpace(this Prompt<string> prompt) {

		// Define and add validator
		static void Validator(string value) {
			if (string.IsNullOrWhiteSpace(value)) {
				throw new ArgumentException("Input cannot be empty or whitespace");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(PromptStyler.HintStrings.NotWhitespace, PromptHintLevel.Verbose);

		// Return
		return prompt;
	}

	#endregion

	#region //// Path Limits

	/// <summary>Limits input to be a valid filesystem path.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	public static Prompt<string> OfPath(this Prompt<string> prompt) {

		// Define and add validator
		static void Validator(string value) {

			if (value.Trim().Length == 0) throw new ArgumentException("Path is empty.");

			if (value.IndexOfAny(Path.GetInvalidPathChars()) != -1) {
				throw new ArgumentException("Path contains invalid characters.");
			}

			Path.GetFullPath(value); // This call may throw exceptions

		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(PromptStyler.HintStrings.Path, PromptHintLevel.Verbose);

		// Return
		return prompt;
	}

	/// <summary>Limits input to be a path to a file.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	public static Prompt<string> OfFilePath(this Prompt<string> prompt, bool mustExist = false) {

		// Define and add validator
		void Validator(string value) {

			// Throw if is not a file
			if (Directory.Exists(value)) {
				throw new ArgumentException("Given path is for a directory, not a file.");
			}

			// Throw if does not exist
			if (mustExist && !File.Exists(value)) {
				throw new ArgumentException("File does not exist.");
			}

		}

		prompt.OfPath().AddValidator(Validator);

		// Add hint
		prompt.RemoveLastHint(); // remove HintStrings.Path hint
		prompt.AddHint(PromptStyler.HintStrings.FilePath, PromptHintLevel.Verbose);

		if (mustExist) {
			prompt.AddHint(PromptStyler.HintStrings.MustExist, PromptHintLevel.Verbose);
		}

		// Return
		return prompt;
	}

	/// <summary>Limits input to be a path to a directory.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	public static Prompt<string> OfDirectoryPath(this Prompt<string> prompt, bool mustExist = false) {

		// Define and add validator
		void Validator(string value) {

			// Throw if is not a directory
			if (File.Exists(value)) {
				throw new ArgumentException("Given path is for a file, not a directory.");
			}

			// Throw if does not exist
			if (mustExist && !Directory.Exists(value)) {
				throw new ArgumentException("Directory does not exist.");
			}

		}

		prompt.OfPath().AddValidator(Validator);

		// Add hint
		prompt.RemoveLastHint(); // remove HintStrings.Path hint
		prompt.AddHint(PromptStyler.HintStrings.DirectoryPath, PromptHintLevel.Verbose);

		if (mustExist) {
			prompt.AddHint(PromptStyler.HintStrings.MustExist, PromptHintLevel.Verbose);
		}

		// Return
		return prompt;
	}

	#endregion

	#region //// Numeric Limits

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
			#pragma warning disable CA2208 // Instantiate argument exceptions correctly
			if (min is not null && value < min) throw new ArgumentOutOfRangeException();
			if (max is not null && value > max) throw new ArgumentOutOfRangeException();
			#pragma warning restore CA2208 // Instantiate argument exceptions correctly
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(PromptStyler.MakeRangeHintString(min, max), PromptHintLevel.Standard);

		// Return
		return prompt;
	}

	/// <summary>Denies input of infinite values.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> DisallowInfinities<T>(this Prompt<T> prompt) where T : INumber<T> {

		// Define and add validator
		static void Validator(T value) {
			if (T.IsInfinity(value)) {
				throw new ArgumentException("Value is too large or to small to be considered finite");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(PromptStyler.HintStrings.NotInfinite, PromptHintLevel.Verbose);

		// Return
		return prompt;
	}

	/// <summary>Denies input of NaN.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> DisallowNaN<T>(this Prompt<T> prompt) where T : INumber<T> {

		// Define and add validator
		static void Validator(T value) {
			if (T.IsNaN(value)) {
				throw new ArgumentException("Value cannot be NaN");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(PromptStyler.HintStrings.NotNan, PromptHintLevel.Verbose);

		// Return
		return prompt;
	}

	/// <summary>Forces input value to be finite and not NaN.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> ForceFinite<T>(this Prompt<T> prompt) where T : INumber<T> {

		// Define and add validator
		static void Validator(T value) {
			if (!T.IsFinite(value)) {
				throw new ArgumentException("Value must be finite");
			}
		}

		prompt.AddValidator(Validator);

		// Add hint
		prompt.AddHint(PromptStyler.HintStrings.Finite, PromptHintLevel.Verbose);

		// Return
		return prompt;
	}

	#endregion

}
