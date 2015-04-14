using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Show Achivements UI")]
	public class UM_ShowAchievementsUI : FsmStateAction {
		
		public override void Reset() {
			
		}
		
		public override void OnEnter() {
			UM_GameServiceManager.instance.ShowAchievementsUI ();
			Finish ();
		}
	}
}
