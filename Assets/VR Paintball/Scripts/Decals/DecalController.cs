using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This controller is responsible for placing decals in the world.
/// It handles all the paint splats by placing the decalPrefab that's associated.
/// Look at the decalPrefab for customization options.
/// </summary>
public class DecalController : MonoBehaviour
{
    #region Singleton
    public static DecalController Instance { get; private set; }

    private void InitializeSingleton()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple Decal controllers active, only one should exist.");
            return;
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    [SerializeField]
	[Tooltip("The prefab for the decal")]
	private Decal decalPrefab;

    [SerializeField]
	[Tooltip("The number of decals to keep alive at a time.  After this number are around, old ones will be replaced.")]
	private int maxConcurrentDecals = 10;

    [SerializeField]
    [Tooltip("Minimum distance a decal must be from other decals.  Any decals closer than this distance will be disabled.")]
    private float minimumDistanceBetweenDecals = 0.2f;

    [SerializeField]
    [Tooltip("Distance from the hit point to offset the decal")]
    private float placedPositionOffset = 0.002f;

    private Queue<GameObject> decalsInPool;
    private Queue<GameObject> decalsActiveInWorld;
    
    private void Awake()
	{
        InitializeSingleton();
        InitializeDecals();
		Shootable.OnAnyTookShot += (shootable, hit) => SpawnDecal(hit);
	}

	private void InitializeDecals()
	{
		decalsInPool = new Queue<GameObject>();
		decalsActiveInWorld = new Queue<GameObject>();

		for (int i = 0; i < maxConcurrentDecals; i++)
		{
			InstantiateDecal();
		}
	}

	private void InstantiateDecal()
	{
		var spawned = GameObject.Instantiate(decalPrefab);
		spawned.transform.SetParent(this.transform);

		decalsInPool.Enqueue(spawned.gameObject);
		spawned.gameObject.SetActive(false);
	}

	public void SpawnDecal(RaycastHit hit)
	{
		GameObject decal = GetNextAvailableDecal();
		if (decal != null)
		{
            var point = GetClosestPoint(hit);

			decal.transform.position = point + (hit.normal * placedPositionOffset);
			decal.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);

			decal.SetActive(true);

			decalsActiveInWorld.Enqueue(decal);
		}
	}

    private Vector3 GetClosestPoint(RaycastHit hit)
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(hit.point, -hit.normal);

        if (Physics.Raycast(ray, out hitInfo, 0.1f))
        {
            return hitInfo.point;
        }

        return hit.point;
    }

    internal void SpawnDecal(Vector3 point, Vector3 normal)
    {
        //Debug.LogError("Collision at point: " + point + " Normal: " + normal);
        GameObject decal = GetNextAvailableDecal();
        if (decal != null)
        {
            point = GetClosestPoint(point, normal);

            decal.transform.position = point + (normal * 0.01f);
            decal.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, normal);

            decal.SetActive(true);

            decalsActiveInWorld.Enqueue(decal);

            ReturnAnyDecalsTooCloseToPool(decal);
        }
    }

    private Vector3 GetClosestPoint(Vector3 point, Vector3 normal)
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(point, -normal);
        Debug.DrawRay(point, -normal, Color.green);

        if (Physics.Raycast(ray, out hitInfo, 0.1f))
        {
            return hitInfo.point;
        }

        return point;
    }

    private void ReturnAnyDecalsTooCloseToPool(GameObject decal)
    {
        var decalsTooClose = decalsActiveInWorld.Where(t => t != decal && Vector3.Distance(t.transform.position, decal.transform.position) < minimumDistanceBetweenDecals);
        foreach (var decalToDisable in decalsTooClose)
        {
            ReturnDecalToPool(decalToDisable);
        }
    }

    private void ReturnDecalToPool(GameObject decalToDisable)
    {
        decalsInPool.Enqueue(decalToDisable);
        decalToDisable.SetActive(false);
    }

    private GameObject GetNextAvailableDecal()
	{
		if (decalsInPool.Count > 0)
			return decalsInPool.Dequeue();

		var oldestActiveDecal = decalsActiveInWorld.Dequeue();
		return oldestActiveDecal;
	}

#if UNITY_EDITOR

	private void Update()
	{
		if (transform.childCount < maxConcurrentDecals)
			InstantiateDecal();
		else if (ShoudlRemoveDecal())
			DestroyExtraDecal();
	}

	private bool ShoudlRemoveDecal()
	{
		return transform.childCount > maxConcurrentDecals;
	}

	private void DestroyExtraDecal()
	{
		if (decalsInPool.Count > 0)
			Destroy(decalsInPool.Dequeue());
		else if (ShoudlRemoveDecal() && decalsActiveInWorld.Count > 0)
			Destroy(decalsActiveInWorld.Dequeue());
	}

#endif
}