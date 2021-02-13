using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CotcSdk;

public class Scores : MonoBehaviour
{
	private Login loginScript;

	private void Start()
	{
		loginScript = FindObjectOfType<Login>();
		if (loginScript == null)
		{
			Debug.LogError("There is no login on this scene");
			return;
		}

	}

	public void PostScore(int score)
	{
		// currentGamer is an object retrieved after one of the different Login functions.

		loginScript.Gamer.Scores.Domain("private").Post(score, "normalMode", ScoreOrder.HighToLow,
		"Best score", false)
		.Done(postScoreRes => {
			GameObject.Find("Rank").GetComponent<Text>().text = "Rank: " + postScoreRes.Rank.ToString();
		}, ex => {
			// The exception should always be CotcException
			CotcException error = (CotcException)ex;
			Debug.LogError("Could not post score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
		});
	}
}
