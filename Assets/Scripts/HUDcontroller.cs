using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDcontroller : MonoBehaviour
{
	private Slider healthBar;

	private TextMeshProUGUI currentMagazine;
	private TextMeshProUGUI maxMagazine;
	private TextMeshProUGUI remainingAmmo;

	private void Start()
	{
		healthBar = transform.Find("HealthBar").GetComponent<Slider>();
		var Ammo = transform.Find("Ammo");
		currentMagazine = Ammo.Find("CurrentMagazine").GetComponent<TextMeshProUGUI>();
		maxMagazine = Ammo.Find("MaxMagazine").GetComponent<TextMeshProUGUI>();
		remainingAmmo = Ammo.Find("RemainingAmmo").GetComponent<TextMeshProUGUI>();

#if DEBUG
		if (healthBar == null)
		{
			Debug.LogError("HealthBar not found!");
		}
		if (currentMagazine == null)
		{
			Debug.LogError("CurrentMagazine not found!");
		}
		if (maxMagazine == null)
		{
			Debug.LogError("MaxMagazine not found!");
		}
		if (remainingAmmo == null)
		{
			Debug.LogError("RemainingAmmo not found!");
		}
#endif

	}
	public void SetCurrentHealth(int p)
	{
		healthBar.value = p;
	}
	public void SetMaxHealth(int p)
	{
		healthBar.maxValue = p;
	}
	public void SetMaxMagazine(int p)
	{
		maxMagazine.text = p.ToString();
	}

	public void SetCurrentMagazine(int p)
	{
		currentMagazine.text = p.ToString();
	}

	public void SetRemainingAmmo(int p)
	{
		remainingAmmo.text = p.ToString();
	}
}
