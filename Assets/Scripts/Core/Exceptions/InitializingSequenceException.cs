using System;

public class InitializingSequenceException : Exception
{
	private string message;

	public override string Message
	{
		get { return message; }
	}

	protected InitializingSequenceException()
		: base()
	{
	}

	public InitializingSequenceException(string message)
		: base()
	{
		this.message = message;
	}

}
