using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;

    private void Start()
    {
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        // Eliminar cualquier entrada de quest existente
        foreach(Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        // Crear entradas de quest 
        //// Pendiente de cambio
        foreach(var quest in QuestController.Instance.activateQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            //questNameText.text = quest.quest.name;
            questNameText.text = quest.quest.questName;

            foreach (var objective in quest.objectives)
            {
                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                objText.text = $"{objective.description} ({objective.currentAmount} /{objective.requiredAmount})"; // Conseguir los objetos (0/3)
            }
        }
    }
}
