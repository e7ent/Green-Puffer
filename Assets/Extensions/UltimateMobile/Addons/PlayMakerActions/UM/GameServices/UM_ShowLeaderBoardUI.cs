using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Show UI for certain Leaderboard")]
	public class UM_ShowLeaderBoardUI : FsmStateAction {

		public FsmString leaderboardId;

		public override void Reset() {

		}
		
		public override void OnEnter() {
			UM_GameServiceManager.instance.ShowLeaderBoardUI (leaderboardId.Value);
			Finish();
			
		}
	}
}
