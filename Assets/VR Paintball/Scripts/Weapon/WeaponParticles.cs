using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponParticles : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The particle prefab for muzzle flashes")]
    private GameObject muzzleFlashParticlePrefab;

    [SerializeField]
    [Tooltip("A transform that determines the position and rotation to spawn our muzzle flash at(the particle is NOT a child)")]
    private Transform muzzleFlashTransform;

    private void Start()
    {
        GetComponent<Weapon>().OnWeaponFired += SpawnMuzzleFlash;
    }

    private void SpawnMuzzleFlash()
    {
        var particle = Instantiate(muzzleFlashParticlePrefab,
        muzzleFlashTransform.position, muzzleFlashTransform.rotation);
        Destroy(particle, 1f);
    }
}