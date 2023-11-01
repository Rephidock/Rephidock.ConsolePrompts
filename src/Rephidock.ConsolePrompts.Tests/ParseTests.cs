using System;
using Xunit;
using Rephidock.ConsolePrompts;


namespace Rephidock.ConsolePrompts.Tests;


public sealed class ParseTests {

	[Theory]
	[InlineData("")]
	[InlineData("a")]
	[InlineData(".")]
	[InlineData("-e")]
	[InlineData("-")]
	[InlineData("five")]
	[InlineData("-0.1")]
	[InlineData("0.1")]
	[InlineData("1.1.1")]
	[InlineData("--5")]
	[InlineData("++0")]
	[InlineData("++5")]
	public void PromptIParsableT_InputParsingInvalid_Throws(string input) {

		// Arrange
		var prompt = Prompt.For<int>("", int.Parse);

		if (int.TryParse(input, out int _)) {
			Assert.Fail("Test case has bad semantics.");
		}

		// Act and Assert
		Assert.ThrowsAny<Exception>(() => prompt.ParseAndValidate(input));

	}

	[Theory]
	[InlineData("0")]
	[InlineData("-1")]
	[InlineData("1")]
	[InlineData("64")]
	[InlineData("-64")]
	[InlineData("-0")]
	[InlineData("+0")]
	[InlineData("+5")]
	public void PromptIParsableT_InputParsingValid_ReturnsSameValueAsParser(string input) {

		// Arrange
		var prompt = Prompt.For<int>("", int.Parse);
		
		if (!int.TryParse(input, out int resultParse)) {
			Assert.Fail("Test case has bad semantics.");
		}

		// Act
		int resultPrompt = prompt.ParseAndValidate(input);

		// Assert
		Assert.Equal(resultParse, resultPrompt);

	}

	[Theory]
	[InlineData("")]
	[InlineData("  ")]
	[InlineData("aaa")]
	[InlineData("  aaa  ")]
	[InlineData("\t")]
	[InlineData("\tqqq")]
	[InlineData("I am a string of text")]
	[InlineData("I am a string of text ")]
	public void PromptString_ParsingWithNoTrim_ReturnsInput(string input) {

		// Arrange
		var prompt = Prompt.ForString(trim: false);

		// Act
		string resultPrompt = prompt.ParseAndValidate(input);

		// Assert
		Assert.Equal(input, resultPrompt);

	}

	[Theory]
	[InlineData("")]
	[InlineData("  ")]
	[InlineData("aaaa")]
	[InlineData("  aaa  ")]
	[InlineData("\t")]
	[InlineData("\tqqq")]
	[InlineData("I am a string of text")]
	[InlineData("I am a string of text ")]
	public void PromptString_ParsingWithTrim_ReturnsInputTrimmed(string input) {

		// Arrange
		var prompt = Prompt.ForString(trim: true);
		string resultTrim = input.Trim();

		// Act
		string resultPrompt = prompt.ParseAndValidate(input);

		// Assert
		Assert.Equal(resultTrim, resultPrompt);

	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("\t\t")]
	[InlineData(" \t  \t ")]
	public void PromptBool_InputParsingTrimmedEmpty_ReturnsDefaultValue(string input) {

		// Arrange
		var promptDefaultTrue = Prompt.ForBool(defaultValue: true);
		var promptDefaultFalse = Prompt.ForBool(defaultValue: false);

		// Act
		bool resultTrue = promptDefaultTrue.ParseAndValidate(input);
		bool resultFalse = promptDefaultFalse.ParseAndValidate(input);

		// Assert
		Assert.True(resultTrue);
		Assert.False(resultFalse);

	}

	[Theory]
	[InlineData("longer string that's not a boolean")]
	[InlineData("yn")]
	[InlineData("nya")]
	[InlineData("?")]
	[InlineData("yyeeaaa")]
	[InlineData("maybe")]
	public void PromptBool_InputParsingInvalid_Throws(string input) {

		// Arrange
		var prompt = Prompt.ForBool();

		// Act and Assert
		Assert.ThrowsAny<Exception>(() => prompt.ParseAndValidate(input));

	}

	[Theory]
	[InlineData("y", true)]
	[InlineData("Y", true)]
	[InlineData("n", false)]
	[InlineData("N", false)]
	[InlineData("    y", true)]
	[InlineData("\ty", true)]
	[InlineData("y ", true)]
	[InlineData(" y    ", true)]
	public void PromptBool_InputParsingValid_ReturnsExpectedResult(string input, bool expected) {

		// Arrange
		var prompt = Prompt.ForBool();

		// Act
		bool resultPrompt = prompt.ParseAndValidate(input);

		// Assert
		Assert.Equal(expected, resultPrompt);

	}
}