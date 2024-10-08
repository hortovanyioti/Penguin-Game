using TMPro;
using UnityEngine;

public class VSync : CheckBox
{
	void Start()
	{
		base.Init();

		QualitySettings.vSyncCount = m_Player.PlayerConfig.VSync;
		m_Toggle.isOn = QualitySettings.vSyncCount == 1 ? true : false;

		UpdateColor();
	}
	public override void OnValueChange()
	{
		base.OnValueChange();
		QualitySettings.vSyncCount = m_Toggle.isOn ? 1 : 0;
		m_Player.PlayerConfig.VSync = QualitySettings.vSyncCount;
	}
}

