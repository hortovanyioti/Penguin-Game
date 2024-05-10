
using System;
using UnityEngine;

public class PlayerConfig
{
    public readonly float MinLookSensitivity = 0.1f;
    public readonly float MaxLookSensitivity = 100f;
    public readonly float DefaultLookSensitivity = 20f;

    public readonly float MinGlobalVolume = 0f;
    public readonly float MaxGlobalVolume = 3f;
    public readonly float DefaultGlobalVolume = 1f;

    [SerializeField] private float lookSensitivity;
    public float LookSensitivity
    {
        get { return lookSensitivity; }
        set
        {
            if (value < MinLookSensitivity || value > MaxLookSensitivity)
            {
                lookSensitivity = DefaultLookSensitivity;
            }
            else
            {
                lookSensitivity = (float)Math.Round(value,2);
            }
        }
    }

    [SerializeField] private float globalVolume;
    public float GlobalVolume
    {
        get { return globalVolume; }
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
    public void SetDefault()
    {
        LookSensitivity = DefaultLookSensitivity;
        GlobalVolume = DefaultGlobalVolume;
    }
    public void Save()
    {
        new FileDataHandler("config.cfg", "", false).SaveData(this,true);
    }
}