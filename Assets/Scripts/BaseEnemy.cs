using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.InputSystem.HID;

[RequireComponent(typeof(NavMeshAgent))]
public class BaseEnemy : GameCharacter
{

	[SerializeField] protected List<GameObject> attackTargets;
	protected int currentAttackTargetIndex;
	protected NavMeshAgent navMeshAgent;

	public const float closeDistance = 10f;
	public const float farDistance = 100f;

	public const float closeDistaneUpdateInterval = 0.1f;
	public const float middleDistanceUpdateInterval = 0.3f;
	public const float farDistanceUpdateInterval = 1f;

	[SerializeField] private float AttackDistance = 3f;
	private float trackingUpdateTimer;
	private Cooldown attackCooldown;
	private BoxCollider attackCollider;

	void Start()
	{
		base.Init();
		navMeshAgent = GetComponent<NavMeshAgent>();
		weapon = GetComponentInChildren<Weapon>();
		attackCollider = GetComponent<BoxCollider>();
		attackCooldown = new Cooldown();
		attackCooldown.CoolDownTime = 1f;
		attackTargets = CustomNetworkManager.Instance.Players.GameObjects;

#if UNITY_EDITOR
		if (attackCollider.enabled != false)
		{
			Debug.LogError("Attack collider should be disabled at start! [UNITY BUG]");
		}
#endif
		StartCoroutine(DelayAttackEnable());
		//TODO: fill attackTargets array with all player objects
	}
	private void Update()
	{
		/*
		trackingUpdateTimer += Time.deltaTime;
		TryUpdateAttackTarget();
		TryRangedAttack();
		UpdateCamera();
		*/
	}

	public override void Hurt(float damage)
	{
		base.Hurt(damage);
	}

	public override void Die()
	{
		base.Die();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			TryMeleeAttack(other.gameObject);
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			TryMeleeAttack(other.gameObject);
		}
	}

	/// <summary>
	/// Updates the attack target, update frequency depends on the distance to the target
	/// </summary>
	private void TryUpdateAttackTarget()
	{
		if (attackTargets.Count == 0 || trackingUpdateTimer < closeDistaneUpdateInterval)  //no target or too early to update
		{
			return;
		}

		currentAttackTargetIndex = 0;
		float minDistance = Vector3.Distance(attackTargets[0].transform.position, transform.position);

		for (int i = 1; attackTargets.Count > i; i++)
		{
			float distance = Vector3.Distance(attackTargets[i].transform.position, transform.position);
			if (distance < minDistance)
			{
				minDistance = distance;
				currentAttackTargetIndex = i;
			}
		}

		if (minDistance > farDistance)  //far distance
		{
			if (trackingUpdateTimer > farDistanceUpdateInterval)
			{
				UpdateAttackTarget();
			}
		}
		else if (minDistance > closeDistance)   //middle distance
		{
			if (trackingUpdateTimer > middleDistanceUpdateInterval)
			{
				UpdateAttackTarget();
			}
		}
		else    //close distance
		{
			UpdateAttackTarget();
		}
	}
	private void UpdateAttackTarget()
	{
		navMeshAgent.SetDestination(attackTargets[currentAttackTargetIndex].transform.position);
		trackingUpdateTimer = 0;
	}

	private void TryMeleeAttack(GameObject target)
	{
		if (attackCooldown.IsCoolingDown)
		{
			return;
		}

		MeleeAttack(target);

		attackCooldown.StartCoolDown();
	}
	protected virtual void MeleeAttack(GameObject target)
	{
		target.gameObject.GetComponent<PlayerScript>().Hurt(10);
	}

	public void TryRangedAttack()
	{
		if (attackCooldown.IsCoolingDown || weapon == null)
		{
			return;
		}

		RangedAttack();
		attackCooldown.StartCoolDown();
	}

	protected virtual void RangedAttack()
	{
		weapon.Fire();
	}

	private void UpdateCamera()
	{
		if (attackTargets.Count == 0)
		{
			return;
		}
		camera.transform.rotation = Quaternion.LookRotation(attackTargets[currentAttackTargetIndex].transform.position - camera.transform.position);
	}

	IEnumerator DelayAttackEnable() //retarded bug-fix
	{
		yield return new WaitForSeconds(0.5f);
		attackCollider.enabled = true;
	}
}
