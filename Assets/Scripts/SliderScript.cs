using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public abstract class SliderScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	protected Slider m_Slider;
	[SerializeField] protected float value; //All the time shoud be the same as m_Slider.value (for saving purpose only)
	protected PlayerScript m_Player;

	protected void Init()
	{
		m_Slider = GetComponent<Slider>();
		m_Player = GameManagerScript.instance.PlayerScripts[0];

		new FileDataHandler("config.cfg", "", false).LoadData<SliderScript>(this);
	}

	public virtual void OnBeginDrag(PointerEventData eventData) { }
	public virtual void OnDrag(PointerEventData eventData) { }
	public virtual void OnEndDrag(PointerEventData eventData)
	{
		value = m_Slider.value;
	}

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