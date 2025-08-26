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

    [Header("NPC or Object")]
    [SerializeField] bool isNPC; // Para saber si con quien hablamos es NPC o no
    //[SerializeField] bool itGuivesQuest; // Si el npc en question te da una Quest
    int rnd; // El numero aleatorio para los dialogos del NPC

    //[Header("NPC Quests Controller")]
    ////public bool[] givesQuest;
    //public int questInProgressIndex; // Lo que dira mientras la quest esta en progreso
    //public int questCompletedIndex; // Lo que dira cuando completes la quest
    //public Quest quest; // La quest que da el NPC

    //private enum QuestState {  NotStarted, InProgress, Completed }
    //private QuestState questState = QuestState.NotStarted;

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

        #region Intento de sistema de quests
        // Sincronizarlo con quest data
        //SyncQuestState();

        // Setear una linea de dialogo segun el questState
        //if (questState == QuestState.NotStarted)
        //{
        //    currentIndex = 0;

        //} else if(questState == QuestState.InProgress)
        //{
        //    currentIndex = questInProgressIndex;

        //} else if(questState == QuestState.Completed)
        //{
        //    currentIndex = questCompletedIndex;

        //} else
        //{
        //    currentIndex = 0;
        //}
        #endregion

        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        promptPanel.SetActive(false);

        currentIndex = 0;

        if (isNPC == true)
        {
            StartTypingText(dialogueLines[rnd]);
        }
        //else if (isNPC == true && itGuivesQuest == true)
        //{
        //    while(currentIndex < 2)
        //    {
        //        StartTypingText(dialogueLines[currentIndex]);
        //    }
        //}
        else
        {
            StartTypingText(dialogueLines[currentIndex]);
        }

        if (playerControllerScript != null)
            playerControllerScript.enabled = false; // Bloquea movimiento del jugador
    }

    // Intento de hacer un sistema de quest pero no funciona tal y como tenemos montado los dialogos
    //private void SyncQuestState()
    //{
    //    if (dialogueLines == null) return;
    //    if (quest == null) return;

    //    string questID = quest.questID;

    //    // Próxima actualización agregará completar misiones y entregarlas.
    //    if(questID != null)
    //    {
    //        Debug.Log("Quest Controller: " + QuestController.Instance);
    //        Debug.Log("Quest ID string: " + questID);
    //        Debug.Log("Quest ID quest.questID: " + quest.questID);
    //        Debug.Log("NPC Quest: " + npcQuest);
    //        Debug.Log("Quest Controller Instance: " + QuestController.Instance.IsQuestActive(npcQuest.questID));

    //        if (QuestController.Instance.IsQuestActive(npcQuest.questID))
    //        {
    //            questState = QuestState.InProgress;

    //        } else
    //        {
    //            questState = QuestState.NotStarted;
    //        }
    //    }
    //}

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

        //if(isNPC == true && itGuivesQuest == true)
        //{
        //    QuestController.Instance.AcceptQuest(quest);
        //    questState = QuestState.InProgress;
        //    playerControllerScript.enabled = true; // Reactiva movimiento
        //    promptPanel.SetActive(true);
        //}

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

