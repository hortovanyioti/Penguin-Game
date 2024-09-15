using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{
	[SerializeField] private float moveSpeed;
	public float MoveSpeed { get { return moveSpeed; } private set { moveSpeed = value; } }


	private PlayerConfig playerConfig;
	public PlayerConfig PlayerConfig { get { return playerConfig; } set { playerConfig = value; } }


	[SerializeField] private float jumpForce;
	public float JumpForce { get { return jumpForce; } private set { jumpForce = value; } }


	[SerializeField] AudioSource jumpSound;

	private Vector2 m_Rotation;
	private Vector2 m_Look;
	private Vector2 m_Move;

	private WeaponScript weapon;
	private PlayerInput input;

	public bool isGrounded { get; private set; }
	public Statistics stats { get; private set; } = new Statistics();

	void Awake()
	{
		//PlayerConfig = new PlayerConfig();
		PlayerConfig = new FileDataHandler("config.cfg", "", false).LoadData<PlayerConfig>(PlayerConfig);

		if (PlayerConfig == null)
		{
			PlayerConfig = new PlayerConfig();
			PlayerConfig.SetDefault();
		}

		AudioListener.volume = PlayerConfig.GlobalVolume;
	}
	void Start()
	{
		input = GetComponent<PlayerInput>();
		weapon = GetComponentInChildren<WeaponScript>();

	}
	void Update()
	{
		if (SceneManager.GetActiveScene().name == "MainScene")
		{
			if (input.enabled == false)
			{
				input.enabled = true;
			}
			Look(m_Look);
			Move(m_Move);
		}
		else
		{
			if (input.enabled == true)
			{
				input.enabled = false;
			}
		}
	}

	private void OnDestroy()
	{
		PlayerConfig.Save();
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		m_Move = context.ReadValue<Vector2>();
	}
	public void OnJump(InputAction.CallbackContext context)
	{
		Jump();
	}
	public void OnLook(InputAction.CallbackContext context)
	{
		m_Look = context.ReadValue<Vector2>();
	}
	public void OnFire(InputAction.CallbackContext context) //If the button is pressed, set isPrimaryFire to true, if it is released, set it to false
	{
		weapon.OnFire(context);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Ground")
		{
			isGrounded = true;
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "Ground")
		{
			isGrounded = false;
		}
	}

	private void Move(Vector2 direction)
	{
		if (direction.sqrMagnitude < 0.01)
			return;

		var scaledMoveSpeed = MoveSpeed * Time.deltaTime;
		var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
		this.transform.position += move * scaledMoveSpeed;
	}
	private void Jump()
	{
		if (!isGrounded)    // Only jump if grounded
			return;

		Vector3 jump = new Vector3(0f, 1f, 0f);
		this.GetComponent<Rigidbody>().AddForce(jump * jumpForce, ForceMode.Impulse);
		jumpSound.Play(0);
	}
	private void Look(Vector2 rotate)
	{
		if (rotate.sqrMagnitude < 0.01)
			return;

		var scaledRotateSpeed = PlayerConfig.LookSensitivity * Time.deltaTime;
		m_Rotation.y += rotate.x * scaledRotateSpeed;
		m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89f, 89f);
		this.transform.localEulerAngles = m_Rotation;
	}
	public void TargetHit(float reactionTime)
	{
		stats.TargetHit(reactionTime);
	}
	public void Spawn()
	{
		float spawnRange = 10;
		transform.position = new Vector3(Random.Range(-spawnRange, spawnRange), 3, Random.Range(-spawnRange, spawnRange));
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.useGravity = true;
		Debug.Log(gameObject.name + " Player spawned at: " + transform.position);
	}
}
