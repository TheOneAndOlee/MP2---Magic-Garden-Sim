using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.UI;
using TMPro;
using System.Collections.Generic;

public class ProgressionMenu : MonoBehaviour
{
    private class ResearchButton
    {
        public ResearchTreeData.ResearchNode node;
        public Button button;
        public TMP_Text label;
        public GameObject row;
    }

    private BuildingManager manager;
    private ResearchProgression progression;
    private Canvas panel;
    private TMP_FontAsset font;
    private TMP_Text resourceText;
    private TMP_Text goalText;
    private TMP_Text feedbackText;
    private List<ResearchButton> researchButtons = new List<ResearchButton>();
    private List<TMP_Text> achievementLabels = new List<TMP_Text>();
    private Transform trophyShelf;
    private float nextRefresh;

    public void Initialize(BuildingManager buildingManager, Canvas sourceCanvas, TMP_Text sourceText)
    {
        manager = buildingManager;
        progression = manager.Inventory.Progression;
        font = sourceText != null ? sourceText.font : TMP_Settings.defaultFontAsset;

        GameObject panelObject = new GameObject("Research and Achievements", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TrackedDeviceGraphicRaycaster));
        panelObject.transform.SetParent(sourceCanvas.transform, false);
        panel = panelObject.GetComponent<Canvas>();
        panel.renderMode = RenderMode.WorldSpace;
        panel.worldCamera = sourceCanvas.worldCamera;
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.sizeDelta = new Vector2(1200f, 1180f);
        panelRect.anchoredPosition = new Vector2(670f, 540f);
        panelRect.localScale = Vector3.one;

        MakePanel(panelRect, "Background", 0, 0, 1200, 1180, new Color(0.035f, 0.065f, 0.09f, 0.98f));
        MakeText(panelRect, "Research lab", 28, 18, 1140, 62, 42);
        resourceText = MakeText(panelRect, "", 28, 85, 1140, 52, 26);
        goalText = MakeText(panelRect, "", 28, 140, 880, 132, 24);
        MakeButton(panelRect, "Check next\nbuilding unlock", 930, 150, 240, 90, CheckNextBuilding);
        MakeText(panelRect, "RESEARCH PATH   /   Complete research to reveal the next branch", 28, 284, 1140, 42, 24);

        if (progression.Tree != null)
        {
            for (int family = 0; family < 5; family++)
            {
                int rowY = 334 + family * 82;
                RectTransform row = MakePanel(panelRect, "Research row " + family, 24, rowY, 1152, 74, new Color(0.08f, 0.13f, 0.17f));
                foreach (ResearchTreeData.ResearchNode node in progression.Tree.nodes)
                {
                    if ((int)node.militaryType != family)
                    {
                        continue;
                    }
                    if (node.upgradeLevel == 0)
                    {
                        MakeText(row, node.title, 12, 10, 265, 58, 26);
                    }
                    ResearchTreeData.ResearchNode clickedNode = node;
                    Button button = MakeButton(row, "", 290 + node.upgradeLevel * 211, 8, 202, 58,
                        () => progression.TryResearch(clickedNode.id));
                    researchButtons.Add(new ResearchButton
                    {
                        node = node, button = button, label = button.GetComponentInChildren<TMP_Text>(), row = row.gameObject
                    });
                }
            }
        }

        MakeText(panelRect, "ACHIEVEMENTS", 28, 756, 1140, 42, 28);
        for (int i = 0; i < ResearchProgression.AchievementNames.Length; i++)
        {
            int index = i;
            Button button = MakeButton(panelRect, "", 24 + (i % 4) * 290, 806 + (i / 4) * 84, 282, 76,
                () => progression.Notify(ResearchProgression.AchievementDescriptions[index], true));
            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            label.fontSize = 21;
            label.enableAutoSizing = true;
            label.fontSizeMin = 16;
            label.fontSizeMax = 21;
            achievementLabels.Add(label);
        }

        MakePanel(panelRect, "Feedback popup", 24, 990, 1152, 164, new Color(0.12f, 0.24f, 0.29f));
        feedbackText = MakeText(panelRect, "", 44, 1005, 1110, 135, 27);
        RectTransform shelf = new GameObject("Achievement Trophies", typeof(RectTransform)).GetComponent<RectTransform>();
        trophyShelf = shelf;
        trophyShelf.SetParent(panelRect, false);
        shelf.anchorMin = shelf.anchorMax = new Vector2(0, 1);
        shelf.pivot = new Vector2(0, 1);
        shelf.sizeDelta = Vector2.zero;
        shelf.anchoredPosition = Vector2.zero;
        progression.AchievementEarned += CreateTrophy;
        for (int i = 0; i < 8; i++)
        {
            if (progression.HasAchievement(i))
            {
                CreateTrophy(i);
            }
        }
        Refresh();
    }

    private void Update()
    {
        if (progression != null && Time.unscaledTime >= nextRefresh)
        {
            nextRefresh = Time.unscaledTime + 0.2f;
            Refresh();
        }
    }

    private void Refresh()
    {
        resourceText.text = "Research Points: " + manager.Inventory.GetAmount(SimResource.ResourceType.ResearchPoints).ToString("0")
            + "  |  Income: " + manager.Inventory.ResearchPerSecond.ToString("0.0") + " RPP/s";
        goalText.text = progression.NextBuildingGoals();
        feedbackText.text = progression.CurrentMessage;
        foreach (ResearchButton item in researchButtons)
        {
            ResearchTreeData.ResearchNode unlock = null;
            foreach (ResearchTreeData.ResearchNode node in progression.Tree.nodes)
            {
                if (node.militaryType == item.node.militaryType && node.upgradeLevel == 0)
                {
                    unlock = node;
                    break;
                }
            }
            bool revealed = unlock != null && progression.PrerequisitesComplete(unlock);
            item.row.SetActive(revealed);
            bool completed = progression.IsResearched(item.node.id);
            bool ready = progression.PrerequisitesComplete(item.node);
            string rank = item.node.upgradeLevel == 0 ? "Unlock" : "Rank " + item.node.upgradeLevel;
            item.label.text = completed ? rank + " complete" : rank + " - " + item.node.researchPointCost + " RP";
            item.button.interactable = !completed;
            item.button.image.color = completed ? new Color(0.12f, 0.38f, 0.24f)
                : ready ? new Color(0.12f, 0.32f, 0.46f) : new Color(0.20f, 0.21f, 0.24f);
        }
        for (int i = 0; i < achievementLabels.Count; i++)
        {
            achievementLabels[i].text = (progression.HasAchievement(i) ? "EARNED\n" : "LOCKED\n")
                + ResearchProgression.AchievementNames[i];
            achievementLabels[i].color = progression.HasAchievement(i) ? new Color(1f, 0.83f, 0.35f) : Color.white;
        }
    }

    private void CheckNextBuilding()
    {
        foreach (Building_Data data in manager.Inventory.AvailableBuildings)
        {
            if (data != null && !progression.IsBuildingUnlocked(data))
            {
                manager.SelectBuilding(data);
                return;
            }
        }
        progression.Notify("Every building is unlocked. Try earning the placement achievement!", true);
    }

    private void CreateTrophy(int index)
    {
        // One physical gold trophy per earned achievement, displayed above the research board.
        GameObject trophy = new GameObject("Trophy - " + ResearchProgression.AchievementNames[index]);
        trophy.transform.SetParent(trophyShelf, false);
        trophy.transform.localPosition = new Vector3(90 + index * 145, 38, -50);
        MakeTrophyPart(trophy.transform, PrimitiveType.Cylinder, "Base", 0, new Vector3(66, 8, 66));
        MakeTrophyPart(trophy.transform, PrimitiveType.Cylinder, "Stem", 33, new Vector3(17, 25, 17));
        MakeTrophyPart(trophy.transform, PrimitiveType.Sphere, "Cup", 74, new Vector3(56, 45, 56));
        TMP_Text label = MakeText(trophyShelf, (index + 1).ToString(), index * 145 + 45, -165, 90, 45, 32);
        label.alignment = TextAlignmentOptions.Center;
    }

    private void MakeTrophyPart(Transform parent, PrimitiveType shape, string partName, float height, Vector3 scale)
    {
        GameObject part = GameObject.CreatePrimitive(shape);
        part.name = partName;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = new Vector3(0, height, 0);
        part.transform.localScale = scale;
        part.GetComponent<Collider>().enabled = false;
        Shader shader = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null
            ? Shader.Find("Universal Render Pipeline/Lit") : Shader.Find("Standard");
        Material gold = new Material(shader);
        gold.color = new Color(1f, 0.73f, 0.12f);
        if (gold.HasProperty("_BaseColor"))
        {
            gold.SetColor("_BaseColor", new Color(1f, 0.73f, 0.12f));
        }
        part.GetComponent<Renderer>().sharedMaterial = gold;
    }

    private RectTransform MakePanel(Transform parent, string objectName, float x, float y, float width, float height, Color color)
    {
        GameObject child = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        RectTransform rect = child.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
        child.GetComponent<Image>().color = color;
        return rect;
    }

    private TMP_Text MakeText(Transform parent, string text, float x, float y, float width, float height, float size)
    {
        GameObject child = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        RectTransform rect = child.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
        TMP_Text label = child.GetComponent<TMP_Text>();
        label.font = font;
        label.fontSize = size;
        label.color = Color.white;
        label.text = text;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;
        return label;
    }

    private Button MakeButton(Transform parent, string label, float x, float y, float width, float height, UnityAction action)
    {
        RectTransform rect = MakePanel(parent, label, x, y, width, height, new Color(0.12f, 0.22f, 0.30f));
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = rect.GetComponent<Image>();
        button.onClick.AddListener(action);
        TMP_Text text = MakeText(rect, label, 8, 4, width - 16, height - 8, 24);
        text.alignment = TextAlignmentOptions.Center;
        return button;
    }

    private void OnDestroy()
    {
        if (progression != null)
        {
            progression.AchievementEarned -= CreateTrophy;
        }
        if (panel != null)
        {
            foreach (Renderer trophy in panel.GetComponentsInChildren<Renderer>())
            {
                Destroy(trophy.sharedMaterial);
            }
            Destroy(panel.gameObject);
        }
    }
}
