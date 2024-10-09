using UnityEngine;
using UnityEngine.InputSystem;

public class BallisticWeapon : Weapon
{
	[SerializeField] private float bulletSize = 0.1f;
	public float BulletSize { get => bulletSize; private set => bulletSize = value; }


	[SerializeField] private float bulletForce = 30f;
	public float BulletForce { get => bulletForce; private set => bulletForce = value; }


	[SerializeField] protected GameObject projectile; //Add bullet prefab in editor

	void Start()
	{
		base.Init();
	}


	protected override void SpawnBullet()
	{
		base.SpawnBullet();

		var newProjectile = Instantiate(projectile);
		//TODO: toggle comments off below
		//newProjectile.transform.parent = GameManagerScript.Instance.BulletPool.transform; 

		newProjectile.transform.position = bulletSpawnPoint.position;//Spawn the projectile in front of the player

		newProjectile.transform.rotation = transform.rotation;  //Set the rotation of the projectile to the rotation of the player
		newProjectile.transform.Rotate(90f, 0f, 0f);

		newProjectile.transform.localScale *= bulletSize;
		newProjectile.GetComponent<Rigidbody>().mass = Mathf.Pow(bulletSize, 3);
		newProjectile.GetComponent<Rigidbody>().AddForce(GetAimDirection() * BulletForce, ForceMode.Impulse);   //Add force to the projectile
	}
}
