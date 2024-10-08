using TMPro;
using UnityEngine;

public class AnisoptricFiltering : CheckBox
{
	private AnisotropicFiltering[] filters;

	void Start()
	{
		base.Init();

		QualitySettings.anisotropicFiltering = m_Player.PlayerConfig.AnisotropicFiltering;
		m_Toggle.isOn = QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable ? true : false;

		UpdateColor();
	}
	public override void OnValueChange()
	{
		base.OnValueChange();
		QualitySettings.anisotropicFiltering = m_Toggle.isOn ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;
		m_Player.PlayerConfig.AnisotropicFiltering = QualitySettings.anisotropicFiltering;
	}
}

