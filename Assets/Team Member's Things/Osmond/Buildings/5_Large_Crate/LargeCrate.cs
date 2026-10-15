public class LargeCrate : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.MaximumResearchProgress)
        {
            return 20f;
        }

        return 0f;
    }
}
