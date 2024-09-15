using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public abstract class SliderScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    protected Slider m_Slider;
    protected PlayerScript m_Player;
    protected TextMeshProUGUI m_ValueText;

    protected void Init()
    {
        m_Slider = GetComponent<Slider>();
        m_Player = GameManager.Instance.PlayerScripts[0]; //TODO: Multiplayer
        m_ValueText = transform.Find("ValueText").GetComponent<TextMeshProUGUI>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData) { }
    public virtual void OnDrag(PointerEventData eventData)
    {
        m_Slider.value = (float)Math.Round(m_Slider.value, 2);
        m_ValueText.text = m_Slider.value.ToString().Replace(',', '.');
    }
    public virtual void OnEndDrag(PointerEventData eventData) { }

    public void OnValueChange()
    {
        if (m_Player != null && m_Slider != null && m_ValueText != null)
        {
            OnDrag(null);
        }
    }
}