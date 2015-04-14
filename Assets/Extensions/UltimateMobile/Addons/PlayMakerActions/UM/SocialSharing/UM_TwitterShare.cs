using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions {
	
	[ActionCategory("Ultimate Mobile - Social Sharing")]
	[Tooltip("Posting to Twitter")]
	public class UM_TwitterShare : FsmStateAction {

		public FsmString status;
		public FsmTexture texture;
		
		public override void OnEnter() {
			UM_ShareUtility.TwitterShare (status.Value, texture.Value as Texture2D);
			Finish ();
		}
		
		public override void Reset() {
			base.Reset();
			status   = "Status Text";			
		}
	}
}
