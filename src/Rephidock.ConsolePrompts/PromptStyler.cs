using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;


namespace Rephidock.ConsolePrompts;


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
	/// /// <remarks>
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
	public static string MakePromptDisplayString(string? textPrompt, IReadOnlyList<PromptHint> hints) {
		
		// Check for null prompt
		if (string.IsNullOrWhiteSpace(textPrompt)) {
			return NullPromptDisplay;
		}

		// Get hint texts
		string hintsString = string.Join(HintSeparator, FilterHints(hints));

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
	public static string MakeInvalidInputString(Exception ex) {
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
	/// Filters given hints based on <see cref="HintLevel"/> and grabs only hint texts.
	/// Empty and whitespace only texts are also skipped.
	/// </summary>
	public static IEnumerable<string> FilterHints(IReadOnlyList<PromptHint> hints) {

		return hints
			.Where(hint => HintLevel >= hint.Level)
			.Where(hint => !string.IsNullOrWhiteSpace(hint.Text))
			.Select(hint => hint.Text);
	}

	#endregion

	#region //// Hint Strings

	/// <summary>
	/// Hint strings used by <see cref="PromptInputLimiter"/>.
	/// </summary>
	public static class HintStrings {

		#region //// Boolean

		public static string BoolDefaultTrue { get; set; } = "Y/n";
		public static string BoolDefaultFalse { get; set; } = "y/N";

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

	}

	/// <summary>
	/// Returns a hint for a numeric range.
	/// Either of the bounds can be <see langword="null"/> to indicate no bound.
	/// </summary>
	public static string MakeRangeHintString<T>(T? min, T? max) where T : struct, INumber<T> {
	
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
