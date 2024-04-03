using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{
	[field: SerializeField] public float moveSpeed { get; private set; }
	[field: SerializeField] public float rotateSpeed { get; private set; }
	[field: SerializeField] public float jumpForce { get; private set; }

	[SerializeField] AudioSource jumpSound;

	private Vector2 m_Rotation;
	private Vector2 m_Look;
	private Vector2 m_Move;

	private WeaponScript weapon;

	public bool isGrounded { get; private set;}
    public Statistics stats { get; private set; } = new Statistics();
    
	public void OnMove(InputAction.CallbackContext context)
	{
		m_Move = context.ReadValue<Vector2>();
		//Debug.Log("Move: " + m_Move);
	}
	public void OnJump(InputAction.CallbackContext context)
	{
		Jump();
		//Debug.Log("Jump");
	}
	public void OnLook(InputAction.CallbackContext context)
	{
		m_Look = context.ReadValue<Vector2>();
		//Debug.Log("Look: " + m_Look);
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

		var scaledMoveSpeed = moveSpeed * Time.deltaTime;
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

		var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
		m_Rotation.y += rotate.x * scaledRotateSpeed;
		m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89f, 89f);
		this.transform.localEulerAngles = m_Rotation;
	}
	public void TargetHit(float reactionTime)
	{
		stats.TargetHit(reactionTime);
	}
}
