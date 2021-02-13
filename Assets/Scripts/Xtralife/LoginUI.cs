using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CotcSdk;

public class LoginUI : MonoBehaviour
{

	public bool connexion = false; //False for register and true for login
	
	public GameObject registerArea;

	public InputField textEmail;
	public InputField textPassword;

	public GameObject textLogin;
	public GameObject textRegister;

	private Login loginScript;

	private bool isLogin = false;

	private void Start()
	{
		loginScript = FindObjectOfType<Login>();

		updateUI();

	}

	public void updateUI()
	{
		

		isLogin = loginScript.Gamer != null && loginScript.Gamer["network"] != "anonymous";

		registerArea.SetActive(!isLogin);
	}

	//Use for change protocole between login and register
	public void RegisterLoginState()
	{
		connexion = !connexion;
		if (connexion)
		{
			textLogin.SetActive(true);
			textRegister.SetActive(false);
		}
		else
		{
			textLogin.SetActive(false);
			textRegister.SetActive(true);
		}
	}

	public void Logout()
	{
		loginScript.Logout();
	}

	public void Mail()
	{
		if (connexion)
		{
			loginScript.LoginToAccount("email", textEmail.text, textPassword.text);
		}
		else
		{
			loginScript.Convert("email", textEmail.text, textPassword.text);
		}
	}

	public void Facebook()
	{
		if (connexion)
		{
			loginScript.LoginToAccount("facebook", textEmail.text, textPassword.text);
		}
		else
		{
			loginScript.Convert("facebook", textEmail.text, textPassword.text);
		}
	}

	public void Google()
	{
		if (connexion)
		{
			loginScript.LoginToAccount("googleplus", textEmail.text, textPassword.text);
		}
		else
		{
			loginScript.Convert("googleplus", textEmail.text, textPassword.text);
		}
	}

	public void Gamecenter()
	{
		if (connexion)
		{
			loginScript.LoginToAccount("gamecenter", textEmail.text, textPassword.text);
		}
		else
		{
			loginScript.Convert("gamecenter", textEmail.text, textPassword.text);
		}
	}
}
