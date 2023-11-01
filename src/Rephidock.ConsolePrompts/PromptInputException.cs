using System;


namespace Rephidock.ConsolePrompts;


/// <summary>
/// An exception used by <see cref="Prompt{T}"/>
/// parsers and validators to indicate invalid input.
/// </summary>
public class PromptInputException : Exception {

	/// <summary>
	/// The default message of the exception if none is provided.
	/// </summary>
	public const string DefaultMessage = "Input was not in the correct format or value was invalid";

	/// <summary>
	/// Creates a new instance of <see cref="PromptInputException"/>
	/// with default message.
	/// </summary>
	public PromptInputException() : this(DefaultMessage) { }

	/// <summary>
	/// Creates a new instance of <see cref="PromptInputException"/>
	/// with a given message.
	/// </summary>
	public PromptInputException(string? message) : base(message) { }

	/// <summary>
	/// Creates a new instance of <see cref="PromptInputException"/>
	/// with default message and a given inner exception
	/// </summary>
	public PromptInputException(Exception? innerException) : this(DefaultMessage, innerException) { }

	/// <summary>
	/// Creates a new instance of <see cref="PromptInputException"/>
	/// with a given message and an inner exception
	/// </summary>
	public PromptInputException(string? message, Exception? innerException) : base(message, innerException) { }

}
