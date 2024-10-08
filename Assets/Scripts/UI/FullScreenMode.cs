using TMPro;
using UnityEngine;

public class FullScreen : Dropdown
{
	void Start()
	{
		base.Init();

		Screen.fullScreenMode = m_Player.PlayerConfig.FullScreenMode;

		//Do not change the order of the options, order determined by the FullScreenMode enum
		m_Dropdown.options.Add(new TMP_Dropdown.OptionData("Exclusive Fullscreen"));
		m_Dropdown.options.Add(new TMP_Dropdown.OptionData("Borderless Window"));
		m_Dropdown.options.Add(new TMP_Dropdown.OptionData("Maximized Window"));
		m_Dropdown.options.Add(new TMP_Dropdown.OptionData("Windowed"));

		m_Dropdown.value = (int)Screen.fullScreenMode;
	}

	public override void OnValueChange()
	{
		base.OnValueChange();
		m_Player.PlayerConfig.FullScreenMode = Screen.fullScreenMode = (FullScreenMode)m_Dropdown.value;
	}
}