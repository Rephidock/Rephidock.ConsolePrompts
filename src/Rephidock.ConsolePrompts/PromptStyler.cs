using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;


namespace Rephidock.ConsolePrompts;


static class PromptStyler {

	#region //// Text prompt

	/// <summary>Text to display when no text prompt is given.</summary>
	public static string NullPromptDisplay = "> ";

	public static string MakePromptDisplayString(string? textPrompt, IReadOnlyList<PromptHint> hints) {
		
		// Check for null prompt
		if (string.IsNullOrWhiteSpace(textPrompt)) {
			return NullPromptDisplay;
		}

		string hintsString = GetHintsString(hints);

		if (string.IsNullOrWhiteSpace(hintsString)) {
			return $"{textPrompt}: ";
		} else {
			return $"{textPrompt} [{hintsString}]: ";
		}

	}

	#endregion

	#region //// Hints

	/// <summary>
	/// <para>
	/// Current hint level. Only hints with
	/// this level or lower will be displayed.
	/// </para>
	/// <para>
	/// <see cref="PromptHintLevel.None"/> is the lowest and
	/// is reserved to disable all hints.
	/// </para>
	/// <para>
	/// <see cref="PromptHintLevel.Standard"/> by default.
	/// </para>
	/// </summary>
	public static PromptHintLevel HintLevel = PromptHintLevel.Standard;

	/// <summary>
	/// Generates a string comprised of all applicable hint texts.
	/// </summary>
	/// <remarks>
	/// Hint level is taken into account.
	/// Empty and whitespace only texts are skipped.
	/// </remarks>
	public static string GetHintsString(IReadOnlyList<PromptHint> hints) {

		var hintStrings = hints
			.Where(hint => HintLevel >= hint.Level)
			.Where(hint => !string.IsNullOrWhiteSpace(hint.Text))
			.Select(hint => hint.Text);

		return string.Join(", ", hintStrings);
	}

	#endregion

	#region //// Hint Strings

	/// <summary>
	/// Hint strings used by <see cref="PromptInputLimiter"/>.
	/// </summary>
	public static class HintStrings {

		#region //// Boolean

		public static string BoolDefaultTrue = "Y/n";
		public static string BoolDefaultFalse = "y/N";

		#endregion

		#region //// Strings

		/// <remarks>
		/// {0} -- Character count or range string
		/// </remarks>
		public static string LengthFormat = "{0} characters";

		public static string NotEmpty = "not empty";

		public static string NotWhitespace = "not empty";

		#endregion

		#region //// Path

		public static string Path = "filesystem path";
		public static string FilePath = "path a file";
		public static string DirectoryPath = "path to a directory";

		public static string MustExist = "must exist";

		#endregion

		#region //// Numeric

		public static string NotInfinite = "not infinite";
		public static string NotNan = "not NaN";
		public static string Finite = "finite";

		/// <remarks>
		/// {0} -- Low bound.
		/// {1} -- High bound.
		/// </remarks>
		public static string RangeFormat = "{0}..{1}";

		/// <remarks>
		/// {0} -- Low bound.
		/// </remarks>
		public static string NoLessThanFormat = "{0}..";

		/// <remarks>
		/// {0} -- High bound.
		/// </remarks>
		public static string NoGreaterThanFormat = "..{0}";

		#endregion

	}

	/// <summary>
	/// Returns a hint for a numeric range.
	/// Either of the bounds can be <see langword="null"/> to indicate no bound.
	/// </summary>
	public static string GetRangeHintString<T>(T? min, T? max) where T : struct, INumber<T> {
	
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
