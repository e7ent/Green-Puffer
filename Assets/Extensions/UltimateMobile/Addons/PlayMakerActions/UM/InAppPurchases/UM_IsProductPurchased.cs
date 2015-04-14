using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - InAppPurchases")]
	[Tooltip("Action will start purchase flow on device. In Editor mode Success event will fired immediately")]
	public class UM_IsProductPurchased : FsmStateAction {

		[RequiredField]
		public FsmString productIdentifier;
		
		[ActionSection("Result")]

		[UIHint(UIHint.Variable)]
		public FsmBool isPurchased;
		
		public FsmEvent successEvent;
		public FsmEvent failEvent;

		public override void Reset() {
			productIdentifier = null;
			isPurchased = null;
			successEvent = null;
			failEvent = null;
		}
		
		
		public override void OnEnter() {
			
			if (!UM_InAppPurchaseManager.instance.IsInited) {
				UM_InAppPurchaseManager.OnBillingConnectFinishedAction += OnStoreInitComplete;
				UM_InAppPurchaseManager.instance.Init();
			} else {
				OnBillingInited();
			}
			
		}
		
		private void OnBillingInited() {			
			isPurchased.Value = UM_InAppPurchaseManager.instance.IsProductPurchased(productIdentifier.Value);
			Fsm.Event(successEvent);			
			Finish ();
		}

		private void OnStoreInitComplete (UM_BillingConnectionResult res) {
			UM_InAppPurchaseManager.OnBillingConnectFinishedAction -= OnStoreInitComplete;
			if(res.isSuccess) {
				OnBillingInited();
			} else {
				Fsm.Event(failEvent);
				Finish();
			}
		}
	}
}
