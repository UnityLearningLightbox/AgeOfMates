using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string gameSceneName = "MainMenu"; // nombre de la escena de juego
    [SerializeField] private string saveFileName = "save.json";

    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
    }

    public void NewGame()
    {
        // Borra el archivo de guardado si quieres empezar desde cero
        if (File.Exists(savePath))
            File.Delete(savePath);

        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        if (File.Exists(savePath))
        {
            // hay partida guardada -> cargamos directamente la escena de juego
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.Log("No hay partida guardada. Iniciando juego nuevo...");
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}



