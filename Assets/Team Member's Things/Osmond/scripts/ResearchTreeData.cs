using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Research Tree", menuName = "Osmond/Research Tree")]
public class ResearchTreeData : ScriptableObject
{
    public enum MilitaryType { Bow, ShooterTower, MobileTurret, AirSupport, AirStrike }

    [Serializable]
    public class ResearchNode
    {
        public string id;
        public string title;
        [TextArea] public string description;
        [Min(0)] public int researchPointCost;
        public List<string> prerequisites = new List<string>();
        public MilitaryType militaryType;
        [Tooltip("Zero unlocks the weapon; 1 to 3 upgrade it.")]
        [Range(0, 3)] public int upgradeLevel;
    }

    public List<ResearchNode> nodes = new List<ResearchNode>();
}
