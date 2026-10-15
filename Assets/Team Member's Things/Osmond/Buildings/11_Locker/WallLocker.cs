public class WallLocker : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.MaximumResearchPoints)
        {
            return 20f;
        }

        return 0f;
    }
}
