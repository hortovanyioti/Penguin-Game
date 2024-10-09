using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class AntiAliasing : Dropdown
{
	private int[] m_AntiAliasingValues = new int[] { 0, 2, 4, 8 };

	void Start()
	{
		base.Init();

		QualitySettings.antiAliasing = m_Player.PlayerConfig.AntiAliasing;

		for (int i = 0; i < m_AntiAliasingValues.Length; i++)
		{
			m_Dropdown.options.Add(new TMP_Dropdown.OptionData("X" + m_AntiAliasingValues[i]));
			if (m_AntiAliasingValues[i] == QualitySettings.antiAliasing)
			{
				m_Dropdown.value = i;
			}
		}
	}

	public override void OnValueChange()
	{
		base.OnValueChange();
		QualitySettings.antiAliasing = m_AntiAliasingValues[m_Dropdown.value];
		m_Player.PlayerConfig.AntiAliasing = QualitySettings.antiAliasing;
	}
}