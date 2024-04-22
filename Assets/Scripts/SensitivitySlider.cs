using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SensitivitySlider : SliderScript
{
	void Start()
	{
		base.Init();
		m_Slider.value = m_Player.LookSensitivity;//TODO: Multiplayer
	}
	override public void OnDrag(PointerEventData eventData)
	{
		m_Player.LookSensitivity = m_Slider.value; //TODO: Multiplayer
	}
}