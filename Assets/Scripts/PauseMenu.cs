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
    [SerializeField] private InventoryUI playerInventory; // ← si no está asignado, lo resolvemos automáticamente

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

        // Aseguramos refs críticas antes de cargar
        ResolveReferences();

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

        if (playerControllerScript != null) playerControllerScript.enabled = true;

        LockCursor(true);
    }

    public void SaveGame()
    {
        ResolveReferences();

        if (playerTransform == null || playerInventory == null || cinematic == null)
        {
            Debug.LogWarning("PauseMenu: Player, Inventario o Cinematic no asignados.");
            return;
        }

        // Una vez que se guarda, la intro se considera vista
        cinematic.cinematicPlayed = true;

        // Obtenemos IDs y deduplicamos por seguridad
        var ids = playerInventory.GetInventoryIDs() ?? new List<string>();
        var seen = new HashSet<string>();
        var uniqueIds = new List<string>();
        foreach (var id in ids)
        {
            if (!string.IsNullOrEmpty(id) && seen.Add(id))
                uniqueIds.Add(id);
        }

        SaveData data = new SaveData
        {
            playerPosX = playerTransform.position.x,
            playerPosY = playerTransform.position.y,
            playerPosZ = playerTransform.position.z,
            inventoryItems = uniqueIds,
            cinematicPlayed = cinematic.cinematicPlayed
        };

        Debug.Log("[DEBUG] Guardando inventario con " + uniqueIds.Count + " items: " + string.Join(", ", uniqueIds));

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"[Save] Juego guardado en: {savePath} | Items: {uniqueIds.Count} -> [{string.Join(", ", uniqueIds)}]");

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
        ResolveReferences();

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

        Debug.Log("[DEBUG] Cargado inventario: " + data.inventoryItems.Count + " items: " + string.Join(", ", data.inventoryItems));

        // Posición del jugador
        if (playerTransform != null)
            playerTransform.position = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);

        // Inventario
        if (playerInventory != null)
        {
            playerInventory.ClearInventory();

            // Garantizamos lista no nula
            if (data.inventoryItems == null)
                data.inventoryItems = new List<string>();

            // Aplicamos inventario
            foreach (var id in data.inventoryItems)
            {
                if (string.IsNullOrEmpty(id)) continue;
                playerInventory.AddItemByID(id);
            }

            // Eliminar de escena los objetos correspondientes
            foreach (var id in data.inventoryItems)
            {
                if (string.IsNullOrEmpty(id)) continue;

                try
                {
                    GameObject[] objetosEnEscena = GameObject.FindGameObjectsWithTag(id);
                    foreach (var obj in objetosEnEscena)
                        Destroy(obj);
                }
                catch (UnityException e)
                {
                    // Ocurre si el Tag no existe en el Tag Manager
                    Debug.LogWarning($"[Load] Tag '{id}' no existe en Tag Manager. No se pudo limpiar objetos de escena. {e.Message}");
                }
            }

            Debug.Log($"[Load] Inventario aplicado: {data.inventoryItems.Count} -> [{string.Join(", ", data.inventoryItems)}]");
        }

        // Cinemática
        if (cinematic != null)
        {
            cinematic.cinematicPlayed = data.cinematicPlayed;
            cinematic.InitCinematic();
            Debug.Log("Cinemática jugada (flag): " + cinematic.cinematicPlayed);
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

    /// <summary>
    /// Se asegura de que las referencias críticas estén resueltas.
    /// </summary>
    private void ResolveReferences()
    {
        // Player Transform (fallback por tag "Player" si no está asignado)
        if (playerTransform == null)
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null) playerTransform = playerGO.transform;
        }

        // InventoryUI: intentar detectar la instancia "correcta"
        if (playerInventory == null)
        {
            // Incluye inactivos por si el inventario está en un Canvas desactivado
            var all = GameObject.FindObjectsOfType<InventoryUI>(true);

            // 1) si hay una activa en jerarquía, priorízala
            foreach (var inv in all)
            {
                if (inv.isActiveAndEnabled)
                {
                    playerInventory = inv;
                    break;
                }
            }

            // 2) si no había activa, coge la primera que tenga ítems configurados
            if (playerInventory == null)
            {
                foreach (var inv in all)
                {
                    if (inv != null && inv.objetos != null && inv.objetos.Count > 0)
                    {
                        playerInventory = inv;
                        break;
                    }
                }
            }

            // 3) como último recurso, la primera que exista
            if (playerInventory == null && all.Length > 0)
                playerInventory = all[0];

            if (playerInventory == null)
                Debug.LogWarning("PauseMenu: No se encontró ningún InventoryUI en la escena.");
        }
    }
}


//________________________________________________________________________________

//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;
//using UnityEngine.SceneManagement;

//[System.Serializable]
//public class SaveData
//{
//    public float playerPosX;
//    public float playerPosY;
//    public float playerPosZ;
//    public List<string> inventoryItems;
//    public bool cinematicPlayed;
//}

//public class PauseMenu : MonoBehaviour
//{
//    [Header("UI")]
//    [SerializeField] private GameObject pauseMenuUI;
//    [SerializeField] private GameObject saveMessageUI;
//    [SerializeField] private float messageDuration = 2f;

//    [Header("Input")]
//    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

//    [Header("Player & Inventory")]
//    [SerializeField] private MonoBehaviour playerControllerScript;
//    [SerializeField] private Transform playerTransform;
//    [SerializeField] private InventoryUI playerInventory;

//    [Header("Save Settings")]
//    [SerializeField] private string saveFileName = "save.json";
//    [SerializeField] LevelController cinematic;

//    public static bool GameIsPaused { get; private set; }

//    private string savePath;

//    void Awake()
//    {
//        savePath = Path.Combine(Application.persistentDataPath, saveFileName);

//        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
//        if (saveMessageUI != null) saveMessageUI.SetActive(false);

//        Time.timeScale = 1f;
//        GameIsPaused = false;

//        LockCursor(true);

//        LoadGame(); // carga inicial
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(pauseKey))
//        {
//            if (GameIsPaused) Resume();
//            else Pause();
//        }
//    }

//    public void Pause()
//    {
//        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);

//        Time.timeScale = 0f;
//        GameIsPaused = true;

//        if (playerControllerScript != null) playerControllerScript.enabled = false;
//        LockCursor(false);
//    }

//    public void Resume()
//    {
//        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

//        Time.timeScale = 1f;
//        GameIsPaused = false;

//        if (playerControllerScript != null) playerControllerScript.enabled = true;

//        LockCursor(true);
//    }

//    public void SaveGame()
//    {
//        if (playerTransform == null || playerInventory == null || cinematic == null)
//        {
//            Debug.LogWarning("PauseMenu: Player, Inventario o Cinematic no asignados.");
//            return;
//        }

//        // Una vez que se guarda, la intro se considera vista
//        cinematic.cinematicPlayed = true;

//        SaveData data = new SaveData
//        {
//            playerPosX = playerTransform.position.x,
//            playerPosY = playerTransform.position.y,
//            playerPosZ = playerTransform.position.z,
//            inventoryItems = playerInventory.GetInventoryIDs(),
//            cinematicPlayed = cinematic.cinematicPlayed
//        };

//        string json = JsonUtility.ToJson(data, true);
//        File.WriteAllText(savePath, json);
//        Debug.Log("Juego guardado en: " + savePath);

//        if (saveMessageUI != null)
//        {
//            saveMessageUI.SetActive(true);
//            CancelInvoke(nameof(HideSaveMessage));
//            Invoke(nameof(HideSaveMessage), messageDuration);
//        }

//        Resume();
//    }

//    private void HideSaveMessage()
//    {
//        if (saveMessageUI != null)
//            saveMessageUI.SetActive(false);
//    }

//    public void LoadGame()
//    {
//        if (!File.Exists(savePath))
//        {
//            Debug.Log("No se encontró archivo de guardado, juego nuevo.");

//            if (cinematic != null)
//                cinematic.InitCinematic();

//            return;
//        }

//        string json = File.ReadAllText(savePath);
//        SaveData data = JsonUtility.FromJson<SaveData>(json);

//        // Posición del jugador
//        if (playerTransform != null)
//            playerTransform.position = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);

//        // Inventario
//        if (playerInventory != null)
//        {
//            playerInventory.ClearInventory();

//            foreach (var id in data.inventoryItems)
//            {
//                playerInventory.AddItemByID(id);

//                // 🔥 destruir en escena cualquier objeto con ese tag
//                GameObject[] objetosEnEscena = GameObject.FindGameObjectsWithTag(id);
//                foreach (var obj in objetosEnEscena)
//                {
//                    Destroy(obj);
//                }
//            }
//        }

//        // Cinemática
//        if (cinematic != null)
//        {
//            cinematic.cinematicPlayed = data.cinematicPlayed;
//            cinematic.InitCinematic();
//            Debug.Log("Cinemática jugada: " + cinematic.cinematicPlayed);
//        }

//        Debug.Log("Juego cargado desde: " + savePath);
//    }

//    public void QuitGame()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("MainMenu");
//    }

//    private void LockCursor(bool locked)
//    {
//        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
//        Cursor.visible = !locked;
//    }
//}
//___________________________________________________________________________________________________________________

//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;
//using UnityEngine.SceneManagement;

//[System.Serializable]
//public class SaveData
//{
//    public float playerPosX;
//    public float playerPosY;
//    public float playerPosZ;
//    public List<string> inventoryItems;
//    public bool cinematicPlayed; 
//}

//public class PauseMenu : MonoBehaviour
//{
//    [Header("UI")]
//    [SerializeField] private GameObject pauseMenuUI;
//    [SerializeField] private GameObject saveMessageUI;
//    [SerializeField] private float messageDuration = 2f;

//    [Header("Input")]
//    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

//    [Header("Player & Inventory")]
//    [SerializeField] private MonoBehaviour playerControllerScript;
//    [SerializeField] private Transform playerTransform;
//    [SerializeField] private InventoryUI playerInventory;

//    [Header("Save Settings")]
//    [SerializeField] private string saveFileName = "save.json";
//    [SerializeField] LevelController cinematic;

//    public static bool GameIsPaused { get; private set; }

//    private string savePath;

//    void Awake()
//    {
//        savePath = Path.Combine(Application.persistentDataPath, saveFileName);

//        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
//        if (saveMessageUI != null) saveMessageUI.SetActive(false);

//        Time.timeScale = 1f;
//        GameIsPaused = false;

//        LockCursor(true);

//        LoadGame(); // carga inicial
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(pauseKey))
//        {
//            if (GameIsPaused) Resume();
//            else Pause();
//        }
//    }

//    public void Pause()
//    {
//        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);

//        Time.timeScale = 0f;
//        GameIsPaused = true;

//        if (playerControllerScript != null) playerControllerScript.enabled = false;
//        LockCursor(false);
//    }

//    public void Resume()
//    {
//        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

//        Time.timeScale = 1f;
//        GameIsPaused = false;

//        if (playerControllerScript != null) playerControllerScript.enabled = true; // siempre reactivar

//        LockCursor(true);
//    }

//    public void SaveGame()
//    {
//        if (playerTransform == null || playerInventory == null || cinematic == null)
//        {
//            Debug.LogWarning("PauseMenu: Player, Inventario o Cinematic no asignados.");
//            return;
//        }

//        // Una vez que se guarda, la intro se considera vista para siempre
//        cinematic.cinematicPlayed = true;

//        SaveData data = new SaveData
//        {
//            playerPosX = playerTransform.position.x,
//            playerPosY = playerTransform.position.y,
//            playerPosZ = playerTransform.position.z,
//            inventoryItems = playerInventory.GetInventoryIDs(),
//            cinematicPlayed = cinematic.cinematicPlayed
//        };

//        string json = JsonUtility.ToJson(data, true);
//        File.WriteAllText(savePath, json);
//        Debug.Log("Juego guardado en: " + savePath);

//        if (saveMessageUI != null)
//        {
//            saveMessageUI.SetActive(true);
//            CancelInvoke(nameof(HideSaveMessage));
//            Invoke(nameof(HideSaveMessage), messageDuration);
//        }

//        Resume();
//    }

//    private void HideSaveMessage()
//    {
//        if (saveMessageUI != null)
//            saveMessageUI.SetActive(false);
//    }

//    public void LoadGame()
//    {
//        if (!File.Exists(savePath))
//        {
//            Debug.Log("No se encontró archivo de guardado, juego nuevo.");

//            // Si no hay partida previa, inicializar cinemática desde cero
//            if (cinematic != null)
//                cinematic.InitCinematic();

//            return;
//        }

//        string json = File.ReadAllText(savePath);
//        SaveData data = JsonUtility.FromJson<SaveData>(json);

//        // Posición del jugador
//        if (playerTransform != null)
//            playerTransform.position = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);

//        // Inventario
//        if (playerInventory != null)
//        {
//            playerInventory.ClearInventory();
//            foreach (var id in data.inventoryItems)
//            {
//                playerInventory.AddItemByID(id);
//            }
//        }

//        // Destruir objetos en la escena que ya están en el inventario
//        foreach (var id in data.inventoryItems)
//        {
//            GameObject[] objetosEnEscena = GameObject.FindGameObjectsWithTag(id);
//            foreach (var obj in objetosEnEscena)
//            {
//                Destroy(obj);
//            }
//        }

//        // Restaurar estado de la cinemática
//        if (cinematic != null)
//        {
//            cinematic.cinematicPlayed = data.cinematicPlayed;
//            cinematic.InitCinematic(); // aquí decidimos si mostrarla o saltarla
//            Debug.Log("Cinemática jugada: " + cinematic.cinematicPlayed);
//        }

//        Debug.Log("Juego cargado desde: " + savePath);
//    }

//    public void QuitGame()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("MainMenu");
//    }

//    private void LockCursor(bool locked)
//    {
//        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
//        Cursor.visible = !locked;
//    }
//}