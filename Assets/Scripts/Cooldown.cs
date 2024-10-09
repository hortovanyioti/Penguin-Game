using UnityEngine;

[System.Serializable]

public class Cooldown
{
	[SerializeField] private float coolDownTime;
	public float CoolDownTime { get => coolDownTime; set => coolDownTime = value; }

	private float _nextActionTime = float.MinValue;

	public bool IsCoolingDown => Time.time <= _nextActionTime;
	public void StartCoolDown() => _nextActionTime = Time.time + coolDownTime;
}
