using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resourcesText;
    [SerializeField] private TMP_Text warningText;

    [SerializeField] private GameObject towerPanel;
    [SerializeField] private GameObject towerCardPrefab;
    [SerializeField] private Transform cardsContainer;

    [SerializeField] private TowerData[] towers;
    private List<GameObject> activeCards = new List<GameObject>();

    private Platform _currentPlatform;

    [SerializeField] private Button speed1Button;
    [SerializeField] private Button speed2Button;
    [SerializeField] private Button speed3Button;
    // white means the sprite is untinted and unaltered
    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color selectedButtonColor = Color.grey;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.white;

    [SerializeField] private GameObject pausePanel;
    private bool _isGamePaused = false;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject FinalLevelCompletePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {// prevent multiple instances
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    public void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLivesText;
        GameManager.OnResourcesChanged += UpdateResourcesText;
        Platform.OnPlatformClicked += HandlePlatformClicked;
        TowerCard.OnTowerSelected += HandleTowerSelected;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Spawner.OnLevelComplete += ShowLevelComplete;
    }

    public void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLivesText;
        GameManager.OnResourcesChanged -= UpdateResourcesText;
        Platform.OnPlatformClicked -= HandlePlatformClicked;
        TowerCard.OnTowerSelected -= HandleTowerSelected;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Spawner.OnLevelComplete -= ShowLevelComplete;
    }

    private void Start()
    {
        // lambda to do something
        speed1Button.onClick.AddListener(() => SetGameSpeed(0.2f));
        speed2Button.onClick.AddListener(() => SetGameSpeed(1f));
        speed3Button.onClick.AddListener(() => SetGameSpeed(2f));

        HighlightSelectedSpeedButton(GameManager.Instance.GameSpeed);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }

    private void UpdateLivesText(int currentLives)
    {
        livesText.text = $"Lives: {currentLives}";

        if (currentLives <= 0)
        {
            ShowGameOver();
        }
    }

    private void UpdateResourcesText(int currentResources)
    {
        resourcesText.text = $"Currency: {currentResources}";
    }

    private void HandlePlatformClicked(Platform platform)
    {
        _currentPlatform = platform;
        ShowTowerPanel();
    }

    private void ShowTowerPanel()
    {
        if (pausePanel.activeSelf)
            return;

        towerPanel.SetActive(true);
        Platform.towerPanelOpen = true;
        GameManager.Instance.SetTimeScale(0f);
        PopulateTowerCards();
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        Platform.towerPanelOpen = false;
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
    }

    private void PopulateTowerCards()
    {
        foreach (var card in activeCards)
        {
            Destroy(card);
        }
        activeCards.Clear();

        foreach (var data in towers)
        {
            GameObject cardGameObject = Instantiate(towerCardPrefab, cardsContainer);
            TowerCard card = cardGameObject.GetComponent<TowerCard>();
            card.Initialize(data);
            activeCards.Add(cardGameObject);
        }
    }

    private void HandleTowerSelected(TowerData towerData)
    {
        if (_currentPlatform.transform.childCount > 0)
        {
            HideTowerPanel();
            StartCoroutine(ShowWarningMessage("This spot already has a tower."));
            return;
        }
        if (GameManager.Instance.Resources >= towerData.cost)
        {
            GameManager.Instance.SpendResources(towerData.cost);
            _currentPlatform.PlaceTower(towerData);
            SpriteRenderer Ren = _currentPlatform.GetComponent<SpriteRenderer>();
            Ren.enabled = false;
        }
        else
        {
            StartCoroutine(ShowWarningMessage("Not Enough Resources!"));
        }
            HideTowerPanel();
    }

    private IEnumerator ShowWarningMessage(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        warningText.gameObject.SetActive(false);
    }

    private void SetGameSpeed(float timeScale)
    {
        HighlightSelectedSpeedButton(timeScale);
        GameManager.Instance.SetGameSpeed(timeScale);
    }

    private void UpdateButtonVisual(Button button, bool isSelected)
    {
        // ternary conditional operator, single line if else
        button.image.color = isSelected ? selectedButtonColor : normalButtonColor;

        TMP_Text text = button.GetComponent<TMP_Text>();
        if (text != null)
        {
            text.color = isSelected? selectedTextColor : normalTextColor;
        }
    }

    private void HighlightSelectedSpeedButton(float selectedSpeed)
    {
        // speed matches whichever button was pressed
        UpdateButtonVisual(speed1Button, selectedSpeed == 0.2f);
        UpdateButtonVisual(speed2Button, selectedSpeed == 1f);
        UpdateButtonVisual(speed3Button, selectedSpeed == 2f);
    }

    public void TogglePause()
    {
        if (towerPanel.activeSelf)
            return;

        if (_isGamePaused)
        {
            pausePanel.SetActive(false);
            _isGamePaused = false;
            GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
        }
        else
        {
            pausePanel.SetActive(true);
            _isGamePaused = true;
            GameManager.Instance.SetTimeScale(0f);
        }
    }

    public void RestartLevel()
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowGameOver()
    {
        GameManager.Instance.SetTimeScale(1f);
        gameOverPanel.SetActive(true);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ShowObjective());
    }

    private IEnumerator ShowObjective()
    {
        objectiveText.text = $"Survive {LevelManager.Instance.CurrentLevel.wavesToWin} Waves!";
        objectiveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        objectiveText.gameObject.SetActive(false);
    }

    private void ShowLevelComplete()
    {
        if (LevelManager.Instance.levelIndex >= 2)
        {
            FinalLevelCompletePanel.SetActive(true);
            GameManager.Instance.SetTimeScale(0f);
        }
        else
        {
            
            levelCompletePanel.SetActive(true);
            GameManager.Instance.SetTimeScale(0f);
        }

        
    }

    public void EnterEndlessMode()
    {
        levelCompletePanel.SetActive(false);
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
        Spawner.Instance.EnableEndlessMode();
    }

    public void NextLevel()
    {
        if (LevelManager.Instance.levelIndex >= 2)
        {
            // do nothing because we've finished all levels
        }
        else {
            LevelManager.Instance.levelIndex++;
            LevelManager.Instance.LoadLevel(LevelManager.Instance.allLevels[LevelManager.Instance.levelIndex]);
        }
            
    }

}
