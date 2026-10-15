public class Pizza_Building : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.ResearchFundsOutput)
        {
            return 100f;
        }

        return 0f;
    }
}
