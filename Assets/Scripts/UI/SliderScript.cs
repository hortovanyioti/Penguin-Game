using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public abstract class SliderScript : CustomUI, IBeginDragHandler, IEndDragHandler
{
	protected Slider m_Slider;
	protected TextMeshProUGUI m_ValueText;

	protected override void Init()
	{
		base.Init();
		m_Slider = GetComponent<Slider>();
		m_ValueText = transform.Find("ValueText").GetComponent<TextMeshProUGUI>();

		m_Slider.onValueChanged.AddListener(delegate { OnValueChange(); });
	}

	public virtual void OnValueChange()
	{
		if (m_Player == null || m_Slider == null || m_ValueText == null)        //OnValueChange gets called on startup, prevent exeptions
			return;

		m_Slider.value = (float)Math.Round(m_Slider.value, 2);
		m_ValueText.text = m_Slider.value.ToString().Replace(',', '.');
	}
	public virtual void OnBeginDrag(PointerEventData eventData) { }
	public virtual void OnEndDrag(PointerEventData eventData) { }
}