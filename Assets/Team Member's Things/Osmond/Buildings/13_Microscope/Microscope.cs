public class Microscope : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.ResearchPointThreshold)
        {
            return -10f;
        }

        return 0f;
    }
}
