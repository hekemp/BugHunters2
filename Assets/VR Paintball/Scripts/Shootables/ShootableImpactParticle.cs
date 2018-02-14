using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Shootable))]
public class ShootableImpactParticle : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Particle to play on impact")]
	private ParticleSystem impactParticlePrefab;
	
	private void Start()
	{
		GetComponent<Shootable>().OnTookShot += HandleTookShot;
	}

	private void HandleTookShot(RaycastHit hit)
	{
		var particleInstance = GameObject.Instantiate(impactParticlePrefab.gameObject, hit.point, Quaternion.identity);
		GameObject.Destroy(particleInstance, 2f);
	}
}