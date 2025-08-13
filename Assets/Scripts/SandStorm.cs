using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SandStorm : MonoBehaviour
{
    [SerializeField] Transform teleportPosition; // El prefab SandStorm tiene una position para teletransportar al player
    [SerializeField] PlayerController playerController;
    //[SerializeField] TademiusController playerController;
    [SerializeField] Transform playerPosition;
    //[SerializeField] Volume boxVolume;

    private void Start()
    {
        InitalSettings();
    }

    private void Update()
    {
        
    }

    void InitalSettings()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("El player entra en el collider?");

        if (other.CompareTag("Player"))
        {
            playerController.playerSpeed /= 2;
            playerController.runningSpeed /= 2;

            //if (boxVolume != null)
            //{
            //    var vig = boxVolume.profile.TryGet(out Vignette vignette);
            //    Debug.Log("aaaaaaaAAAAAAAa " + vig);

            //    for (int i = 0; i < 100; i++)
            //    {
            //        vignette.intensity.value += (i / 100);
            //    }
            //}

            StartCoroutine(FaintingPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.playerSpeed *= 2;
            playerController.runningSpeed *= 2;
        }
    }

    IEnumerator FaintingPlayer()
    {
        Debug.Log("Antes de irse de la vida");
        yield return new WaitForSeconds(2f);
        Debug.Log("El player se desmaya.");
        Debug.Log("Animacion desmallandose?");

        playerPosition.position = teleportPosition.position;

    }
}
