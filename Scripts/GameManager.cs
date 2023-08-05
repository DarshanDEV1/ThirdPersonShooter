using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentGameState { get; private set; } = GameState.MainMenu;

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

    private void Start()
    {
        // Initialize game settings, load main menu, etc.
        SetGameState(GameState.MainMenu);
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);
        SceneManager.LoadScene("GameScene"); // Replace with your actual game scene name
    }

    public void PauseGame()
    {
        if (CurrentGameState == GameState.Playing)
            SetGameState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (CurrentGameState == GameState.Paused)
            SetGameState(GameState.Playing);
    }

    public void EndGame()
    {
        SetGameState(GameState.GameOver);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SetGameState(GameState.Playing);
    }

    private void SetGameState(GameState newState)
    {
        if (CurrentGameState != newState)
        {
            CurrentGameState = newState;
            HandleGameStateChange();
        }
    }

    private void HandleGameStateChange()
    {
        // Perform actions based on the current game state
        switch (CurrentGameState)
        {
            case GameState.MainMenu:
                // Handle main menu logic
                break;
            case GameState.Playing:
                // Handle playing logic
                break;
            case GameState.Paused:
                // Handle pause logic
                break;
            case GameState.GameOver:
                // Handle game over logic
                break;
        }
    }
}
