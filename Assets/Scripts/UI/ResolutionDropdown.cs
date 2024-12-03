using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionDropdown : Dropdown
{
	private Resolution[] resolutions;
	private List<Resolution> filteredResolutions;
	private RefreshRate currentRefreshRate;

	void Awake()  //!Important to call Init() in Awake() because OnEnable() is called after Awake()
	{
		base.Init();

		Screen.SetResolution(
			m_Player.PlayerConfig.ScreenWidth,
			m_Player.PlayerConfig.ScreenHeight,
			m_Player.PlayerConfig.FullScreenMode,
			m_Player.PlayerConfig.RefreshRate);
	}
	void OnEnable()
	{
		if (m_Dropdown == null || m_Player == null)
		{
			return;
		}

		resolutions = Screen.resolutions;
		filteredResolutions = new List<Resolution>();
		currentRefreshRate = Screen.currentResolution.refreshRateRatio;

		for (int i = 0; i < resolutions.Length; i++)
		{
			if (resolutions[i].refreshRateRatio.Equals(currentRefreshRate))
			{
				filteredResolutions.Add(resolutions[i]);
			}
		}

		for (int i = 0; i < filteredResolutions.Count; i++)
		{
			m_Dropdown.options.Add(new TMP_Dropdown.OptionData(filteredResolutions[i].width + "x" + filteredResolutions[i].height));
			if (filteredResolutions[i].width == m_Player.PlayerConfig.ScreenWidth && filteredResolutions[i].height == m_Player.PlayerConfig.ScreenHeight)
			{
				m_Dropdown.value = i;
			}
		}
	}
	public override void OnValueChange()
	{
		base.OnValueChange();

		m_Player.PlayerConfig.ScreenWidth = filteredResolutions[m_Dropdown.value].width;
		m_Player.PlayerConfig.ScreenHeight = filteredResolutions[m_Dropdown.value].height;

		Screen.SetResolution(
			m_Player.PlayerConfig.ScreenWidth,
			m_Player.PlayerConfig.ScreenHeight,
			m_Player.PlayerConfig.FullScreenMode,
			m_Player.PlayerConfig.RefreshRate);

	}
}

