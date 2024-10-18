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
		Console.WriteLine("Welcome to ConsolePrompts demo / tutorial!");

		
		

		// ---------------------------------
		// -======- Quick/Basic Use -======-
		// ---------------------------------
		StartNextSection("Quick/Basic Use");


		// Use Prompt static class for quick prompt creation.
		string name = Prompt.ForString("What is your name", trim: true).DisallowEmpty().Display();
		Console.WriteLine($"Hello, {name}");


		// Supports IParsable, including numbers.
		// Be careful with floats: "Infinity" and "NaN" are valid inputs
		// (unless disabled with forceFinite parameter).
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


		// Further section has an example with a DateOnly.

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




		// ---------------------------
		// -======- Null Text -======-
		// ---------------------------
		StartNextSection("Null Text");


		// Empty or null text displays are supported and show up differently.
		string mysteryInput = Prompt.ForString().Display();
		Console.WriteLine($"Mystery Length: {mysteryInput.Length}");


		// Common hints are still shown (unless disabled, see in a further section)
		double mysteryNumericInput = Prompt
			.For<double>(null, forceFinite: true)
			.NoLessThan(0)
			.Display();

		Console.WriteLine($"Mystery Square Root: {Math.Sqrt(mysteryNumericInput)}");




		// --------------------------
		// -======- Prompter -======-
		// --------------------------
		StartNextSection("Prompter");


		// For advanced features (explored further) use a Prompter.
		Prompter prompter = new();

		float favoriteFloat = prompter
			.PromptFor<float>("What's your favorite float")
			.Display();

		if (float.IsFinite(favoriteFloat))
		{
			Console.WriteLine("Ah, finite as expected.");
		}
		else
		{
			Console.WriteLine("Oh, not finite. Great choice.");
		}


		// Only a single prompter object is required for multiple prompts.
		string nameSecond = prompter
			.PromptForString("What's your name again?")
			.DisallowEmpty()
			.Display();

		if (name != nameSecond) Console.WriteLine("I am confused...");




		// ---------------------------------------
		// -======- Further Customization -======-
		// ---------------------------------------
		StartNextSection("Further Customization");


		// Hints and format prompts is customizable.
		// Requires a prompter.
		prompter = new Prompter(autoSetupHints: false);

		// Set up all hints to be displayed
		// (includes uncommon hints, like "not empty").
		prompter.SetHintHandlers(PromptHintHandlers.GetAllHandlers());
		prompter.UnknownHintHandler = PromptHintHandlers.DebugHintHandler;

		// Change format of prompts
		prompter.PromptFormat = "[{1}] {0} := ";
		prompter.HintSeparator = " & ";
		prompter.InvalidInputFormat = "Not accepted: {0}";

		// Example prompt
		float x = prompter
			.PromptFor<float>("x")
			.AddTypeHint()
			.ForceFinite()
			.OfRange(-1, 1)
			.NotEqualTo(0)
			.Display();

		Console.WriteLine($"f(x) = 60 + 10 * {x} = {60 + 10 * x}");


		// Alternatively all hints can be disabled like so:
		prompter = new Prompter(autoSetupHints: false) {
			UnknownHintHandler = PromptHintHandlers.SkipHintHandler
		};




		// ----------------------------------
		// -======- More About Hints -======-
		// ----------------------------------
		StartNextSection("More About Hints");


		// When a parser a created without hint setup
		// it has no hint handlers and a debug unknown hint handler.
		prompter = new Prompter(autoSetupHints: false);

		// Lets add some handlers!
		prompter.SetHintHandler(PromptHintKeys.BasicText, hint => (hint as PromptHint<string>)?.Payload);
		prompter.SetHintHandler(PromptHintKeys.StringNotEmptyOrWhitespace, _ => "not empty");
		prompter.UnknownHintHandler = hint => $"<unknown hint key {hint.Key}>";

		// Lets create a prompt with some hints and see what is displayed.
		_ = prompter
			.PromptForString("Give me 3 characters")
			.AddTextHint("3 chrs.")     // This adds a BasicText hint. The given text becomes Payload.
			.DisallowOnlyWhiteSpace()   // Also adds a StringNotEmptyOrWhitespace hint.
			.OfLength(3)                // Also adds a StringLength hint; Handler not set, unknown will be used.

			// When a prompt is displayed all hints are passed through
			// the prompter's hint handlers based on the hints' keys.
			.Display();

		// autoSetupHints is true by default.
		// It adds CommonHintHandlers from PromptHintHandlers
		// and sets a SkipHintHandler as an UnknownHintHandler.

		// The Prompt static class uses a prompter with automatically set up hints.
		

		// Additionally type hints exist.
		// Prompter can add them automatically or they can be added manually.
		DateOnly userBirthday = Prompt
			.For<DateOnly>("When is your birthday")
			.AddTypeHint()
			.Display();

		DateOnly today = DateOnly.FromDateTime(DateTime.Now);

		if (userBirthday == today) {
			Console.WriteLine("Happy birthday!");
		} else {
			Console.WriteLine($"Ah, on {userBirthday}. Got it.");
		}


		// You can define your own hints and handlers for them from the ground up.
		const string exampleHintKey = "userExample";

		static string? ExampleHintHandler(PromptHint hint) {

			// Skip unrelated hints if assigned
			if (hint.Key != exampleHintKey) return null;

			// Handle hints with integer payload
			if (hint is PromptHint<int> intHint) return $"example.{intHint.Payload}";

			// Handle other hints, including with no payload
			return "example";
		}

		prompter.SetHintHandler(exampleHintKey, ExampleHintHandler);

		_ = prompter
			.PromptForString()
			.AddHint(new PromptHint(exampleHintKey))
			.AddHint(new PromptHint<int>(exampleHintKey, 331))
			.Display();




		// ---------------------------------------
		// -======- More About Validators -======-
		// ---------------------------------------
		StartNextSection("More About Validators");


		// All restrictions already seen just add their internal validators and hints.

		// Let's define own validator. Must throw on invalid input
		static void MustNotContainLetterAValidator(string input) {
			if (input.Contains('A', StringComparison.InvariantCultureIgnoreCase)) {
				throw new PromptInputException("Input must not contain letter 'A'");
			}
		}

		// and add it with an appropriate hint.
		string stringWithoutLetterA = Prompt
			.ForString("Give me a sentence", trim: true)
			.AddValidator(MustNotContainLetterAValidator)
			.AddTextHint("without letter A")
			.Display();

		Console.WriteLine($"Given sentence (trimmed) has \"{stringWithoutLetterA.Length}\" characters");




		// - Outro -
		StartNextSection("The End");
		Console.WriteLine("This concludes the ConsolePrompts demo!");
		Console.Write("Press any key to exit...");
		Console.ReadKey(intercept: true);
		
	}

}
