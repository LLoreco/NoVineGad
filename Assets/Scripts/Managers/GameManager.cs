using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameSpeed = 5f;
    private GameState currentState = GameState.Playing;
    public static GameManager Instance { get; private set; }
    public GameState CurrentState => currentState;
    public float GameSpeed => gameSpeed;

    public System.Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void InitializeGame()
    {
        Debug.Log("Игровой менеджер иницализирован");
        InitializeServices();
    }

    private void InitializeServices()
    {
        PlayerInput inputService = new GameObject("PlayerInput").AddComponent<PlayerInput>();
        ServiceManager.Instance.RegisterService<IInputHandler>(inputService);
    }

    public void ChangeGameState(GameState newState)
    {
        if (currentState != newState)
        {
            GameState previousState = currentState;
            currentState = newState;

            Debug.Log($"Game state changed from {previousState} to {newState}");
            OnGameStateChanged?.Invoke(newState);

            HandleStateChange(newState);
        }
    }

    private void HandleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Menu:
                Time.timeScale = 0f;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }

    public void StartGame()
    {
        ChangeGameState(GameState.Playing);
    }

    public void PauseGame()
    {
        ChangeGameState(GameState.Paused);
    }

    public void ResumeGame()
    {
        ChangeGameState(GameState.Playing);
    }

    public void GameOver()
    {
        ChangeGameState(GameState.GameOver);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
