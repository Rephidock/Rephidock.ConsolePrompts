using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;


namespace Rephidock.ConsolePrompts;


/// <summary>A listing of default hint handlers.</summary>
public static class PromptHintHandlers {

	#region //// Generic handlers (no designated hint key)

	/// <summary>
	/// A handler that shows raw contents of the hint.
	/// Not included in <see cref="GetAllHandlers"/>.
	/// </summary>
	public static string? DebugHintHandler(PromptHint hint) => hint.ToString();

	/// <summary>
	/// Hint handler that skips all hints provided.
	/// Not included in <see cref="GetAllHandlers"/>.
	/// </summary>
	public static string? SkipHintHandler(PromptHint _) => null;

	#endregion

	#region //// Simple Handlers

	/// <summary>Hint handler for key <see cref="PromptHintKeys.BasicText"/></summary>
	public static string? BasicTextHintHandler(PromptHint hint) => (hint as PromptHint<string>)?.Payload;

	/// <summary>Hint handler for key <see cref="PromptHintKeys.Boolean"/></summary>
	public static string? BooleanHintHandler(PromptHint hint) {
		if (hint is not PromptHint<bool> boolHint) return "y/n";
		return boolHint.Payload ? "Y/n" : "y/N";
	}

	#endregion

	#region //// Type Handler

	/// <summary>Hint handler for key <see cref="PromptHintKeys.TypeHint"/></summary>
	public static string? TypeHintHandler(PromptHint hint) {
	
		// Skip hints of incorrect type.
		if (hint is not PromptHint<Type> typeHint) return null;

		// Also pass the type through a renaming table
		return TypeHintRenamingTable.GetValueOrDefault(typeHint.Payload, typeHint.Payload.Name);
	}

	/// <summary>
	/// Dictionary that is used by <see cref="TypeHintHandler(PromptHint)"/>
	/// to change the displayed name of specific types.
	/// Type inheritance is not accounted for.
	/// </summary>
	public readonly static ReadOnlyDictionary<Type, string> TypeHintRenamingTable =
		new Dictionary<Type, string>() {
			{ typeof(float), "Float" },
			{ typeof(sbyte), "Int8" },
			{ typeof(BigInteger), "Int" },
			{ typeof(DateOnly), "Date" },
			{ typeof(TimeOnly), "Time" }
		}.AsReadOnly();

	#endregion

	#region //// String restrictions

	/// <summary>Hint handler for key <see cref="PromptHintKeys.StringLength"/></summary>
	public static string? StringLengthHintHandler(PromptHint hint) {

		// Fixed legnth
		if (hint is PromptHint<int> fixedLengthHint) return $"{fixedLengthHint.Payload} chrs.";

		// Length range
		if (hint is PromptHint<(int? min, int? max)> rangeLengthHint) {
			return $"{rangeLengthHint.Payload.min}..{rangeLengthHint.Payload.max} chrs.";
		}

		// Unknown hint
		return null;
	}

	/// <summary>
	/// Hint handler for keys <see cref="PromptHintKeys.StringNotEmpty"/>
	/// and <see cref="PromptHintKeys.StringNotEmptyOrWhitespace"/>
	/// </summary>
	public static string? StringNotEmptyHintHandler(PromptHint _) => "not empty";

	#endregion

	#region //// Path restrictions

	/// <summary>Hint handler for keys <see cref="PromptHintKeys.Path"/></summary>
	public static string? PathHintHandler(PromptHint _) => "path";

	/// <summary>Hint handler for keys <see cref="PromptHintKeys.FilePath"/></summary>
	public static string? FilePathHintHandler(PromptHint _) => "file path";

	/// <summary>Hint handler for keys <see cref="PromptHintKeys.DirectoryPath"/></summary>
	public static string? DirectoryPathHintHandler(PromptHint _) => "directory path";

	#endregion

	#region //// Numeric restrictions

	/// <summary>Hint handler for key <see cref="PromptHintKeys.NumericRange"/></summary>
	public static string? NumericRangeHintHandler(PromptHint hint) {

		// Stringified range
		if (hint is PromptHint<(string? min, string? max)> rangeHint) {
			return $"{rangeHint.Payload.min}..{rangeHint.Payload.max}";
		}

		// Unknown hint
		return null;
	}

	/// <summary>Hint handler for keys <see cref="PromptHintKeys.NumericFinite"/></summary>
	public static string? NumericFiniteHintHandler(PromptHint _) => "finite";

	/// <summary>Hint handler for keys <see cref="PromptHintKeys.NumericNotInfinity"/></summary>
	public static string? NumericNotInfnityHintHandler(PromptHint _) => "not inf.";

	/// <summary>Hint handler for keys <see cref="PromptHintKeys.NumericNotNan"/></summary>
	public static string? NumericNotNaNHintHandler(PromptHint _) => "not NaN";

	#endregion

	#region //// Collections

	/// <summary>Returns all handlers of this listing.</summary>
	/// <remarks>Creates a new dictionary.</remarks>
	public static Dictionary<string, Func<PromptHint, string?>> GetAllHandlers() {
		return new() {
			{ PromptHintKeys.BasicText, BasicTextHintHandler },
			{ PromptHintKeys.TypeHint, TypeHintHandler },
			{ PromptHintKeys.Boolean, BooleanHintHandler },
			{ PromptHintKeys.StringLength, StringLengthHintHandler },
			{ PromptHintKeys.StringNotEmpty, StringNotEmptyHintHandler },
			{ PromptHintKeys.StringNotEmptyOrWhitespace, StringNotEmptyHintHandler },
			{ PromptHintKeys.Path, PathHintHandler },
			{ PromptHintKeys.FilePath, FilePathHintHandler },
			{ PromptHintKeys.DirectoryPath, DirectoryPathHintHandler },
			{ PromptHintKeys.NumericRange, NumericRangeHintHandler },
			{ PromptHintKeys.NumericFinite, NumericFiniteHintHandler },
			{ PromptHintKeys.NumericNotInfinity, NumericNotInfnityHintHandler },
			{ PromptHintKeys.NumericNotNan, NumericNotNaNHintHandler },
		};
	}

	/// <summary>Returns common handlers of this listing, skipping more technical once.</summary>
	/// <remarks>
	/// <para>Creates a new dictionary</para>
	/// <para>A handler for <see cref="PromptHintKeys.TypeHint"/> is still included.</para>
	/// </remarks>
	public static Dictionary<string, Func<PromptHint, string?>> GetCommonHandlers() {
		return new() {
			{ PromptHintKeys.BasicText, BasicTextHintHandler },
			{ PromptHintKeys.TypeHint, TypeHintHandler },
			{ PromptHintKeys.Boolean, BooleanHintHandler },
			{ PromptHintKeys.StringLength, StringLengthHintHandler },
			{ PromptHintKeys.NumericRange, NumericRangeHintHandler },
		};
	}

	/// <summary>Returns the most essential handlers of this listing.</summary>
	/// <remarks>
	/// <para>Creates a new dictionary</para>
	/// <para>A handler for <see cref="PromptHintKeys.TypeHint"/> is still included.</para>
	/// </remarks>
	public static Dictionary<string, Func<PromptHint, string?>> GetEssentalHandlers() {
		return new() {
			{ PromptHintKeys.BasicText, BasicTextHintHandler },
			{ PromptHintKeys.TypeHint, TypeHintHandler },
			{ PromptHintKeys.Boolean, BooleanHintHandler },
		};
	}

	#endregion

}
