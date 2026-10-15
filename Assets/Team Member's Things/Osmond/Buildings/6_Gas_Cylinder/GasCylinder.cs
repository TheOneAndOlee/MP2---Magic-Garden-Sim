public class GasCylinder : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.MaximumResearchPoints)
        {
            return 10f;
        }

        return 0f;
    }
}
