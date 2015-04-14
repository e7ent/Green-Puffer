using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Camera And Gallery")]
	[Tooltip("Get Image from device Camera")]
	public class UM_GetImageFromCamera : FsmStateAction {

		public FsmEvent successEvent;
		public FsmEvent failEvent;
		
		[ActionSection("Result")]
		
		[UIHint(UIHint.Variable)]
		public FsmTexture image;
		
		public override void Reset() {
			
		}
		
		public override void OnEnter() {
			UM_Camera.instance.OnImagePicked += OnImagePicked;
			UM_Camera.instance.GetImageFromCamera ();
		}

		private void OnImagePicked (UM_ImagePickResult result) {
			UM_Camera.instance.OnImagePicked -= OnImagePicked;

			if(result.IsSucceeded) {
				image.Value = result.image as Texture;

				Fsm.Event(successEvent);
				Finish ();
			} else {
				Fsm.Event(failEvent);
				Finish ();
			}
		}
	}
}
