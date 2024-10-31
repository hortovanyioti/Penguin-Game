using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : GameCharacter
{
	[SerializeField] private float slideSpeed;
	public float SlideSpeed { get => slideSpeed; private set => slideSpeed = value; }


	private PlayerConfig playerConfig;
	public PlayerConfig PlayerConfig { get => playerConfig; set => playerConfig = value; }


	[SerializeField] private float jumpForce;
	public float JumpForce { get => jumpForce; private set => jumpForce = value; }


	[SerializeField] private bool isGrounded;
	public bool IsGrounded { get => isGrounded; private set => isGrounded = value; }


	[SerializeField] AudioSource jumpSound;

	private Vector2 m_Rotation;
	private Vector2 c_Rotation;
	private Vector2 m_Look;
	private Vector2 m_Move;

	private PlayerInput input;
	private Transform weaponTransform;
	private Animator camAnimator;
	[SerializeField] private GameObject headCenter;
	[SerializeField] private GameObject headAim;
	[SerializeField] private Transform rightHand;
	[SerializeField] private Transform leftHand;
	[SerializeField] private TwoBoneIKConstraint leftHandConstraint;
	[SerializeField] private HUDcontroller hud;

	[SerializeField] private Cooldown slideCooldown;    //time until next slide
	[SerializeField] private Cooldown slideTimeCooldown;    //duration of slide
	private Vector2 slideDir;


	void Awake()
	{
		PlayerConfig = new FileDataHandler("config.cfg", "", false).LoadData<PlayerConfig>(PlayerConfig);

		if (PlayerConfig == null)
		{
			PlayerConfig = new PlayerConfig();
			PlayerConfig.SetDefault();
		}

		AudioListener.volume = PlayerConfig.GlobalVolume;

		CurrentHealth = MaxHealth;
		leftHandConstraint.weight = 0;
	}
	void Start()
	{
		base.Init();
		input = GetComponent<PlayerInput>();
		rigidbody = GetComponent<Rigidbody>();
		weapon = GetComponentInChildren<Weapon>(true);
		animator = GetComponent<Animator>();
		camAnimator = camera.GetComponent<Animator>();
		hud = GetComponentInChildren<HUDcontroller>(true);

		weaponTransform = weapon.transform.parent;
	}
	void Update()
	{
		if (input.enabled)
		{
			Move(m_Move);
			Look(m_Look);
			MoveWeapon();

			UpdateHUD();    //TODO only update hub when something changed

			AnimationControl();
			LeftHandMovement();
		}
	}

	private Vector3 ShootRay()
	{
		Vector3 point = new Vector3(camera.pixelWidth / 2f, camera.pixelHeight / 2f, 0f);
		Ray ray = camera.ScreenPointToRay(point);

		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			return hit.point;
		}
		else
		{
			return Vector3.zero;
		}
	}

	private void OnDestroy()
	{
		PlayerConfig.Save();
	}


	public void OnMove(InputAction.CallbackContext context)
	{

		m_Move = context.ReadValue<Vector2>();

		//TODO
		//CHECK IF SLIDING
		//if (slideTimeCooldown.IsCoolingDown) return;


		if (context.started)
		{
			animator.SetBool("Move", true);
			camAnimator.SetBool("Move", true);
		}
		else if (context.canceled)
		{
			animator.SetBool("Move", false);
			camAnimator.SetBool("Move", false);
		}
	}
	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Jump();
		}
	}
	public void OnLook(InputAction.CallbackContext context)
	{
		m_Look = context.ReadValue<Vector2>();
	}

	public void OnFire(InputAction.CallbackContext context)
	{
		weapon.OnFire(context);
	}
	public void OnCrouch(InputAction.CallbackContext context)
	{
		throw new NotImplementedException();
	}
	public void OnToggleCrouch(InputAction.CallbackContext context)
	{
		throw new NotImplementedException();
	}


	public void OnSlide(InputAction.CallbackContext context)
	{
		Slide(context);
	}

	public void OnReload(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			StartCoroutine(IncreaseIKWeight());
			weapon.StartReload();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			IsGrounded = true;
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			IsGrounded = false;
		}
	}

	private void Slide(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			if (slideCooldown.IsCoolingDown) return;

			slideDir = m_Move;
			animator.SetBool("Slide", true);

			camAnimator.SetBool("Slide", true);

			//weapon.StartSlide();

			slideCooldown.StartCoolDown();
			slideTimeCooldown.StartCoolDown();
		}
	}

	private void Move(Vector2 direction)
	{
		//SLIDE
		if (slideTimeCooldown.IsCoolingDown)
		{
			var scaledSlideSpeed = SlideSpeed * Time.deltaTime;
			//todo? var slide = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(m_Move.x, 0, m_Move.y);
			this.transform.position += Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(slideDir.x, 0, slideDir.y) * scaledSlideSpeed;
			return;
		}

		if (direction.sqrMagnitude < 0.01)
			return;

		var scaledMoveSpeed = MoveSpeed * Time.deltaTime;
		var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
		this.transform.position += move * scaledMoveSpeed;
	}
	private void Jump()
	{
		if (!IsGrounded)    // Only jump if grounded
			return;

		Vector3 jump = new Vector3(0f, 1f, 0f);
		rigidbody.AddForce(jump * jumpForce, ForceMode.Impulse);

		//TODO
		//jumpSound.Play(0);
	}
	private void Look(Vector2 rotate)
	{
		if (rotate.sqrMagnitude < 0.01)
			return;


		////////////   ROTATE CAMERA    ////////////////
		var scaledRotateSpeed = PlayerConfig.LookSensitivity * Time.deltaTime;
		m_Rotation.y += rotate.x * scaledRotateSpeed;
		m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89f, 89f);
		m_Rotation.x = 0;
		c_Rotation.x = Mathf.Clamp(c_Rotation.x - rotate.y * scaledRotateSpeed, -89f, 89f);
		c_Rotation.y = 0;
		this.transform.localEulerAngles = m_Rotation;
		camera.transform.localEulerAngles = c_Rotation;


		////////////   ROTATE WEAPON, CHANGE RIGHT HAND POSITION    ////////////////
		float rot;
		if (camera.transform.eulerAngles.x <= 180f)
		{
			rot = camera.transform.eulerAngles.x;
		}
		else
		{
			rot = camera.transform.eulerAngles.x - 360f;
		}
		headAim.transform.position = headCenter.transform.position + (camera.transform.position - headCenter.transform.position).normalized + new Vector3(0, Mathf.Sin(-rot * Mathf.Deg2Rad), 0);
		rightHand.transform.position = weapon.rightHandle.transform.position;
	}
	private void OpenBuildMenu()
	{
		//todo
		Debug.Log("opened build menu");
	}

	private void AnimationControl()
	{
		if (!slideTimeCooldown.IsCoolingDown)
		{
			animator.SetBool("Slide", false);
			//weapon.EndSlide();
			camAnimator.SetBool("Slide", false);
		}
	}

	public override void Hurt(float damage)
	{
		base.Hurt(damage);
		hud.SetCurrentHealth((int)CurrentHealth);
	}

	private void LeftHandMovement()
	{
		if (weapon.IsReloading)
		{
			leftHand.transform.position = weapon.magazine.position;
		}
		else
		{
			leftHandConstraint.weight = 0;
		}
	}

	private IEnumerator IncreaseIKWeight()
	{
		while (leftHandConstraint.weight != 1)
		{
			leftHandConstraint.weight += Time.deltaTime * 3;
			yield return null;
		}
	}

	void UpdateHUD()
	{
		hud.SetMaxHealth((int)MaxHealth);
		hud.SetCurrentHealth((int)CurrentHealth);
		hud.SetRemainingAmmo(weapon.CurrentAmmo - weapon.CurrentMagazine);
		hud.SetCurrentMagazine(weapon.CurrentMagazine);
		hud.SetMaxMagazine(weapon.MaxMagazine);
	}

	private void MoveWeapon()
	{
		if (slideTimeCooldown.IsCoolingDown)
		{
			weaponTransform.rotation = new Quaternion(0, 0, 0, 0);
		}
		else
		{
			weaponTransform.rotation = camera.transform.rotation;
		}
	}
	public void ActivateInput(bool isTrue = true)
	{
		camera.gameObject.SetActive(isTrue);
		hud.gameObject.SetActive(isTrue);
		input.enabled = isTrue;
	}

	public void DeactivateInput()
	{
		ActivateInput(false);
	}
}
