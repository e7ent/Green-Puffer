using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Reset Player Achievements progress for testing purposes")]
	public class UM_ResetAchievements : FsmStateAction {

		public override void Reset() {
			
		}	
		
		public override void OnEnter() {
			UM_GameServiceManager.instance.ResetAchievements ();
			Finish();			
		}
	}
}
