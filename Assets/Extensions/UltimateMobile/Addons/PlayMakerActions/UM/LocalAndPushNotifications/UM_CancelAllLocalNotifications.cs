using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Local And Push Notifications")]
	[Tooltip("Cancel all scheduled notifications")]
	public class UM_CancelAllLocalNotifications : FsmStateAction {

		public override void Reset() {

		}
		
		public override void OnEnter() {
			UM_NotificationController.instance.CancelAllLocalNotifications ();
			Finish ();
		}
	}
}
