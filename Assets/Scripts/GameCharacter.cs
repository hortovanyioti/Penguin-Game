using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class GameCharacter : MonoBehaviour
{
	[SerializeField] private float maxHealth = 100f;
	public float MaxHealth
	{
		get => maxHealth;
		protected set
		{
			if (value < 0)
			{
				maxHealth = 0;
			}
			else
			{
				maxHealth = value;
			}
		}
	}


	[SerializeField] private float currentHealth;
	public float CurrentHealth
	{
		get => currentHealth;
		protected set
		{
			if (value < 0)
			{
				currentHealth = 0;
			}
			else
			{
				currentHealth = value;
			}
		}
	}


	[SerializeField] private float moveSpeed;
	public float MoveSpeed { get => moveSpeed; protected set => moveSpeed = value; }

	protected new Rigidbody rigidbody;
	protected new Camera camera;
	protected Animator animator;

	protected void Init()
	{
		CurrentHealth = MaxHealth;
		camera = GetComponentInChildren<Camera>();
	}
	public virtual void Hurt(float damage)
	{
		CurrentHealth -= damage;
	}

	public virtual void Die()
	{
		//TODO dieSound.Play();
		Destroy(gameObject);
	}
}
