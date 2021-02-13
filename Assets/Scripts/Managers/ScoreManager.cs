using UnityEngine;
using UnityEngine.UI;
using CotcSdk;

public class ScoreManager : MonoBehaviour {
    
    private Login LoginScript;

    private void Start()
    {
        LoginScript = FindObjectOfType<Login>();
        if (LoginScript == null)
        {
            Debug.LogError("There is no login on this scene");
            return;
        }
        else
        {
            CenteredScores();
        }

    }

    private void CenteredScores()
    {
        // currentGamer is an object retrieved after one of the different Login functions.

        LoginScript.Gamer.Scores.Domain("private").PagedCenteredScore("normalMode")
        .Done(centeredScoresRes => {
            string txt = "";
            foreach (var score in centeredScoresRes)
                txt += score.Rank + ". " + score.GamerInfo["profile"]["displayName"] + ": " + score.Value + "\n";
            GameObject.FindGameObjectWithTag("ScoresText").GetComponent<Text>().text = txt;

            Debug.Log(txt);
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not get centered scores: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }

}
