using System;
using UnityEngine;

public class Shootable : MonoBehaviour, ITakeShot
{
	public static event Action<Shootable, RaycastHit> OnAnyTookShot = (shootable, impactPoint) => { };

	public event Action<RaycastHit> OnTookShot = (impactPoint) => { };

	public void TakeShot(RaycastHit hit)
	{
		OnTookShot(hit);
		OnAnyTookShot(this, hit);
	}
}