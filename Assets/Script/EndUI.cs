using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndUI : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text scoreText;

    public string gameSceneName = "SampleScene";
    public string menuSceneName = "Menu";

    void Start()
    {
        int score = PlayerPrefs.GetInt("LAST_SCORE", 0);
        string result = PlayerPrefs.GetString("LAST_RESULT", "GAME OVER");

        if (resultText != null) resultText.text = result;
        if (scoreText != null) scoreText.text = "SCORE: " + score;
    }

    public void Retry()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
