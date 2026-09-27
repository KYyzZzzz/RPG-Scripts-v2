using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using NUnit.Framework;

public class SkillSlot : MonoBehaviour
{
    public List<SkillSlot> prerequisiteSkillSlot;
    public SkillSO skillSO;

    public int currentLevel;
    public bool isUnlocked;

    public Image skillIcon;
    public Button skillButton;
    public TMP_Text skillLevelText;

    public static event Action<SkillSlot> OnAbilityPointSpent;
    public static event Action<SkillSlot> OnSkillMaxed;

    private void OnValidate()
    {
        if (skillSO != null && skillLevelText != null)
        {
            UpdateUI(); 
        }
    }



    public void TryUpgradeSkill()
    {
        if (isUnlocked && currentLevel < skillSO.maxLevel)
        {
            currentLevel++;

            UpdateUI();

            OnAbilityPointSpent?.Invoke(this);

            if(currentLevel >= skillSO.maxLevel)
            {
                OnSkillMaxed?.Invoke(this);
            }

        }
    }


    public bool CanUnlockSkill()
    {
        foreach (SkillSlot slot in prerequisiteSkillSlot)
        {
            if(!slot.isUnlocked || slot.currentLevel < slot.skillSO.maxLevel)
            {
                return false;
            }
        }
        return true;
    }


    public void Unlock()
    {
        isUnlocked = true;
        UpdateUI();
    }


    private void UpdateUI()
    {
        if (skillSO == null || skillIcon == null) return;

        skillIcon.sprite = skillSO.skillIcon;

        if (isUnlocked)
        {
            if (skillButton != null) skillButton.interactable = true;
            if (skillLevelText != null) skillLevelText.text = currentLevel.ToString() + "/" + skillSO.maxLevel.ToString();
            skillIcon.color = Color.white;
        }
        else
        {
            if (skillButton != null) skillButton.interactable = false;
            if (skillLevelText != null) skillLevelText.text = "Locked";
            skillIcon.color = Color.grey;
        }
    }
}
