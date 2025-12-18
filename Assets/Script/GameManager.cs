using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    // Cuando el jugador pierde
    public void GameOver()
    {
        PlayerPrefs.SetInt("LAST_SCORE", score);
        PlayerPrefs.SetString("LAST_RESULT", "GAME OVER");
        PlayerPrefs.Save();

        SceneManager.LoadScene("End");
    }

    // Cuando el jugador gana TODO el juego
    public void Victory()
    {
        PlayerPrefs.SetInt("LAST_SCORE", score);
        PlayerPrefs.SetString("LAST_RESULT", "VICTORY");
        PlayerPrefs.Save();

        SceneManager.LoadScene("End");
    }
}
