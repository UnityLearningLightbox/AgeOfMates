using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] Transform startPosition;
    [SerializeField] Transform playerPosition;

    [Header("Canvas")]
    [SerializeField] GameObject startCanvas;
    [SerializeField] GameObject compassCanvas;

    [Header("Player Controller")]
    [SerializeField] TademiusController playerController;

    private void Awake()
    {
        startCanvas.SetActive(true);
        compassCanvas.SetActive(false);
        playerController.enabled = false;

        playerPosition.position = startPosition.position;

        StartCoroutine(IntroScene());
    }

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    // Código a ejecutar después de cargar la escena
    //    Debug.Log("Nivel cargado: " + scene.name);

    //    playerController.enabled = false;
    //    StartCoroutine(IntroScene());

    //    // Aquí puedes acceder a objetos en la escena y realizar acciones como:
    //    // - Activar/desactivar objetos
    //    // - Configurar la posición inicial del jugador
    //    // - Cargar datos guardados
    //}

    IEnumerator IntroScene()
    {
        yield return new WaitForSeconds(3f);
        startCanvas.SetActive(true);

        yield return new WaitForSeconds(7f);
        startCanvas.SetActive(false);
        playerController.enabled = true;

        yield return new WaitForSeconds(2f);
        compassCanvas.SetActive(true);
    }
}
