using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[System.Serializable]
public class SaveData
{
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    // public List<string> inventoryItems; // <-- comentado para usar más adelante
}

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("Input")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Player & Inventory")]
    [SerializeField] private MonoBehaviour playerControllerScript;
    [SerializeField] private Transform playerTransform;
    // [SerializeField] private InventoryUI playerInventory; // <-- comentado

    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "save.json";

    private bool playerControllerWasEnabled = true;
    public static bool GameIsPaused { get; private set; }

    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);

        playerControllerWasEnabled = playerControllerScript != null ? playerControllerScript.enabled : true;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        LockCursor(true);

        LoadGame(); // Cargar datos al iniciar la escena
    }

    void Update()
    {
        if (PauseKeyPressed())
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    bool PauseKeyPressed()
    {
        // Legacy Input
        if (Input.GetKeyDown(pauseKey)) return true;

        // New Input System
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) return true;
#endif

        return false;
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

        if (playerControllerScript != null) playerControllerScript.enabled = playerControllerWasEnabled;
        LockCursor(true);
    }

    public void SaveGame()
    {
        if (playerTransform == null /*|| playerInventory == null*/)
        {
            Debug.LogWarning("PauseMenu: Player no asignado para guardar.");
            return;
        }

        SaveData data = new SaveData
        {
            playerPosX = playerTransform.position.x,
            playerPosY = playerTransform.position.y,
            playerPosZ = playerTransform.position.z,
            // inventoryItems = playerInventory.GetInventoryIDs() // <-- comentado
        };

        Debug.Log("Datos guardados");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Juego guardado en: " + savePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No se encontró archivo de guardado, se inicia juego desde cero.");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (playerTransform != null)
            playerTransform.position = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);

        /*
        if (playerInventory != null)
        {
            playerInventory.ClearInventory(); // Limpia antes de cargar
            foreach (var id in data.inventoryItems)
            {
                playerInventory.AddItemByID(id); // Añade cada item por su ID
            }
        }
        */

        Debug.Log("Juego cargado desde: " + savePath);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
