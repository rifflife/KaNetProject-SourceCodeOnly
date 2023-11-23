using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;
using Utils.Service;

public enum InputState
{
	None = 0,
	InGame,
	InGame_Event,
	InGame_Chat,
	GUI,
	SystemDialog
}

public enum InputType
{
	None = 0,

	// Mouse
	Mouse_Left = 0,
	Mouse_Right = 1,

	// Environment
	Key_Escape = 10,
	Key_Tab,

	// Movement
	Key_ArrowLeft,
	Key_ArrowRight,
	Key_ArrowUp,
	Key_ArrowDown,

	// Interact
	Key_Reload,
	Key_Interact,
	Key_HealSelf,

	Key_Jump,
	Key_Crouch,

	// Swap
	Key_SwapEquipment_Primary,
	Key_SwapEquipment_Secondary,
	Key_SwapEquipment_Auxiliary,
}

public abstract class InputAction
{
	/// <summary>키가 눌렸다가 떼어졌을 때</summary>
	public abstract event Action OnReleased;
	/// <summary>키가 눌렸을 때</summary>
	public abstract event Action OnPressed;
	/// <summary>키가 눌리고 있는 도중</summary>
	public abstract event Action OnPressing;
	/// <summary>키가 눌리고 있는지 여부</summary>
	public abstract event Action<bool> OnIsPressing;
	protected bool mIsPressed = false;

	public abstract void UpdateInput();

	public abstract void Clear();
}

public class MouseAction : InputAction
{
	public override event Action OnReleased;
	public override event Action OnPressed;
	public override event Action OnPressing;
	public override event Action<bool> OnIsPressing;

	private int mMouseButton;

	public MouseAction(int mouseButton)
	{
		mMouseButton = mouseButton;
	}

	public override void UpdateInput()
	{
		if (Input.GetMouseButton(mMouseButton))
		{
			OnPressing?.Invoke();

			if (!mIsPressed)
			{
				OnPressed?.Invoke();
				mIsPressed = true;
			}
		}
		else
		{
			if (mIsPressed)
			{
				OnReleased?.Invoke();
				mIsPressed = false;
			}
		}

		OnIsPressing?.Invoke(mIsPressed);
	}

	public override void Clear()
	{
		OnReleased = null;
		OnPressed = null;
		OnPressing = null;
		OnIsPressing = null;
	}
}

public class KeyboardAction : InputAction
{
	public override event Action OnReleased;
	public override event Action OnPressed;
	public override event Action OnPressing;
	public override event Action<bool> OnIsPressing;

	private KeyCode[] mKeyCode;

	public KeyboardAction(params KeyCode[] keyCode)
	{
		mKeyCode = keyCode;
	}

	public override void UpdateInput()
	{
		bool isPressed = false;

		foreach (var key in mKeyCode)
		{
			isPressed |= Input.GetKey(key);
		}

		if (isPressed)
		{
			OnPressing?.Invoke();

			if (!mIsPressed)
			{
				OnPressed?.Invoke();
				mIsPressed = true;
			}
		}
		else
		{
			if (mIsPressed)
			{
				OnReleased?.Invoke();
				mIsPressed = false;
			}
		}

		OnIsPressing?.Invoke(mIsPressed);
	}

	public override void Clear()
	{
		OnReleased = null;
		OnPressed = null;
		OnPressing = null;
		OnIsPressing = null;
	}
}

public class InputService : MonoService
{
	public event Action OnUpdateInput;

	private Dictionary<InputType, InputAction> mInputActions = new();

	public InputState State { get; private set; } = InputState.None;

	public void SetInputState(InputState state)
	{
		State = state;
	}

	public InputAction GetInputAction(InputType inputType)
	{
		return mInputActions[inputType];
	}

	public override void OnRegistered()
	{
		base.OnRegistered();

		// Input 관련 초기화
		mInputActions.Add(InputType.Key_Escape, new KeyboardAction(KeyCode.Escape));
		mInputActions.Add(InputType.Key_Tab, new KeyboardAction(KeyCode.Tab));

		mInputActions.Add(InputType.Key_ArrowLeft, new KeyboardAction(KeyCode.A, KeyCode.LeftArrow));
		mInputActions.Add(InputType.Key_ArrowRight, new KeyboardAction(KeyCode.D, KeyCode.RightArrow));
		mInputActions.Add(InputType.Key_ArrowUp, new KeyboardAction(KeyCode.W, KeyCode.UpArrow));
		mInputActions.Add(InputType.Key_ArrowDown, new KeyboardAction(KeyCode.S, KeyCode.DownArrow));

		mInputActions.Add(InputType.Key_SwapEquipment_Primary, new KeyboardAction(KeyCode.Alpha1));
		mInputActions.Add(InputType.Key_SwapEquipment_Secondary, new KeyboardAction(KeyCode.Alpha2));
		mInputActions.Add(InputType.Key_SwapEquipment_Auxiliary, new KeyboardAction(KeyCode.Alpha3));

		mInputActions.Add(InputType.Key_Reload, new KeyboardAction(KeyCode.R));
		mInputActions.Add(InputType.Key_Interact, new KeyboardAction(KeyCode.E));
		mInputActions.Add(InputType.Key_HealSelf, new KeyboardAction(KeyCode.Q));

		mInputActions.Add(InputType.Key_Jump, new KeyboardAction(KeyCode.Space));
		mInputActions.Add(InputType.Key_Crouch, new KeyboardAction(KeyCode.LeftControl));

		mInputActions.Add(InputType.Mouse_Left, new MouseAction(0));
		mInputActions.Add(InputType.Mouse_Right, new MouseAction(1));
	}

	public override void OnUnregistered()
	{
		base.OnUnregistered();
	}

	private void Update()
	{
		foreach (var action in mInputActions.Values)
		{
			action.UpdateInput();
		}

		OnUpdateInput?.Invoke();
	}

	public void Clear()
	{
		State = InputState.None;

		foreach (var action in mInputActions.Values)
		{
			action.Clear();
		}
	}
}
