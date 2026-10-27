using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class ProgressionMenu : MonoBehaviour
{
    #region Variables

    [Serializable]
    public class ResearchButton
    {
        public string nodeId;
        public Button button;
        public TMP_Text label;
        public GameObject row;
        [NonSerialized] public ResearchTreeData.ResearchNode node;
        [NonSerialized] public ResearchTreeData.ResearchNode unlock;
    }

    [Serializable]
    public class AchievementButton
    {
        public int achievementIndex;
        public TMP_Text label;
        public GameObject trophy;
    }

    [Header("Scene Connection")]
    [SerializeField] private BuildingManager buildingManager;

    [Header("Menu References")]
    [SerializeField] private TMP_Text resourceText;
    [SerializeField] private TMP_Text goalText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private List<ResearchButton> researchButtons = new List<ResearchButton>();
    [SerializeField] private List<AchievementButton> achievementButtons = new List<AchievementButton>();

    [Header("Status Colors")]
    [SerializeField] private Color completedColor = new Color(0.12f, 0.38f, 0.24f);
    [SerializeField] private Color readyColor = new Color(0.12f, 0.32f, 0.46f);
    [SerializeField] private Color lockedColor = new Color(0.20f, 0.21f, 0.24f);
    [SerializeField] private Color earnedTextColor = new Color(1f, 0.83f, 0.35f);
    [SerializeField] private Color lockedTextColor = Color.white;

    private ResearchProgression progression;
    private float nextRefresh;

    #endregion

    #region Unity Methods

    private void Start()
    {
        if (progression == null && buildingManager != null)
        {
            Initialize(buildingManager);
        }
    }

    private void Update()
    {
        if (progression != null && Time.unscaledTime >= nextRefresh)
        {
            nextRefresh = Time.unscaledTime + 0.2f;
            Refresh();
        }
    }

    #endregion

    #region Menu Setup

    public void Initialize(BuildingManager manager)
    {
        buildingManager = manager;
        progression = manager.Inventory.Progression;

        // The prefab stores node IDs; the research asset supplies costs and prerequisites.
        foreach (ResearchButton item in researchButtons)
        {
            item.node = FindNode(item.nodeId);
            item.unlock = null;
            if (item.node == null)
            {
                item.button.interactable = false;
                item.label.text = "Research unavailable";
                continue;
            }

            foreach (ResearchTreeData.ResearchNode node in progression.Tree.nodes)
            {
                if (node.militaryType == item.node.militaryType && node.upgradeLevel == 0)
                {
                    item.unlock = node;
                    break;
                }
            }
        }

        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && transform.parent != null)
        {
            Canvas parentCanvas = transform.parent.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                canvas.worldCamera = parentCanvas.worldCamera;
            }
        }
        Refresh();
    }

    private ResearchTreeData.ResearchNode FindNode(string id)
    {
        if (progression.Tree != null)
        {
            foreach (ResearchTreeData.ResearchNode node in progression.Tree.nodes)
            {
                if (node.id == id)
                {
                    return node;
                }
            }
        }
        return null;
    }

    #endregion

    #region Menu Display

    private void Refresh()
    {
        resourceText.text = "Research Points: " + buildingManager.Inventory.GetAmount(SimResource.ResourceType.ResearchPoints).ToString("0")
            + "  |  Income: " + buildingManager.Inventory.ResearchPerSecond.ToString("0.0") + " RPP/s";
        goalText.text = progression.NextBuildingGoals();
        feedbackText.text = progression.CurrentMessage;

        foreach (ResearchButton item in researchButtons)
        {
            if (item.node == null)
            {
                continue;
            }

            bool revealed = item.unlock != null && progression.PrerequisitesComplete(item.unlock);
            item.row.SetActive(revealed);
            bool completed = progression.IsResearched(item.node.id);
            bool ready = progression.PrerequisitesComplete(item.node);
            string rank = item.node.upgradeLevel == 0 ? "Unlock" : "Rank " + item.node.upgradeLevel;
            item.label.text = completed ? rank + " complete" : rank + " - " + item.node.researchPointCost + " RP";
            item.button.interactable = !completed;
            item.button.image.color = completed ? completedColor : ready ? readyColor : lockedColor;
        }

        foreach (AchievementButton item in achievementButtons)
        {
            bool earned = progression.HasAchievement(item.achievementIndex);
            item.label.text = (earned ? "EARNED\n" : "LOCKED\n")
                + ResearchProgression.AchievementNames[item.achievementIndex];
            item.label.color = earned ? earnedTextColor : lockedTextColor;
            item.trophy.SetActive(earned);
        }
    }

    #endregion

    #region Button Actions

    // Buttons call these methods through their saved On Click events in the prefab.
    public void BuyResearch(string nodeId)
    {
        if (progression != null)
        {
            progression.TryResearch(nodeId);
            Refresh();
        }
    }

    public void ShowAchievement(int index)
    {
        if (progression != null && index >= 0 && index < ResearchProgression.AchievementDescriptions.Length)
        {
            progression.Notify(ResearchProgression.AchievementDescriptions[index], true);
            Refresh();
        }
    }

    public void CheckNextBuilding()
    {
        if (progression == null)
        {
            return;
        }

        foreach (Building_Data data in buildingManager.Inventory.AvailableBuildings)
        {
            if (data != null && !progression.IsBuildingUnlocked(data))
            {
                buildingManager.SelectBuilding(data);
                Refresh();
                return;
            }
        }
        progression.Notify("Every building is unlocked. Try earning the placement achievement!", true);
        Refresh();
    }

    #endregion
}
