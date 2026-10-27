using UnityEngine;
using System;
using System.Collections.Generic;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-500)]
[RequireComponent(typeof(PlayerInventory))]
public class ResearchProgression : MonoBehaviour
{
    #region Variables and their Getters & Setters

    [SerializeField] private ResearchTreeData researchTree;
    private PlayerInventory inventory;
    private HashSet<Building_Data> placedBuildings = new HashSet<Building_Data>();
    private HashSet<string> completedResearch = new HashSet<string>();
    private HashSet<int> earnedAchievements = new HashSet<int>();
    private Queue<string> messages = new Queue<string>();
    private float nextAchievementCheck;
    private float messageExpires;

    public event Action Changed;
    public event Action<int> AchievementEarned;
    public string CurrentMessage { get; private set; } = "Place a Retro Terminal to begin your building unlock path.";

    public ResearchTreeData Tree
    {
        get => researchTree;
    }

    public static readonly string[] AchievementNames =
    {
        "Oh yeah, it's big brain time", "For Science!", "Serial Hoarder", "German Efficiency",
        "Bob the Builder", "High Spirits", "The Future is now!", "This is what peak performance looks like"
    };

    public static readonly string[] AchievementDescriptions =
    {
        "Unlock every research node.", "Generate more than 100 RPP per second.",
        "Fill your current Research Point capacity.", "Reach more than 150% research efficiency.",
        "Place every building in the construction catalog at least once.", "Reach more than 300 morale.",
        "Have more Computer Panels than Retro Terminals and Small Monitors combined.",
        "Reduce the conversion threshold below 500 RPP."
    };

    #endregion

    #region Unity Methods

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        if (researchTree == null)
        {
            researchTree = Resources.Load<ResearchTreeData>("OsmondProgression/Research Tree");
        }
    }

    private void Update()
    {
        if (Time.unscaledTime >= nextAchievementCheck)
        {
            nextAchievementCheck = Time.unscaledTime + 0.25f;
            CheckAchievements();
        }

        if (Time.unscaledTime >= messageExpires && messages.Count > 0)
        {
            CurrentMessage = messages.Dequeue();
            messageExpires = Time.unscaledTime + 5f;
        }
    }

    #endregion

    #region Building Unlocks

    public bool HasPlaced(Building_Data data)
    {
        return data != null && placedBuildings.Contains(data);
    }

    public bool IsBuildingUnlocked(Building_Data data)
    {
        if (data == null)
        {
            return false;
        }

        foreach (Building_Data required in data.requiredBuildings)
        {
            if (!HasPlaced(required))
            {
                return false;
            }
        }

        return true;
    }

    public string BuildingRequirement(Building_Data data)
    {
        string requirement = "";
        foreach (Building_Data required in data.requiredBuildings)
        {
            if (required != null && !HasPlaced(required))
            {
                if (requirement.Length > 0)
                {
                    requirement += " and ";
                }
                requirement += required.buildingName;
            }
        }

        return requirement.Length > 0 ? "Place " + requirement + " at least once."
            : "This building is not in the current construction catalog.";
    }

    public void RecordPlacement(Building_Data data)
    {
        if (data == null || HasPlaced(data))
        {
            return;
        }

        List<Building_Data> previouslyLocked = new List<Building_Data>();
        foreach (Building_Data entry in inventory.AvailableBuildings)
        {
            if (entry != null && !IsBuildingUnlocked(entry))
            {
                previouslyLocked.Add(entry);
            }
        }

        placedBuildings.Add(data);
        foreach (Building_Data entry in previouslyLocked)
        {
            if (IsBuildingUnlocked(entry))
            {
                Notify("Unlocked " + entry.buildingName + "! " + entry.description);
            }
        }

        Changed?.Invoke();
        CheckAchievements();
    }

    public string NextBuildingGoals()
    {
        string goals = "";
        foreach (Building_Data data in inventory.AvailableBuildings)
        {
            if (data == null || IsBuildingUnlocked(data))
            {
                continue;
            }

            bool prerequisiteVisible = true;
            foreach (Building_Data required in data.requiredBuildings)
            {
                if (!IsBuildingUnlocked(required))
                {
                    prerequisiteVisible = false;
                }
            }

            if (prerequisiteVisible)
            {
                goals += BuildingRequirement(data) + " Unlocks " + data.buildingName + ".\n";
            }
        }

        if (goals.Length == 0)
        {
            return "All building types unlocked. Try combining neighboring buildings!";
        }
        return goals.TrimEnd();
    }

    #endregion

    #region Research

    public bool IsResearched(string id)
    {
        return !string.IsNullOrEmpty(id) && completedResearch.Contains(id);
    }

    public ResearchTreeData.ResearchNode FindResearch(string id)
    {
        if (researchTree != null)
        {
            foreach (ResearchTreeData.ResearchNode node in researchTree.nodes)
            {
                if (node.id == id)
                {
                    return node;
                }
            }
        }

        return null;
    }

    public bool PrerequisitesComplete(ResearchTreeData.ResearchNode node)
    {
        foreach (string required in node.prerequisites)
        {
            if (!IsResearched(required))
            {
                return false;
            }
        }
        return true;
    }

    public bool TryResearch(string id)
    {
        ResearchTreeData.ResearchNode node = FindResearch(id);
        if (node == null || IsResearched(id))
        {
            Notify("This research is already complete or unavailable.", true);
            return false;
        }

        foreach (string required in node.prerequisites)
        {
            if (!IsResearched(required))
            {
                ResearchTreeData.ResearchNode prerequisite = FindResearch(required);
                string title = prerequisite != null ? prerequisite.title : required;
                Notify("Prior research required: complete " + title + " first.", true);
                return false;
            }
        }

        if (!inventory.TrySpendResource(SimResource.ResourceType.ResearchPoints, node.researchPointCost))
        {
            Notify("Not enough Research Points. " + node.title + " needs " + node.researchPointCost
                + " RP. Use Convert Research to earn more.", true);
            return false;
        }

        completedResearch.Add(node.id);
        Notify("Research complete: " + node.title + ". " + node.description);
        Changed?.Invoke();
        CheckAchievements();
        return true;
    }

    #endregion

    #region Achievements and Feedback

    public bool HasAchievement(int index)
    {
        return earnedAchievements.Contains(index);
    }

    public void CheckAchievements()
    {
        bool allResearch = researchTree != null && researchTree.nodes.Count > 0;
        if (allResearch)
        {
            foreach (ResearchTreeData.ResearchNode node in researchTree.nodes)
            {
                allResearch &= IsResearched(node.id);
            }
        }

        bool allBuildings = inventory.AvailableBuildings.Count > 0;
        foreach (Building_Data data in inventory.AvailableBuildings)
        {
            allBuildings &= HasPlaced(data);
        }

        Award(0, allResearch);
        Award(1, inventory.ResearchPerSecond > 100f);
        Award(2, inventory.GetAmount(SimResource.ResourceType.ResearchPoints) >= inventory.MaximumResearchPoints);
        Award(3, inventory.ResearchEfficiency > 150f);
        Award(4, allBuildings);
        Award(5, inventory.Morale > 300f);
        Award(6, inventory.CountBuildings<ComputerPanel>() > inventory.CountBuildings<Terminal_Building>()
            + inventory.CountBuildings<SmallMonitor>());
        Award(7, inventory.ResearchPointThreshold < 500f);
    }

    private void Award(int index, bool condition)
    {
        if (condition && earnedAchievements.Add(index))
        {
            Notify("Achievement earned: " + AchievementNames[index] + "!");
            AchievementEarned?.Invoke(index);
            Changed?.Invoke();
        }
    }

    public void Notify(string message, bool immediate = false)
    {
        if (immediate || Time.unscaledTime >= messageExpires)
        {
            CurrentMessage = message;
            messageExpires = Time.unscaledTime + 5f;
        }
        else
        {
            messages.Enqueue(message);
        }
    }

    #endregion
}
