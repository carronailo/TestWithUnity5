
using System;
using UnityEngine;

public enum ERawInputEventType
{
	TOUCH_EVENT_BEGIN,
	TouchBegun,
	TouchMoved,
	TouchHolden,
	TouchEnded,
	TouchCanceled,
	TOUCH_EVENT_END,

	AXIS_EVENT_BEGIN,
	AxisMainHorizontal,         // （主/左）水平轴参数
	AxisMainVertical,           // （主/左）垂直轴参数
	AxisMainVector,             // （主/左）水平轴和垂直轴组合后的二维向量
	AxisSecondaryHorizontal,    // （右）水平轴参数
	AxisSecondaryVertical,      // （右）垂直轴参数
	AxisSecondaryVector,        // （右）水平轴和垂直轴组合后的二维向量
	AxisThirdValue,				// 第三轴参数（鼠标滚轮模拟）				
	AxisButtonPressed,          // 按键按下（0~N，Submit，Cancel）
	AxisButtonHolden,           // 按键按住（0~N，Submit，Cancel）
	AxisButtonReleased,         // 按键松开（0~N，Submit，Cancel）
	AXIS_EVENT_END,

	KEY_EVENT_BEGIN,
	KeyPressed,                 // 键盘按键按下
	KeyHolden,                  // 键盘按键按住
	KeyReleased,                // 键盘按键松开
	KEY_EVENT_END,

	MOUSE_EVENT_BEGIN,
	MouseButtonPressed,         // 鼠标按键按下（左，中，右）
	MouseButtonHolden,          // 鼠标按键按住（左，中，右）
	MouseButtonReleased,        // 鼠标按键松开（左，中，右）
	MOUSE_EVENT_END,
}

/// <summary>
/// 原始输入类型定义
/// </summary>
[Flags]
public enum ERawInputMetaType
{
	/// <summary>
	/// 不支持的类型
	/// </summary>
	Unknown = 0,
	/// <summary>
	/// 触摸原始输入类型
	/// </summary>
	Touch = 1,
	/// <summary>
	/// 手柄控制器原始输入类型
	/// </summary>
	Axis = 2,
	/// <summary>
	/// 键盘原始输入类型
	/// </summary>
	Keyboard = 4,
	/// <summary>
	/// 鼠标原始输入类型
	/// </summary>
	Mouse = 8,
}

/// <summary>
/// 逻辑输入事件定义
/// </summary>
public enum ELogicInputEventType
{
	MAIN_JOYSTICK_EVENT_BEGIN,
	/// <summary>
	/// 主摇杆输入事件
	/// </summary>
	MainJoystickSway,
	MAIN_JOYSTICK_EVENT_END,

	SECONDARY_JOYSTICK_EVENT_BEGIN,
	/// <summary>
	/// 次摇杆输入事件
	/// </summary>
	SecondaryJoystickSway,
	SECONDARY_JOYSTICK_EVENT_END,


}

/// <summary>
/// 逻辑输入类型定义
/// </summary>
[Flags]
public enum ELogicInputMetaType
{
	/// <summary>
	/// 不支持的类型
	/// </summary>
	Unknown = 0,
	/// <summary>
	/// 主摇杆逻辑输入事件类型
	/// </summary>
	MainJoytick = 1,
	/// <summary>
	/// 次摇杆逻辑输入事件类型
	/// </summary>
	SecondaryJoystick = 2,
}

[Flags]
public enum EInputModule
{
	MonoJoystick = 1,
	DualJoystick = 2,
	ButtonPad = 4,
	GestureRecognizer = 8,
	MonoJoystickSimulator = 16,
	DualJoystickSimulator = 32,
	ButtonPadSimulator = 64,
}

/// <summary>
/// 摇杆状态
/// </summary>
public enum EJoystickStatus
{
	/// <summary>
	/// 无效状态，未初始化状态
	/// </summary>
	Invalid = 0,
	/// <summary>
	/// 摇杆位置保持没有改变
	/// </summary>
	Holden = 1,
	/// <summary>
	/// 摇杆位置已经发生变化
	/// </summary>
	Swayed = 2,
	/// <summary>
	/// 摇杆位置已经归零
	/// </summary>
	Released = 3,
}

/// <summary>
/// 手柄（虚拟手柄）摇杆输入数据
/// </summary>
public struct JoystickInputData
{
	/// <summary>
	/// 当前摇杆的摇摆偏移数值
	/// </summary>
	public Vector2 swayVector;
	/// <summary>
	/// 对比上一帧，摇杆的摇摆偏移数值的变化值
	/// </summary>
	public Vector2 swayDelta;
	/// <summary>
	/// 当前摇杆的状态
	/// </summary>
	public EJoystickStatus status;
}

/// <summary>
/// 手柄（虚拟手柄）按钮输入数据
/// </summary>
public struct ButtonPadInputData
{

}

/// <summary>
/// 手势识别输入数据
/// </summary>
public struct GestureInputData
{

}

