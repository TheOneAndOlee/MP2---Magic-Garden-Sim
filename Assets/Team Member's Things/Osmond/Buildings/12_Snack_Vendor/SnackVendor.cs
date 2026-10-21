public class SnackVendor : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.ResearchEfficiency)
        {
            return 20f;
        }

        return 0f;
    }
}
