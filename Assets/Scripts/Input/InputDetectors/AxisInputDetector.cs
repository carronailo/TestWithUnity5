using UnityEngine;

public class AxisInputDetector : InputDetector
{
	private static AxisInputDetector instance;

	private void Awake()
	{
		// 同一种InputDetector只能同时存在一个
		if (instance == null)
			instance = this;
		else
			Destroy(this);
	}

	private void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	private void Update()
	{
		ProcessInputAxis();
		ProcessInputButton();
	}

	private void FixedUpdate()
	{
		ProcessInputAxis();
		ProcessInputButton();
	}

	private void ProcessInputAxis()
	{
		float h = Input.GetAxis("Horizontal");
		SubmitMainAxisHorizontal(h);
		float v = Input.GetAxis("Vertical");
		SubmitMainAxisVertical(v);
		SubmitMainAxisVector(new Vector2(h, v));
		h = Input.GetAxis("Mouse X");
		SubmitSecondaryAxisHorizontal(h);
		v = Input.GetAxis("Mouse Y");
		SubmitSecondaryAxisVertical(v);
		SubmitSecondaryAxisVector(new Vector2(h, v));
		float z = Input.GetAxis("Mouse ScrollWheel");
		SubmitThirdAxisValue(z);
	}

	private void ProcessInputButton()
	{
		if (Input.GetButtonDown("Fire1"))
			SubmitButtonPressed(0);
		if (Input.GetButton("Fire1"))
			SubmitButtonHolden(0);
		if (Input.GetButtonUp("Fire1"))
			SubmitButtonReleased(0);

		if (Input.GetButtonDown("Fire2"))
			SubmitButtonPressed(1);
		if (Input.GetButton("Fire2"))
			SubmitButtonHolden(1);
		if (Input.GetButtonUp("Fire2"))
			SubmitButtonReleased(1);

		if (Input.GetButtonDown("Fire3"))
			SubmitButtonPressed(2);
		if (Input.GetButton("Fire3"))
			SubmitButtonHolden(2);
		if (Input.GetButtonUp("Fire3"))
			SubmitButtonReleased(2);

		if (Input.GetButtonDown("Jump"))
			SubmitButtonPressed(3);
		if (Input.GetButton("Jump"))
			SubmitButtonHolden(3);
		if (Input.GetButtonUp("Jump"))
			SubmitButtonReleased(3);

		if (Input.GetButtonDown("Submit"))
			SubmitButtonPressed(4);
		if (Input.GetButton("Submit"))
			SubmitButtonHolden(4);
		if (Input.GetButtonUp("Submit"))
			SubmitButtonReleased(4);

		if (Input.GetButtonDown("Cancel"))
			SubmitButtonPressed(5);
		if (Input.GetButton("Cancel"))
			SubmitButtonHolden(5);
		if (Input.GetButtonUp("Cancel"))
			SubmitButtonReleased(5);
	}

	private void SubmitMainAxisHorizontal(float value)
	{
		InputSystem.ProcessRawInputEvent(ERawInputEventType.AxisMainHorizontal, value);
	}

	private void SubmitMainAxisVertical(float value)
	{
		InputSystem.ProcessRawInputEvent(ERawInputEventType.AxisMainVertical, value);
	}

	private void SubmitMainAxisVector(Vector2 value)
	{
		InputSystem.ProcessRawInputEvent(ERawInputEventType.AxisMainVector, value);
	}

	private void SubmitSecondaryAxisHorizontal(float value)
	{
		InputSystem.ProcessRawInputEvent(ERawInputEventType.AxisSecondaryHorizontal, value);
	}

	private void SubmitSecondaryAxisVertical(float value)
	{
		InputSystem.ProcessRawInputEvent(ERawInputEventType.AxisSecondaryVertical, value);
	}

	private void SubmitSecondaryAxisVector(Vector2 value)
	{
		InputSystem.ProcessRawInputEvent(ERawInputEventType.AxisSecondaryVector, value);
	}

	private void SubmitThirdAxisValue(float value)
	{
		InputSystem.ProcessRawInputEvent(ERawInputEventType.AxisThirdValue, value);
	}

	private void SubmitButtonPressed(int buttonIndex)
	{
		InputSystem.ProcessRawInputEvent(ERawInputEventType.AxisButtonPressed, buttonIndex);
	}

	private void SubmitButtonHolden(int buttonIndex)
	{
		InputSystem.ProcessRawInputEvent(ERawInputEventType.AxisButtonHolden, buttonIndex);
	}

	private void SubmitButtonReleased(int buttonIndex)
	{
		InputSystem.ProcessRawInputEvent(ERawInputEventType.AxisButtonReleased, buttonIndex);
	}

}
