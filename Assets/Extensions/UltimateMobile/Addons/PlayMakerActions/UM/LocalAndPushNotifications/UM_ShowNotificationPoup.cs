using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Local And Push Notifications")]
	[Tooltip("Show simple Notification Banner/PopUp")]
	public class UM_ShowNotificationPoup : FsmStateAction {

		public FsmString popupTitle;
		public FsmString popupMessage;
		
		public override void Reset() {
			
		}
		
		public override void OnEnter() {
			UM_NotificationController.instance.ShowNotificationPoup (popupTitle.Value, popupMessage.Value);
			Finish ();
		}
	}
}
