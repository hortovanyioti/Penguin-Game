using UnityEngine;
using UnityEngine.InputSystem;

public class DemoPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } private set { moveSpeed = value; } }

    private Vector2 m_Move;
    public void OnMove(InputAction.CallbackContext context)
    {
        m_Move = context.ReadValue<Vector2>();
    }

    void Update()
    {
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
}