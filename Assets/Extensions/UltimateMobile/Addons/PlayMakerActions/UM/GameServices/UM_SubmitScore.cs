using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Submit Player Score for certain Leaderboard")]
	public class UM_SubmitScore : FsmStateAction {

		public FsmString leaderboardId;
		public FsmInt score;		
		
		public override void OnEnter() {
			UM_GameServiceManager.instance.SubmitScore (leaderboardId.Value, score.Value);
			Finish();			
		}
	}
}
