using System;
using Rephidock.ConsolePrompts;


namespace Rephidock.ConsolePrompts.Demo;


internal class Program
{

	static void WriteSectionSeparator() => Console.WriteLine("\n-=-=-=-=-=-=-\n");


	static void Main(string[] args)
	{

		// - Intro -
		Console.WriteLine("Welcome to ConsolePrompts demo!");
		WriteSectionSeparator();


		//  -======- Strings -======-
		string name = Prompt.ForString("What is your name", trim: true).DisallowEmpty().Display();
		Console.WriteLine($"Hello, {name}");

		WriteSectionSeparator();


		//  -======- Numbers -======-
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

		WriteSectionSeparator();


		//  -======- IParsable -======-
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

		WriteSectionSeparator();


		//  -======- Boolean -======-
		bool isHexagonEnjoyer = Prompt.ForBool("Do you like hexagons?", defaultValue: true).Display();

		if (isHexagonEnjoyer)
		{
			Console.WriteLine("Hexagons are the bestagons!");
		}
		else
		{
			Console.WriteLine("They are alright I guess...");
		}

		WriteSectionSeparator();


		//  -======- Null Text -======-
		Console.WriteLine("Empty or null text displays are supported.");
		Console.WriteLine("Write Anything!");

		string anything = Prompt.ForString().Display(); 
		Console.WriteLine($"Length of Anything (trimmed): {anything.Length}");

		WriteSectionSeparator();


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

		WriteSectionSeparator();


		//  -======- Styling -======-
		PromptStyler.PromptFormat = "[{1}] {0} = ";
		PromptStyler.InvalidInputFormat = "I can't accept that: {0}";
		PromptStyler.HintLevel = PromptHintLevel.Verbose;

		Console.WriteLine("f(x) = 60 + 10x");

		float x = Prompt
			.For<float>("x")
			.ForceFinite()
			.OfRange(0, 1)
			.AddHint("real", PromptHintLevel.Verbose)
			.Display();

		Console.WriteLine($"f(x) = 60 + 10 * {x} = {60 + 10 * x}");

		WriteSectionSeparator();


		//  -======- Styling: Type hints -======-
		PromptStyler.PromptFormat = "{0} ({1}): ";
		PromptStyler.HintLevel = PromptHintLevel.Standard;
		PromptStyler.TypeHintsEnabled = true;

		Console.WriteLine("Type hints can also be enabled.");

		float rating = Prompt
			.For<float>("Give us a rating")
			.ForceFinite()
			.OfRange(0, 5)
			.Display();

		Console.WriteLine($"Given rating: {rating}");

		WriteSectionSeparator();


		// - Outro -
		Console.WriteLine("This concludes the ConsolePrompts demo!");
		Console.Write("Press any key to exit...");
		Console.ReadKey(intercept: true);

	}

}
