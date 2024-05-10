using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIScript : MonoBehaviour
{
    public static UIScript instance;
    [SerializeField] [Tooltip("Button to minimize and maximize panel")] private Button startScanButton;
    [SerializeField] [Tooltip("Button to start game after scan")] private Button startGameButton;
    [SerializeField] [Tooltip("Button to minimize scan panell")] private Button hideScanPanelButton;
    [SerializeField] [Tooltip("Button to open settings panel")] private Button settingsButton;
    [SerializeField] [Tooltip("Button to open settings panel")] private Button backButton;
    [SerializeField] [Tooltip("Button to restart the game")] private Button restartButton;
    [SerializeField] [Tooltip("Button to open settings panel while in game")] private Button inGameSettingsButton;
    [SerializeField] [Tooltip("The beginning panel")] private GameObject startingScreen;
    [SerializeField] [Tooltip("The scan panel")] private GameObject scanPanel;
    [SerializeField] [Tooltip("The scan panel")] private GameObject settingsPanel;
    [SerializeField] [Tooltip("The scan message")] private GameObject scanMessage;
    [SerializeField] [Tooltip("The game panel")] private GameObject gamePanel;
    [SerializeField] [Tooltip("The end game panel")] private GameObject endPanel;
    [SerializeField] [Tooltip("The wave message")] private GameObject waveMessage;
    [SerializeField] [Tooltip("The enemies remaining in the wave")] private GameObject enemyMessage;
    [SerializeField] [Tooltip("The game manager to handle game sequences")] private GameManager gameManager;
    [SerializeField] [Tooltip("The enemy spawner")] private EnemySpawner enemySpawner;
    [SerializeField] [Tooltip("The score")] private TMP_Text scoreText;
    public int score;
    private string[] numberWords = { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN" };


    void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    void Start()
    {
        AssignButtonListeners();
        scanPanel.SetActive(false);
        gamePanel.SetActive(false);
        settingsPanel.SetActive(false);
        endPanel.SetActive(false);

    }

    //function to assign the buttons their functions
     void AssignButtonListeners()
    {
        startScanButton.onClick.AddListener(StartScan);
        hideScanPanelButton.onClick.AddListener(HideScanMessage);
        startGameButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(SettingsPanel);
        backButton.onClick.AddListener(BackButton);
        inGameSettingsButton.onClick.AddListener(SettingsPanel);
        restartButton.onClick.AddListener(RestartGame);
    }

    void HideScanMessage() {
        scanMessage.SetActive(false);
    }

    void BackButton() {
        settingsPanel.SetActive(false);
        ResumeGame();
    }


    void StartScan() {
        startingScreen.SetActive(false);
        // backButton.SetActive(true);
        scanPanel.SetActive(true);
    }

    void SettingsPanel() {
        settingsPanel.SetActive(true);
        PauseGame();
    }

    public void StartGame() {
        scanPanel.SetActive(false);
        gameManager.StartGame();
        InitializeGamePanel();
        Debug.Log("Game Started");
    }

    public void EndGame() {
        endPanel.SetActive(true);
    }

    public void RestartGame() {
        endPanel.SetActive(false);
        GameManager.instance.RestartGame();
    }

    void PauseGame() {
        Time.timeScale = 0; // This freezes the game
        Debug.Log("Game Paused");
    }

    void ResumeGame() {
        Time.timeScale = 1; // This resumes the game
        Debug.Log("Game Resumed");
    }
    public void InitializeGamePanel() {
        gamePanel.SetActive(true);
        Debug.Log("InitializeGamePanel Started");
        waveMessage.GetComponent<TextMeshProUGUI>().text = "WAVE " + numberWords[enemySpawner.waveCount];
        enemyMessage.GetComponent<TextMeshProUGUI>().text =  numberWords[enemySpawner.numEnemies] + " ENEMIES REMAINING";
    }

    public void updateWaveMessage() {
        waveMessage.GetComponent<TextMeshProUGUI>().text = "WAVE " + numberWords[enemySpawner.waveCount];
    }

    public void updateEnemyMessage() {
        enemyMessage.GetComponent<TextMeshProUGUI>().text =  numberWords[enemySpawner.numEnemies] + " ENEMIES REMAINING";
    }

    public void updateScore() {
        scoreText.text = score.ToString();
    }



}
