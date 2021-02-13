using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class GameGUINavigation : MonoBehaviour {

	//------------------------------------------------------------------
	// Variable declarations
	
	private bool _paused;
    private bool quit;
    private string _errorMsg;
	//public bool initialWaitOver = false;

	public float initialDelay;

	// canvas
	public Canvas PauseCanvas;
	public Canvas QuitCanvas;
	public Canvas ReadyCanvas;
	public Canvas ScoreCanvas;
    public Canvas ErrorCanvas;
    public Canvas GameOverCanvas;
	
	// buttons
	public Button MenuButton;

	//
	public Scores scoreScript;


	//------------------------------------------------------------------
	// Function Definitions

	// Use this for initialization
	void Start () 
	{
		StartCoroutine("ShowReadyScreen", initialDelay);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			// if scores are show, go back to main menu
			if(GameManager.gameState == GameManager.GameState.Scores)
				Menu();

			// if in game, toggle pause or quit dialogue
			else
			{
				if(quit == true)
					ToggleQuit();
				else
					TogglePause();
			}
		}
	}

	// public handle to show ready screen coroutine call
	public void H_ShowReadyScreen()
	{
		StartCoroutine("ShowReadyScreen", initialDelay);
	}

    public void H_ShowGameOverScreen()
    {
        StartCoroutine("ShowGameOverScreen");
    }

	IEnumerator ShowReadyScreen(float seconds)
	{
		//initialWaitOver = false;
		GameManager.gameState = GameManager.GameState.Init;
		ReadyCanvas.enabled = true;
		yield return new WaitForSeconds(seconds);
		ReadyCanvas.enabled = false;
		GameManager.gameState = GameManager.GameState.Game;
		//initialWaitOver = true;
	}

    IEnumerator ShowGameOverScreen()
    {
        Debug.Log("Showing GAME OVER Screen");
        GameOverCanvas.enabled = true;
        yield return new WaitForSeconds(2);
        Menu();
    }

	public void getScoresMenu()
	{
		Time.timeScale = 0f;		// stop the animations
		GameManager.gameState = GameManager.GameState.Scores;

		

		MenuButton.enabled = false;
		ScoreCanvas.enabled = true;

		scoreScript.PostScore(GameManager.score);
	}

	//------------------------------------------------------------------
	// Button functions

	public void TogglePause()
	{
		// if paused before key stroke, unpause the game
		if(_paused)
		{
			Time.timeScale = 1;
			PauseCanvas.enabled = false;
			_paused = false;
			MenuButton.enabled = true;
		}
		
		// if not paused before key stroke, pause the game
		else
		{
			PauseCanvas.enabled = true;
			Time.timeScale = 0.0f;
			_paused = true;
			MenuButton.enabled = false;
		}


        Debug.Log("PauseCanvas enabled: " + PauseCanvas.enabled);
	}
	
	public void ToggleQuit()
	{
		if(quit)
        {
            PauseCanvas.enabled = true;
            QuitCanvas.enabled = false;
			quit = false;
		}
		
		else
        {
            QuitCanvas.enabled = true;
			PauseCanvas.enabled = false;
			quit = true;
		}
	}

	public void Menu()
	{
		Application.LoadLevel("menu");
		Time.timeScale = 1.0f;

        // take care of game manager
	    GameManager.DestroySelf();
	}

	//Change
	//
    public void AddScore(int score)
    {
        
    }

    public void LoadLevel()
    {
        GameManager.Level++;
        Application.LoadLevel("game");
    }

	

}
