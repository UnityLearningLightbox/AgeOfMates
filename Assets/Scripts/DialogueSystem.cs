using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text dialogueText;

    [Header(".txt archives")]
    [SerializeField] TextAsset dialogueFile;

    [Header("Dialogues")]
    [SerializeField] string[] dialogueLines;
    [SerializeField] float typingSpeed;

    //[Header("Typing Sounds")]
    //[SerializeField] AudioClip typingSound;
    //[SerializeField] AudioSource typingAudioSource;
    //[SerializeField] float randomA;
    //[SerializeField] float randomB;

    private int currentIndex = 0;
    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;


    private void Start()
    {
        LoadDialogueFromArchive();
        InitialSettings();
    }

    private void Update()
    {
        DialogueManager();
    }

    private void OnTriggerEnter2D(Collider2D other) //Al entrar al collider (trigger en este caso)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            dialoguePanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) //al salir del collider (trigger en este caso)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialoguePanel.SetActive(false);
        }
    }

    void InitialSettings()
    {
        dialogueText.text = ""; //Restaura el texto a "vacio", lo limpia.
        dialoguePanel.SetActive(false);
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

        currentIndex = 0;
        dialogueText.text = dialogueLines[currentIndex];

        //Desactivar controlador personaje para evitar que se mueva durante el dialogo

        Time.timeScale = 0f;
    }

    void DisplayNextLine()
    {
        currentIndex++;
        if (currentIndex < dialogueLines.Length)
        {
            //dialogueText.text = dialogueLines[currentIndex];
            StartTypingText(dialogueLines[currentIndex]);
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

        Time.timeScale = 1f;
    }

    void LoadDialogueFromArchive()
    {
        //TextAsset textFile = Resources.Load<TextAsset>(txtFile);

        if (dialogueFile != null)
        {
            dialogueLines = dialogueFile.text.Split("\n");
            for (int i = 0; i < dialogueLines.Length; i++)
            {
                dialogueLines[i] = dialogueLines[i].Trim();
            }
        }
        else
        {
            dialogueLines = new string[] { "No hay archivo txt" };
        }

        dialoguePanel.SetActive(false);
    }

    void StartTypingText(string line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(LineCourutine(line));

    }

    IEnumerator LineCourutine(string line)
    {
        isTyping = true;
        dialogueText.text = "";//Restaura el texto a "vacio", lo limpia de caracteres

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;

            /*if (typingSound != null && typingAudioSource != null)
            {
                typingAudioSource.pitch = Random.Range(randomA, randomB + 0.1f);
                typingAudioSource.PlayOneShot(typingSound);
            }*/

            yield return new WaitForSecondsRealtime(typingSpeed);
            //yield return new WaitForSeconds(typingSpeed);
        }


        isTyping = false;

       // typingAudioSource.pitch = 1f;
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
}
