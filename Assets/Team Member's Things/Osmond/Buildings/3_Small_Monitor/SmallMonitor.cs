public class SmallMonitor : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.ResearchOutput)
        {
            return 10f;
        }

        return 0f;
    }
}
