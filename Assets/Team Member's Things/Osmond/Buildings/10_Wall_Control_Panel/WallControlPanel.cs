public class WallControlPanel : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.ResearchConversionFunds)
        {
            return -10f;
        }

        return 0f;
    }
}
