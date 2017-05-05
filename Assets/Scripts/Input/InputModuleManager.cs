using UnityEngine;

public class InputModuleManager : MonoBehaviour
{
	public EInputModule usedInputModules;
	private EInputModule prevUsedInputModules;

	private bool HasMonoJoystick { get { return (usedInputModules & EInputModule.MonoJoystick) != 0; } }
	private bool HasDualJoystick { get { return (usedInputModules & EInputModule.DualJoystick) != 0; } }
	private bool HasButtonPad { get { return (usedInputModules & EInputModule.ButtonPad) != 0; } }
	private bool HasGestureRecognizer { get { return (usedInputModules & EInputModule.GestureRecognizer) != 0; } }
	private bool HasMonoJoystickSimulator { get { return (usedInputModules & EInputModule.MonoJoystickSimulator) != 0; } }
	private bool HasDualJoystickSimulator { get { return (usedInputModules & EInputModule.DualJoystickSimulator) != 0; } }
	private bool HasButtonPadSimulator { get { return (usedInputModules & EInputModule.ButtonPadSimulator) != 0; } }

	private void Start()
	{
		ReloadInputModule();
		prevUsedInputModules = usedInputModules;
	}

	private void Update()
	{
		if(prevUsedInputModules != usedInputModules)
		{
			ReloadInputModule();
			prevUsedInputModules = usedInputModules;
		}
	}

	public void ReloadInputModule()
	{
		CheckConflictModule();
		ReloadMonoJoystick();
		ReloadDualJoystick();
		ReloadButtonPad();
		ReloadGestureRecognizer();
		ReloadMonoJoystickSimulator();
		ReloadDualJoystickSimulator();
		ReloadButtonPadSimulator();
	}

	void CheckConflictModule()
	{
		bool hasJoystick = false;
		hasJoystick = HasMonoJoystick;
		if (hasJoystick && HasDualJoystick)
			LogConsole.LogWarning("InputModuleManager", "MonoJoystick，DualJoystick，MonoJoystickSimulator，DualJoystickSimulator，这四种输入模块只能选用一个");
		else if (HasDualJoystick)
			hasJoystick = true;
		if (hasJoystick && HasMonoJoystickSimulator)
			LogConsole.LogWarning("InputModuleManager", "MonoJoystick，DualJoystick，MonoJoystickSimulator，DualJoystickSimulator，这四种输入模块只能选用一个");
		else if (HasMonoJoystickSimulator)
			hasJoystick = true;
		if (hasJoystick && HasDualJoystickSimulator)
			LogConsole.LogWarning("InputModuleManager", "MonoJoystick，DualJoystick，MonoJoystickSimulator，DualJoystickSimulator，这四种输入模块只能选用一个");
		else if (HasDualJoystickSimulator)
			hasJoystick = true;

		if(HasButtonPad && HasButtonPadSimulator)
			LogConsole.LogWarning("InputModuleManager", "ButtonPad，ButtonPadSimulator，这两种输入模块只能选用一个");
	}

	void ReloadMonoJoystick()
	{
		MonoJoystick monoJoystick = GetComponent<MonoJoystick>();
		if (monoJoystick == null && HasMonoJoystick)
			gameObject.AddComponent<MonoJoystick>();
		else if (monoJoystick != null && !HasMonoJoystick)
		{
			if (Application.isPlaying)
				Destroy(monoJoystick);
			else
				DestroyImmediate(monoJoystick);
		}
	}

	void ReloadDualJoystick()
	{
		DualJoystick dualJoystick = GetComponent<DualJoystick>();
		if (dualJoystick == null && HasDualJoystick)
			gameObject.AddComponent<DualJoystick>();
		else if (dualJoystick != null && !HasDualJoystick)
		{
			if (Application.isPlaying)
				Destroy(dualJoystick);
			else
				DestroyImmediate(dualJoystick);
		}
	}

	void ReloadButtonPad()
	{

	}

	void ReloadGestureRecognizer()
	{

	}

	void ReloadMonoJoystickSimulator()
	{

	}

	void ReloadDualJoystickSimulator()
	{

	}

	void ReloadButtonPadSimulator()
	{

	}

}
