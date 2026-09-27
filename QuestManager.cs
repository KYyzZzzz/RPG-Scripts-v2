using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();
    private List<QuestSO> completedQuests = new();


    private void OnEnable()
    {
        QuestEvents.IsQuestComplete += IsQuestComplete;
    }

    private void OnDisable()
    {
        QuestEvents.IsQuestComplete -= IsQuestComplete;
    }


    #region Quest Accept Logic

    public bool IsQuestAccepted(QuestSO questSO)
    {
        return questProgress.ContainsKey(questSO);
    }


    public List<QuestSO> GetActiveQuests()
    {
        return new List<QuestSO>(questProgress.Keys);
    }


    public void AcceptQuest(QuestSO questSO)
    {
        questProgress[questSO] = new Dictionary<QuestObjective, int>();

        foreach (var objective in questSO.objectives)
        {
            UpdateObjectiveProgress(questSO, objective);
        }
    }



    public bool IsQuestComplete(QuestSO questSO)
    {
        if (!questProgress.TryGetValue(questSO, out var progressDict))
            return false;

        foreach (var objective in questSO.objectives)
        {
            UpdateObjectiveProgress(questSO, objective);
        }

        foreach (var objective in questSO.objectives)
        {
            if (progressDict[objective] < objective.requiredAmount)
                return false;
        }

        return true;
    }

    #endregion

    public void CompleteQuest(QuestSO questSO)
    {
        questProgress.Remove(questSO);
        completedQuests.Add(questSO);

        foreach (var objective in questSO.objectives)
        {
            if (objective.targetItem != null && objective.requiredAmount > 0)
            {
                InventoryManager.Instance.RemoveItem(objective.targetItem, objective.requiredAmount);
            }
        }


        foreach (var reward in questSO.rewards)
        {
            InventoryManager.Instance.AddItem(reward.itemSO, reward.quantity);
        }
    }


    public bool GetCompleteQuest(QuestSO questSO)
    {
        return completedQuests.Contains(questSO);
    }




    public void UpdateObjectiveProgress(QuestSO questSO, QuestObjective objective)
    {
        if (!questProgress.ContainsKey(questSO))
            return;

        var progressDictionary = questProgress[questSO];
        int newAmount = 0;

        if (objective.targetItem != null)
            newAmount = InventoryManager.Instance.GetItemQuantity(objective.targetItem);

        else if (objective.targetLocation != null && GameManager.Instance.LocationHistoryTracker.HasVisited(objective.targetLocation))
            newAmount = objective.requiredAmount;

        else if (objective.targetNPC != null && GameManager.Instance.DialogueHistoryTracker.HasSpokenWith(objective.targetNPC))
            newAmount = objective.requiredAmount;


        progressDictionary[objective] = newAmount;
    }




    public string GetProgressText(QuestSO questSO, QuestObjective objective)
    {
        int currentAmount = GetCurrentAmount(questSO, objective);

        if (currentAmount >= objective.requiredAmount)
            return "Complete";

        else if (objective.targetItem != null)
            return $"{currentAmount}/{objective.requiredAmount}";

        else
            return "In Progress";
    }


    public int GetCurrentAmount(QuestSO questSO, QuestObjective objective)
    {
        if (questProgress.TryGetValue(questSO, out var objectiveDictionary))
            if (objectiveDictionary.TryGetValue(objective, out int amount))
                return amount;
        return 0;
    }
}
