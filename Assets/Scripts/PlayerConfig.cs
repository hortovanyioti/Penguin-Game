using System;
using UnityEngine;
public class PlayerConfig
{
	//GENERAL

	//GAMEPLAY

	//DISPLAY

	[SerializeField] private FullScreenMode fullScreenMode;
	public FullScreenMode FullScreenMode
	{
		get => fullScreenMode;
		set
		{
			fullScreenMode = value;
		}
	}

	[SerializeField] private int vSync;
	public int VSync
	{
		get => vSync;
		set
		{
			vSync = value;
		}
	}

	[SerializeField] private int screenWidth;
	public int ScreenWidth
	{
		get => screenWidth;
		set
		{
			screenWidth = value;
		}
	}

	[SerializeField] private int screenHeight;
	public int ScreenHeight
	{
		get => screenHeight;
		set
		{
			screenHeight = value;
		}
	}

	[SerializeField] private RefreshRate refreshRate;
	public RefreshRate RefreshRate
	{
		get => refreshRate;
		set
		{
			refreshRate = value;
		}
	}

	public const float MinBrightness = -5f;
	public const float MaxBrightness = 5f;
	public const float DefaultBrightness = 1f;

	[SerializeField] private float brightness;
	public float Brightness
	{
		get => brightness;
		set
		{
			if (value < MinBrightness || value > MaxBrightness)
			{
				brightness = DefaultBrightness;
			}
			else
			{
				brightness = (float)Math.Round(value, 2);
			}
		}
	}

	//GRAPHICS

	[SerializeField] private AnisotropicFiltering anisotropicFiltering;
	public AnisotropicFiltering AnisotropicFiltering
	{
		get => anisotropicFiltering;
		set
		{
			anisotropicFiltering = value;
		}
	}

	public const int DefaultAntiAliasing = 4;

	[SerializeField] private int antiAliasing;
	public int AntiAliasing
	{
		get => antiAliasing;
		set
		{
			antiAliasing = value;
		}
	}

	//AUDIO

	public const float MinGlobalVolume = 0f;
	public const float MaxGlobalVolume = 3f;
	public const float DefaultGlobalVolume = 1f;

	[SerializeField] private float globalVolume;
	public float GlobalVolume
	{
		get => globalVolume;
		set
		{
			if (value < MinGlobalVolume || value > MaxGlobalVolume)
			{
				globalVolume = DefaultGlobalVolume;
			}
			else
			{
				globalVolume = (float)Math.Round(value, 2);
			}
		}
	}

	//CONTROLS

	public const float MinLookSensitivity = 0.1f;
	public const float MaxLookSensitivity = 100f;
	public const float DefaultLookSensitivity = 20f;

	[SerializeField] private float lookSensitivity;
	public float LookSensitivity
	{
		get => lookSensitivity;
		set
		{
			if (value < MinLookSensitivity || value > MaxLookSensitivity)
			{
				lookSensitivity = DefaultLookSensitivity;
			}
			else
			{
				lookSensitivity = (float)Math.Round(value, 2);
			}
		}
	}

	//KEYBINDS

	//OTHER

	public void SetDefault()
	{
		LookSensitivity = DefaultLookSensitivity;
		GlobalVolume = DefaultGlobalVolume;
		AntiAliasing = DefaultAntiAliasing;
		Brightness = DefaultBrightness;
	}
	public void Save()
	{
		new FileDataHandler("config.cfg", "", false).SaveData(this, true);
	}
}