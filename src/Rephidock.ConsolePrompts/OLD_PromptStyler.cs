

namespace Rephidock.ConsolePrompts;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class TO_BE_REMOVED_PromptStyler {

	public static class HintStrings {

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

		#endregion

		#region //// IEquatable

		/// <remarks>
		/// {0} -- excluded value
		/// </remarks>
		public static string NotEqualsFormat { get; set; } = "!= {0}";

		#endregion

	}

}

#pragma warning restore CS1591
