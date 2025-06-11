using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private int successThreshold = 3;

    [Header("Win SFX")]
    [SerializeField] private AudioSource winSoundSource;
    [SerializeField] private AudioClip winSFX;

    [Header("References")]
    [SerializeField] private GameObject successMenu;
    [SerializeField] private Button restartButton;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    private int currentSuccesses = 0;
    private bool hasWon = false;
    private float timer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (winSoundSource == null)
        {
            winSoundSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (successMenu != null)
            successMenu.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(() => RestartGame());

        Time.timeScale = 1f;
        timer = 0f;
    }

    private void Update()
    {
        if (!hasWon)
        {
            timer += Time.deltaTime;
        }
    }

    public void RegisterSuccess()
    {
        if (hasWon)
            return;

        currentSuccesses++;
        Debug.Log($"Success registered! Current: {currentSuccesses}/{successThreshold}");

        if (currentSuccesses >= successThreshold)
        {
            HandleSuccess();
        }
    }

    private void HandleSuccess()
    {
        hasWon = true;

        if (winSoundSource != null && winSFX != null)
        {
            winSoundSource.PlayOneShot(winSFX);
        }

        successMenu.SetActive(true);
        Debug.Log("🎉 You Win!");

        float currentScore = Mathf.Round(timer * 100f) / 100f; // round to 2 decimal places
        float bestScore = PlayerPrefs.GetFloat("HighScore", float.MaxValue);

        scoreText.text = $"Time: {currentScore} s";

        if (currentScore < bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetFloat("HighScore", bestScore);
        }

        highScoreText.text = $"Best: {bestScore:F2} s";

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
