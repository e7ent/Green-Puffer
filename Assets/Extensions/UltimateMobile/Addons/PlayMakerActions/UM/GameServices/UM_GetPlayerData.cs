using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Get Player ID")]
	public class UM_GetPlayerData : FsmStateAction {

		public FsmString playerId;
		public FsmString playerName;
		public FsmTexture playerAvatar;

		public FsmEvent successEvent;
		public FsmEvent failEvent;
		
		public override void Reset() {
			
		}
		
		public override void OnEnter() {
			
			bool IsInEdditorMode = false;
			
#if UNITY_EDITOR
			IsInEdditorMode = true;
#endif
			
			
			if(IsInEdditorMode) {
				playerId.Value = "1";
				Fsm.Event(successEvent);
				Finish();
				return;
			}			
						
			if(UM_GameServiceManager.instance.ConnectionSate != UM_ConnectionState.CONNECTED) {
				UM_GameServiceManager.OnPlayerConnected += OnPlayerConnected;
				UM_GameServiceManager.OnPlayerDisconnected += OnPlayerDisconnected;
				UM_GameServiceManager.instance.Connect ();
			} else {
				playerId.Value = UM_GameServiceManager.instance.player.PlayerId;
				playerName.Value = UM_GameServiceManager.instance.player.Name;
				playerAvatar.Value = UM_GameServiceManager.instance.player.Avatar;

				Fsm.Event(successEvent);
				Finish();
			}
			
			
		}

		private void OnPlayerConnected () {
			UM_GameServiceManager.OnPlayerConnected -= OnPlayerConnected;
			UM_GameServiceManager.OnPlayerDisconnected -= OnPlayerDisconnected;

			playerId.Value = UM_GameServiceManager.instance.player.PlayerId;
			playerName.Value = UM_GameServiceManager.instance.player.Name;
			playerAvatar.Value = UM_GameServiceManager.instance.player.Avatar;

			Fsm.Event(successEvent);			
			Finish();
		}
		
		private void OnPlayerDisconnected () {
			UM_GameServiceManager.OnPlayerConnected -= OnPlayerConnected;
			UM_GameServiceManager.OnPlayerDisconnected -= OnPlayerDisconnected;

			playerId.Value = "0";
			Fsm.Event(failEvent);			
			Finish();
		}
	}
}
