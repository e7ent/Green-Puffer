using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Social Sharing")]
	[Tooltip("Posting to Facebook")]
	public class UM_FacebookShare : FsmStateAction {
		
		public FsmString message;
		public FsmTexture texture;
		
		public override void OnEnter() {
			UM_ShareUtility.FacebookShare (message.Value, texture.Value as Texture2D);
			Finish ();
		}
		
		public override void Reset() {
			base.Reset();
			message   = "Status Text";			
		}
	}
}
