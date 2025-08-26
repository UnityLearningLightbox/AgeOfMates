using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }
    public List<QuestProgress> activateQuests = new();
    private QuestUI questUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        questUI = FindFirstObjectByType<QuestUI>();
    }

    public void AcceptQuest(Quest quest)
    {
        // Antes de añadir la quest a la lista comprobamos si ya existe
        if (IsQuestActive(quest.questID)) return;

        activateQuests.Add(new QuestProgress(quest));

        questUI.UpdateQuestUI();
    }

    public bool IsQuestActive(string questID) => activateQuests.Exists(q => q.QuestID == questID); // Si el ID ya existe en quests activas devolvemos true o false
}
