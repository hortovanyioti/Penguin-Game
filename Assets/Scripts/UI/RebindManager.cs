using UnityEngine;
using UnityEngine.InputSystem;

public class RebindManager : MonoBehaviour
{
	[SerializeField] private InputActionReference moveRef;
	private void OnEnable()
	{
		moveRef.action.Disable();
	}
	private void OnDisable()
	{
		moveRef.action.Enable();
	}
}
