using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BrightnessSlider : SliderScript
{
	private Volume brightness;
	ColorAdjustments colorAdjustments;

	void Start()
	{
		base.Init();

		m_Slider.minValue = PlayerConfig.MinBrightness;
		m_Slider.maxValue = PlayerConfig.MaxBrightness;
		m_Slider.value = m_Player.PlayerConfig.Brightness;

		m_ValueText.text = m_Slider.value.ToString().Replace(',', '.');

		brightness = GameManager.Instance.Brightness;
		brightness.profile.TryGet(out colorAdjustments);

		colorAdjustments.postExposure.Override(m_Slider.value);
	}
	override public void OnValueChange()
	{
		if (brightness == null)
			return;

		base.OnValueChange();

		brightness.profile.TryGet(out colorAdjustments);
		colorAdjustments.postExposure.Override(m_Slider.value);
	}
	override public void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		m_Player.PlayerConfig.Brightness = m_Slider.value;
	}
}