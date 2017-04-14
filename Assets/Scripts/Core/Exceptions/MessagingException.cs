using System;

public class MessagingException : Exception
{
	private string message;

	public override string Message
	{
		get { return message; }
	}

	protected MessagingException()
		: base()
	{
	}

	public MessagingException(string message)
		: base()
	{
		this.message = message;
	}
}
