using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SensitivitySlider : SliderScript
{
	void Awake()
	{
		base.Init();
		if (value == 0)
		{
			m_Slider.value = value = m_Player.LookSensitivity;  //If sensitivity is unset or load failed, set to default
		}
		else
		{
			m_Player.LookSensitivity = m_Slider.value = value;
		}   //TODO: Multiplayer
	}
	override public void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		m_Player.LookSensitivity = value = m_Slider.value; //TODO: Multiplayer
	}
	override public void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		new FileDataHandler("config.cfg", "", false).SaveData(this);
	}
}