public class Terminal_Building : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.ResearchOutput)
        {
            return 30f;
        }

        return 0f;
    }
}
