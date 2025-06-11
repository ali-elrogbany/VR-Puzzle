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

    private int currentSuccesses = 0;
    private bool hasWon = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        winSoundSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (successMenu != null)
        {
            successMenu.SetActive(false);
        }
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => RestartGame());
        }
        Time.timeScale = 1f;
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

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
