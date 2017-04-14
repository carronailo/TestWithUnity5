using System;

public class ModuleCompileException : Exception
{
	private string message;

	public override string Message
	{
		get { return message; }
	}

	protected ModuleCompileException()
		: base()
	{
	}

	public ModuleCompileException(string message)
		: base()
	{
		this.message = message;
	}
}
