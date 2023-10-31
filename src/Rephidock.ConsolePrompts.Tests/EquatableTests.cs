using System;
using Xunit;
using Rephidock.ConsolePrompts;


namespace Rephidock.ConsolePrompts.Tests;


public sealed class EquatableLimiterTests {


	[Theory]
	[InlineData(0, "0", new string[] { "1", "3", "64", "-1" })]
	[InlineData(-1, "-1", new string[] { "1", "3", "64", "0" })]
	[InlineData(255, "255", new string[] { "-255", "0", "254", "256" })]
	public void OfNotEqualToLimiter_Input_ThrowsIfEquals(int exclusion, string invalidInput, string[] validInputs) {

		// Arrange
		var prompt = Prompt.For<int>().NotEqualTo(exclusion);

		// Act and Assert
		foreach (var input in validInputs) {
			prompt.ParseAndValidate(input);
		}

		Assert.ThrowsAny<Exception>(() => prompt.ParseAndValidate(invalidInput));

	}

}
