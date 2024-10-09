using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Weapon : MonoBehaviour
{
	[SerializeField] private float fireRate = 600f;
	public float FireRate { get => fireRate; protected set => fireRate = value; } //RPM


	[SerializeField] private float damage;
	public float Damage { get => damage; protected set => damage = value; }


	[SerializeField] private int currentMagazine;
	public int CurrentMagazine
	{
		get => currentMagazine;
		protected set
		{
			if (value < 0)
			{
				currentMagazine = 0;
			}
			else
			{
				currentMagazine = value;
			}
		}
	}


	[SerializeField] private int maxMagazine;
	public int MaxMagazine
	{
		get => maxMagazine;
		protected set
		{
			if (value < 0)
			{
				maxMagazine = 0;
			}
			else
			{
				maxMagazine = value;
			}
		}
	}


	[SerializeField] private int currentAmmo;
	public int CurrentAmmo
	{
		get => currentAmmo;
		protected set
		{
			if (value < 0)
			{
				currentAmmo = 0;
			}
			else
			{
				currentAmmo = value;
			}
		}
	}


	[SerializeField] private int maxAmmo;
	public int MaxAmmo
	{
		get => maxAmmo;
		protected set
		{
			if (value < 0)
			{
				maxAmmo = 0;
			}
			else
			{
				maxAmmo = value;
			}
		}
	}


	[SerializeField] private float bulletSpreadVariance = 0.1f;
	public float BulletSpreadVariance
	{
		get => bulletSpreadVariance;
		protected set
		{
			if (value < 0)
			{
				bulletSpreadVariance = 0;
			}
			else
			{
				bulletSpreadVariance = value;
			}
		}
	}


	[SerializeField] private bool addBulletSpread = true;
	public bool AddBulletSpread { get => addBulletSpread; protected set => addBulletSpread = value; }


	[SerializeField] private bool automatic = true;
	public bool Automatic { get => automatic; protected set => automatic = value; }


	[SerializeField] private bool isPrimaryFire = false;
	public bool IsPrimaryFire { get => isPrimaryFire; protected set => isPrimaryFire = value; }


	[SerializeField] private bool isReloading = false;
	public bool IsReloading { get => isReloading; protected set => isReloading = value; }


	[SerializeField] protected ParticleSystem shootingSystem;
	[SerializeField] protected ParticleSystem impactParticleSystem;
	[SerializeField] protected Transform bulletSpawnPoint;	
	[SerializeField] protected AudioSource fireSound;
	[SerializeField] protected AudioSource emptyMagazineSound;
	[SerializeField] protected AudioSource refillSound;
	public Transform magazine;
	public Transform rightHandle;

	protected GameObject Owner;
	protected GameCharacter OwnerScript;
	protected Camera OwnerCamera;
	protected float timeSinceFire = 99f;
	protected Animator animator;

	protected void Init()
	{
		Owner = this.transform.parent.parent.gameObject;
		OwnerScript = Owner.GetComponent<PlayerScript>();
		OwnerCamera = Owner.GetComponentInChildren<Camera>();
		fireSound = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();

		CurrentMagazine = MaxMagazine;
		CurrentAmmo = MaxAmmo;

	}
	protected virtual void Update()
	{
		timeSinceFire += Time.deltaTime;    //Increment the time since the last shot
		if (IsPrimaryFire)
		{
			Fire();
		}

		if (CurrentMagazine == 0)
		{
			StartReload();
		}
	}

	public virtual void OnFire(InputAction.CallbackContext context) //Implements HOLD functionality
	{
		if (context.started)
		{
			isPrimaryFire = true;
		}
		else if (context.canceled)
		{
			isPrimaryFire = false;
		}
	}

	public void Fire()
	{
		if (Time.timeScale == 0 || timeSinceFire < (1 / (FireRate / 60)))    //Restrict gun to fire rate
		{
			return;
		}

		if (currentMagazine == 0)
		{
			//TODO
			//emptyMagazineSound.Play();
			return;
		}

		if (!Automatic)     //If weapon is not automatic, fire once
		{
			isPrimaryFire = false;
		}

		SpawnBullet();
		CurrentMagazine--;
		CurrentAmmo--;
		timeSinceFire = 0;

		//TODO
		//fireSound.Play();
	}

	protected virtual void SpawnBullet()
	{
		//TODO
		//shootingSystem.Play();
	}

	protected Vector3 ShootRay()
	{

		Vector3 point = new Vector3(OwnerCamera.pixelWidth / 2f, OwnerCamera.pixelHeight / 2f, 0f);
		Ray ray = OwnerCamera.ScreenPointToRay(point);

		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			return hit.point;
		}
		else
		{
			return Vector3.zero;
		}
	}

	public void StartReload()
	{
		if (CurrentMagazine == MaxMagazine || CurrentMagazine == CurrentAmmo || IsReloading)
		{
			return;
		}

		animator.SetTrigger("Reload");
		IsReloading = true;
	}

	private void Reload()
	{
		if (CurrentAmmo < MaxMagazine)
		{
			CurrentMagazine = CurrentAmmo;
		}
		else
		{
			CurrentMagazine = MaxMagazine;
		}

		IsReloading = false;
	}

	public void RefillAmmo()
	{
		currentAmmo = maxAmmo;

		//TODO
		//refillSound.Play();
	}

	//TODO Minek?
	/*
	public void StartSlide()
	{
		animator.SetBool("slide", true);
		IsSliding = true;
	}

	public void EndSlide()
	{
		animator.SetBool("slide", false);
		IsSliding = false;
	}*/

	protected Vector3 GetAimDirection()
	{
		Vector3 direction;
		Vector3 aimingAt = ShootRay();

		if (aimingAt == Vector3.zero)   //if aiming at nothing, weapon won't be accurate
		{
			direction = transform.parent.forward;
		}
		else
		{
			direction = (aimingAt - bulletSpawnPoint.position).normalized;
		}

		if (addBulletSpread)
		{
			direction += new Vector3(
				Random.Range(-BulletSpreadVariance, BulletSpreadVariance),
				Random.Range(-BulletSpreadVariance, BulletSpreadVariance),
				Random.Range(-BulletSpreadVariance, BulletSpreadVariance)
			);

			direction.Normalize();
		}

		return direction;
	}
}
