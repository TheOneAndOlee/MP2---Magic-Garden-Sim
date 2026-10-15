public class SmallCrate : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.MaximumResearchProgress)
        {
            return 10f;
        }

        return 0f;
    }
}
