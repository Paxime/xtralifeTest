using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CotcSdk;

public class ProfileScript : MonoBehaviour
{
    public Text profileName;
    public Text Balance;

    private Login LoginScript;

    private void Start()
    {
		print("start profile");
		LoginScript = FindObjectOfType<Login>();
		if (LoginScript == null)
		{
			Debug.LogError("There is no login on this scene");
			return;
		}
		profileName.text = LoginScript.username;
	}

    public void UpdateProfile()
    {
		print("update profile");
		LoginScript.Gamer.Profile.Get()
		.Done(profileRes => {
			LoginScript.username = profileRes["displayName"];
			profileName.text = LoginScript.username;
		}, ex => {
			// The exception should always be CotcException
			CotcException error = (CotcException)ex;
			Debug.LogError("Could not get profile data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
		});
		//Get current Gamer Profile
		
       
    }

	private void changePseudo(Gamer gamer,string textPseudo)
	{
		if (textPseudo.Length > 0)
		{
			Bundle profileUpdates = Bundle.CreateObject();
			profileUpdates["displayName"] = new Bundle(textPseudo);
			gamer.Profile.Set(profileUpdates)
			.Done(profileRes => {
				Debug.Log("Profile data set: " + profileRes.ToString());
				UpdateProfile();
			}, ex => {
				// The exception should always be CotcException
				CotcException error = (CotcException)ex;
				Debug.LogError("Could not set profile data due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
			});
		}
		else
		{
			UpdateProfile();
		}
	}

}
