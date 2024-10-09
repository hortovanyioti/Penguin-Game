using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RaycastWeaponScript : Weapon
{
	[SerializeField] private TrailRenderer bulletTrail;
	[SerializeField] private LayerMask mask;
	[SerializeField] private float bulletSpeed = 100;

	private void Start()
	{
		base.Init();
	}

	protected override void SpawnBullet()
	{
		base.SpawnBullet();
		Vector3 direction = GetAimDirection();

		if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, mask))
		{
			TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
			trail.gameObject.GetComponent<RaycastBulletScript>().SetDamage(Damage);

			StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));

			timeSinceFire = Time.time;
		}
		else
		{
			TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);

			StartCoroutine(SpawnTrail(trail, bulletSpawnPoint.position + GetAimDirection() * 100, Vector3.zero, false));

			timeSinceFire = Time.time;
		}
	}

	protected IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
	{
		Vector3 startPosition = Trail.transform.position;
		float distance = Vector3.Distance(Trail.transform.position, HitPoint);
		float remainingDistance = distance;

		while (remainingDistance > 0)
		{
			Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

			remainingDistance -= bulletSpeed * Time.deltaTime;

			yield return null;
		}
		Trail.transform.position = HitPoint;
		if (MadeImpact)
		{
			Instantiate(impactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
		}

		Destroy(Trail.gameObject, Trail.time);
	}
}
