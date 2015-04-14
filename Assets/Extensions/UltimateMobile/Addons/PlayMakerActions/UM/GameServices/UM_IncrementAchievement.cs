using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Increment Achivement Progress")]
	public class UM_IncrementAchievement : FsmStateAction {

		public FsmString achivmentId;
		public FsmFloat progress;
		
		public override void OnEnter() {
			UM_GameServiceManager.instance.IncrementAchievement (achivmentId.Value, progress.Value);
			Finish();
		}
	}
}
