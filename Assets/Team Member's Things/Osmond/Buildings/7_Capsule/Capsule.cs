public class Capsule : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.ResearchPointThreshold)
        {
            return -20f;
        }

        return 0f;
    }
}
