using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Show UI for All Leaderboards")]
	public class UM_ShowLeaderBoardsUI : FsmStateAction {

		public override void Reset() {
			
		}
		
		public override void OnEnter() {
			UM_GameServiceManager.instance.ShowLeaderBoardsUI ();
			Finish ();
		}
	}
}
