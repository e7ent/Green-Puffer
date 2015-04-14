using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Social Sharing")]
	[Tooltip("Sharing Media")]
	public class UM_ShareMedia : FsmStateAction {

		public FsmString caption;
		public FsmString message;
		public FsmTexture texture;
		
		public override void OnEnter() {
			UM_ShareUtility.ShareMedia(caption.Value, message.Value, texture.Value as Texture2D);
			Finish ();
		}
		
		public override void Reset() {
			base.Reset();
			message   = "Message Text";			
		}
	}
}
