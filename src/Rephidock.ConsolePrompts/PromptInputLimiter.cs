using System;
using System.IO;
using System.Text;
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

		// Define validator
		void Validator(string value) {
			if (value.Length != length) {
				throw new ArgumentException("Input is not of the correct length");
			}
		}

		// Add validator
		return prompt.AddValidator(Validator);
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

		// Define validator
		void Validator(string value) {
			if (value.Length < minLength || (maxLength.HasValue && value.Length > maxLength)) {
				throw new ArgumentException("Input length is out of range of value values");
			}
		}

		// Add validator
		return prompt.AddValidator(Validator);
	}

	/// <summary>Denies empty input.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	public static Prompt<string> DisallowEmpty(this Prompt<string> prompt) {

		static void Validator(string value) {
			if (value == "") {
				throw new ArgumentException("Input cannot be empty");
			}
		}

		return prompt.AddValidator(Validator);
	}

	/// <summary>Denies input that is whitespace only.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	public static Prompt<string> DisallowOnlyWhiteSpace(this Prompt<string> prompt) {

		static void Validator(string value) {
			if (string.IsNullOrWhiteSpace(value)) {
				throw new ArgumentException("Input cannot be empty or whitespace");
			}
		}

		return prompt.AddValidator(Validator);
	}

	#endregion

	#region //// Path Limits

	/// <summary>Limits input to be a valid filesystem path.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	public static Prompt<string> OfPath(this Prompt<string> prompt) {

		static void Validator(string value) {

			if (value.Trim().Length == 0) throw new ArgumentException("Path is empty.");

			if (value.IndexOfAny(Path.GetInvalidPathChars()) != -1) {
				throw new ArgumentException("Path contains invalid characters.");
			}

			Path.GetFullPath(value); // This call may throw exceptions

		}

		// Add validator
		return prompt.AddValidator(Validator);
	}

	/// <summary>Limits input to be a path to a file.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	public static Prompt<string> OfFilePath(this Prompt<string> prompt, bool mustExist = false) {

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

		// Add validator
		return prompt.OfPath().AddValidator(Validator);
	}

	/// <summary>Limits input to be a path to a directory.</summary>
	/// <returns>The <see cref="Prompt{string}"/> instance operated on.</returns>
	public static Prompt<string> OfDirectoryPath(this Prompt<string> prompt, bool mustExist = false) {

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

		// Add validator
		return prompt.OfPath().AddValidator(Validator);
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

		// Define validator
		void Validator(T value) {
			#pragma warning disable CA2208 // Instantiate argument exceptions correctly
			if (min is not null && value < min) throw new ArgumentOutOfRangeException();
			if (max is not null && value > max) throw new ArgumentOutOfRangeException();
			#pragma warning restore CA2208 // Instantiate argument exceptions correctly
		}

		// Add validator
		return prompt.AddValidator(Validator);
	}

	/// <summary>Denies input of infinite values.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> DisallowInfinities<T>(this Prompt<T> prompt) where T : INumber<T> {

		static void Validator(T value) {
			if (T.IsInfinity(value)) {
				throw new ArgumentException("Value is too large or to small to be considered finite");
			}
		}

		return prompt.AddValidator(Validator);
	}

	/// <summary>Denies input of NaN.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> DisallowNaN<T>(this Prompt<T> prompt) where T : INumber<T> {

		static void Validator(T value) {
			if (T.IsNaN(value)) {
				throw new ArgumentException("Value cannot be NaN");
			}
		}

		return prompt.AddValidator(Validator);
	}

	/// <summary>Forces input value to be finite and not NaN.</summary>
	/// <returns>The <see cref="Prompt{T}"/> instance operated on.</returns>
	public static Prompt<T> ForceFinite<T>(this Prompt<T> prompt) where T : INumber<T> {

		static void Validator(T value) {
			if (!T.IsFinite(value)) {
				throw new ArgumentException("Value must be finite");
			}
		}

		return prompt.AddValidator(Validator);
	}

	#endregion

}
