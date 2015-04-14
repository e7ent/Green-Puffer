using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Camera And Gallery")]
	[Tooltip("Save an Image to Device Gallery")]
	public class UM_SaveImageToGallery : FsmStateAction {

		public FsmTexture image;

		public FsmEvent successEvent;
		public FsmEvent failEvent;

		public override void Reset() {

		}

		public override void OnEnter() {
			UM_Camera.instance.OnImageSaved += OnImageSaved;
			UM_Camera.instance.SaveImageToGalalry (image.Value as Texture2D);
		}

		private void OnImageSaved (UM_ImageSaveResult result) {
			UM_Camera.instance.OnImageSaved -= OnImageSaved;

			if(result.IsSucceeded) {
				Fsm.Event(successEvent);
				Finish ();
			} else {
				Fsm.Event(failEvent);
				Finish ();
			}
		}
	}
}
