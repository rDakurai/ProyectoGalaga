using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text messageText;
    public TMP_Text timerText; // solo se usa en Nivel 2

    [Header("Scenes")]
    public string nextSceneName = "";

    [Header("Delays")]
    public float nextLevelDelay = 5f;

    Coroutine messageCo;

    void Start()
    {
        if (messageText != null) messageText.text = "";
        if (timerText != null) timerText.text = "";
    }

    public void ShowMessage(string msg, float seconds = 2f)
    {
        if (messageText == null) return;

        if (messageCo != null) StopCoroutine(messageCo);
        messageCo = StartCoroutine(MessageRoutine(msg, seconds));
    }

    IEnumerator MessageRoutine(string msg, float seconds)
    {
        messageText.text = msg;
        yield return new WaitForSeconds(seconds);
        messageText.text = "";
    }

    public void LoadNextLevelAfterDelay()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            StartCoroutine(LoadNextRoutine());
    }

    IEnumerator LoadNextRoutine()
    {
        // Mensaje fijo hasta cambiar de escena
        if (messageText != null)
            messageText.text = "Siguiente nivel en " + nextLevelDelay + "s...";

        yield return new WaitForSeconds(nextLevelDelay);
        SceneManager.LoadScene(nextSceneName);
    }

    // Para victoria final (reusar End)
    public void WinGameToEnd(int finalScore)
    {
        PlayerPrefs.SetInt("LAST_SCORE", finalScore);
        PlayerPrefs.SetString("LAST_RESULT", "VICTORY");
        PlayerPrefs.Save();
        SceneManager.LoadScene("End");
    }
}
