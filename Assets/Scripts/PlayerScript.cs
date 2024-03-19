using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{
	[field: SerializeField] public float moveSpeed { get; private set; }
	[field: SerializeField] public float rotateSpeed { get; private set; }
	[field: SerializeField] public float jumpForce { get; private set; }
	[field: SerializeField] public float bulletSize { get; private set; }
	[field: SerializeField] public float bulletForce { get; private set; }
	[field: SerializeField] public float fireRate { get; private set; } //RPM

	[SerializeField] GameObject projectile;

	private bool isGrounded { get; set; }

	private Vector2 m_Rotation;
	private Vector2 m_Look;
	private Vector2 m_Move;

	private float timeSinceFire;
	private bool isPrimaryFire;

	public void OnMove(InputAction.CallbackContext context)
	{
		m_Move = context.ReadValue<Vector2>();
		Debug.Log("Move: " + m_Move);
	}
	public void OnJump(InputAction.CallbackContext context)
	{
		Jump();
		Debug.Log("Jump");
	}
	public void OnLook(InputAction.CallbackContext context)
	{
		m_Look = context.ReadValue<Vector2>();
		Debug.Log("Look: " + m_Look);
	}
	public void OnFire(InputAction.CallbackContext context)	//If the button is pressed, set isPrimaryFire to true, if it is released, set it to false
	{
		if (context.started)
			isPrimaryFire = true;

		else if (context.canceled)
			isPrimaryFire = false;

		Debug.Log("Fire");
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

	public void Update()
	{
		Look(m_Look);
		Move(m_Move);
		timeSinceFire += Time.deltaTime;
		if (isPrimaryFire)
		{
			Fire();
		}
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

		Vector3 jump = new Vector3(0.0f, 1.0f, 0.0f);
		this.GetComponent<Rigidbody>().AddForce(jump * jumpForce, ForceMode.Impulse);
	}
	private void Look(Vector2 rotate)
	{
		if (rotate.sqrMagnitude < 0.01)
			return;

		var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
		m_Rotation.y += rotate.x * scaledRotateSpeed;
		m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89, 89);
		this.transform.localEulerAngles = m_Rotation;
	}

	private void Fire()
	{
		if (timeSinceFire < 1 / (fireRate / 60))
			return;

		var transform = this.transform;
		var newProjectile = Instantiate(projectile);
		newProjectile.transform.position = transform.position + transform.forward * 0.6f;
		newProjectile.transform.rotation = transform.rotation;
		newProjectile.transform.Rotate(90f, 0f, 0f);

		newProjectile.transform.localScale *= bulletSize;
		newProjectile.GetComponent<Rigidbody>().mass = Mathf.Pow(bulletSize, 3);
		newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * bulletForce, ForceMode.Impulse);
		timeSinceFire = 0;
	}
}
