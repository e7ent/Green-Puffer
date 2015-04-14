using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Local And Push Notifications")]
	[Tooltip("Cancel particular local notification")]
	public class UM_CancelLocalNotification : FsmStateAction {

		public FsmInt notificationId;

		public override void Reset() {
			
		}

		public override void OnEnter() {
			UM_NotificationController.instance.CancelLocalNotification (notificationId.Value);
			Finish ();
		}
	}
}
