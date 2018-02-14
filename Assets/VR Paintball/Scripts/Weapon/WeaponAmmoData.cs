/// <summary>
/// Used to store and pass weapon ammo info between classes.
/// </summary>
public struct WeaponAmmoData
{
    public readonly int RemainingInClip;
    public readonly int RemainingOutOfClip;
    public WeaponAmmoData(int remainingInClip, int remainingOutOfClip)
    {
        RemainingInClip = remainingInClip;
        RemainingOutOfClip = remainingOutOfClip;
    }
}