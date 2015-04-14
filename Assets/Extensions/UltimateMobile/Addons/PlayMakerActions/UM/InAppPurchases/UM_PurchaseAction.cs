using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - InAppPurchases")]
	[Tooltip("Action will start purchase flow on device. In Editor mode Purchased event will fired immediately")]
	public class UM_PurchaseAction : FsmStateAction {		
		[Tooltip("Event fired when InApp purchase is complete")]
		public FsmEvent purchasedEvent;
		
		[Tooltip("Event fired when InApp purchase is failed")]
		public FsmEvent failEvent;
		
		[Tooltip("Purhase product Id")]
		public string ProductID  = "";
				
		public override void OnEnter() {
			
			if (!UM_InAppPurchaseManager.instance.IsInited) {
				UM_InAppPurchaseManager.OnBillingConnectFinishedAction += OnStoreInitComplete;
				UM_InAppPurchaseManager.instance.Init();
			} else {
				OnBillingInited();
			}
			
		}
		
		private void OnBillingInited() {
			UM_InAppPurchaseManager.OnPurchaseFlowFinishedAction += OnTransactionComplete;
			UM_InAppPurchaseManager.instance.Purchase(ProductID);
		}
		
		private void OnTransactionComplete (UM_PurchaseResult result) {
			if(!result.product.id.Equals(ProductID)) {
				return;
			}
			
			UM_InAppPurchaseManager.OnPurchaseFlowFinishedAction -= OnTransactionComplete;

			if (result.isSuccess) {
				Fsm.Event(purchasedEvent);
			} else {
				Fsm.Event(failEvent);
			}
			
			Finish();
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
