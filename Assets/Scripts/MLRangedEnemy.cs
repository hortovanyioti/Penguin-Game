using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(BaseEnemy))]
public class MLRangedEnemy : Agent
{
	BaseEnemy Base;
	BehaviorParameters behaviorParameters;


	[SerializeField] GameObject targetContainer;
	[SerializeField] GameObject obstacleContainer;
	[SerializeField] readonly int numOfObservedTargets = 4;

	[SerializeField] float rewardTimer = 0;
	[SerializeField] readonly float rewardTime = 0.1f;               // Every n seconds
	[SerializeField] readonly float timebasedRewardAmount = -0.0003f;   // Awards m amount

	[SerializeField] float lifeTime = 0;
	[SerializeField] readonly float forceEndSeconds = 60;

	[SerializeField] bool isTraining;
	[SerializeField] float movementSpeed = 5;
	[SerializeField] float rotationSpeed = 180;
	[SerializeField] float hitsToKillTarget = 2;

	private float cumulativeRotation = 0;
	[SerializeField] int spawnRange = 25;

	[SerializeField] readonly float rotationPenaltyThreshold = 720; // Threshold for cumulative rotation penalty
	[SerializeField] readonly float rotationPenaltyAmount = -0.01f; // Penalty for exceeding the threshold
	[SerializeField] readonly float smallRotationPenaltyAmount = -0.0001f; // Smaller penalty for any rotation
	internal class CumulativeRewards
	{
		public float bullets = 0;
		public float time = 0;
		public float fullRotation = 0;
		public float smallRotation = 0;
		public float collision = 0;
		public float hits = 0;
	}

	CumulativeRewards cumulativeRewards = new CumulativeRewards();

	private void Start()
	{
		Base = GetComponent<BaseEnemy>();
		behaviorParameters = GetComponent<BehaviorParameters>();
		Base.EnableUpdate = false;

		if (isTraining)
		{
			behaviorParameters.BehaviorType = BehaviorType.InferenceOnly;
			hitsToKillTarget = targetContainer.transform.GetChild(0).GetComponent<GameCharacter>().MaxHealth / Base.weapon.Damage;
			ResetEnviroment();
		}
	}

	private void Update()
	{
		if (isTraining)
		{
			TrainingUpdate();
		}
	}

	#region TRAINING
	private void TrainingUpdate()
	{
		rewardTimer += Time.deltaTime;
		lifeTime += Time.deltaTime;

		if (rewardTimer >= rewardTime)
		{
			AddReward(timebasedRewardAmount);
			cumulativeRewards.time += timebasedRewardAmount;
			rewardTimer = 0;
		}

		if (lifeTime >= forceEndSeconds)
		{
			EndEpisode();
		}
	}

	public override void OnEpisodeBegin()
	{
		if (isTraining)
		{
			LogCumulativeRewards();
			lifeTime = 0;
			ResetEnviroment();
		}
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(transform.localPosition);
		sensor.AddObservation(transform.rotation);
		/*
				for (int i = 0; i < numOfObservedTargets; i++)      //TODO Replace this logic if 4+ players is a requirement (observe 4 closest players)
				{
					if (i < targets.Count)
					{
						sensor.AddObservation(targets[i].transform.position - transform.position);
					}
					else
					{
						sensor.AddObservation(Vector3.zero);
					}
				}*/
	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		float moveX = actions.ContinuousActions[0];
		float moveY = actions.ContinuousActions[1];
		float rotateY = actions.ContinuousActions[2];

		int doAttack = actions.DiscreteActions[0];

		Vector3 moveDir = new Vector3(moveX, 0, moveY);

		var prevRotation = transform.rotation;

		transform.Rotate(new Vector3(0, rotateY * Time.deltaTime * rotationSpeed, 0));
		transform.localPosition += moveDir * Time.deltaTime * movementSpeed * CustomMathFunction.GetMoveSpeedCoeff(moveDir, transform.forward);

		// Apply small penalty for any rotation

		var rotationPenalty = Mathf.Abs(prevRotation.y - transform.rotation.y) / 360 * smallRotationPenaltyAmount;
		AddReward(rotationPenalty);
		cumulativeRewards.smallRotation += rotationPenalty;

		// Calculate rotation penalties
		cumulativeRotation += rotateY;

		// Apply penalty if cumulative rotation exceeds the threshold
		if (cumulativeRotation >= rotationPenaltyThreshold)
		{
			AddReward(rotationPenaltyAmount);
			cumulativeRewards.fullRotation += rotationPenaltyAmount;
			cumulativeRotation = 0;
		}

		if (doAttack == 1)
		{
			Base.TryRangedAttack();
		}
	}

	public override void Heuristic(in ActionBuffers actionsOut) { }

	private void OnCollisionEnter(Collision collision)
	{
		switch (collision.gameObject.tag)
		{
			case "Player":
				/*AddReward(0.05f);
				cumulativeRewards.collision += 0.05f;
				EndEpisode();*/
				break;
			case "Bullet":
				AddReward(-0.1f);
				cumulativeRewards.collision += -0.1f;
				EndEpisode();
				break;
			case "Wall":
				AddReward(-0.02f);
				cumulativeRewards.collision += -0.02f;
				//EndEpisode();
				break;
		}
	}

	private void ResetEnviroment()
	{
		cumulativeRewards = new CumulativeRewards();

		for (int i = 0; i < targetContainer.transform.childCount; i++)
		{
			var target = targetContainer.transform.GetChild(i).gameObject;
			if (!target.activeSelf)
			{
				target.SetActive(true);
			}
			target.transform.localPosition = new Vector3(Random.Range(-spawnRange, spawnRange), 1, Random.Range(-spawnRange, spawnRange));
		}

		for (int i = 0; i < obstacleContainer.transform.childCount; i++)
		{
			var obstacle = obstacleContainer.transform.GetChild(i).gameObject;
			obstacle.transform.localPosition = new Vector3(Random.Range(-spawnRange, spawnRange), 1, Random.Range(-spawnRange, spawnRange));
		}

		transform.localPosition = new Vector3(Random.Range(-spawnRange, spawnRange), 2, Random.Range(-spawnRange, spawnRange));
	}

	public void BulletFeedback(bool hit)
	{
		if (!isTraining)
		{
			return;
		}

		if (hit)
		{
			AddReward(1f / targetContainer.transform.childCount / hitsToKillTarget);
			cumulativeRewards.hits += 1f / targetContainer.transform.childCount / hitsToKillTarget;
			for (int i = 0; i < targetContainer.transform.childCount; ++i)
			{
				if (targetContainer.transform.GetChild(i).gameObject.activeSelf)
				{
					return;
				}
			}
			EndEpisode();
		}
		else
		{
			AddReward(-0.005f);
			cumulativeRewards.bullets += -0.005f;
		}
	}

	private void LogCumulativeRewards()
	{
		float totalRewards = cumulativeRewards.bullets + cumulativeRewards.time + cumulativeRewards.fullRotation + cumulativeRewards.smallRotation + cumulativeRewards.collision + cumulativeRewards.hits;
		Debug.Log($"Bullets: {cumulativeRewards.bullets:0.0000}, Time: {cumulativeRewards.time:0.0000}, Full Rotation: {cumulativeRewards.fullRotation:0.0000}, Small Rotation: {cumulativeRewards.smallRotation:0.0000}, Collision: {cumulativeRewards.collision:0.0000}, Hits: {cumulativeRewards.hits:0.0000}, Total: {totalRewards:0.0000}");
	}
	#endregion
}
