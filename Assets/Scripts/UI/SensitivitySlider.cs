using UnityEngine.EventSystems;

public class SensitivitySlider : SliderScript
{
    void Start()
    {
        base.Init();

        m_Slider.minValue = PlayerConfig.MinLookSensitivity;
        m_Slider.maxValue = PlayerConfig.MaxLookSensitivity;
        m_Slider.value = m_Player.PlayerConfig.LookSensitivity;

        m_ValueText.text = m_Slider.value.ToString().Replace(',', '.');
    }

    override public void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        m_Player.PlayerConfig.LookSensitivity = m_Slider.value;
    }
}