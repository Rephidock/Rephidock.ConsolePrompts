# Console Prompts

[![GitHub License Badge](https://img.shields.io/github/license/Rephidock/Rephidock.ConsolePrompts)](https://github.com/Rephidock/Rephidock.ConsolePrompts/blob/main/LICENSE)

A .NET library to take user input in a console with exception handling and fluent syntax.

## Features

- User input queries with fluent syntax
- Support for strings, `IParsable`, including numbers, and booleans
- Input restrictions (e.g. numeric range, string length, path to an existing file)
- Invalid input handling
- Hints and styling

## Usage

Use the `Prompt` class to create a query with one of the `For` methods, add restrictions to the query with fluent syntax and `Display` the query to the user.

```csharp
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
```

![image: example_prompt_age](https://raw.github.com/Rephidock/Rephidock.ConsolePrompts/main/media/example_prompt_age.png)


### Advanced Use

Use the `Prompter` class to customize how prompts and hints are displayed. 

```csharp
Prompter prompter = new Prompter(autoSetupHints: false);

// Set up all hints to be displayed
prompter.SetHintHandlers(PromptHintHandlers.GetAllHandlers());

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
```

![image: example_styled_float](https://raw.github.com/Rephidock/Rephidock.ConsolePrompts/main/media/example_styled_float.png)

See [Demo Project](https://github.com/Rephidock/Rephidock.ConsolePrompts/blob/main/src/Rephidock.ConsolePrompts.Demo) on the source repository for a more full tutorial.
