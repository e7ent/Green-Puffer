using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Game Service")]
	[Tooltip("Unlock Achievement")]
	public class UM_UnlockAchievement : FsmStateAction {

		public FsmString achivmentId;

		public bool UseCustomNotification = false;
		public FsmString NotificationText;

		public override void Reset() {
			
		}
		
		public override void OnEnter() {
			UM_GameServiceManager.instance.UnlockAchievement (achivmentId.Value);
			if(UseCustomNotification) {
				UM_NotificationController.instance.ShowNotificationPoup("Achievement Unlocked", NotificationText.Value);
			}
			Finish();
		}
	}
}
