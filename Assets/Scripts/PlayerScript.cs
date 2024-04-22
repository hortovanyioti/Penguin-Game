using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{
	[SerializeField] private float moveSpeed;
	[SerializeField] private float lookSensitivity;
	[SerializeField] private float jumpForce;
	[SerializeField] AudioSource jumpSound;
	public float MoveSpeed { get { return moveSpeed; } private set { moveSpeed = value; } }
	public float LookSensitivity { get { return lookSensitivity; } set { lookSensitivity = value; } }
	public float JumpForce { get { return jumpForce; } private set { jumpForce = value; } }

	private Vector2 m_Rotation;
	private Vector2 m_Look;
	private Vector2 m_Move;

	private WeaponScript weapon;
	private PlayerInput input;

	public bool isGrounded { get; private set; }
	public Statistics stats { get; private set; } = new Statistics();

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
	void Start()
	{
		input = GetComponent<PlayerInput>();
		weapon = GetComponentInChildren<WeaponScript>();
	}
	void Update()
	{
		Look(m_Look);
		Move(m_Move);
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

		var scaledRotateSpeed = LookSensitivity * Time.deltaTime;
		m_Rotation.y += rotate.x * scaledRotateSpeed;
		m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89f, 89f);
		this.transform.localEulerAngles = m_Rotation;
	}
	public void TargetHit(float reactionTime)
	{
		stats.TargetHit(reactionTime);
	}
}
