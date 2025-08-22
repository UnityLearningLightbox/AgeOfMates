using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private GameObject dialoguePanel;    // Panel de diálogos (Canvas)
    [SerializeField] private TMP_Text dialogueText;       // Texto del diálogo
    [SerializeField] private GameObject promptPanel;      // Panel "Pulsa E para hablar"

    [Header(".txt archive")]
    [SerializeField] private TextAsset dialogueFile;      // Archivo .txt opcional

    [Header("Dialogues")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Player Control")]
    [SerializeField] private MonoBehaviour playerControllerScript;
    // Script de movimiento del Player (ej: FirstPersonController)

    private int currentIndex = 0;
    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    [SerializeField] bool isNPC; // Para saber si con quien hablamos es NPC o no
    int rnd; // El numero aleatorio para los dialogos del NPC

    private void Start()
    {
        LoadDialogueFromArchive();
        InitialSettings();
    }

    private void Update()
    {
        DialogueManager();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            promptPanel.SetActive(true); // Muestra "Pulsa E para hablar"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            promptPanel.SetActive(false); // Oculta el prompt
            EndDialogue();
        }
    }

    void InitialSettings()
    {
        dialogueText.text = "";
        dialoguePanel.SetActive(false);
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    void DialogueManager()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isDialogueActive)
            {
                StartDialogue();
            }
            else if (!isTyping)
            {
                DisplayNextLine();
            }
            else
            {
                CompleteCurrentLine();
            }
        }
    }

    void StartDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        promptPanel.SetActive(false);

        currentIndex = 0;

        if (isNPC == true)
        {
            StartTypingText(dialogueLines[rnd]);
        }
        else
        {
            StartTypingText(dialogueLines[currentIndex]);
        }

        if (playerControllerScript != null)
            playerControllerScript.enabled = false; // Bloquea movimiento del jugador
    }

    void DisplayNextLine()
    {
        rnd = Random.Range(0, dialogueLines.Length);

        currentIndex++;
        if (currentIndex < dialogueLines.Length)
        {
            if(isNPC == true)
            {
                StartTypingText(dialogueLines[rnd]);
                EndDialogue();

            } else
            {
                StartTypingText(dialogueLines[currentIndex]);
            }
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;

        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true; // Reactiva movimiento
            promptPanel.SetActive(true);
        }
    }

    void LoadDialogueFromArchive()
    {
        if (dialogueFile != null)
        {
            dialogueLines = dialogueFile.text.Split("\n");
            for (int i = 0; i < dialogueLines.Length; i++)
            {
                dialogueLines[i] = dialogueLines[i].Trim();
            }
        }
        else if (dialogueLines == null || dialogueLines.Length == 0)
        {
            dialogueLines = new string[] { "No hay archivo txt" };
        }
    }

    void StartTypingText(string line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(LineCoroutine(line));
    }

    IEnumerator LineCoroutine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void CompleteCurrentLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (isNPC == true)
        {
            dialogueText.text = dialogueLines[rnd];
            //EndDialogue();
        }
        else
        {
            dialogueText.text = dialogueLines[currentIndex];
        }

        isTyping = false;
    }
}

