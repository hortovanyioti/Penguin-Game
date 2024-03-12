using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Use a separate PlayerInput component for setting up input.
public class PlayerScript : MonoBehaviour
{
	public float moveSpeed;
	public float rotateSpeed;

	public GameObject projectile;

	private Vector2 m_Rotation;
	private Vector2 m_Look;
	private Vector2 m_Move;

	public void OnMove(InputAction.CallbackContext context)
	{
		m_Move = context.ReadValue<Vector2>();
		Debug.Log("Move: " + m_Move);
	}

	public void OnLook(InputAction.CallbackContext context)
	{
		m_Look = context.ReadValue<Vector2>();
		Debug.Log("Look: " + m_Look);
	}

	public void OnFire(InputAction.CallbackContext context)
	{
		//Fire();
	}

	/*public void OnGUI()
	{
		if (m_Charging)
			GUI.Label(new Rect(100, 100, 200, 100), "Charging...");
	}*/
	
	public void Update()
	{
		Look(m_Look);
		Move(m_Move);
	}

	private void Move(Vector2 direction)
	{
		if (direction.sqrMagnitude < 0.01)
			return;
		var scaledMoveSpeed = moveSpeed * Time.deltaTime;
		// For simplicity's sake, we just keep movement in a single plane here. Rotate
		// direction according to world Y rotation of player.
		var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
		transform.position += move * scaledMoveSpeed;
	}

	private void Look(Vector2 rotate)
	{
		if (rotate.sqrMagnitude < 0.01)
			return;
		var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
		m_Rotation.y += rotate.x * scaledRotateSpeed;
		m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89, 89);
		transform.localEulerAngles = m_Rotation;
	}

	/*private void Fire()
	{
		var transform = this.transform;
		var newProjectile = Instantiate(projectile);
		newProjectile.transform.position = transform.position + transform.forward * 0.6f;
		newProjectile.transform.rotation = transform.rotation;
		const int size = 1;
		newProjectile.transform.localScale *= size;
		newProjectile.GetComponent<Rigidbody>().mass = Mathf.Pow(size, 3);
		newProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
		newProjectile.GetComponent<MeshRenderer>().material.color =
			new Color(Random.value, Random.value, Random.value, 1.0f);
	}*/
}
