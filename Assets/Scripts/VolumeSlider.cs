using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : SliderScript
{
	void Start()
	{
		base.Init();
		m_Slider.value = AudioListener.volume;//TODO: Multiplayer
	}
	override public void OnDrag(PointerEventData eventData)
	{
		AudioListener.volume = m_Slider.value; //TODO: Multiplayer
	}
}