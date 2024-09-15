using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : SliderScript
{
    void Start()
    {
        base.Init();

        m_Slider.minValue = m_Player.PlayerConfig.MinGlobalVolume;
        m_Slider.maxValue = m_Player.PlayerConfig.MaxGlobalVolume;
        m_Slider.value = m_Player.PlayerConfig.GlobalVolume;

        m_ValueText.text=m_Slider.value.ToString().Replace(',','.');
    }
    override public void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        AudioListener.volume = m_Slider.value;
    }
    override public void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        m_Player.PlayerConfig.GlobalVolume = m_Slider.value;
    }
}