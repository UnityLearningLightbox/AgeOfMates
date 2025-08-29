using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData
{
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    public List<string> inventoryItems;
    public bool cinematicPlayed; 
}

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject saveMessageUI;
    [SerializeField] private float messageDuration = 2f;

    [Header("Input")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Player & Inventory")]
    [SerializeField] private MonoBehaviour playerControllerScript;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private InventoryUI playerInventory;

    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "save.json";
    [SerializeField] LevelController cinematic;

    public static bool GameIsPaused { get; private set; }

    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (saveMessageUI != null) saveMessageUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;

        LockCursor(true);

        LoadGame(); // carga inicial
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;

        if (playerControllerScript != null) playerControllerScript.enabled = false;
        LockCursor(false);
    }

    public void Resume()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;

        if (playerControllerScript != null) playerControllerScript.enabled = true; // siempre reactivar

        LockCursor(true);
    }

    public void SaveGame()
    {
        if (playerTransform == null || playerInventory == null || cinematic == null)
        {
            Debug.LogWarning("PauseMenu: Player, Inventario o Cinematic no asignados.");
            return;
        }

        // Una vez que se guarda, la intro se considera vista para siempre
        cinematic.cinematicPlayed = true;

        SaveData data = new SaveData
        {
            playerPosX = playerTransform.position.x,
            playerPosY = playerTransform.position.y,
            playerPosZ = playerTransform.position.z,
            inventoryItems = playerInventory.GetInventoryIDs(),
            cinematicPlayed = cinematic.cinematicPlayed
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Juego guardado en: " + savePath);

        if (saveMessageUI != null)
        {
            saveMessageUI.SetActive(true);
            CancelInvoke(nameof(HideSaveMessage));
            Invoke(nameof(HideSaveMessage), messageDuration);
        }

        Resume();
    }

    private void HideSaveMessage()
    {
        if (saveMessageUI != null)
            saveMessageUI.SetActive(false);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No se encontró archivo de guardado, juego nuevo.");

            // Si no hay partida previa, inicializar cinemática desde cero
            if (cinematic != null)
                cinematic.InitCinematic();

            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Posición del jugador
        if (playerTransform != null)
            playerTransform.position = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);

        // Inventario
        if (playerInventory != null)
        {
            playerInventory.ClearInventory();
            foreach (var id in data.inventoryItems)
            {
                playerInventory.AddItemByID(id);
            }
        }

        // Destruir objetos en la escena que ya están en el inventario
        foreach (var id in data.inventoryItems)
        {
            GameObject[] objetosEnEscena = GameObject.FindGameObjectsWithTag(id);
            foreach (var obj in objetosEnEscena)
            {
                Destroy(obj);
            }
        }

        // Restaurar estado de la cinemática
        if (cinematic != null)
        {
            cinematic.cinematicPlayed = data.cinematicPlayed;
            cinematic.InitCinematic(); // aquí decidimos si mostrarla o saltarla
            Debug.Log("Cinemática jugada: " + cinematic.cinematicPlayed);
        }

        Debug.Log("Juego cargado desde: " + savePath);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}