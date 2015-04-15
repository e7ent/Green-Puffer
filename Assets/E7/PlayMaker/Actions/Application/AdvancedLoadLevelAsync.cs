using UnityEngine;
using HutongGames.PlayMaker;

namespace E7.PlayMaker.Actions
{
	using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("AsyncOperation 기능이 추가된 레벨 로드 엑션")]
	public class AdvancedLoadLevelAsync : FsmStateAction
	{
		[RequiredField]
		public FsmString levelName;
		public bool additive;
		public FsmBool allowSceneActivation;
		public FsmEvent loadedEvent;
		public FsmBool dontDestroyOnLoad;
		public FsmFloat progress;

		private static AsyncOperation asyncOperation;

		public override void Reset()
		{
			levelName = "";
			additive = false;
			allowSceneActivation = null;
			loadedEvent = null;
			dontDestroyOnLoad = false;
			progress = null;
		}

		public override void OnEnter()
		{
			
			if (dontDestroyOnLoad.Value)
			{
				var root = Owner.transform.root;
				Object.DontDestroyOnLoad(root.gameObject);
			}

			if (additive)
				asyncOperation = Application.LoadLevelAdditiveAsync(levelName.Value);
			else
				asyncOperation = Application.LoadLevelAsync(levelName.Value);

			asyncOperation.allowSceneActivation = allowSceneActivation.Value;
		}

		public override void OnUpdate()
		{
			progress.Value = asyncOperation.progress;

			if (asyncOperation.allowSceneActivation != allowSceneActivation.Value)
				asyncOperation.allowSceneActivation = allowSceneActivation.Value;


			if (
				(!allowSceneActivation.Value && asyncOperation.progress >= 0.9f)
				|| asyncOperation.isDone)
			{
				Fsm.Event(loadedEvent);
				Finish();
			}
		}
	}
}