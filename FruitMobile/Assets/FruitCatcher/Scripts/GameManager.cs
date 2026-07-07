using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Game Rules")]
    [SerializeField] private int startLives = 3;
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;
    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip gameOverSound;
    private int score;
    private int lives;
    public bool IsGameActive { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        AutoWireSceneReferences();
        WireButtons();
        ShowMainMenu();
    }
    public void ShowMainMenu()
    {
        score = 0;
        lives = startLives;
        IsGameActive = false;
        SetPanels(true, false, false);
        UpdateUI();
        ClearFallingItems();
    }
    public void StartGame()
    {
        score = 0;
        lives = startLives;
        IsGameActive = true;
        SetPanels(false, true, false);
        UpdateUI();
        ClearFallingItems();

        FruitRainSpawner spawner = FindFirstObjectByType<FruitRainSpawner>();
        if (spawner != null)
            spawner.OnGameStarted();
    }
    public void RestartGame()
    {
        StartGame();
    }
    public void AddScore(int amount)
    {
        if (!IsGameActive)
            return;
        score += amount;
        UpdateUI();
        PlaySound(collectSound);
    }
    public void LoseLife(int amount)
    {
        if (!IsGameActive)
            return;
    lives -= amount;

 if (lives < 0)
            lives = 0;
        UpdateUI();
        PlaySound(damageSound);
        if (lives <= 0)
            GameOver();
    }
    private void GameOver()
    {
        IsGameActive = false;
        SetPanels(false, false, true);
        if (finalScoreText != null)
            finalScoreText.text = "Score: " + score;
        PlaySound(gameOverSound);
    }
    private void SetPanels(bool mainMenu, bool hud, bool gameOver)
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(mainMenu);
        if (hudPanel != null)
            hudPanel.SetActive(hud);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(gameOver);
    }
    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
        if (livesText != null)
            livesText.text = "Lives: " + lives;
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;
        audioSource.PlayOneShot(clip);
    }
    private void ClearFallingItems()
    {
        FallingItem[] fallingItems = FindObjectsByType<FallingItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (FallingItem item in fallingItems)
        {
            if (item == null)
                continue;

            Destroy(item.gameObject);
        }
    }

    private void AutoWireSceneReferences()
    {
        if (mainMenuPanel == null)
            mainMenuPanel = GameObject.Find("MainMenuPanel");

        if (hudPanel == null)
            hudPanel = GameObject.Find("HUDPanel");

        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");

        TextMeshProUGUI[] texts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (TextMeshProUGUI tmp in texts)
        {
            string lowerName = tmp.gameObject.name.ToLowerInvariant();
            if (scoreText == null && lowerName.Contains("score") && !lowerName.Contains("gameover"))
                scoreText = tmp;
            else if (livesText == null && lowerName.Contains("live"))
                livesText = tmp;
            else if (finalScoreText == null && lowerName.Contains("finalscore"))
                finalScoreText = tmp;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void WireButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button == null)
                continue;

            string lowerName = button.gameObject.name.ToLowerInvariant();
            if (lowerName.Contains("restart"))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(RestartGame);
            }
            else if (lowerName == "button" || lowerName.Contains("start"))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(StartGame);
            }
            else if (lowerName.Contains("menu"))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ShowMainMenu);
            }
        }
    }
}