using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;


namespace Rephidock.ConsolePrompts;


/// <summary>
/// A class that holds all the style information from Prompts.
/// </summary>
public static class PromptStyler {

	#region //// Text prompt

	/// <summary>Text to display when no text prompt is given.</summary>
	public static string NullPromptDisplay { get; set; } = "> ";

	/// <summary>
	/// Format for the text prompts if
	/// there are no hints to be displayed.
	/// </summary>
	/// <remarks>
	/// {0} -- text prompt.
	/// </remarks>
	public static string PromptFormatNoHints { get; set; } = "{0}: ";

	/// <summary>
	/// Format for the text prompts.
	/// </summary>
	/// <remarks>
	/// {0} -- text prompt.
	/// {1} -- hints.
	/// </remarks>
	public static string PromptFormat { get; set; } = "{0} ({1}): ";

	/// <summary>
	/// The separator between hints used for prompt formatting
	/// </summary>
	public static string HintSeparator { get; set; } = ", ";

	/// <summary>
	/// Creates a formatted display prompt,
	/// taking hints into account.
	/// </summary>
	internal static string MakePromptDisplayString(string? textPrompt, IEnumerable<string> hintTexts) {
		
		// Check for null prompt
		if (string.IsNullOrWhiteSpace(textPrompt)) {
			return NullPromptDisplay;
		}

		// Get hint texts
		string hintsString = string.Join(HintSeparator, hintTexts);

		// Format prompt display text
		if (string.IsNullOrWhiteSpace(hintsString)) {
			return string.Format(PromptFormatNoHints, textPrompt);
		} else {
			return string.Format(PromptFormat, textPrompt, hintsString);
		}

	}

	#endregion

	#region //// Invalid input

	/// <summary>
	/// Format used for invalid input message.
	/// </summary>
	/// <remarks>
	/// {0} -- Exception message.
	/// </remarks>
	public static string InvalidInputFormat { get; set; } = "Invalid input: {0}";

	/// <summary>
	/// Creates a formatted invalid input message
	/// </summary>
	internal static string MakeInvalidInputString(Exception ex) {
		return string.Format(InvalidInputFormat, ex.Message);
	}

	#endregion

	#region //// Hints

	/// <summary>
	/// <para>
	/// Current hint level. Only hints with this level or lower will be displayed.
	/// </para>
	/// <para>
	/// <see cref="PromptHintLevel.None"/> is the lowest and disables all hints.
	/// </para>
	/// <para>
	/// <see cref="PromptHintLevel.Standard"/> by default.
	/// </para>
	/// </summary>
	public static PromptHintLevel HintLevel { get; set; } = PromptHintLevel.Standard;

	/// <summary>
	/// Enable or disable types as first hints.
	/// Note that type hints are more technical.
	/// False by default.
	/// </summary>
	/// <remarks>
	/// Type hints are considered to have hint level <see cref="PromptHintLevel.Standard"/>
	/// </remarks>
	public static bool TypeHintsEnabled { get; set; } = false;

	/// <summary>
	/// Hint level for type hints.
	/// See also <see cref="TypeHintsEnabled"/>
	/// </summary>
	public const PromptHintLevel TypeHintsLevel = PromptHintLevel.Standard;

	/// <summary>
	/// Filters given hints based on <see cref="HintLevel"/> and grabs only hint texts.
	/// Empty and whitespace only texts are also skipped.
	/// </summary>
	internal static IEnumerable<string> FilterHints(this IEnumerable<PromptHint> hints) {

		return hints
			.Where(hint => HintLevel >= hint.Level)
			.Where(hint => !string.IsNullOrWhiteSpace(hint.Text))
			.Select(hint => hint.Text);
	}

	/// <summary>Tries to prepend a type hint to given hints if hints are enabled.</summary>
	/// <typeparam name="THint">The type, the hint of which is to be prepended.</typeparam>
	/// <returns>A new IEnumerable with prepended type hint or given hints without changes.</returns>
	internal static IEnumerable<PromptHint> HintsTryPrependTypeHint<THint>(this IEnumerable<PromptHint> hints) {

		if (!TypeHintsEnabled) return hints;

		string hintName = HintStrings.TypeHintRenamingTable.GetValueOrDefault(typeof(THint), typeof(THint).Name);
		return hints.Prepend(new PromptHint { Level = TypeHintsLevel, Text = hintName });
	}

	#endregion

	#region //// Hint Strings

	/// <summary>
	/// Standard strings for hints added to the <see cref="Prompt{T}"/> instance.
	/// </summary>
	public static class HintStrings {

		#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

		#region //// Boolean

		public static string BoolDefaultTrue { get; set; } = "Y/n";
		public static string BoolDefaultFalse { get; set; } = "y/N";

		#endregion

		#region //// Types

		/// <summary>
		/// Dictionary that is used to change the type hint
		/// displayed for specific types.
		/// Type inheritance is not accounted for.
		/// If a type is not present, type's Name will be used.
		/// </summary>
		public readonly static Dictionary<Type, string> TypeHintRenamingTable = new() {
			{ typeof(float), "Float" },
			{ typeof(BigInteger), "Int" },
			{ typeof(DateOnly), "Date" },
			{ typeof(TimeOnly), "Time" }
		};

		#endregion

		#region //// Strings

		/// <remarks>
		/// {0} -- Character count or range string
		/// </remarks>
		public static string LengthFormat { get; set; } = "{0} characters";

		public static string NotEmpty { get; set; } = "not empty";

		public static string NotWhitespace { get; set; } = "not empty";

		#endregion

		#region //// Path

		public static string Path { get; set; } = "filesystem path";
		public static string FilePath { get; set; } = "path a file";
		public static string DirectoryPath { get; set; } = "path to a directory";

		public static string MustExist { get; set; } = "must exist";

		#endregion

		#region //// Numeric

		public static string NotInfinite { get; set; } = "not infinite";
		public static string NotNan { get; set; } = "not NaN";
		public static string Finite { get; set; } = "finite";

		/// <remarks>
		/// {0} -- Low bound.
		/// {1} -- High bound.
		/// </remarks>
		public static string RangeFormat { get; set; } = "{0}..{1}";

		/// <remarks>
		/// {0} -- Low bound.
		/// </remarks>
		public static string NoLessThanFormat { get; set; } = "{0}..";

		/// <remarks>
		/// {0} -- High bound.
		/// </remarks>
		public static string NoGreaterThanFormat { get; set; } = "..{0}";

		#endregion

		#pragma warning restore CS1591

	}

	/// <summary>
	/// Returns a hint for a numeric range.
	/// Either of the bounds can be <see langword="null"/> to indicate no bound.
	/// </summary>
	/// <remarks>
	/// Returns an empty string if both bounds are set to <see langword="null"/>.
	/// </remarks>
	internal static string MakeRangeHintString<T>(T? min, T? max) where T : struct, INumber<T> {
	
		if (min.HasValue && max.HasValue) {
			return string.Format(HintStrings.RangeFormat, min.Value, max.Value);
		}

		if (min.HasValue) {
			return string.Format(HintStrings.NoLessThanFormat, min.Value);
		}

		if (max.HasValue) {
			return string.Format(HintStrings.NoGreaterThanFormat, max.Value);
		}

		return "";
	}

	#endregion

}
