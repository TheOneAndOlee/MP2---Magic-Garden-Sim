public class ArcadeMachine : StatBuilding
{
    public override float GetBaseModifier(SimResource.StatType stat)
    {
        if (stat == SimResource.StatType.Morale)
        {
            return 20f;
        }

        return 0f;
    }
}
