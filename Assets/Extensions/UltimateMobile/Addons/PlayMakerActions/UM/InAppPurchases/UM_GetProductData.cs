using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {

	[ActionCategory("Ultimate Mobile - InAppPurchases")]
	[Tooltip("Get product ID's data. UM_BillingInit action has to be called first")]
	public class UM_GetProductData : FsmStateAction {

		[RequiredField]
		public FsmString productIdentifier;
		
		[ActionSection("Result")]
		
		[UIHint(UIHint.Variable)]
		public FsmString title;
		
		[UIHint(UIHint.Variable)]
		public FsmString description;
		
		[UIHint(UIHint.Variable)]
		public FsmString price;
		
		public FsmEvent productFoundEvent;
		public FsmEvent productNotFoundEvent;
		
		
		public override void Reset() {
			productIdentifier = null;
			title = null;
			description = null;
			price = null;
			productFoundEvent = null;
			productNotFoundEvent = null;
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
			
			UM_InAppProduct tpl = UM_InAppPurchaseManager.instance.GetProductById(productIdentifier.Value);
			if(tpl != null) {
				title.Value 			= tpl.template.title;
				description.Value 		= tpl.template.description;
				price.Value				= tpl.template.price;
				Fsm.Event(productFoundEvent);
			} else {
				Fsm.Event(productNotFoundEvent);
			}			
			
			Finish ();
		}

		private void OnStoreInitComplete (UM_BillingConnectionResult res) {
			UM_InAppPurchaseManager.OnBillingConnectFinishedAction -= OnStoreInitComplete;
			if(res.isSuccess) {
				OnBillingInited();
			} else {
				Fsm.Event(productNotFoundEvent);
				Finish();
			}
		}
	}
}
