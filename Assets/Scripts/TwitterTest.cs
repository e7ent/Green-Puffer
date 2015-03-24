using UnityEngine;
using System.Collections;
using Soomla.Profile;

public class TwitterTest : MonoBehaviour
{
	void Start()
	{
		SoomlaProfile.Initialize();
	}

	public void Login()
	{
		print("Login test");
		ProfileEvents.OnLoginStarted = (Provider p, string s) =>
		{
			print("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!OnLoginStarted");
		};
		ProfileEvents.OnLoginFailed = (Provider provider, string message, string payload) =>
		{
			print("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!OnLoginFailed");
			print(message);
		};
		ProfileEvents.OnLoginCancelled = (Provider p, string s) =>
		{
			print("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!OnLoginCancelled");
		};
		SoomlaProfile.Login(Provider.TWITTER);
	}

	public void TwitterWrite()
	{
		print(SoomlaProfile.IsLoggedIn(Provider.TWITTER));
		SoomlaProfile.UpdateStatus(Provider.TWITTER, "Test Status");
	}
}
