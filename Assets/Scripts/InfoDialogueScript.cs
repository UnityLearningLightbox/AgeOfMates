using TMPro;
using UnityEngine;

public class InfoDialogueScript : MonoBehaviour
{
    [Header("Object or NPC Settings")]
    [SerializeField] GameObject objetOrNPC; // El objeto o el NPC con el que interactuar
    [SerializeField] bool isNPC; // En caso de ser true, el array se recorrerá de manera aleatoria
    [SerializeField] string[] textDialogueInfo; // El array de los texto a mostrar

    [Header("Canvas Settings")]
    [SerializeField] TMP_Text canvasText;
    [SerializeField] Canvas pressECanvas;
    [SerializeField] Canvas infoCanvas;

    int cnt = 0;
    bool playerInside;

    private void Start()
    {
        InitialSettings();
    }

    private void Update()
    {
        ReadText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("El player entra");
            pressECanvas.enabled = true;
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pressECanvas.enabled = false;
            infoCanvas.enabled = false;
            playerInside = false;
        }
    }

    void InitialSettings()
    {
        pressECanvas.enabled = false;
        infoCanvas.enabled = false;
    }

    void ReadText()
    {
        int rnd = Random.Range(0, textDialogueInfo.Length);

        if (Input.GetKeyDown(KeyCode.E) && playerInside == true)
        {
            pressECanvas.enabled = false;
            infoCanvas.enabled = true;

            if (isNPC == true)
            {
                for (int i = 0; i < textDialogueInfo.Length; i++)
                {
                    canvasText.text = textDialogueInfo[rnd];
                }
            }
            else
            {
                if(cnt < textDialogueInfo.Length && Input.GetKeyDown(KeyCode.E))
                {
                    canvasText.text = textDialogueInfo[cnt];
                    cnt++;
                }
                else
                {
                    cnt = 0;
                    infoCanvas.enabled = false;
                    pressECanvas.enabled = true;
                }
            }
        }
    }
}
