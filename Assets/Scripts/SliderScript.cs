using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SliderScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, ISaveable
{
	protected Slider m_Slider;
	protected PlayerScript m_Player;

	protected void Init()
	{
		m_Slider = GetComponent<Slider>();
		m_Player = GameManagerScript.instance.PlayerScripts[0];
	}

	public void OnBeginDrag(PointerEventData eventData) { }
	public abstract void OnDrag(PointerEventData eventData);
	public void OnEndDrag(PointerEventData eventData) { }

	public object CaptureState()
	{
		return new Dictionary<string, object>
		{
			{ "slider", m_Slider.value }
		};
	}
	public void RestoreState(object state)
	{
		m_Slider.value = (float)((Dictionary<string, object>)state)["slider"];
	}
}