using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Local And Push Notifications")]
	[Tooltip("Scheduling local notification")]
	public class UM_ScheduleLocalNotification : FsmStateAction {

		public FsmString notificationTitle;
		public FsmString notificationMessage;
		public FsmInt notificationTime;

		[ActionSection("Result")]
		
		[UIHint(UIHint.Variable)]
		public FsmInt notificationId;
		
		public override void Reset() {
			
		}
		
		public override void OnEnter() {
			notificationId.Value = UM_NotificationController.instance.ScheduleLocalNotification (notificationTitle.Value,
			                                                                                    notificationMessage.Value,
			                                                                                    notificationTime.Value);
			Finish ();
		}
	}
}
