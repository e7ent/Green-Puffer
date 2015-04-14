using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - InAppPurchases")]
	[Tooltip("Initialize billing. Best practice to do this on appplicaton start")]
	public class UM_BillingInit : FsmStateAction {

		[Tooltip("Event fired when Store Kit initlization is complete")]
		public FsmEvent successEvent;
		
		[Tooltip("Event fired when Store Kit initlization is failed")]
		public FsmEvent failEvent;
		
		public override void Reset() {
			
		}		
		
		public override void OnEnter() {
			UM_InAppPurchaseManager.OnBillingConnectFinishedAction += OnStoreInitComplete;
			UM_InAppPurchaseManager.instance.Init ();
		}
		
		private void OnStoreInitComplete (UM_BillingConnectionResult res) {
			UM_InAppPurchaseManager.OnBillingConnectFinishedAction -= OnStoreInitComplete;
			if(res.isSuccess) {
				Fsm.Event(successEvent);
			} else {
				Fsm.Event(failEvent);
			}
			
			
			Finish();
		}
	}
}
