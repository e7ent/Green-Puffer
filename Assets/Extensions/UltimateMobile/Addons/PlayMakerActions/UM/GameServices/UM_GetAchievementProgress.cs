using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Get Achivements Progress")]
	public class UM_GetAchievementsProgress : FsmStateAction {

		public FsmString achievementId;

		[ActionSection("Result")]
		
		[UIHint(UIHint.Variable)]
		public FsmFloat achievementProgress;		
		
		public override void Reset() {
			
		}		
		
		public override void OnEnter() {
			achievementProgress.Value = UM_GameServiceManager.instance.GetAchievementProgress (achievementId.Value);
			Finish ();
		}
	}
}
