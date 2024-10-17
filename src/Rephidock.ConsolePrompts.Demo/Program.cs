using System;
using Rephidock.ConsolePrompts;


namespace Rephidock.ConsolePrompts.Demo;


internal class Program
{

	static void StartNextSection(string sectionName)
	{
		Console.WriteLine($"\n\n-=-=-=- {sectionName} -=-=-=-\n");
	}


	static void Main(string[] args)
	{

		// - Intro -
		Console.WriteLine("Welcome to ConsolePrompts demo!");

		

		// ---------------------------------
		// -======- Quick/Basic Use -======-
		// ---------------------------------
		StartNextSection("Quick/Basic Use");


		// Use Prompt static class for quick prompt creation
		string name = Prompt.ForString("What is your name", trim: true).DisallowEmpty().Display();
		Console.WriteLine($"Hello, {name}");


		// Numbers supported.
		// Be careful with floats: "Infinity" and "NaN" are valid inputs unless disabled.
		int userAge = Prompt.For<int>("Your age").NoLessThan(1).Display();
		const int drinkingAge = 21;

		if (userAge >= drinkingAge)
		{
			Console.WriteLine("You are of drinking age!");
		}
		else
		{
			Console.WriteLine("Sorry, you can't have a drink.");
		}


		// Supports IParsable
		DateOnly userBirthday = Prompt.For<DateOnly>("When is your birthday").Display();
		DateOnly today = DateOnly.FromDateTime(DateTime.Now);

		if (userBirthday == today)
		{
			Console.WriteLine("Happy birthday!");
		}
		else
		{
			Console.WriteLine($"Your birthday is on {userBirthday}");
		}


		// Booleans are supported and allow single character answers.
		bool isHexagonEnjoyer = Prompt.ForBool("Do you like hexagons?", defaultValue: true).Display();

		if (isHexagonEnjoyer)
		{
			Console.WriteLine("Hexagons are the bestagons!");
		}
		else
		{
			Console.WriteLine("They are alright I guess...");
		}

		


		// --------------------------
		// -======- Prompter -======-
		// --------------------------
		StartNextSection("Prompter");


		// For multiple prompts or advanced features use Prompter.
		Prompter prompter = new();

		float favouriteFloat = prompter
			.PromptFor<float>("What's your favourite float")
			.Display();

		if (float.IsFinite(favouriteFloat))
		{
			Console.WriteLine("Ah, finite as expected.");
		}
		else
		{
			Console.WriteLine("Oh, not finte. Great choice.");
		}


		// Only a single prompter object is required for multiple prompts
		string nameSecond = prompter
			.PromptForString("What's your name again?")
			.DisallowEmpty()
			.Display();

		if (name != nameSecond) Console.WriteLine("I am confused...");




		// ---------------------------
		// -======- Null Text -======-
		// ---------------------------
		StartNextSection("Null Text");


		// Empty or null text displays are supported
		string mysteryInput = prompter.PromptForString().Display(); 
		Console.WriteLine($"Mystery Length: {mysteryInput.Length}");


		// Common hints are still shown (unless disabled)
		double mysteryNumericInput = prompter
			.PromptFor<double>(null, forceFinite: true)
			.NotEqualTo(0)
			.Display();

		Console.WriteLine($"Mystery Sqaure: {mysteryNumericInput * mysteryNumericInput}");


		/*
		//  -======- Custom Validators -======-
		static void MustNotContainLetterAValidator(string input) {
			if (input.Contains('A', StringComparison.InvariantCultureIgnoreCase)) {
				throw new PromptInputException("Input must not contain letter 'A'");
			}
		}

		string stringWithoutLetterA = Prompt
			.ForString("I need a sentence without using letter A")
			.AddValidator(MustNotContainLetterAValidator)
			.Display();

		Console.WriteLine($"Given sentence: \"{stringWithoutLetterA}\"");

		StartNextSection();


		//  -======- Styling -======-
		TO_BE_REMOVED_PromptStyler.PromptFormat = "[{1}] {0} = ";
		TO_BE_REMOVED_PromptStyler.InvalidInputFormat = "I can't accept that: {0}";
		TO_BE_REMOVED_PromptStyler.HintLevel = PromptHintLevel.Verbose;

		Console.WriteLine("f(x) = 60 + 10x");

		float x = Prompt
			.For<float>("x")
			.ForceFinite()
			.OfRange(0, 1)
			.AddHint("real", PromptHintLevel.Verbose)
			.Display();

		Console.WriteLine($"f(x) = 60 + 10 * {x} = {60 + 10 * x}");

		StartNextSection();


		//  -======- Styling: Type hints -======-
		TO_BE_REMOVED_PromptStyler.PromptFormat = "{0} ({1}): ";
		TO_BE_REMOVED_PromptStyler.HintLevel = PromptHintLevel.Standard;
		TO_BE_REMOVED_PromptStyler.TypeHintsEnabled = true;

		Console.WriteLine("Type hints can also be enabled.");

		float rating = Prompt
			.For<float>("Give us a rating")
			.ForceFinite()
			.OfRange(0, 5)
			.Display();

		Console.WriteLine($"Given rating: {rating}");

		StartNextSection();

		*/
		



		// - Outro -
		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine("This concludes the ConsolePrompts demo!");
		Console.Write("Press any key to exit...");
		Console.ReadKey(intercept: true);
		
	}

}
