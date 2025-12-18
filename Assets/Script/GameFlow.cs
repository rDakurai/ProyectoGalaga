using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
    public string endSceneName = "End";

    public void GameOver(int finalScore)
    {
        PlayerPrefs.SetInt("LAST_SCORE", finalScore);
        PlayerPrefs.SetString("LAST_RESULT", "GAME OVER");
        PlayerPrefs.Save();
        SceneManager.LoadScene(endSceneName);
    }

    public void Victory(int finalScore)
    {
        PlayerPrefs.SetInt("LAST_SCORE", finalScore);
        PlayerPrefs.SetString("LAST_RESULT", "VICTORY");
        PlayerPrefs.Save();
        SceneManager.LoadScene(endSceneName);
    }
}
