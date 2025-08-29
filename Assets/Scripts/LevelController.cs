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

    [Header("Cinematic State")]
    public bool cinematicPlayed;

    private Coroutine cinematicRoutine;


    /*private void Awake()
    {
        if(cinematicPlayed == false)
        {
            startCanvas.SetActive(true);
            compassCanvas.SetActive(false);
            playerController.enabled = false;

            playerPosition.position = startPosition.position;
            
            StartCoroutine(IntroScene());
        }
                
    }*/

    private void Awake()
    {
        // Por defecto desactivamos todo
        if (startCanvas != null) startCanvas.SetActive(false);
        if (compassCanvas != null) compassCanvas.SetActive(false);
        if (playerController != null) playerController.enabled = false;
    }


    /*private void Start()
    {
        // Aquí ya PauseMenu puede haber cargado el estado
        if (!cinematicPlayed)
        {
            // Si nunca se vio la cinemática -> la reproducimos
            if (startCanvas != null) startCanvas.SetActive(true);
            if (compassCanvas != null) compassCanvas.SetActive(false);
            if (playerController != null) playerController.enabled = false;

            if (playerPosition != null && startPosition != null)
                playerPosition.position = startPosition.position;

            StartCoroutine(IntroScene());
        }
        else
        {
            // Si ya se vio → activamos todo directamente
            if (startCanvas != null) startCanvas.SetActive(false);
            if (compassCanvas != null) compassCanvas.SetActive(true);
            if (playerController != null) playerController.enabled = true;
        }
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
        //startCanvas.SetActive(true);
        if (startCanvas != null) startCanvas.SetActive(true);

        yield return new WaitForSeconds(7f);
        //startCanvas.SetActive(false);
        //playerController.enabled = true;
        if (startCanvas != null) startCanvas.SetActive(false);
        if (playerController != null) playerController.enabled = true;

        yield return new WaitForSeconds(2f);
        //compassCanvas.SetActive(true);
        if (compassCanvas != null) compassCanvas.SetActive(true);

        cinematicPlayed = true;
    }*/

    // <summary>
    /// Se llama desde PauseMenu después de LoadGame()
    /// </summary>
    public void InitCinematic()
    {
        if (!cinematicPlayed)
        {
            // Reproducir intro
            if (startCanvas != null) startCanvas.SetActive(true);
            if (compassCanvas != null) compassCanvas.SetActive(false);
            if (playerController != null) playerController.enabled = false;

            if (playerPosition != null && startPosition != null)
                playerPosition.position = startPosition.position;

            cinematicRoutine = StartCoroutine(IntroScene());
        }
        else
        {
            // Saltar intro, activar juego directamente
            if (startCanvas != null) startCanvas.SetActive(false);
            if (compassCanvas != null) compassCanvas.SetActive(true);
            if (playerController != null) playerController.enabled = true;
        }
    }

    IEnumerator IntroScene()
    {
        yield return new WaitForSeconds(3f);
        if (startCanvas != null) startCanvas.SetActive(true);

        yield return new WaitForSeconds(7f);
        if (startCanvas != null) startCanvas.SetActive(false);
        if (playerController != null) playerController.enabled = true;

        yield return new WaitForSeconds(2f);
        if (compassCanvas != null) compassCanvas.SetActive(true);

        cinematicPlayed = true; // marcar como vista
    }
}

