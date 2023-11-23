using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.SceneManagement;
using UnityEngine;
using Utils.Service;
using Utils;

/// <summary>Scene의 타입입니다. GetSceneName으로 Scene이름을 받아올 수 있습니다.</summary>
public enum SceneType
{
	None = 0,
	// InGame
	scn_game_initialize,
	scn_game_loader,
	scn_game_title,
	scn_game_hideout,
	scn_game_ingame,

	// Art
	scn_art_ui_concepts,

	// Editor
	scn_map_editor,
}

/// <summary>비동기로 Scene을 전환하는 서비스입니다.</summary>
public class AsyncSceneLoadService : IServiceable
{
	public bool IsRunning => mCurrentOperation != null;

	private SceneType mTargetSceneType = SceneType.None;
	private SceneType mDefaultLoaderSceneType = SceneType.scn_game_loader;

	private AsyncOperation mCurrentOperation;
	private Action mOnSceneChangedCompleted;
	private Action mResetAction;

	public void OnRegistered() => reset();
	public void OnUnregistered() => reset();

	private void reset()
	{
		mTargetSceneType = SceneType.None;
		mDefaultLoaderSceneType = SceneType.scn_game_loader;
		mCurrentOperation = null;
		mOnSceneChangedCompleted = null;
	}

	public void BindResetAction(Action resetAction)
	{
		mResetAction = resetAction;
	}

	/// <summary>Scene을 비동기로 전환 합니다. 전환 시도가 실패하면 false를 반환합니다.</summary>
	/// <param name="targetSceneType">목표 Scene입니다.</param>
	/// <param name="onSceneChangedCompleted">Scene변경이 완료되면 호출할 이벤트입니다.</param>
	/// <returns>Scene 비동기 전환 시도 결과입니다.</returns>
	public bool TryLoadSceneAsync(SceneType targetSceneType, Action onSceneChangedCompleted = null)
	{
		if (!targetSceneType.IsLoadableScene())
		{
			Ulog.LogError(this, $"You cannot load scene \"{targetSceneType}\"");
			return false;
		}

		if (IsRunning)
		{
			Ulog.LogError(this, $"It's still load to \"{mTargetSceneType}\"");
			return false;
		}

		// Reset
		mResetAction?.Invoke();

		// Try Load Scene
		mOnSceneChangedCompleted = onSceneChangedCompleted;
		mTargetSceneType = targetSceneType;

		mCurrentOperation = SceneManager.LoadSceneAsync(mDefaultLoaderSceneType.GetSceneName());
		mCurrentOperation.completed += onLoaderSceneLoadCompleted;
		return true;
	}

	private void onLoaderSceneLoadCompleted(AsyncOperation operation)
	{
		mCurrentOperation.completed -= onLoaderSceneLoadCompleted;
		mCurrentOperation = SceneManager.LoadSceneAsync(mTargetSceneType.GetSceneName());
		mCurrentOperation.completed += onTargetSceneLoadCompleted;
	}

	private void onTargetSceneLoadCompleted(AsyncOperation operation)
	{
		mCurrentOperation.completed -= onTargetSceneLoadCompleted;
		mCurrentOperation = null;
		mOnSceneChangedCompleted?.Invoke();
	}

	/// <summary>Scene 전환 진행도를 반환받습니다.</summary>
	/// <returns>Scene 전환 진행도</returns>
	public float GetPrograss()
	{
		return mCurrentOperation == null ? 0 : mCurrentOperation.progress;
	}
}

public static class SceneTypeExtension
{
	/// <summary>Scene 이름을 반환받습니다.</summary>
	/// <param name="sceneType">Scene 타입</param>
	/// <returns>Scene 문자열</returns>
	public static string GetSceneName(this SceneType sceneType)
	{
		// Enum 타입에 대해서 ToString을 GetSceneName으로 캡슐화 한 이유는
		// 추후 특수한 이유로 Type에 대해서 별도의 Scene 이름 관리가 필요한 경우를 대비하기 위함입니다.
		return sceneType.ToString();
	}

	public static bool IsLoadableScene(this SceneType sceneType)
	{
		return !(sceneType == SceneType.None || sceneType == SceneType.scn_game_loader);
	}
}