public class DrinkDispenser : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.ResearchEfficiency
            || stat == SimResource.StatType.Morale)
        {
            return 10f;
        }

        return 0f;
    }
}
