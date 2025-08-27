using System.Collections;
using TMPro;
using UnityEngine;

public class KhepriDialogues : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject promptPanel;

    [Header(".txt archive")]
    [SerializeField] private TextAsset dialogueFile;

    [Header("Dialogues")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Player Control")]
    [SerializeField] private MonoBehaviour playerControllerScript;

    private int currentIndex = 0;
    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    [SerializeField] bool automaticText = false;

    private void Start()
    {
        LoadDialogueFromArchive();
        InitialSettings();
    }

    private void Update()
    {
        //DialogueManager();

        if (automaticText == true)
        {
            AutoDialogueManager();
        }
        else
        {
            DialogueManager();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            promptPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            promptPanel.SetActive(false);
            EndDialogue();
        }
    }

    void InitialSettings()
    {
        dialogueText.text = "";
        dialoguePanel.SetActive(false);
        promptPanel.SetActive(false);
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    void DialogueManager()
    {
        
        if (isPlayerInRange)
        {
            if (!isDialogueActive)
            {
                StartDialogue();
            }
            else if (!isTyping && Input.GetKeyDown(KeyCode.E))
            {
                DisplayNextLine();
            }
            else
            {
                CompleteCurrentLine();
            }
        }
    }

    void AutoDialogueManager()
    {
        if (isPlayerInRange)
        {
            if (!isDialogueActive)
            {
                StartDialogue();
            }
            else if (!isTyping && Input.GetKeyDown(KeyCode.E)) // poniendo un "&& Input.GetKeyDown(KeyCode.E)" lo hace bien pero el jugador tiene que darle a la E al iniciar la partida
            {
                AutoNextLine();
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

        StartTypingText(dialogueLines[currentIndex]);

        if (playerControllerScript != null)
            playerControllerScript.enabled = false;
    }

    void DisplayNextLine()
    {
        currentIndex++;

        if (currentIndex < dialogueLines.Length)
        {
            StartTypingText(dialogueLines[currentIndex]);
        }
        else
        {
            EndDialogue();
        }
    }

    void AutoNextLine()
    {
        StartCoroutine(AutomaticText());
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;

        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true; // Reactiva movimiento
            //promptPanel.SetActive(true);
        }
        Destroy(gameObject);
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

        dialogueText.text = dialogueLines[currentIndex];

        isTyping = false;
    }

    IEnumerator AutomaticText()
    {
        while (currentIndex < dialogueLines.Length)
        {
            yield return new WaitForSeconds(4f);
            //DisplayNextLine();
            //currentIndex++;
            //StartTypingText(dialogueLines[currentIndex]);
            StartCoroutine(Waiting());
        }

        EndDialogue();
    }

    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(3f);
        currentIndex++;
        StartTypingText(dialogueLines[currentIndex]);
        yield return new WaitForSeconds(3f);

    }
}
