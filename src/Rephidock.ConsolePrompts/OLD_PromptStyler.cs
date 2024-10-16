using System.Numerics;


namespace Rephidock.ConsolePrompts;


/// <summary>
/// A class that holds all the style information for the Prompts.
/// </summary>
public static class TO_BE_REMOVED_PromptStyler {

	/// <summary>
	/// Standard strings for hints added to the <see cref="Prompt{T}"/> instance.
	/// </summary>
	public static class HintStrings {

		#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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

		#region //// IEquatable

		/// <remarks>
		/// {0} -- excluded value
		/// </remarks>
		public static string NotEqualsFormat { get; set; } = "!= {0}";

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

}
