using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Initialize Game Service. Best practice to do this on appplicaton start")]
	public class UM_GameServiceSignIn : FsmStateAction {

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
				Fsm.Event(successEvent);
				Finish();
				return;
			}

			UM_GameServiceManager.OnPlayerConnected += OnPlayerConnected;
			UM_GameServiceManager.OnPlayerDisconnected += OnPlayerDisconnected;
			UM_GameServiceManager.instance.Connect ();
			
		}
		
		private void OnPlayerConnected () {
			UM_GameServiceManager.OnPlayerConnected -= OnPlayerConnected;
			UM_GameServiceManager.OnPlayerDisconnected -= OnPlayerDisconnected;

			Fsm.Event(successEvent);			
			Finish();
		}

		private void OnPlayerDisconnected () {
			UM_GameServiceManager.OnPlayerConnected -= OnPlayerConnected;
			UM_GameServiceManager.OnPlayerDisconnected -= OnPlayerDisconnected;

			Fsm.Event(failEvent);			
			Finish();
		}
	}
}
