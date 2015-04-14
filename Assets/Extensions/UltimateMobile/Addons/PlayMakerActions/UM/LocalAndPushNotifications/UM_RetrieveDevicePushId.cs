using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Local And Push Notifications")]
	[Tooltip("Retrieve the push Device ID")]
	public class UM_RetrieveDevicePushId : FsmStateAction {

		public FsmEvent successEvent;
		public FsmEvent failEvent;
		
		[ActionSection("Result")]
		
		[UIHint(UIHint.Variable)]
		public FsmString devicePushId;

		public override void Reset() {
			
		}
		
		public override void OnEnter() {
			UM_NotificationController.instance.OnPushIdLoaded += OnPushIdLoaded;
			UM_NotificationController.instance.RetrieveDevicePushId ();
		}

		private void OnPushIdLoaded (UM_PushRegistrationResult res) {
			UM_NotificationController.instance.OnPushIdLoaded -= OnPushIdLoaded;

			if(res.IsSucceeded) {
				devicePushId.Value = res.deviceId;

				Fsm.Event(successEvent);
				Finish();
			} else {
				devicePushId.Value = string.Empty;

				Fsm.Event(failEvent);
				Finish();
			}
		}
	}
}
