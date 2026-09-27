using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;

    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private QuestObjectiveSlot[] objectiveSlots;
    [SerializeField] private QuestRewardSlot[] rewardSlots;

    private QuestSO questSO;

    [SerializeField] private QuestSO noAvailableQuestSO;
    [SerializeField] private QuestLogSlot[] questSlots;

    [SerializeField] private CanvasGroup questCanvas;

    [SerializeField] private CanvasGroup acceptCanvasGroup;
    [SerializeField] private CanvasGroup declineCanvasGroup;
    [SerializeField] private CanvasGroup completeCanvasGroup;


    private void OnEnable()
    {
        QuestEvents.OnQuestOfferRequested += ShowQuestOffer;
        QuestEvents.OnQuestTurnInRequested += ShowQuestTurnIn;
    }

    private void OnDisable()
    {
        QuestEvents.OnQuestOfferRequested -= ShowQuestOffer;
        QuestEvents.OnQuestTurnInRequested -= ShowQuestTurnIn;
    }




    public void ShowQuestOffer(QuestSO incomingQuestSO)
    {
        if (questManager.IsQuestAccepted(incomingQuestSO) || questManager.GetCompleteQuest(incomingQuestSO))
        {
            questSO = noAvailableQuestSO;
            SetCanvasState(acceptCanvasGroup, false);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, false);
        }
        else
        {
            questSO = incomingQuestSO;
            SetCanvasState(acceptCanvasGroup, true);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, false);
        }

        HandleQuestClicked(questSO);
        SetCanvasState(questCanvas, true);
    }



    public void ShowQuestTurnIn(QuestSO incomingQuestSO)
    {
        questSO = incomingQuestSO;

        HandleQuestClicked(questSO);

        SetCanvasState(completeCanvasGroup, true);
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, false);
        SetCanvasState(questCanvas, true);
    }




    public void OnAcceptQuestClicked()
    {
        QuestEvents.OnQuestAccepted?.Invoke(questSO);


        questManager.AcceptQuest(questSO);
        SetCanvasState(completeCanvasGroup, false);
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, false);
        RefreshQuestList();
        HandleQuestClicked(noAvailableQuestSO);
    }

    public void OnDeclineQuestClicked()
    {
        SetCanvasState(questCanvas, false);
    }

    public void OnCompleteQuestClicked()
    {
        questManager.CompleteQuest(questSO);

        RefreshQuestList();
        HandleQuestClicked(noAvailableQuestSO);
        SetCanvasState(completeCanvasGroup, false);
    }



    private void SetCanvasState(CanvasGroup group, bool activate)
    {
        group.alpha = activate ? 1 : 0;
        group.blocksRaycasts = activate;
        group.interactable = activate;
    }


    public void RefreshQuestList()
    {
        List<QuestSO> activeQuests = questManager.GetActiveQuests();

        for (int i = 0; i < questSlots.Length; i++)
        {
            if (i < activeQuests.Count)
            {
                questSlots[i].SetQuest(activeQuests[i]);
            }
            else
            {
                questSlots[i].ClearSlot();
            }
        }
    }


    public void HandleQuestClicked(QuestSO questSO)
    {
        this.questSO = questSO;

        questNameText.text = questSO.questName;
        questDescriptionText.text = questSO.questDescription;

        DisplayObjectives();
        DisplayRewards();
    }



    private void DisplayObjectives()
    {
        for (int i = 0; i < objectiveSlots.Length; i++)
        {
            if (i < questSO.objectives.Count)
            {
                var objective = questSO.objectives[i];
                questManager.UpdateObjectiveProgress(questSO, objective);


                int currentAmount = questManager.GetCurrentAmount(questSO, objective);
                string progress = questManager.GetProgressText(questSO, objective);
                bool isComplete = currentAmount >= objective.requiredAmount;

                objectiveSlots[i].gameObject.SetActive(true);
                objectiveSlots[i].RefreshObjectives(objective.description, progress, isComplete);
            }
            else
            {
                objectiveSlots[i].gameObject.SetActive(false);
            }
        }
    }


    private void DisplayRewards()
    {
        for (int i = 0; i < rewardSlots.Length; i++)
        {
            if (i < questSO.rewards.Count)
            {
                var reward = questSO.rewards[i];
                rewardSlots[i].DisplayReward(reward.itemSO.icon, reward.quantity);
                rewardSlots[i].gameObject.SetActive(true);
            }
            else
            {
                rewardSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
