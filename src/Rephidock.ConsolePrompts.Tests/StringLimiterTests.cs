using System;
using Xunit;
using Rephidock.ConsolePrompts;


namespace Rephidock.ConsolePrompts.Tests;


public sealed class StringLimiterTests {

	[Theory]
	[InlineData("", true, true)]
	[InlineData(" ", false, true)]
	[InlineData("\t", false, true)]
	[InlineData("\t\t  \t  \t", false, true)]
	[InlineData("a", false, false)]
	[InlineData("0", false, false)]
	[InlineData("something ", false, false)]
	[InlineData("\tsomething", false, false)]
	public void OfEmptyLimiters_Input_ThrowsIfInvalid(string input, bool isEmpty, bool isOnlyWhitespace) {

		// Arrange
		var promptNotEmpty = Prompt.ForString(trim: false).DisallowEmpty();
		var promptNotWhitespace = Prompt.ForString(trim: false).DisallowOnlyWhiteSpace();

		// Act
		void ActDelegateNotEmpty() { promptNotEmpty.ParseAndValidate(input); }
		void ActDelegateNotWhitespace() { promptNotWhitespace.ParseAndValidate(input); }

		// Assert
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
	public void OfExactLengthLimiter_Input_ThrowsIfNotCorrectLength(string input, int limiterLengthIncorrect, int limiterLengthCorrect) {

		// Arrange
		var promptThrowing = Prompt.ForString(trim: false).OfLength(limiterLengthIncorrect);
		var promptNonThrowing = Prompt.ForString(trim: false).OfLength(limiterLengthCorrect);

		var promptRangeThrowing = Prompt.ForString(trim: false).OfLength(limiterLengthIncorrect, limiterLengthIncorrect);
		var promptRangeNonThrowing = Prompt.ForString(trim: false).OfLength(limiterLengthCorrect, limiterLengthCorrect);

		// Act
		void ThrowingDelegate() {
			promptThrowing.ParseAndValidate(input);
			promptRangeThrowing.ParseAndValidate(input);
		}

		promptNonThrowing.ParseAndValidate(input);
		promptRangeNonThrowing.ParseAndValidate(input);

		// Assert
		Assert.ThrowsAny<Exception>(ThrowingDelegate);

	}

	[Theory]
	[InlineData("", 1, null, 0, null)]
	[InlineData("aa", 3, 4, 2, 4)]
	[InlineData("aa", 0, 1, 0, 4)]
	[InlineData("aa", 0, 0, 0, 2)]
	[InlineData("longer string", 10, 12, 10, 15)]
	[InlineData("spikes!!!", 20, null, 9, null)]
	[InlineData("spikes!!!", 20, 25, 0, 25)]
	[InlineData("hello", 0, 2, 2, 5)]
	[InlineData("something", 0, 0, 0, null)]
	public void OfRangeLengthLimiter_Input_ThrowsIfNotInRange(string input, int badRangeLow, int? badRangeHigh, int goodRangeLow, int? goodRangeHight) {

		// Arrange
		var promptThrowing = Prompt.ForString(trim: false).OfLength(badRangeLow, badRangeHigh);
		var promptNonThrowing = Prompt.ForString(trim: false).OfLength(goodRangeLow, goodRangeHight);

		// Act
		void ThrowingDelegate() { promptThrowing.ParseAndValidate(input); }
		promptNonThrowing.ParseAndValidate(input);

		// Assert
		Assert.ThrowsAny<Exception>(ThrowingDelegate);

	}

}
