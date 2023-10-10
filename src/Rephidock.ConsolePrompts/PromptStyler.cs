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

	public static PromptHintLevel HintLevel = DefaultHintLevel;

	public const PromptHintLevel DefaultHintLevel = PromptHintLevel.Standard;

	public static string GetHintsString(IReadOnlyList<PromptHint> hints) {

		var hintStrings = hints
			.Where(hint => HintLevel >= hint.Level)
			.Select(hint => hint.Text);

		return string.Join(", ", hintStrings);
	}

	#endregion

}
