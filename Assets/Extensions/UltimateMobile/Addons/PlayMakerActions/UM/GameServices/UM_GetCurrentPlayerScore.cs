using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Get Current Player Score for certain Leaderboard")]
	public class UM_GetCurrentPlayerScore : FsmStateAction {

		public FsmString leaderboardId;

		[ActionSection("Result")]

		[UIHint(UIHint.Variable)]
		public FsmInt score;

		public override void Reset() {
			
		}
		
		public override void OnEnter() {
			score.Value = (int)UM_GameServiceManager.instance.GetCurrentPlayerScore (leaderboardId.Value);
			Finish();
		}
	}
}
