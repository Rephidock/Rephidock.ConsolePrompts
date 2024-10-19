using System;
using Xunit;
using Rephidock.ConsolePrompts;


namespace Rephidock.ConsolePrompts.Tests;


public sealed class NumericRestrictionsTests {

	[Theory]
	[InlineData(null, null, new string[] { "0", "1", "-1", "-99", "99", "2147483647", "-2147483648" }, new string[] { })]
	[InlineData(0, null, new string[] { "0", "1", "99", "2147483647"}, new string[] { "-1", "-99", "-2147483648" })]
	[InlineData(1, null, new string[] { "1", "99", "2147483647"}, new string[] { "0", "-1", "-99", "-2147483648" })]
	[InlineData(null, 1, new string[] { "1", "0", "-1", "-99", "-2147483648"}, new string[] { "99", "2147483647" })]
	[InlineData(0, 1, new string[] { "1", "0" }, new string[] { "99", "2147483647", "-1", "-99", "-2147483648", "2", "3", "63", "64", "15" })]
	[InlineData(2, 64, new string[] { "2", "3", "63", "64", "15" }, new string[] { "2147483647", "-1", "-2147483648", "1", "0", "65" })]
	public void OfRangeRestriction_InputFinite_ThrowsIfOutOfRange(int? minBound, int? maxBound, string[] validInputs, string[] invalidInputs) {

		// Arrange
		var prompt = Prompt.For<int>().OfRange(minBound, maxBound);

		// Act and Assert
		foreach (var input in validInputs) {
			prompt.ParseAndValidate(input);
		}

		foreach (var input in invalidInputs) {
			Assert.ThrowsAny<Exception>(() => prompt.ParseAndValidate(input));
		}

	}

	[Theory]
	[InlineData(null, null, new string[] { "infinity", "-infinity", "nan" }, new string[] { })]
	[InlineData(0f, null, new string[] { "infinity" }, new string[] { "-infinity" })]
	[InlineData(null, 0f, new string[] { "-infinity" }, new string[] { "infinity" })]
	[InlineData(0f, 1f, new string[] { }, new string[] { "infinity", "-infinity" })]
	public void OfRangeRestriction_InputNotFinite_ThrowsIfOutOfRange(float? minBound, float? maxBound, string[] validInputs, string[] invalidInputs) {

		// Arrange
		var prompt = Prompt.For<float>().OfRange(minBound, maxBound);

		// Act and Assert
		foreach (var input in validInputs) {
			prompt.ParseAndValidate(input);
		}

		foreach (var input in invalidInputs) {
			Assert.ThrowsAny<Exception>(() => prompt.ParseAndValidate(input));
		}

	}

	[Theory]
	[InlineData(float.NegativeInfinity, null, new string[] { "infinity", "-infinity", "0", "1", "-1", "64", "-64" })]
	[InlineData(null, float.PositiveInfinity, new string[] { "infinity", "-infinity", "0", "1", "-1", "64", "-64" })]
	[InlineData(float.NegativeInfinity, float.PositiveInfinity, new string[] { "infinity", "-infinity", "0", "1", "-1", "64", "-64" })]
	public void OfRangeRestriction_InputWithInfiniteBounds_DoesNotThrown(float? minBound, float? maxBound, string[] validInputs) {

		// Arrange
		var prompt = Prompt.For<float>().OfRange(minBound, maxBound);

		// Act and Assert
		foreach (var input in validInputs) {
			prompt.ParseAndValidate(input);
		}

	}

	[Theory]
	[InlineData("0", false)]
	[InlineData("-0", false)]
	[InlineData("1", false)]
	[InlineData("-1", false)]
	[InlineData("+infinity", false)]
	[InlineData("-infinity", false)]
	[InlineData("nan", true)]
	public void DisallowNaNRestriction_Input_ThrowIfNaN(string input, bool isNan) {

		// Arrange
		var promptNoNan = Prompt.For<float>().DisallowNaN();

		// Act and Assert
		void ActDelegateNotNan() {
			promptNoNan.ParseAndValidate(input);
		}

		if (isNan) {
			Assert.ThrowsAny<Exception>(ActDelegateNotNan);
		} else {
			ActDelegateNotNan();
		}

	}

	[Theory]
	[InlineData("0", false)]
	[InlineData("-0", false)]
	[InlineData("1", false)]
	[InlineData("-1", false)]
	[InlineData("+infinity", true)]
	[InlineData("-infinity", true)]
	[InlineData("nan", false)]
	public void DisallowInfinitiesRestriction_Input_ThrowIfInfinity(string input, bool isInfinity) {

		// Arrange
		var promptNoInf = Prompt.For<float>().DisallowInfinities();

		// Act and Assert
		void ActDelegateNotInfinity() {
			promptNoInf.ParseAndValidate(input);
		}

		if (isInfinity) {
			Assert.ThrowsAny<Exception>(ActDelegateNotInfinity);
		} else {
			ActDelegateNotInfinity();
		}

	}

	[Theory]
	[InlineData("0", false)]
	[InlineData("-0", false)]
	[InlineData("1", false)]
	[InlineData("-1", false)]
	[InlineData("+infinity", true)]
	[InlineData("-infinity", true)]
	[InlineData("nan", true)]
	public void ForceFiniteRestriction_Input_ThrowIfNotFinite(string input, bool isNotFinite) {

		// Arrange
		var promptForceFinite = Prompt.For<float>().ForceFinite();
		var promptForceFiniteCreation = Prompt.For<float>("", forceFinite: true);
		var promptForceFiniteSeparated = Prompt.For<float>().DisallowInfinities().DisallowNaN();

		// Act and Assert
		void ActDelegateForceFinite() {
			promptForceFinite.ParseAndValidate(input);
			promptForceFiniteCreation.ParseAndValidate(input);
			promptForceFiniteSeparated.ParseAndValidate(input);
		}

		if (isNotFinite) {
			Assert.ThrowsAny<Exception>(ActDelegateForceFinite);
		} else {
			ActDelegateForceFinite();
		}

	}

	[Theory]
	[InlineData("0")]
	[InlineData("-0")]
	[InlineData("1")]
	[InlineData("-1")]
	[InlineData("+infinity")]
	[InlineData("-infinity")]
	[InlineData("nan")]
	public void ForceFiniteOnCreationDisabled_Input_DoesNotThrow(string input) {

		// Arrange
		var promptParsable = Prompt.For<float>();
		var promptParsableWithText = Prompt.For<float>("");
		var promptNumeric = Prompt.For<float>("", forceFinite: false);

		// Act and Assert
		promptParsable.ParseAndValidate(input);
		promptParsableWithText.ParseAndValidate(input);
		promptNumeric.ParseAndValidate(input);

	}

}
