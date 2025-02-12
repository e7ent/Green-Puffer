﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;

using UnityEngine;
using UnityEditor;

using MyUtils = Net.FabreJean.UnityEditor; // conflict with Unity Remote utils public class... odd

using Net.FabreJean.UnityEditor;

using Net.FabreJean.UnityEditor.MarkdownSharp;

//using Net.FabreJean.UnityEditor.Parse;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
	public class EcosystemBrowser : EditorWindow {

		private static bool ShowToolBar = false;
		private static bool ShowDisclaimer = false;



		#region EDITOR SETTINGS
		// used for editor prefs prefix
		private static string __namespace__ = "net.fabrejean.playmaker.ecosystem";

		static bool DiscreteTooBar_on = false;
		static bool Debug_on = false;

		const string pathToVersionInfoSource = "Assets/Net.Fabrejean/Editor/PlayMaker/Ecosystem/VersionInfo.json";

		static VersionInfo CurrentVersion;
		static string CurrentVersionAsString = "";//Utils.UpdateVersion(pathToVersionInfoSource).ToShortString();

		/// <summary>
		/// The live version, to compare to the local version, and prompt for updates if necessary
		/// </summary>
		static VersionInfo ECO_BrowserVersion;

		/// <summary>
		/// The live version package, this is the update to get if version is newer.
		/// </summary>
		//static string ECO_BrowserVersion_package;

		#endregion

		enum PlayMakerEcosystemFilters {Actions,Templates,Samples,Packages};
		static int PlayMakerEcosystemFiltersLength = 0;// deduced from the enum when editor inits

		private enum PlayMakerEcosystemRepositoryMasks {Unity3x,Unity4x,PlayMakerBeta};

		static private bool _disclaimer_pass = false;

		static public string __REST_URL_BASE__ = "http://www.fabrejean.net/projects/playmaker_ecosystem/";

		//static private string RepositoryPath = "jeanfabre/PlayMakerCustomActions";//"pbhogan/InControl";

		string searchString = "";
	//	string searchStringFeedback = "";
		string lastSearchString = "";

		string rawSearchResult="";

	//	string searchUrlBase = "https://api.github.com/search/";

		WWW wwwSearch;

		string selectedAction;

		Hashtable searchResultHash;


		Item[] resultItems;

		Dictionary<string,string> downloadsLUT;

		Dictionary<string,Item> itemsLUT;

		List<WWW> downloads;

		private bool filterTouched;

		private List<PlayMakerEcosystemFilters> searchFilters;
		private List<string> repositoryMask;
	
		private Rect ActionListRect;

		private string _lastError;

	//	private RssReader rssFeed; // maybe twitter actually...


		#region Editor window Public properties

		/// <summary>
		/// Check if the user has turned on Debugging
		/// </summary>
		/// <returns><c>true</c> if debug is on; otherwise, <c>false</c>.</returns>
		/// <param name="">.</param>
		public static bool IsDebugOn
		{
			get{
				return Debug_on;  
			}
		}

		#endregion Editor window Public properties

		#region Editor window properties
	//	Vector2 mousePos;
		Vector2 _scroll;
		
		private string editorPath;
		private string editorSkinPath;
		
		private GUISkin editorSkin;
		
		private Vector2 lastMousePosition;
		private int mouseOverRowIndex;
		private Rect[] rowsArea;
		
		private Texture2D bg;

	//	private GUIStyle GUIStyleArrowInBuildSettings;

		private bool ShowFilterUI;

		private bool isCompiling;

		#endregion

		public static EcosystemBrowser Instance;

		// Add menu named "My Window" to the Window menu
		[MenuItem ("PlayMaker/Addons/Ecosystem &e")]
		static void Init () {
		
			//Debug.Log("################ Init");

			RefreshDisclaimerPref();

			// Get existing open window or if none, make a new one:
			Instance = (EcosystemBrowser)EditorWindow.GetWindow (typeof (EcosystemBrowser));

			Instance.position = new Rect(100,100, 430,400);
			Instance.minSize = new Vector2(430,100);
			Instance.title = "Ecosystem";


			// init static vars
			PlayMakerEcosystemFiltersLength = Enum.GetNames(typeof(PlayMakerEcosystemFilters)).Length;
		}

		#region Disclaimer

		static string _disclaimerPass_key = "Ecosystem Disclaimer Pass";
		static string _disclaimer_label = @"### Ecosystem disclaimer

By using this **Ecosystem**, you understand that you will be able to download content (raw scripts and Unity packages) from various online sources and install them on your computer within this project.
In doubt, do not use this and get in touch with us to learn more before you work with it.

##Tips##
- Use online repositories and keep regular backup of your projects.
- When Searching for content, use the filter button to narrow the context of your search, for example if you only want to find actions or samples";

			/*
			"By using this ecosystem, you understand that you will be able to download content (raw scripts and Unity packages)" +
			"from various online sources and install them on your computer within this project. " +
			"In doubt, do not use this and get in touch with us to learn more before you work with it." +
			"\nTips, make use of online repositories and keep regular backup of your projects.";
			*/
		static string _disclaimer_license_label = 
			"THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED," +
			"INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT." +
			"\nIN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY," +
			"WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM," +
			"OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";

		static void RefreshDisclaimerPref()
		{
			// Debug.Log(_disclaimerPass_key+"-"+Application.dataPath);
			_disclaimer_pass = EditorPrefs.GetBool(_disclaimerPass_key+"-"+Application.dataPath);
		}

		private MarkdownGUI _disclaimerMarkdownGUI ;

		private GUIContent _youtubeQuickIntroGUIContent;

		void OnGUI_Disclaimer()
		{
			GUILayout.BeginVertical();

			if ( _disclaimerMarkdownGUI == null )
			{
				_disclaimerMarkdownGUI = new MarkdownGUI() ;
				_disclaimerMarkdownGUI.ProcessSource(_disclaimer_label);
				_disclaimerMarkdownGUI.UserGuiSkin(GUI.skin);

			}

			_disclaimerMarkdownGUI.OnGUILayout_MardkDownTextArea("Label Medium");

			GUILayout.Label(_disclaimer_license_label,"Label FinePrint");


			GUILayout.BeginHorizontal();
				if ( GUILayout.Button("Learn more (online)","Button",GUILayout.Width(200)) )
				{

					Help.BrowseURL(__REST_URL_BASE__+ "link/wiki");
				}

				GUILayout.FlexibleSpace();

				if ( GUILayout.Button(_youtubeQuickIntroGUIContent,"Button",GUILayout.Width(200)) )
				{
					Help.BrowseURL(__REST_URL_BASE__+ "youtube/intro");
				}
		
			GUILayout.EndHorizontal();
				
				GUILayout.Space(5);

				if (!_disclaimer_pass)
				{
					if ( GUILayout.Button("Use the ecosystem!","Button Green") )
					{
						_disclaimer_pass = true;
						//Debug.Log(_disclaimerPass_key+"-"+Application.dataPath);
						EditorPrefs.SetBool(_disclaimerPass_key+"-"+Application.dataPath,true);
						
					}
				}else{
					if ( GUILayout.Button("Back to the Browser","Button") )
					{
						ShowDisclaimer = false;
						
					}
				}
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			if( GUILayout.Button("","Label UI Kit Credit") )
			{
				Application.OpenURL("http://www.killercreations.co.uk/volcanic-ui-kit.php");
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("","Label Jean Fabre Url") )
			{
				Application.OpenURL("http://hutonggames.com/playmakerforum/index.php?action=profile;u=33");
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

		}

		#endregion

		private void OnFocus()
		{
			RefreshDisclaimerPref();
			Repaint();
		}

		private WWW _test;

		private GUIContent _youtubeWatchVideoButtonGuiContent;

		void OnGUI_SetUpSkin()
		{
			// set up the skin if not done yet.
			if (editorSkin==null)
			{
				editorSkin =  MyUtils.Utils.GetGuiSkin("VolcanicGuiSkin",out editorSkinPath);
				bg = (Texture2D)(Resources.LoadAssetAtPath(editorSkinPath+"images/bg.png",typeof(Texture2D))); // Get the texture manually as we have some trickes for bg tiling
				
				//GUIStyleArrowInBuildSettings = editorSkin.FindStyle("Help Arrow 90 degree");
				
			}
			
			// draw the bg properly. Haven't found a way to do it with guiskin only
			if(bg!=null)
			{
				if (bg.wrapMode!= TextureWrapMode.Repeat)
				{
					bg.wrapMode = TextureWrapMode.Repeat;
				}
				GUI.DrawTextureWithTexCoords(new Rect(0,0,position.width,position.height),bg,new Rect(0, 0, position.width / bg.width, position.height / bg.height));
			}

			// init cached content
			Texture _youtubeTexture = editorSkin.FindStyle("YouTube Play Icon").normal.background as Texture;

			_youtubeWatchVideoButtonGuiContent = new GUIContent(" Watch Video",_youtubeTexture);
			_youtubeQuickIntroGUIContent = new GUIContent(" Quick Intro",_youtubeTexture);


		}

		int mousedowncounter = 0;
		string assetToPing ="";

		int SelectedIndex = -1;

		void OnGUI_Main()
		{



			if(!Application.isPlaying && _disclaimer_pass && !ShowDisclaimer)
			{
				
				
				if (ECO_BrowserVersion>CurrentVersion && UpdateBanner_on)
				{
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal("Table Row Green Last",GUILayout.Width(position.width+3));

					string changeType = "";

					if (ECO_BrowserVersion.Major>CurrentVersion.Major)
					{
						changeType = "Major";
					}else if (ECO_BrowserVersion.Minor>CurrentVersion.Minor)
					{
						changeType = "Minor";
					}else if (ECO_BrowserVersion.Patch>CurrentVersion.Patch)
					{
						changeType = "Patch";
					}else if (ECO_BrowserVersion.Type!=CurrentVersion.Type)
					{
						changeType = ECO_BrowserVersion.Type.ToString();
					}

					GUILayout.Label(changeType+" update available :)","Label Row Green");

					if (GUILayout.Button("?","Button Small",GUILayout.Width(20)))
					{
						Debug.Log("ECO_BrowserVersion:"+ECO_BrowserVersion);
						Debug.Log("CurrentVersion:"+CurrentVersion);

						ShowBrowserUpdateInfo();
					}

					if (GUILayout.Button("Later","Button Small",GUILayout.Width(50)))
					{
						PostPonBrowserUpdate();
					}

					if  (GUILayout.Button("Get","Button Small",GUILayout.Width(50)))
					{
						GetBrowserUpdate();
					}
					GUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}

				if (MyUtils.Utils.isPlayMakerInstalled())
				{
					OnGUI_ToolBar();
				}
			}
			
			
			if (!_disclaimer_pass || ShowDisclaimer)
			{
				OnGUI_Disclaimer();
				return;
			}

			if (! MyUtils.Utils.isPlayMakerInstalled() ) {
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal("Table Row Red Last",GUILayout.Width(position.width+3));
				
				GUILayout.Label("PlayMaker must be installed for the ecosystem to be useful","Label Row Red");
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();


				GUILayout.Space(15);
				if (GUILayout.Button("Open PlayMaker on the AssetStore","Button Medium"))
				{
					Application.OpenURL("com.unity3d.kharma:content/368");
				}
				return;
			}


			if (Application.isPlaying)
			{
				GUILayout.Label("Application is playing. Saves performances to not process anything during playback.");
				return ;
			}
			
			OnGUI_FilterPanel();
			
			
			if (!string.IsNullOrEmpty(_lastError))
			{
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal("Table Row Red Last",GUILayout.Width(position.width+3));
				
				GUILayout.Label(_lastError,"Label Row Red");
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				
			}
			
			OnGUI_ItemList();


		

			// detect mouse over top area of the browser window to toggle the toolbar visibility if required
			if (Event.current.type == EventType.Repaint)
			{
				if (lastMousePosition!= Event.current.mousePosition)
				{
					int topDelta = (int)ActionListRect.y ;
					
					// check if we are few pixels above the first row
					if(new Rect(0,-15,position.width,ShowToolBar?40:30).Contains(Event.current.mousePosition))
					{
						ShowToolBar = true;
					}else{
						ShowToolBar = false;
					}
					
					if ( rowsArea!=null )
					{
						int j=0;
						mouseOverRowIndex = -1;
						foreach(Rect _row in rowsArea)
						{
							Rect _temp = _row;
							_temp.x = _temp.x  -_scroll.x;
							_temp.y = _temp.y + topDelta -_scroll.y;
							if (_temp.Contains(Event.current.mousePosition))
							{
								mouseOverRowIndex = j;
								break;
							}
							j++;
						}
					}
					
					lastMousePosition = Event.current.mousePosition;
				}
				
			}

			OnGUI_BottomPanel();

			// User click on a row.
			if (Event.current.type == EventType.mouseDown && mouseOverRowIndex!=-1)
			{
				SelectedIndex = -1;

				mousedowncounter++;

				Repaint();

				if (resultItems==null || mouseOverRowIndex>=resultItems.Length)
				{
					return;
				}

				// Get the Item
				Hashtable item = resultItems[mouseOverRowIndex].RawData;
				

				// USER click on a row
				try
				{

					assetToPing = (string)item["path"];
					
					Hashtable _metaData = LoadItemMetaData(item,true);
					string metaAssetToPing = (string)_metaData["pingAssetPath"];
					if (!string.IsNullOrEmpty(metaAssetToPing))
					{
						assetToPing= metaAssetToPing;
					}
					
					
					if (Debug_on) Debug.Log("Ping -> "+assetToPing);
					EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(assetToPing));
					SelectedIndex = mouseOverRowIndex;

				}catch(Exception e){
					assetToPing = "OUPS";
					Debug.LogException(e);
				}
				
			}else{
				//	GUI.FocusControl("SearchField");
			}
			
			/*
				if (rssFeed!=null)
				{
					foreach (RssItem item in rssFeed.Items)
					{
						GUILayout.Label(item.Title);
					}
				}
				*/
		}


		void test()
		{

			EditorCoroutine.start(httpsTest());

		}

		IEnumerator httpsTest()
		{
			return null;
			/*
			Hashtable headers = new Hashtable();
			headers["X-Parse-Application-Id"] = "17rKb1PpcAvQNTrDaKY5K2FGfJQBs4h1ArlITGte";
			headers["X-Parse-REST-API-Key"] = "gTJJrnGseABRpZMiLeMYTPGMGphoM6MFlpErMz8c";
			headers["Content-Type"] = "application/json";
			string ourPostData = "{\"objectId\":\"GtYc1JbRTp\",\"level\":6}";
			//string ourPostData = "{\"objectId\":\"dHBZCcprxl\",\"level\":\"10\"}";
			byte[] pData = System.Text.Encoding.ASCII.GetBytes(ourPostData.ToCharArray());
			
			WWW _test = new WWW("https://api.parse.com/1/functions/MyMethod",pData,headers);
			
			while (!_test.isDone) yield return null;
			Debug.Log("Result from www https"+_test.text); // expect {"result":"Hello world!"}
*/

			/*
			//Authenticate an existing user
			var auth_user = ParseClass.Authenticate("test","test");
			while(!auth_user.isDone) yield return null;
			//check for error
			if(auth_user.error != null) {
				Debug.Log("An error occured, likely a bad password!"+auth_user.error);
			}
*/

			//

			/*
			string username = "test";
			string password = "test";

			string level = "10";
			
			string ourPostData = WWW.EscapeURL("username=" + username) + "&" + WWW.EscapeURL ("password=" + password);
		//	string encodedData =  ourPostData;
			string encodedData = "{}";//"{\"objectId\":\"dHBZCcprxl\",\"level\":\"10\"}";

			byte[] pData = System.Text.Encoding.UTF8.GetBytes(encodedData);

			WWWForm _form =  new WWWForm();
			Hashtable headers = _form.headers;
			headers["X-Parse-Application-Id"] = "17rKb1PpcAvQNTrDaKY5K2FGfJQBs4h1ArlITGte";
			headers["X-Parse-REST-API-Key"] = "gTJJrnGseABRpZMiLeMYTPGMGphoM6MFlpErMz8c";
			headers["Content-Type"] = "application/json";

			_form.AddField("objectId","dHBZCcprxl");
			_form.AddField("level","10");

			WWW _test = new WWW("https://api.parse.com/1/functions/updateLevel",pData,headers);
			
			Debug.Log(_test.url);
			while (!_test.isDone) yield return null;
			Debug.Log("Result from www https"+_test.error+" "+_test.text);
*/

			/*
			var user = ParseClass.users.New();
			user.Set("username", "simon");
			user.Set("password", "xyzzy");
			user.Create();
			while(!user.isDone) yield return null;
			//check for error
			if(user.error != null) {
				//A message is printed automatically. We can diagnose the issue by examing the HTTP code.
				Debug.Log(user.code);
			}

		*/
			/*
			//Authenticate an existing user
			var auth_user = ParseClass.Authenticate("test","test");
			while(!auth_user.isDone) yield return null;
			//check for error
			if(auth_user.error != null) {
				Debug.Log("An error occured, likely a bad password!"+auth_user.error);
			}
*/


			/* WORKING
			Hashtable headers = new Hashtable();
			headers["X-Parse-Application-Id"] = "GvUb5RvrzlmyqFAz3fwGQp7yGDy92MOU52bf8qvv";
			headers["X-Parse-REST-API-Key"] = "icUwHhBXrZEmgNjRz8TGpvY50uodn7VhMrMuVFUJ";
			headers["Content-Type"] = "application/json";
			string ourPostData = "{}";
			byte[] pData = System.Text.Encoding.ASCII.GetBytes(ourPostData.ToCharArray());

			WWW _test = new WWW("https://api.parse.com/1/functions/hello",pData,headers);

			while (!_test.isDone) yield return null;
			Debug.Log("Result from www https"+_test.text); // expect {"result":"Hello world!"}
			*/

			/* WORKING
			Hashtable headers = new Hashtable();
			//headers["X-Parse-Application-Id"] = "GvUb5RvrzlmyqFAz3fwGQp7yGDy92MOU52bf8qvv";
			//headers["X-Parse-REST-API-Key"] = "icUwHhBXrZEmgNjRz8TGpvY50uodn7VhMrMuVFUJ";
			//headers["Content-Type"] = "application/json";

			string username = "test";
			string password = "test";
		

			string ourPostData = WWW.EscapeURL("username=" + username) + "&" + WWW.EscapeURL ("password=" + password);
			string encodedData =  ourPostData;
			byte[] pData = System.Text.Encoding.ASCII.GetBytes(encodedData);
			
			WWW _test = new WWW("https://GvUb5RvrzlmyqFAz3fwGQp7yGDy92MOU52bf8qvv:javascript-key=Cs4d4pfuO8HSFha54F4fB9u3TuBPVQ7WCrtgr3pH@api.parse.com/1/login?"+ourPostData);//,pData,headers);

			Debug.Log(_test.url);
			while (!_test.isDone) yield return null;
			Debug.Log("Result from www https"+_test.error+" "+_test.text);

*/

		}




		void OnGUI () { wantsMouseMove = true;

			
			// check for compiling status
			// refresh versioning as well.
			if (isCompiling!=EditorApplication.isCompiling)
			{
				isCompiling = EditorApplication.isCompiling;
				
				if (!isCompiling)
				{
					//Debug.Log("Compiling Ecosystem "+EditorApplication.isCompiling);
					CurrentVersion = MyUtils.Utils.UpdateVersion(pathToVersionInfoSource);
					CurrentVersionAsString = CurrentVersion.ToShortString();
				}
				
			}else if (string.IsNullOrEmpty(CurrentVersionAsString))
			{
				CurrentVersion = MyUtils.Utils.UpdateVersion(pathToVersionInfoSource);
				CurrentVersionAsString = CurrentVersion.ToShortString();
			}

			
			if (Event.current.type == EventType.MouseMove) Repaint ();

			// init skin
			OnGUI_SetUpSkin();

			// the toolbar is unskinned
			if ( !DiscreteTooBar_on || ShowToolBar)
			{
				OnGUI_ToolStrip();
			}

			/*
			if (GUILayout.Button("test"))
			{
				test ();
			}
			*/
			// switch to the custom editor skin
			// TODO: should design the scroll widgets so that it can be matching the skin.
			GUI.skin = editorSkin;


			OnGUI_Main();



		}


		void OnGUI_ToolStrip() {
			
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			
			/*
				if (GUILayout.Button("Refresh", EditorStyles.toolbarButton)) 
				{
					OnProjectChange();
				}
				*/
			GUILayout.Label(CurrentVersionAsString);
			GUILayout.FlexibleSpace();
			


			if (ECO_BrowserVersion>CurrentVersion)
			{
				if (GUILayout.Button("Update Available", EditorStyles.toolbarDropDown)) {
					GenericMenu toolsMenu = new GenericMenu();
					toolsMenu.AddItem(new GUIContent("Download and Install"),false, OnTools_DownloadAndInstallUpdate);
	
					toolsMenu.AddSeparator("");
					
					toolsMenu.AddItem(new GUIContent("Info"), false, OnTools_ShowUpdateInfo);
					
					// Offset menu from right of editor window
					toolsMenu.DropDown(new Rect(Screen.width-230, 0, 0, 16));
					EditorGUIUtility.ExitGUI();
				}

			}

			bool _newShowDisclaimer = GUILayout.Toggle(ShowDisclaimer,"Disclaimer",EditorStyles.toolbarButton);
			if (_newShowDisclaimer!=ShowDisclaimer)
			{
				ShowDisclaimer = !ShowDisclaimer;
				Repaint();
				
			}

			if (GUILayout.Button("Settings", EditorStyles.toolbarDropDown)) {
				GenericMenu toolsMenu = new GenericMenu();
				toolsMenu.AddItem(new GUIContent("Discrete ToolBar"),DiscreteTooBar_on, OnTools_ToggleDiscreteTooBar);
				
				toolsMenu.AddSeparator("");

				toolsMenu.AddItem(new GUIContent("Debug In Console"),Debug_on, OnTools_ToggleDebug);
				
				toolsMenu.AddSeparator("");

				toolsMenu.AddItem(new GUIContent("Help..."), false, OnTools_Help);
				
				// Offset menu from right of editor window
				toolsMenu.DropDown(new Rect(Screen.width-150, 0, 0, 16));
				EditorGUIUtility.ExitGUI();
			}
			
			
			
			GUILayout.EndHorizontal();
		}
	

		void OnGUI_ToolBar()
		{
			Event e = Event.current;
			if (e.isKey)
			{
				if (e.keyCode== KeyCode.KeypadEnter || e.keyCode== KeyCode.Return)
				{
					SearchRep();
				}
			}

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();

			GUILayout.Space(6);

			OnGUI_FilterButton();

				GUILayout.BeginHorizontal("Search Field Background");
					
					GUI.SetNextControlName ("SearchField");
					searchString = GUILayout.TextField(searchString,"Search TextField");

					if (string.IsNullOrEmpty(searchString))
					{
						Rect _last = GUILayoutUtility.GetLastRect();
						_last.x += 2;
						_last.y += 9;
						GUI.Label(_last,GUIContent.none,"Search Empty Tip");
					}
				
					if (!string.IsNullOrEmpty(searchString))
					{
						

						GUILayout.BeginVertical(GUILayout.Width(21));
							GUILayout.FlexibleSpace();
							
							if ( GUILayout.Button(new GUIContent(GUI.skin.FindStyle("Cross Icon").normal.background),"Button Round Small")  )
							{
								searchString = "";
							}
							GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
						
					}	

				GUILayout.EndHorizontal();


			if (wwwSearch!=null)
			{

				GUILayout.Label("Searching...","Label Row Plain",GUILayout.Height(15));

				GUILayout.BeginVertical(GUILayout.Width(21),GUILayout.Height(21));
				GUILayout.FlexibleSpace();
				
				if ( GUILayout.Button(new GUIContent(GUI.skin.FindStyle("Cross Icon").normal.background),"Button Round Small")  )
				{
					wwwSearch.Dispose();
					wwwSearch = null;
					//System.GC.Collect();
					//EditorGUIUtility.ExitGUI();
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();


			}else{

				string style = "Button Medium";

				if (
					filterTouched 
					||  
				    ! string.Equals(lastSearchString,searchString, StringComparison.OrdinalIgnoreCase)
				    )
				{
					style += " Red";
				}

				string _searchButtonLabel = "Search";

				if (string.IsNullOrEmpty(searchString))
				{
					_searchButtonLabel = "Browse";
				}

				if (GUILayout.Button(_searchButtonLabel,style,GUILayout.Width(71)))
				{
					SearchRep();
				}
			};
			GUILayout.Space(4);
			GUILayout.EndHorizontal();


			/*
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);


			OnGUI_FilterMiniButton();


			searchBox.OnGUI();

			GUILayout.Space(5);

			try{
				if (wwwSearch==null)
				{
					if (FsmEditorGUILayout.MiniButton(new GUIContent("Search"),GUILayout.Width(70) )  )                       
					{
						SearchRep();

					}

				}else{
					GUILayout.Label("Searching",GUILayout.Width(66));
				}
			}catch{
				//GUILayout.Label("Project assets being processed, please wait");
				return;
			}
			GUILayout.EndHorizontal();
	*/


			/*
			if (FsmEditorGUILayout.ToolbarSettingsButton())
			{
				//GenerateSettingsMenu().ShowAsContext();
			}
			*/
			//GUILayout.Space(-5);
			
		//	GUILayout.EndHorizontal();

		}

		void OnGUI_FilterButton()
		{

			GUIContent _label = new GUIContent("Filter");

			string ButtonSkin = "Button Medium";

			if (searchFilters!=null && searchFilters.Count>0 && searchFilters.Count!= PlayMakerEcosystemFiltersLength )
			{
				_label.text = "Filter on";
				ButtonSkin += " Green";
			}

			if (GUILayout.Button(_label,ButtonSkin,GUILayout.Width(71)))                      
			{
				ShowFilterUI = !ShowFilterUI;
			}
		}
	
		void OnGUI_FilterPanel()
		{
			if (!ShowFilterUI)
			{
				return;
			}

			if (searchFilters==null)
			{
				searchFilters = new List<PlayMakerEcosystemFilters>();
			}
			if (repositoryMask==null)
			{
				repositoryMask = new List<string>();
			}

			GUILayout.Space(5);
			GUILayout.BeginVertical("Table Row Plain Last");

			// build the feedback
			string FilterFeedback = "Content : ";
			if (searchFilters==null || searchFilters.Count==0 || searchFilters.Count==PlayMakerEcosystemFiltersLength)
			{
				FilterFeedback +=  "Everything is searched";
			}else{
				//FilterFeedback += "Only ";
				int _filterCount = searchFilters.Count;
				int i = 1;
				foreach(PlayMakerEcosystemFilters _filter in searchFilters)
				{
					FilterFeedback += _filter;

					if(i<_filterCount)
					{
						FilterFeedback += " or ";
					}

					i++;

				}
			}

			GUILayout.Label(FilterFeedback,"Label Row Plain");
				GUILayout.BeginHorizontal();
					OnGUI_FilterButton(PlayMakerEcosystemFilters.Actions,"Actions");
					OnGUI_FilterButton(PlayMakerEcosystemFilters.Packages,"Packages");
					OnGUI_FilterButton(PlayMakerEcosystemFilters.Templates,"Templates");
					OnGUI_FilterButton(PlayMakerEcosystemFilters.Samples,"Samples");
				GUILayout.EndHorizontal();

			/*
			GUILayout.Label("Repositories","Label Row Plain");
			GUILayout.BeginHorizontal();
			
			OnGUI_MaskButton(PlayMakerEcosystemRepositoryMasks.Unity3x,"U3","Unity 3.x");
			OnGUI_MaskButton(PlayMakerEcosystemRepositoryMasks.Unity4x,"U4","Unity 4.x");
			OnGUI_MaskButton(PlayMakerEcosystemRepositoryMasks.PlayMakerBeta,"PB","PlayMaker Beta");
			GUILayout.EndHorizontal();
	*/
			GUILayout.EndVertical();


		}

		void OnGUI_FilterButton(PlayMakerEcosystemFilters filter,string label)
		{
			bool isOn = searchFilters.Contains(filter);

			string ButtonFilterSkin = "Button Toggle ";

			if (isOn)
			{
				ButtonFilterSkin += "On";
			}else{
				ButtonFilterSkin += "Off";
			}
			
			if (GUILayout.Button(label,ButtonFilterSkin,null))
			{
				isOn =! isOn;
				if (isOn)
				{
					searchFilters.Add(filter);
				}else{
					searchFilters.Remove(filter);
				}

				filterTouched = true;
			}
		}

		void OnGUI_MaskButton(PlayMakerEcosystemRepositoryMasks mask,string repository,string label)
		{

			if (mask == PlayMakerEcosystemRepositoryMasks.Unity4x)
			{
				if (Application.unityVersion.StartsWith("4."))
				{
					GUI.contentColor = new Color(1f,1f,1f,0.5f);
					GUILayout.Label(label,"Button Toggle Off");
					GUI.contentColor = Color.white;
					return;
				}
			}

			bool isOn = repositoryMask.Contains(repository);

			string ButtonFilterSkin = "Button Toggle ";
			
			if (isOn)
			{
				ButtonFilterSkin += "On";
			}else{
				ButtonFilterSkin += "Off";
			}
			
			if (GUILayout.Button(label,ButtonFilterSkin,null))
			{
				isOn =! isOn;
				if (isOn)
				{
					repositoryMask.Add(repository);
				}else{
					repositoryMask.Remove(repository);
				}
				
				filterTouched = true;
			}
		}
	

		void OnGUI_Warning(string content)
		{
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal("Table Row Orange First",GUILayout.Width(position.width+3));
			
			GUILayout.Label(content,"Label Row Orange");
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		GUIContent _docImageContent = new GUIContent();
		bool loadingImage = false;
		int currentDocImageIndex;

		bool ShowActionDetails =false;
		Vector2 ActionDetailsScroll;

		float DocumentationImageHeight;
		void OnGUI_BottomPanel()
		{

			// action section

			if (ShowActionDetails && SelectedIndex>=0 && resultItems!=null && SelectedIndex<resultItems.Length)
			{

				GUILayout.Space(5);
				GUILayout.BeginVertical("Table Row Plain Last");

				Item item = resultItems[SelectedIndex];

				// top bar
				GUILayout.BeginHorizontal();
					GUILayout.Label(item.PrettyName);//+" Loading "+loadingImage,"label Row Plain");

					GUILayout.FlexibleSpace();

					if (GUILayout.Button(new GUIContent(GUI.skin.FindStyle("Cross Icon").normal.background),"Button Round Small"))
					{
						ShowActionDetails = false;
						Repaint();
					}

				GUILayout.Space(2); // to align with the scrollbar of the screenshot.
					
				GUILayout.EndHorizontal();

				if (item.HasVideo)
				{
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(_youtubeWatchVideoButtonGuiContent,"Button",GUILayout.Width(300)))
					{
						string _url = item.GetUrl(Item.urltypes.YouTube,0); // get the first video 

						if (Debug_on) Debug.Log(_url);
						Application.OpenURL(_url); 
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}
				// content
				if (item.DocumentationImageStatus == Item.AsynchContentStatus.Available)
				{
					if (loadingImage || currentDocImageIndex != SelectedIndex)
					{
						_docImageContent.image = item.DocumentationImage;
						if (item.DocumentationImage == null)
						{
							loadingImage = true;
							_docImageContent = new GUIContent();
							item.LoadDocumentation();
						}else{
							DocumentationImageHeight = item.DocumentationImage.height;
							currentDocImageIndex = SelectedIndex;
							loadingImage = false;
							ActionDetailsScroll = Vector2.zero;
						}
					}

					if (DocumentationImageHeight<250)
					{
						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(_docImageContent);
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
					}else{
						ActionDetailsScroll = GUILayout.BeginScrollView(ActionDetailsScroll,GUILayout.Height(250));
						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(_docImageContent);
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
						GUILayout.EndScrollView(); 
					}

					GUILayout.Space(5); // so the image doesn't get crop with the background border


				}else if (item.DocumentationImageStatus == Item.AsynchContentStatus.Downloading)
				{
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.Label("Downloading Documentation...");
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}else if (item.DocumentationImageStatus == Item.AsynchContentStatus.Unavailable)
				{
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.Label("Documentation Unavailable");
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}else if (item.DocumentationImageStatus == Item.AsynchContentStatus.Pending)
				{
					loadingImage = true;
					_docImageContent = new GUIContent();
					item.LoadDocumentation();
				}


				 
				GUILayout.EndVertical();
				GUILayout.Space(5);
			}






			if ( EditorApplication.isCompiling)
			{
				OnGUI_Warning("UNITY IS COMPILING");
			}
			if ( EditorApplication.isUpdating)
			{
				OnGUI_Warning("UNITY IS UPDATING");
			}
	
		//	FsmEditorGUILayout.Divider();

			/*
			if (selectedAction != null)
			{
				// Action name and help button
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label("selected action description");
				
				GUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
			//	FsmEditorGUILayout.Divider();

			}

			
			
			// Bottom toolbar
			
			GUILayout.BeginHorizontal();
			
			if (FsmEditor.SelectedState == null )  //|| selectedAction == null)
			{
				GUI.enabled = false;
			}
			
			if (GUILayout.Button(new GUIContent("Add Action To State")))
			{
				AddSelectedActionToState();
			}
			
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Space();

			*/
		}

		void AddSelectedActionToState()
		{
			/*
			if (FsmEditor.SelectedState == null)
			{
				return;
			}
			*/
			#if PREVIEW_VERSION
			Dialogs.PreviewVersion();
			#else
		//	FsmEditor.StateInspector.AddAction(selectedAction);
			//FinishAddAction();
			#endif
		}
	
		/// <summary>
		/// Draw the GUI List of Items found from the search
		/// </summary>
		void OnGUI_ItemList()
		{
			if (resultItems==null || resultItems.Length==0)
			{
				GUILayout.Label("No result");
				return;
			}

			if (Event.current.type == EventType.Repaint)
				rowsArea = new Rect[resultItems.Length];


			if(downloads!=null)
			{
			//	GUILayout.Label("Downloads: "+downloads.Count);
			}

			GUILayout.Space(5);
			Vector2 _scrollNew = GUILayout.BeginScrollView(_scroll);

			if (_scrollNew!=_scroll)
			{
				_scroll = _scrollNew;
				lastMousePosition = Vector2.zero;
				Repaint();
			}



			int i=0;

			foreach(Item item in resultItems)
			{
				OnGUI_ActionItem(item,i);
				i++;
			}
			GUILayout.EndScrollView();
			ActionListRect = GUILayoutUtility.GetLastRect();
			GUILayout.Space(5);
		}

		void OnGUI_ViewItem()
		{
			GUILayout.Label("Hello View");
		}

		string mouseOverAction ="";
	
		void OnGUI_ActionItem(Item item,int rowIndex)
		{
			// get the row style
			string rowStyle ="Middle";
			if (resultItems.Length==1)
			{
				rowStyle = "Alone";
			}else if (rowIndex==0) 
			{
				rowStyle = "First";
			}else if (rowIndex == (resultItems.Length-1) )
			{
				rowStyle = "Last";
			}

			// find details about the item itself
			string url = (string)item.RawData["RepositoryRawUrl"];
			bool downloading = !string.IsNullOrEmpty(url) && downloadsLUT.ContainsKey(url) ;

			string itemPath = (string)item.RawData["path"];
			string category = (string)item.RawData["category"];
			string unity_version = (string)item.RawData["unity_version"];


			bool forceloading = false;
			if (!item.RawData.ContainsKey("projectPath")) // Cache the project path to avoid process the same thing over and over again.
			{
				forceloading = true;
				item.RawData["projectPath"] = MyUtils.Utils.GetAssetAbsolutePath(itemPath);
			//	Debug.Log(item.RawData["projectPath"]);
			}

			bool fileExists = File.Exists((string)item.RawData["projectPath"]);


			Hashtable _metaData = LoadItemMetaData(item.RawData,forceloading);
			
			if (_metaData.ContainsKey("pingAssetProjectPath"))
			{
				fileExists = File.Exists((string)_metaData["pingAssetProjectPath"]);
			}
		

			// define the row style based on the item properties.
			string rowStyleType = "Plain";
		
			if (fileExists)
			{
				rowStyleType = "Green";
			}

			if (downloading)
			{
				rowStyleType = "Orange";
			}


			string _name = (string)item.RawData["pretty name"];
			string _type = ((string)item.RawData["type"]);

			GUILayout.BeginVertical(GUIContent.none,"Table Row "+rowStyleType+" "+rowStyle);
			GUILayout.BeginHorizontal();

			string itemLabelSkin = "Label Round Small";

			switch(_type)
			{
				case "Action":
					itemLabelSkin = "Label Round Green Small";	break;
				case "Sample":
					itemLabelSkin = "Label Round Violet Small";	break;
				case "Template":
					itemLabelSkin = "Label Round Cyan Small";	break;
				case "Package":
					itemLabelSkin = "Label Round Blue Small";	break;
			}
		
			GUILayout.Label(_type,itemLabelSkin,GUILayout.Width(61));
			GUI.backgroundColor = Color.white;

			GUILayout.Label(_name,"Label Row "+rowStyleType,GUILayout.MinWidth(0));

			GUILayout.FlexibleSpace();




			if (mouseOverAction == _name)
			{
				var eventType = Event.current.type;
				
				if (eventType == EventType.MouseDown)
				{	

				//	string guid = AssetDatabase..AssetPathToGUID((string)item["path"]);
				//	Debug.Log(itemPath);
				//	EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(itemPath));

					SelectAction(_name);

					if (Event.current.clickCount > 1)
					{
						AddSelectedActionToState();
					}
					
					GUIUtility.ExitGUI();
					return;
				}
			}

			if (downloading)
			{
				GUILayout.Label("downloading...","Label Row "+rowStyleType,GUILayout.Width(80));
			}else if(mouseOverRowIndex==rowIndex )
			{
				if (!ShowActionDetails)
				{
					if (GUILayout.Button("?","Button Small",GUILayout.Width(20)))
					{
						SelectedIndex = rowIndex;
						ShowActionDetails = true;
						Repaint();
					}
					GUILayout.Space(5);
				}

				if (GUILayout.Button("Github","Button Small",GUILayout.Width(60)))
				{
					ViewOnGithub(item);
					return;
				}

				if (fileExists)
				{

					if (GUILayout.Button("Delete","Button Small Red",GUILayout.Width(50)))
					{
						DeleteItem(item);
						Repaint ();
						GUIUtility.ExitGUI();
						return;
					}


					GUILayout.Label("imported","Label Row "+rowStyleType,GUILayout.Width(50));

				}else{
					if (GUILayout.Button("Get","Button Small",GUILayout.Width(50)))
					{
						ImportItem(item);
						Repaint ();
						GUIUtility.ExitGUI();
						return;
					}
				}



			}
			GUILayout.EndHorizontal();

			// tags

			GUILayout.BeginHorizontal();



			GUILayout.Label("Unity "+unity_version,"Tag Small "+rowStyleType);
			GUILayout.Label(category,"Tag Small "+rowStyleType);

			//GUILayout.Label(url,"Tag Small "+rowStyleType);

			if ((string)item.RawData["beta"]=="true")
			{
				GUI.contentColor = Color.yellow;
				GUILayout.Label("Beta","Tag Small "+rowStyleType);
				GUI.contentColor = Color.white;
			}
			GUILayout.FlexibleSpace();

	
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();



			if(rowsArea!=null && rowIndex<rowsArea.Length)
			{
				rowsArea[rowIndex] = GUILayoutUtility.GetLastRect();
			}


		}

		
		void SelectAction(string actionName)
		{
			if (actionName == selectedAction)
			{
				return;
			}

			selectedAction = actionName;
			
			Repaint();
		}
			
		void SearchRep()
		{
			ShowActionDetails = false;
			SelectedIndex = -1;


			if (! string.IsNullOrEmpty(_lastError))
			{
				if (wwwSearch!=null)
				{
					wwwSearch.Dispose();
					wwwSearch = null;
				}
				// = null;
				_lastError = null;
				Repaint ();
			}

		//	bool _classFound = JF_EditorUtils.isClassDefined(searchString);
		//	Debug.Log(searchString+" found? = "+_classFound);
			/*
			if (rssFeed!=null)
			{
				foreach (RssItem item in rssFeed.Items)
				{
					Debug.Log(item.Title);
				}
			}
			*/

			string url = __REST_URL_BASE__+"search/"+WWW.EscapeURL(searchString);



			// CONTENT MASKING
			string ContentTypeMask = ""; // all by default
			
			// if all filters are selected, it's the same as searching for everything so we don't mask for efficiency
			if (searchFilters!=null && searchFilters.Count != Enum.GetNames(typeof(PlayMakerEcosystemFilters)).Length)
			{
				foreach(PlayMakerEcosystemFilters _filter in searchFilters)
				{
					switch(_filter)
					{
					case PlayMakerEcosystemFilters.Actions:
						ContentTypeMask += "-A";
						break;
					case PlayMakerEcosystemFilters.Templates:
						ContentTypeMask += "-T";
						break;
					case PlayMakerEcosystemFilters.Samples:
						ContentTypeMask += "-S";
						break;
					case PlayMakerEcosystemFilters.Packages:
						ContentTypeMask += "-P";
						break;
					}
				}
			}
			
			url += "?content_type_mask="+ContentTypeMask;

			// REPOSITORY MASKING
			string mask = "U3";
			if (Application.unityVersion.StartsWith("4."))
			{
				mask += "U4";
			}
			if (Application.unityVersion.StartsWith("5."))
			{
				mask += "U4";
				mask += "U5";
			}
			
			if (
				MyUtils.Utils.GetPlayMakerVersion().Contains("b")
			    )
			{
				mask += "PB";

			}

			url += "&repository_mask="+mask;


			// put the all the versions as well
			url += "&EcosystemVersion="+CurrentVersion;
			url += "&UnityVersion="+Application.unityVersion;
			url += "&PlayMakerVersion="+MyUtils.Utils.GetPlayMakerVersion();

			if (Debug_on) Debug.Log(url);

			wwwSearch = new WWW(url);//,_form);
			lastSearchString = searchString;

			filterTouched = false;
		}

		/// <summary>
		/// Called by the system. It's a way to check as well that Unity recompiled
		/// We check if the raw search result is not empty and we rebuild the results in that case, 
		/// This is for consistency during unity recompilation, and to avoid having to serialize items.
		/// </summary>
		protected virtual void OnEnable()
		{
			//Debug.Log("################ OnEnable");

			// get editor prefs
			GetEditorPrefs();

			if (!string.IsNullOrEmpty(rawSearchResult))
			{
				ParseSearchResult(rawSearchResult);
			}
		}
		protected virtual void OnDisable()
		{
			//Debug.Log("################ OnDisable");
		}

		void OnInspectorUpdate() {

			if (wwwSearch!=null)
			{
				if (wwwSearch.isDone)
				{

					if (!String.IsNullOrEmpty(wwwSearch.error))
					{
						_lastError = "Search Error : "+wwwSearch.error;
						
						Debug.LogWarning(_lastError);
					}else{
						try{
							rawSearchResult = wwwSearch.text;
							
						}catch(Exception e)
						{
							_lastError = "Search result Error : "+e.Message;
							
							Debug.LogWarning(_lastError);
						}
					}

					wwwSearch.Dispose();
					wwwSearch = null;

					ParseSearchResult(rawSearchResult);
				}
			}

			if (downloads!=null && downloads.Count>0)
			{
				// only process one download at a time?

			//	for(int i =(downloads.Count-1);i>=0;i--)
			//	{
					//Debug.Log("Checking download "+i);
					int i =0;
					WWW _www = downloads[i];
					if(_www.isDone){
						string _www_url = _www.url;
						string _www_text = _www.text;
						byte[] _www_bytes = _www.bytes;
						_www.Dispose();
						_www = null;
						downloads.RemoveAt(i);
						//Repaint();

				EditorCoroutine.start(
						ProceedWithImport(_www_url,_www_text,_www_bytes)
						);

					}
			//	}
			}



			Repaint();
		}	
	
		/// <summary>
		/// Parses the search result from the server. The search is in json completly.
		/// </summary>
		/// <param name="jsonString">Json string.</param>
		void ParseSearchResult(string jsonString)
		{
			if (Debug_on) Debug.Log("ParseSearchResult");

			try {
				searchResultHash =  (Hashtable)JSON.JsonDecode(jsonString);

				if (searchResultHash == null)
				{
					_lastError = "json content is null. Please search again, it's likely a connection issue :"+wwwSearch.url+" ->"+jsonString;

					if (wwwSearch!=null)
					{
						wwwSearch.Dispose();
						wwwSearch = null;
					}

					Debug.LogWarning(_lastError);

					return;
				}

			} catch (System.Exception e) {

				if (wwwSearch!=null)
				{
					wwwSearch.Dispose();
					wwwSearch=null;
				}

				_lastError = "Json parsing error "+e.Message;

				Debug.LogWarning(_lastError);
				return;
			}finally
			{
				if (wwwSearch!=null)
				{
					wwwSearch.Dispose();
					wwwSearch=null;
				}
			}

			if (searchResultHash.ContainsKey("message"))
			{
				string _message = (string)searchResultHash["message"];
				if (_message.Contains("API rate limit exceeded"))
				{
					_lastError = "API rate limit (20 requests per minute) exceeded. Please wait one minute to search again";
				}else{
					_lastError = (string)searchResultHash["message"];
				}

				return;
			}


			// check the actual current version of the ecosystem browser, the user may need to update
			if (searchResultHash.ContainsKey("metadata"))
			{
				Hashtable _metaData = (Hashtable)searchResultHash["metadata"];

				if (_metaData.ContainsKey("ECO_BrowserVersion"))
				{
					ECO_BrowserVersion = new VersionInfo((string)_metaData["ECO_BrowserVersion"]);
					//ECO_BrowserVersion_package = (string)_metaData["ECO_BrowserVersion_package"];
					// only force the update banner if the live version is greater then the time when the user choose to dismiss it.
					if (ECO_BrowserVersion>LastUpdateBannerVersion)
					{
						UpdateBanner_on = true;
					}
				}
			}


			int searchResultCount = (int)searchResultHash["total_count"];
			if (Debug_on)Debug.Log("Server search result count"+searchResultCount);

			// reset LUT
			itemsLUT = null;

			ArrayList _arrayList = (ArrayList)searchResultHash["items"];

			resultItems = new Item[_arrayList.Count];
			int i=0;
			foreach(var _obj in _arrayList)
			{
				resultItems[i] = new Item((Hashtable)_obj);
				i++;
			}

		
		}

		#region ITEM

		void DeleteItem(Item item)
		{
		//	Debug.Log((string)item["path"]);

			// first delete the ping asset

			Hashtable _metaData = LoadItemMetaData(item.RawData,true);
			if (_metaData.ContainsKey("pingAssetProjectPath"))
			{
				DeleteAsset( (string)_metaData["pingAssetProjectPath"]);
			}


			DeleteAsset((string)item.RawData["projectPath"]);




		}


		void DeleteAsset(string assetPath)
		{
			if (Debug_on) Debug.Log("Deleting -> "+assetPath);

			string guid = AssetDatabase.AssetPathToGUID(assetPath);
			
			if (string.IsNullOrEmpty(guid))
			{
				if (Debug_on) Debug.Log("we have to delete it manually");
				File.Delete(assetPath);
				AssetDatabase.Refresh();
				
			}else{
				if (Debug_on) Debug.Log("we found a guid -> "+guid);
				AssetDatabase.DeleteAsset(assetPath);
				AssetDatabase.Refresh();
			}
		}



		Hashtable LoadItemMetaData(Hashtable item,bool forceLoading)
		{
			if (! item.ContainsKey("metaData") || forceLoading)
			{
				Hashtable _meta = new Hashtable();

				if(forceLoading)
				{
					string _asset = (string)item["path"];

					if (_asset.EndsWith(".template.txt") || _asset.EndsWith(".sample.txt"))
					{
						try{
							string content = File.ReadAllText( (string)item["projectPath"]);
							_meta = (Hashtable)JSON.JsonDecode(content);
							
							if (_meta==null)
							{
								if (Debug_on) Debug.LogWarning("Could not get the json of this meta file ");
								_meta = new Hashtable();
							}else{

								if (Debug_on) Debug.Log("Meta data for "+(string)item["projectPath"]);
							
							foreach(DictionaryEntry entry in _meta)
							{
								if (Debug_on) Debug.Log(entry.Key + ":" + entry.Value);
							}
							
							}
						}catch(Exception e)
						{
							if (Debug_on) Debug.Log(e.Message);
						}

						
					}	
			

					if (_meta.ContainsKey("pingAssetPath") && !_meta.ContainsKey("pingAssetProjectPath"))
					{
						_meta["pingAssetProjectPath"] =  MyUtils.Utils.GetAssetAbsolutePath((string)_meta["pingAssetPath"]);
					}

				}
				item["metaData"] = _meta;
			}


			return (Hashtable)item["metaData"];
		}


		/// <summary>
		/// Open the browser to view an item source on github
		/// </summary>
		/// <param name="item">Item.</param>
		void ViewOnGithub(Item item)
		{

			string _url = item.GetUrl(Item.urltypes.GithubPreview);
			
			if (Debug_on) Debug.Log(_url);

			Application.OpenURL(_url); 

		}

		void ImportItem(Item item)
		{
			// KEEP THIS: this is a big where the guid persists even if you deleted the file.
			//string guid = AssetDatabase.AssetPathToGUID(itemPath);
			//Debug.Log(itemPath+" -> "+guid);
			//if (! string.IsNullOrEmpty(guid))
			//{
			//	Debug.Log(itemPath+" already exists");
			//}else{

		

			string url = item.GetUrl(Item.urltypes.RestDownload);
			string assetPath =  MyUtils.Utils.GetAssetAbsolutePath(item.Path);

			if (Debug_on) Debug.Log("ImportItem "+url+" to "+assetPath);

			DownloadRawContent(assetPath,url,item);
		}

		#endregion

		void DownloadRawContent(string assetPath,string url,Item item)
		{
		//	Debug.Log("DownloadRawContent "+assetPath+" "+url);
			if (downloadsLUT==null)
			{
				downloadsLUT = new Dictionary<string, string>();

			}

			if (downloads==null)
			{
				downloads = new List<WWW>();
			}

			if (itemsLUT==null)
			{
				itemsLUT = new Dictionary<string, Item>();	
			}


			if (!downloadsLUT.ContainsKey(url))
			{
				downloads.Add(new WWW(url));
				downloadsLUT.Add(url,assetPath);
				itemsLUT.Remove(url);
				itemsLUT.Add(url,item);
			}

		}
	
		IEnumerator ProceedWithImport(string url,string rawContent,byte[] rawBytes)
		{
			if (Debug_on) Debug.Log("ProceedWithImport for "+url+" "+rawContent);

			string assetPath = downloadsLUT[url];
			downloadsLUT.Remove(url);

			Item item =  itemsLUT[url];

			Hashtable rep = (Hashtable)item.RawData["repository"];
			string repositoryPath = (string)rep["full_name"];


			// is it a template?
			if (rawContent.Contains("__TEMPLATE__")  || rawContent.Contains("__SAMPLE__") || rawContent.Contains("__PACKAGE__"))
			{
				if (Debug_on) Debug.Log("This is actually packaged content");

				Hashtable _meta = (Hashtable)JSON.JsonDecode(rawContent);

				if (_meta==null)
				{
					Debug.LogWarning("Could not get the json of this meta file ");
					yield break;
				}

				if (Debug_on) Debug.Log("We have meta data");

				item.RawData["metaData"] = _meta;

				string _packagePath =  Uri.EscapeDataString((string)_meta["unitypackage"]);
				string _repositoryPath = Uri.EscapeDataString(repositoryPath);

				string _packageUrl = __REST_URL_BASE__ +"download?repository="+_repositoryPath+"&file="+_packagePath;

				DownloadRawContent(_packagePath,_packageUrl,item);
			}


		
			// check for Meta data
			// .*?
			Match match = Regex.Match(rawContent,@"(?<=EcoMetaStart)[^>]*(?=EcoMetaEnd)",RegexOptions.IgnoreCase);
			
			// Here we check the Match instance.
			if (match.Success)
			{
			//	Debug.Log("we have meta data :" + match.Value);

				Hashtable _meta = (Hashtable)JSON.JsonDecode(match.Value);
				ArrayList _dependancies = (ArrayList)_meta["script dependancies"];
				if (_dependancies!=null)
				{
					foreach(object dScript in _dependancies)
					{
						string _dscript = (string)dScript;
						//Debug.Log(_dscript);

					//	string itemPathEscaped = _dscript.Replace(" ","%20");
						
						//string dscripturl = "https://raw.github.com/"+RepositoryPath+"/master/"+itemPathEscaped;
						string dscripturl = __REST_URL_BASE__ +"download?repository="+ Uri.EscapeDataString(repositoryPath)+"&file="+ Uri.EscapeDataString(_dscript);

						//Debug.Log(dscripturl);
						DownloadRawContent(_dscript,dscripturl,item);

					}
				}
			}

			if (url.Contains(".unitypackage"))
			{
				if (Debug_on) Debug.Log("we have a unitypackage"+assetPath);

				string unityPackageTempFile = Application.dataPath.Substring(0,Application.dataPath.Length-6) +"Temp/PlayMakerEcosystem.downloaded.unityPackage";

				FileInfo _tempfileInfo = new FileInfo(Application.dataPath);
				if (!Directory.Exists(_tempfileInfo.DirectoryName))
				{
					Directory.CreateDirectory(_tempfileInfo.DirectoryName);
				}
				
				//	if (string.IsNullOrEmpty)
				File.WriteAllBytes(unityPackageTempFile,rawBytes);

				AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

				AssetDatabase.ImportPackage(unityPackageTempFile,true);

				yield return new WaitForSeconds(2f);
				if (Debug_on) Debug.Log("after import call wait done");


				yield break;
			}

			if (string.Equals("No Content",rawContent,StringComparison.InvariantCultureIgnoreCase))
			{

				Debug.LogError("The Ecosystem download failed for "+url+". Please try again to redownload");
				yield break;
			}

			FileInfo _fileInfo = new FileInfo(assetPath);
			if (!Directory.Exists(_fileInfo.DirectoryName))
			{
				Directory.CreateDirectory(_fileInfo.DirectoryName);
			}

			if (Debug_on) Debug.Log("Writing to file"+assetPath);
			File.WriteAllBytes(assetPath,rawBytes);

			AssetDatabase.Refresh();

			yield break;
		}
	
		IEnumerator ImportPackage()
		{
			yield break;
		}

		#region Browser Update 
		//AnimFloat BrowserToViewTransition_animfloat;
		//float BrowserToViewTransition_float;

		void ShowBrowserUpdateInfo()
		{
			OnTools_ShowUpdateInfo();
		}

		/// <summary>
		/// Stop showing Update banner until a newer version is available, and switch to a menu based access to the update to be used at the discretion of the end user
		/// </summary>
		void PostPonBrowserUpdate()
		{
			Debug.Log(" "+ECO_BrowserVersion.ToShortString());

			EditorPrefs.SetString(__namespace__+"."+LastUpdateBannerVersion_prefKey,ECO_BrowserVersion.ToShortString());
			LastUpdateBannerVersion = ECO_BrowserVersion.Clone();

			UpdateBanner_on = false;
			EditorPrefs.SetBool(__namespace__+"."+UpdateBanner_on_prefKey,UpdateBanner_on);
		}

		void GetBrowserUpdate()
		{
			//	BrowserToViewTransition_animfloat =  new AnimFloat(BrowserToViewTransition_float);
			// transit to the download wizard for the update item

			OnTools_DownloadAndInstallUpdate();
		}
		#endregion Browser Update

		/*
		string BuildSearchUrl(string searchQuery)
		{
			//"https://api.github.com/search/code?q=SimpleExample+in:language:cs+repo:pbhogan/InControl";
			string url = searchUrlBase;

			Debug.Log(searchBox.SearchMode);

			string _filter = "";// "__ECO__";

			if (searchBox.SearchMode==1)
			{
				_filter += " __ACTION__";
			}else if (searchBox.SearchMode==2)
			{
				_filter += " __TEMPLATE__ ";
			}if (searchBox.SearchMode==3)
			{
				_filter += " __SAMPLE__ ";
			}



			url += "code?q="+WWW.EscapeURL(_filter+" "+searchQuery);
			//url += "+extension:txt";
			url += "+repo:"+RepositoryPath;

			Debug.Log("search url = "+url);
			return url;

		}
	*/

		/// <summary>
		/// The pref key for the debug flag.
		/// </summary>
		static readonly string Debug_on_prefKey = "Debug";

		/// <summary>
		/// The pref key for the Discrete Toolbar option.
		/// </summary>
		static readonly string DiscreteTooBar_on_prefKey = "DiscreteToolBar";

		/// <summary>
		/// The last update banner version pref key. We can compare this against the latest live version and prompt again if higher
		/// used for the update banner: If it is less then the live version, we show the banner
		/// </summary>
		static readonly string LastUpdateBannerVersion_prefKey = "LastUpdateBannerVersion";
		
		/// <summary>
		/// The last update banner version. If this version is lower then the one found in the result, we show the banner again
		/// </summary>
		static VersionInfo LastUpdateBannerVersion;
		
		/// <summary>
		/// remember the choice of the user not to show the update banner. 
		/// This will be kept until a newer version will be found then the one at the time of the decision
		/// </summary>
		static readonly string UpdateBanner_on_prefKey = "ShowUpdateBannerVersion";
		
		/// <summary>
		/// Cached flag to avoid compare versions when drawing the UI, we only check when parsing results
		/// </summary>
		static bool UpdateBanner_on;


		void GetEditorPrefs()
		{
			DiscreteTooBar_on =  EditorPrefs.GetBool(__namespace__+"."+DiscreteTooBar_on_prefKey,false);
			Debug_on =  EditorPrefs.GetBool(__namespace__+"."+Debug_on_prefKey,false);
			UpdateBanner_on = EditorPrefs.GetBool(__namespace__+"."+UpdateBanner_on_prefKey,UpdateBanner_on);
			// check if we need to show the update banner
			string _lastUpdateBannerVersion_pref = EditorPrefs.GetString(__namespace__+LastUpdateBannerVersion_prefKey);
			//Debug.Log("_lastUpdateBannerVersion_pref : "+_lastUpdateBannerVersion_pref);
			LastUpdateBannerVersion = new VersionInfo(_lastUpdateBannerVersion_pref);

		}


		void OnTools_ToggleDiscreteTooBar()
		{
			DiscreteTooBar_on = !DiscreteTooBar_on;
			EditorPrefs.SetBool(__namespace__+"."+DiscreteTooBar_on_prefKey,DiscreteTooBar_on);
		}


		void OnTools_ToggleDebug()
		{
			Debug_on = !Debug_on;
			EditorPrefs.SetBool(__namespace__+"."+Debug_on_prefKey,Debug_on);
		}

		void OnTools_Help() 
		{
			Help.BrowseURL(__REST_URL_BASE__+"link/wiki");
			
		}


		void OnTools_ShowUpdateInfo()
		{
			Help.BrowseURL(__REST_URL_BASE__+"link/changelog");
		}

		void OnTools_DownloadAndInstallUpdate()
		{
			// let's make it simple right now, I'll deal with a proper internal download later.
			Help.BrowseURL(__REST_URL_BASE__+"link/wiki");

		}
		
	}
}
