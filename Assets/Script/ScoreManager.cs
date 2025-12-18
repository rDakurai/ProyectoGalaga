using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int pointsPerSecond = 5;
    [SerializeField] private float interval = 1f;

    private int score;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
        InvokeRepeating(nameof(AddPassiveScore), interval, interval);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText == null)
        {
            Debug.LogWarning("ScoreText NO asignado en ScoreManager.");
            return;
        }

        scoreText.text = "Score: " + score;
    }

        private void AddPassiveScore()
    {
        AddScore(pointsPerSecond);
    }

}






