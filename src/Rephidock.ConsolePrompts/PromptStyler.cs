using System;
using System.Collections.Generic;
using System.Linq;
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

}
