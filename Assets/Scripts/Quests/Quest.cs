using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;
    public List<QuestObjetive> objectives;

    // Se le llama cuando el objeto scriptable (el que se crea en Unity) es editado
    private void OnValidate()
    {
        if(string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }
}

[System.Serializable]
public class QuestObjetive
{
    public string objectiveID; // Sera el id del objeto en cuestion, el id del enemigo, etc
    public string description;
    public ObjectiveType type;
    public int requiredAmount;
    public int currentAmount;

    public bool IsCompleted => currentAmount >= requiredAmount; // Si tenemos mas o igual a la cantidad requerida, devuelve true y se lo asinga a isCompleted
}

public enum ObjectiveType { CollectItem, DefeatEnemy, ReachLocation, TalkNPC, Custom }

[System.Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjetive> objectives;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjetive>();

        // Para no cambiar el valor original de los objetivos
        foreach (var obj in quest.objectives)
        {
            objectives.Add(new QuestObjetive
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            });
        }
    }

    public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted); // si todos los objetivos estan completos, la quest se completa

    public string QuestID => quest.questID;
}