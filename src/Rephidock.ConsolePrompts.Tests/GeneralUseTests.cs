using System;
using System.IO;
using Xunit;
using Rephidock.ConsolePrompts;
using System.Linq;


namespace Rephidock.ConsolePrompts.Tests;


public sealed class GeneralUseTests {

	[Theory]
	[InlineData("")]
	[InlineData("aaa")]
	[InlineData("gobbledygook")]
	[InlineData("null")]
	[InlineData("99.9")]
	public void Prompt_FirstInputCorrect_ReadsSteamTillEndAndReturnsExpected(string input) {

		// Arrange
		using var outputStram = new StringWriter();
		using var inputStream = new StringReader(input + '\n');
		var prompter = new Prompter(outputStram, inputStream);
		var prompt = prompter.PromptForString(null, trim: false);

		// Act
		string reading = prompt.Display();

		// Assert
		Assert.Equal(-1, inputStream.Peek()); // Assert that stream is at the end
		Assert.Equal(input, reading);

	}

	[Theory]
	[InlineData("a\nb\n\n0\n", 0)]
	[InlineData("\n\n\n\n\n\n-31\n", -31)]
	[InlineData("hello\n100\n", 100)]
	public void Prompt_FirstInputIncorrect_ReadsSteamTillEndAndReturnsExpected(string inputsJoined, int expected) {

		// Arrange
		using var outputStram = new StringWriter();
		using var inputStream = new StringReader(inputsJoined);
		var prompter = new Prompter(outputStram, inputStream);
		var prompt = prompter.PromptFor<int>(null);

		// Act
		int reading = prompt.Display();

		// Assert
		Assert.Equal(-1, inputStream.Peek()); // Assert that stream is at the end
		Assert.Equal(expected, reading);

	}

	[Theory]
	[InlineData("\n\n")]
	[InlineData("aaa\n\n")]
	[InlineData("gobbledygook\ngobbledygook again")]
	[InlineData("let\nme\nin")]
	public void Prompt_FirstInputCorrect_DoesNotOverRead(string inputsJoined) {

		// Arrange
		using var outputStram = new StringWriter();
		using var inputStream = new StringReader(inputsJoined);
		var prompter = new Prompter(outputStram, inputStream);
		var prompt = prompter.PromptForString(null, trim: false);

		// Act
		string reading = prompt.Display();

		// Assert
		Assert.NotEqual(-1, inputStream.Peek()); // Assert that stream is not at the end

	}

}
