using System;
using System.Collections.Generic;
using CameraManagement;
using Controllers;
using Input;
using Inventory;
using Managers;
using Player;
using SaveSystem.Data;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    NullState,
    Intro,
    MainMenu,
    Game
}

public delegate void OnStateChangeHandler();

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    public GameInput gameInput { get; private set; }

    public static GameManager Instance
    {
        get {
            return _instance;
        }
        set => _instance = value;
    }

    private static bool sceneChanged;

    private static bool isInputEnabled = true;
    public bool isMainMenuOn = true;
    public bool isGamePaused = true;
    private bool isPlayerAlive;
    private int playerDeathCount;
    private bool isBlinded;
    public bool isDialogueMode { get; private set; }
    public int checkpointIndex { get; private set; }

    public int currentExp { get; private set; }
    public int currentLevel { get; private set; } = 1;
    public bool isLevelingUp;

    public Vector3 spawnPoint { get; private set; } = new Vector3(-6.03999996f, -1.51999998f, 0);
    
    public List<BaseController> EnemiesInScreen { get; private set; }

    public int PlayerKrowns { get; private set; }

    public float DeltaTime
    {
        get { return isGamePaused ? 0 : Time.deltaTime; }
    }

    private List<EnemySpawner> enemies;

    public PlayerController PlayerController
    {
        get
        {
            return GameObject.Find("Player")
                .gameObject.GetComponent<PlayerController>();
        }
    }

    public Vector3 LastDeathPosition;
    private int mainMenuPhase;

    public static bool IsFirstStart = true;

    public bool isGameOver { get; private set; }

    public float MasterVolume { get; private set; } = 0.2f;
    
    public UnityEvent OnLevelReset { get; private set; }

    public void Awake()
    {
        if(_instance == null)
            _instance = this;
        
        
        if (OnLevelReset == null)
            OnLevelReset = new UnityEvent();
        
        gameInput = new GameInput();
        gameInput.SetupInputs();

        enemies = new List<EnemySpawner>();
        
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject.GetComponent(_instance.GetType()));

        EnemiesInScreen = new List<BaseController>();

        SetupGame();

        isGamePaused = true;
        PauseGame();
        

        DontDestroyOnLoad(transform.root.gameObject);
    }

    private void SetupGame()
    {
        isInputEnabled = false;
        PauseGame();
    }

    public void PauseGame()
    {
        isGamePaused = true;
    }

    public void ResumeGame()
    {
        isGamePaused = false;

        if (!IsFirstStart)
        {
            GetHUDCanvas().enabled = true;
        }
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    public bool GetMainMenuOn()
    {
        return isMainMenuOn;
    }

    public void SetMainMenuOn(bool mainMenuOn)
    {
        isMainMenuOn = mainMenuOn;
    }

    public Transform GetPlayerTransform()
    {
        return PlayerController.transform;
    }

    public void KillPlayer(Transform deathTransf)
    {
        LastDeathPosition = deathTransf.position;
        playerDeathCount++;
        SetInputEnabled(false);
        isPlayerAlive = false;
        SetCameraFollowTarget(false);
    }

    public void RespawnPlayer(bool isFirstRespawn = false)
    {
        if (!isFirstRespawn)
            SetInputEnabled(true);
        isPlayerAlive = true;
        SetCameraFollowTarget(true);
    }

    public void SetInputEnabled(bool isEnabled)
    {
        if (!ReferenceEquals(GetDynamicCamera(), null) && isEnabled) GetDynamicCamera().UpdateSize(10f, 0.3f);
        if (!ReferenceEquals(GetDynamicCamera(), null)) isInputEnabled = isEnabled;
    }

    public void SetCameraTarget(Transform target)
    {
        GetDynamicCamera().ChangeTarget(target);
    }

    public void SetCameraOffset(Vector2 offset)
    {
        GetDynamicCamera().UpdateOffset(offset);
    }

    public void SetCameraOffsetX(float offsetX)
    {
        GetDynamicCamera().UpdateOffsetX(offsetX);
    }

    public bool GetIsInputEnabled()
    {
        return isInputEnabled;
    }

    public void SetCameraOffsetY(float offsetY)
    {
        if (!ReferenceEquals(GetDynamicCamera(), null)) GetDynamicCamera().UpdateOffsetY(offsetY);
    }

    public void SetCameraSize(float size, float duration = 3f)
    {
        if (GetDynamicCamera() != null) GetDynamicCamera().UpdateSize(size, duration);
    }

    public Vector2 GetCameraOffset()
    {
        if (!ReferenceEquals(GetDynamicCamera(), null))
            return GetDynamicCamera().GetOffsets();
        return Vector2.zero;
    }

    public void SetCameraFollowTarget(bool follow)
    {
        if (!ReferenceEquals(GetDynamicCamera(), null))
            GetDynamicCamera().FollowTarget = follow;
    }

    public int GetMainMenuPhase()
    {
        return mainMenuPhase;
    }

    public void SetMainMenuPhase(int mainMenuPhase)
    {
        this.mainMenuPhase = mainMenuPhase;
    }

    public bool IsPlayerAlive()
    {
        return isPlayerAlive;
    }

    public int GetPlayerDeathCount()
    {
        return playerDeathCount;
    }

    public void PlayerExitInteractionMode()
    {
        PlayerController.ExitInteractionMode();
    }

    public bool AddPlayerKrowns(int amount)
    {
        PlayerController.KrownsAdded();
        PlayerKrowns += amount;
        return true;
    }

    public bool RemovePlayerKrowns(int amount)
    {
        if (PlayerKrowns >= amount)
        {
            PlayerController.KrownsRemoved();
            PlayerKrowns -= amount;
            return true;
        }

        return false;
    }

    public void ShowBossHealth(string bossName, BaseController controller)
    {
        GetHUD().ShowBossHealth(bossName, controller);
    }

    public void HideBossHealth()
    {
        GetHUD().HideBossHealth();
    }

    public void ShowHUD()
    {
        GetHUDCanvas().enabled = true;
    }

    public void GameOver()
    {
        isGameOver = true;
        PauseGame();
        GetUIManager().ShowGameOver();
    }

    public void Retry()
    {
        Destroy(PlayerController.transform.gameObject);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }

    public DynamicCamera GetDynamicCamera()
    {
        return Camera.main.GetComponent<DynamicCamera>();
    }

    public UI_HUD GetHUD()
    {
        return GameObject.Find("UI/UI_HUD").GetComponent<UI_HUD>();
    }
    
    public InventorySlotManager GetInventorySlotManager()
    {
        return GameObject.Find("Managers/InvetoryManager").GetComponent<InventorySlotManager>();
    }

    public UIManager GetUIManager()
    {
        return GameObject.Find("Managers/UIManager").GetComponent<UIManager>();
    }

    public static Canvas GetHUDCanvas()
    {
        return GameObject.Find("UI/UI_HUD").GetComponent<Canvas>();
    }

    public void LevelWon()
    {
        isGameOver = true;
        PauseGame();

        if (currentLevel == 1)
        {
            PlayerController.transform.position
                = GetLvlTwoPosition();
            SetSpawnPoint(GetLvlTwoPosition());
        }

        QuickSave();
        currentLevel++;
    }

    private Vector3 GetLvlOnePosition()
    {
        return GameObject.Find("SpawnPoints/LevelOneDefaultSpawn")
            .transform.position;
    }
    
    private Vector3 GetLvlTwoPosition()
    {
        return GameObject.Find("SpawnPoints/LevelTwoDefaultSpawn")
            .transform.position;
    }

    public bool CreateNewSave(int slot)
    {
        var playerData = new PlayerData(PlayerController);
        return SaveSystem.SaveSystem.SaveGame(ref playerData, slot);
    }

    public bool LoadSave(int slot)
    {
        var playerData = SaveSystem.SaveSystem.LoadSaveGame(slot);
        if (playerData == null) return false;

        // TODO: 1. loading screen 2. Reespawn enemies/bosses 3. Set player data
        PlayerController.SetData(playerData);

        return true;
    }

    public bool QuickSave()
    {
        var playerData = new PlayerData(PlayerController);
        return SaveSystem.SaveSystem.QuickSaveGame(ref playerData);
    }

    public bool QuickLoad()
    {
        var playerData = SaveSystem.SaveSystem.LoadQuickSaveGame();
        if (playerData == null) return false;

        // TODO: 1. loading screen 2. Reespawn enemies/bosses 3. Set player data
        PlayerController.SetData(playerData);

        return true;
    }

    public void SetPlayerKrones(int playerDataKrones)
    {
        PlayerKrowns = playerDataKrones;
    }
    
    public void ApplyBlindness(float howLong = 1f)
    {
        GetUIManager().ApplyBlindness(howLong);
    }

    public void ChangeMasterVolume(float newVolume)
    {
        MasterVolume = newVolume;
    }

    public void EnterDialogueMode()
    {
        isDialogueMode = true;
        PauseGame();
    }
    
    public void ExitDialogueMode()
    {
        isDialogueMode = false;
        ResumeGame();
    }

    public void SetSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
    }

    public void AddExperience(int exp)
    {
        if (currentExp + exp > 50)
        {
            currentLevel++;
            currentExp = 0;

            isLevelingUp = true;
            PauseGame();
            GetUIManager().ShowLevelUpScreen();
        }
        else
        {
            currentExp += exp;
        }
    }

    public void AumentHealth()
    {
        int newHealth = PlayerController.maxHealth + (int)(0.15f * PlayerController.maxHealth);
        PlayerController.maxHealth = newHealth;
        
        PlayerController.RefillHealth();
        
        ResumeGame();
        isLevelingUp = true;
    }

    public void AumentDamage()
    {        
        PlayerController.RefillHealth();
        PlayerController.combatCtrl.IncreseDamage();
        ResumeGame();
        isLevelingUp = true;
    }

    public void Aumentagicka()
    {        
        PlayerController.RefillHealth();
        
        ResumeGame();
        isLevelingUp = true;
    }

    public void AumentCdr()
    {        
        PlayerController.RefillHealth();
        
        ResumeGame();
        isLevelingUp = true;
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2", LoadSceneMode.Single);
        currentLevel = 2;
        SetSpawnPoint(GetLvlTwoPosition());
        RespawnPlayer(true);
        PlayerController.RespawnNow();
        PlayerController.transform.position = new Vector3(-17f, -0.6f, 0);
        ResumeGame();
    }

    public void AddEnemySpawner(EnemySpawner enemySpawner)
    {
        enemies.Add(enemySpawner);
        enemySpawner.Spawn();
    }

    public void ReloadLevel()
    {
        isGameOver = false;
        ResumeGame();
        GetUIManager().HideGameOver();
        
        PlayerController.RefillHealth();
        PlayerKrowns = 0;
        currentExp = 0;
        
        if(SceneManager.GetActiveScene().buildIndex == 0)
            SetSpawnPoint(GetLvlOnePosition());
        else if(SceneManager.GetActiveScene().buildIndex == 1)
            SetSpawnPoint(GetLvlTwoPosition());
        
        PlayerController.RespawnNow();
        
        foreach (var spawner in enemies)
            spawner.Respawn();
        
        OnLevelReset.Invoke();
    }

    public int GetSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}