using UnityEngine;
using System.Collections;
using CotcSdk;
using UnityEngine.UI;
using System;

#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#else
using UnityEngine.Experimental.Networking;
#endif


public class Login : MonoBehaviour
{
	// The cloud allows to make generic operations (non user related)
	private Cloud Cloud;
	// The gamer is the base to perform most operations. A gamer object is obtained after successfully signing in.
	public Gamer Gamer { get; private set; }
	// When a gamer is logged in, the loop is launched for domain private. Only one is run at once.
	private DomainEventLoop Loop;

	public User userStorage;

	public string username = "";
	public bool isSetup = false;
	//6023ba0190d488070dc9ce51
	//df9f17d13965f0eb6e9d9a4af9e2a0e2ca1f7ad6



	private void Awake()
	{
		
		var lg = FindObjectOfType<Login>();
		if(lg != null && lg.gameObject != this.gameObject)
		{
			Destroy(this.gameObject);
		}
		else
		{
			DontDestroyOnLoad(this.gameObject);
		}
	}

	// Use this for initialization
	void Start()
	{

		// Link with the CotC Game Object
		var cb = FindObjectOfType<CotcGameObject>();
		if (cb == null)
		{
			Debug.LogError("Please put a Clan of the Cloud prefab in your scene!");
			return;
		}

		// Log unhandled exceptions (.Done block without .Catch -- not called if there is any .Then)
		Promise.UnhandledException += (object sender, ExceptionEventArgs e) => {
			Debug.LogError("Unhandled exception: " + e.Exception.ToString());
		};


		// Initiate getting the main Cloud object
		cb.GetCloud().Done(cloud => {
			Cloud = cloud;
			// Retry failed HTTP requests once
			Cloud.HttpRequestFailedHandler = (HttpRequestFailedEventArgs e) => {
				if (e.UserData == null)
				{
					e.UserData = new object();
					e.RetryIn(1000);
				}
				else
					e.Abort();
			};
			isSetup = true;
			Debug.Log("Setup done");
			DoLogin();
			
		});
		

	}
	
	public void DoLogin()
	{
		//Login User anonymously if it's his first time or it was logout
		if(userStorage.GamerId.Length > 0 && userStorage.GamerSecretID.Length > 0)
		{
			ResumeSession();
		}
		else
		{
			LoginAnonym();
		}
		
	}


	public void LoginToAccount(string network, string email, string password)
	{
		var cotc = FindObjectOfType<CotcGameObject>();

		cotc.GetCloud().Done(cloud =>
		{
			Cloud.Login(
				network: network,
				networkId: email,
				networkSecret: password)
			.Done(gamer =>
			{
				Gamer = gamer;
				SaveGamer(gamer);
				Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
				Debug.Log("Login data: " + gamer);
				Debug.Log("Server time: " + gamer["servertime"]);

				var profile = FindObjectOfType<ProfileScript>();
				profile.UpdateProfile();

				var loginUI = FindObjectOfType<LoginUI>();
				loginUI.updateUI();

			}, ex =>
			{
				// The exception should always be CotcException
				CotcException error = (CotcException)ex;
				Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
			});
		});
	}

	public void Convert(string network, string email, string password)
	{
		Gamer.Account.Convert(network, email, password)
		.Done(convertRes => {
			Debug.Log("Convert succeeded: " + convertRes.ToString());

			var loginUI = FindObjectOfType<LoginUI>();
			loginUI.updateUI();

			var profile = FindObjectOfType<ProfileScript>();
			if (profile != null)
			{
				profile.UpdateProfile();
			}
		}, ex => {
			// The exception should always be CotcException
			CotcException error = (CotcException)ex;
			Debug.LogError("Failed to convert: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
		});
	}

	public void Logout()
	{
		var cotc = FindObjectOfType<CotcGameObject>();
		cotc.GetCloud().Done(cloud => {
			Cloud.Logout(Gamer)
			.Done(result => {
				userStorage.GamerId = "";
				userStorage.GamerSecretID = "";

				DoLogin();
			}, ex => {
		  // The exception should always be CotcException
		  CotcException error = (CotcException)ex;
				Debug.LogError("Failed to logout: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
			});
		});
		
	}

	private void ResumeSession()
	{
		var cotc = FindObjectOfType<CotcGameObject>();

		cotc.GetCloud().Done(cloud => {
			Cloud.ResumeSession(
				gamerId: userStorage.GamerId,
				gamerSecret: userStorage.GamerSecretID)
			.Done(gamer => {
				Gamer = gamer;
				Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
				Debug.Log("Login data: " + gamer);
				Debug.Log("Server time: " + gamer["servertime"]);

				var loginUI = FindObjectOfType<LoginUI>();
				loginUI.updateUI();

				var profile = FindObjectOfType<ProfileScript>();
				if (profile != null)
				{
					profile.UpdateProfile();
				}
			}, ex => {
				// The exception should always be CotcException
				CotcException error = (CotcException)ex;
				Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
			});
		});
	}

	private void LoginAnonym()
	{
		// Call the API method which returns an Promise<Gamer> (promising a Gamer result).
		// It may fail, in which case the .Then or .Done handlers are not called, so you
		// should provide a .Catch handler.
		Cloud.LoginAnonymously()
			.Then(gamer => DidLogin(gamer))
			.Catch(ex => {
				// The exception should always be CotcException
				CotcException error = (CotcException)ex;
				Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
			});
	}

	//Save Gamers id and secret
	private void SaveGamer(Gamer gamer)
	{
		userStorage.GamerId = gamer.GamerId;
		userStorage.GamerSecretID = gamer.GamerSecret;
	}

	// Invoked when any sign in operation has completed
	private void DidLogin(Gamer newGamer)
	{
		if (Gamer != null)
		{
			Debug.LogWarning("Current gamer " + Gamer.GamerId + " has been dismissed");
			Loop.Stop();
		}
		Gamer = newGamer;
		Loop = Gamer.StartEventLoop();
		Loop.ReceivedEvent += Loop_ReceivedEvent;
		Debug.Log("Signed in successfully (ID = " + Gamer.GamerId + ")");
		
		//Save the User for reconnect later 
		SaveGamer(Gamer);

		//Add register field for stop being a guest
		var loginUI = FindObjectOfType<LoginUI>();
		loginUI.updateUI();

		var profile = FindObjectOfType<ProfileScript>();
		//Update profile
		if (profile != null)
		{
			profile.UpdateProfile();
		}
	}

	private void Loop_ReceivedEvent(DomainEventLoop sender, EventLoopArgs e)
	{
		Debug.Log("Received event of type " + e.Message.Type + ": " + e.Message.ToJson());
	}

}
