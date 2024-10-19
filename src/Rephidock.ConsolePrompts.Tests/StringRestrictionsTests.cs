using System;
using Xunit;
using Rephidock.ConsolePrompts;


namespace Rephidock.ConsolePrompts.Tests;


public sealed class StringRestrictionsTests {

	[Theory]
	[InlineData("", true, true)]
	[InlineData(" ", false, true)]
	[InlineData("\t", false, true)]
	[InlineData("\t\t  \t  \t", false, true)]
	[InlineData("a", false, false)]
	[InlineData("0", false, false)]
	[InlineData("something ", false, false)]
	[InlineData("\tsomething", false, false)]
	public void OfEmptyRestrictions_Input_ThrowsIfInvalid(string input, bool isEmpty, bool isOnlyWhitespace) {

		// Arrange
		var promptNotEmpty = Prompt.ForString(trim: false).DisallowEmpty();
		var promptNotWhitespace = Prompt.ForString(trim: false).DisallowOnlyWhiteSpace();

		// Act and Assert
		void ActDelegateNotEmpty() { promptNotEmpty.ParseAndValidate(input); }
		void ActDelegateNotWhitespace() { promptNotWhitespace.ParseAndValidate(input); }

		if (isEmpty) {
			Assert.ThrowsAny<Exception>(ActDelegateNotEmpty);
		} else {
			ActDelegateNotEmpty();
		}

		if (isOnlyWhitespace) {
			Assert.ThrowsAny<Exception>(ActDelegateNotWhitespace);
		} else {
			ActDelegateNotWhitespace();
		}

	}

	[Theory]
	[InlineData("", 1, 0)]
	[InlineData("aa", 1, 2)]
	[InlineData("longer string", 1, 13)]
	[InlineData("not empty", 0, 9)]
	[InlineData(" ", 0, 1)]
	[InlineData(" ", 2, 1)]
	[InlineData("hello", 2, 5)]
	[InlineData(" hello ", 5, 7)]
	[InlineData("hello", 16, 5)]
	public void OfExactLengthRestriction_Input_ThrowsIfNotCorrectLength(string input, int limiterLengthIncorrect, int limiterLengthCorrect) {

		// Arrange
		var promptThrowing = Prompt.ForString(trim: false).OfLength(limiterLengthIncorrect);
		var promptNonThrowing = Prompt.ForString(trim: false).OfLength(limiterLengthCorrect);

		var promptRangeThrowing = Prompt.ForString(trim: false).OfLength(limiterLengthIncorrect, limiterLengthIncorrect);
		var promptRangeNonThrowing = Prompt.ForString(trim: false).OfLength(limiterLengthCorrect, limiterLengthCorrect);

		// Act and Assert
		promptNonThrowing.ParseAndValidate(input);
		promptRangeNonThrowing.ParseAndValidate(input);

		Assert.ThrowsAny<Exception>(
			() => {
				promptThrowing.ParseAndValidate(input);
				promptRangeThrowing.ParseAndValidate(input);
			}
		);
		
	}

	[Theory]
	[InlineData(0, null, new string[] { "", " ", "aa", "something", "longer string" }, new string[] { } )]
	[InlineData(1, null, new string[] { " ", "aa", "longer string" }, new string[] { "" } )]
	[InlineData(10, null, new string[] { "longer string", "1234567890" }, new string[] { "", " ", "aa", "123456789" })]
	[InlineData(0, 1, new string[] { "", "q", "\'", " " }, new string[] { "aa", "longer string" } )]
	[InlineData(0, 4, new string[] { "", "q", "aa", "1234" }, new string[] { "12345", "longer string" } )]
	[InlineData(2, 4, new string[] { "aa", ".?!", "1234" }, new string[] { "", "q", "12345", "longer string" } )]
	[InlineData(10, 14, new string[] { "longer string", "1234567890" }, new string[] { "an even longer string", "me short", "" } )]
	public void OfRangeLengthRestriction_Input_ThrowsIfNotInRange(
		int minLength, int? maxLength,
		string[] validInputs, string[] invalidInputs
	) {

		// Arrange
		var prompt = Prompt.ForString(trim: false).OfLength(minLength, maxLength);

		// Act and Assert
		foreach (var input in validInputs) {
			prompt.ParseAndValidate(input);
		}

		foreach (var input in invalidInputs) {
			Assert.ThrowsAny<Exception>(() => prompt.ParseAndValidate(input));
		}

	}

}
