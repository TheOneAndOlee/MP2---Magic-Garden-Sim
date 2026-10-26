using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    public enum WeaponType { Arrow, TowerTurret, MobileTurret, AirSupport, MissileStrike }
    public WeaponType type;
    
    // 0 = Base, 1 = Upgrade 1, 2 = Upgrade 2
    public int currentWeaponUpgradeLevel = 0; 

    private float[] towerDamage = { 60f, 70f, 80f };
    private float[] mobileDamage = { 30f, 40f, 50f };
    private float[] airDamage = { 80f, 90f, 100f };
    private float[] missileDamage = { 300f, 400f, 500f };

    public float GetCurrentDamage()
    {
        switch (type)
        {
            case WeaponType.Arrow: return 50f;
            case WeaponType.TowerTurret: return towerDamage[currentWeaponUpgradeLevel];
            case WeaponType.MobileTurret: return mobileDamage[currentWeaponUpgradeLevel];
            case WeaponType.AirSupport: return airDamage[currentWeaponUpgradeLevel];
            case WeaponType.MissileStrike: return missileDamage[currentWeaponUpgradeLevel];
            default: return 0f;
        }
    }
}