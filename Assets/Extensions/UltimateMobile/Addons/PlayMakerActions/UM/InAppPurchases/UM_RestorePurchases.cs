using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - InAppPurchases")]
	[Tooltip("Restore purchases, restored event will be fired for ectach restored purchase")]
	public class UM_RestorePurchases : FsmStateAction {
		public FsmString[] restoredProducts;
		public FsmString   currentRestoredItem;
		
		[Tooltip("Event fired when Product restored")]
		public FsmEvent successEvent;
		
		[Tooltip("Event fired when restore Purchase failed")]
		public FsmEvent failEvent;
		
		[Tooltip("Event fired when Purchase restored")]
		public FsmEvent ItemRestoredEvent;	
		
		private List<FsmString> restoredProductsCash =  new List<FsmString>();		
		
		public override void Reset() {
			restoredProductsCash = new List<FsmString>();
		}
		
		
		public override void OnEnter() {

			if (!UM_InAppPurchaseManager.instance.IsInited) {
				UM_InAppPurchaseManager.OnBillingConnectFinishedAction += OnStoreInitComplete;
				UM_InAppPurchaseManager.instance.Init();
			} else {
				OnBillingInited();
			}
			
#if UNITY_EDITOR
			Fsm.Event(successEvent);
			Finish();
			return;
#endif
			
		}
		
		private void OnBillingInited() {
			UM_InAppPurchaseManager.OnPurchaseFlowFinishedAction += OnPurchaseComplete;
			UM_InAppPurchaseManager.instance.addEventListener(UM_InAppPurchaseManager.ON_PURCHASE_FLOW_FINISHED, OnComplete);
			UM_InAppPurchaseManager.instance.RestorePurchases ();
		}		
		
		private void OnPurchaseComplete (UM_PurchaseResult res) {			
			if(res.isSuccess) {
				restoredProductsCash.Add(res.product.id);
				currentRestoredItem.Value = res.product.id;
				Fsm.Event(ItemRestoredEvent);
			} else {
				Fsm.Event(failEvent);
			}			
		}		
		
		private void OnComplete() {
			UM_InAppPurchaseManager.OnPurchaseFlowFinishedAction -= OnPurchaseComplete;
			UM_InAppPurchaseManager.instance.removeEventListener(UM_InAppPurchaseManager.ON_PURCHASE_FLOW_FINISHED, OnComplete);			
			
			restoredProducts = restoredProductsCash.ToArray ();
			Fsm.Event(successEvent);
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
