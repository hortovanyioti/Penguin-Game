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

	[SerializeField] List<GameObject> targets;
	[SerializeField] List<GameObject> objects;
	private readonly int numOfObservedTargets = 4;

	private float rewardTimer = 0;
	private readonly float rewardTime = 0.1f;               // Every n seconds
	private readonly float timebasedRewardAmount = -0.001f;   // Awards m amount

	private float lifeTime = 0;
	private readonly float forceEndSeconds = 30;

	private bool isTraining;
	private float movementSpeed = 5;
	private float rotationSpeed = 180;
	private float hitsToKillTarget=1;

	private void Start()
	{
		Base = GetComponent<BaseEnemy>();
		behaviorParameters = GetComponent<BehaviorParameters>();
		//behaviorParameters.BrainParameters.VectorObservationSize = (numOfObservedTargets + 1) * 3;
		isTraining = behaviorParameters.Model == null;
		hitsToKillTarget= targets[0].GetComponent<GameCharacter>().MaxHealth / Base.weapon.Damage;

		if (isTraining)
		{
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

		transform.Rotate(new Vector3(0, rotateY * Time.deltaTime * rotationSpeed, 0));
		transform.localPosition += moveDir * Time.deltaTime * movementSpeed * CustomMathFunction.GetMoveSpeedCoeff(moveDir, transform.forward);

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
				AddReward(0.05f);
				EndEpisode();
				break;
			case "Bullet":
				AddReward(-0.1f);
				EndEpisode();
				break;
			case "Wall":
				AddReward(-0.02f);
				//EndEpisode();
				break;
		}
	}

	private void ResetEnviroment()
	{
		foreach (var target in targets)
		{
			if (!target.activeSelf)
			{
				target.SetActive(true);
			}
			target.transform.localPosition = new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10));
		}
		foreach (var obj in objects)
		{
			obj.transform.localPosition = new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10));
		}

		transform.localPosition = new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10));
	}


	public void BulletFeedback(bool hit)
	{
		if (hit)
		{
			AddReward(1f / targets.Count / hitsToKillTarget);
			for (int i = 0; i < targets.Count; ++i)
			{
				if (targets[i].activeSelf)
				{
					break;
				}

				EndEpisode();
			}
		}
		else
		{
			AddReward(-0.1f);
		}
	}
	#endregion
}
