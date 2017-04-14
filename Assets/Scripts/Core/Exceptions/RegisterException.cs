using System;

public class RegisterException : Exception
{
	private string message;

	public override string Message
	{
		get { return message; }
	}

	protected RegisterException()
		: base()
	{
	}

	public RegisterException(string message)
		: base()
	{
		this.message = message;
	}

}
