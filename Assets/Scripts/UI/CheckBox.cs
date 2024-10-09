using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public abstract class CheckBox : CustomUI
{
	protected Toggle m_Toggle;
	protected override void Init()
	{
		base.Init();
		m_Toggle = GetComponent<Toggle>();
		m_Toggle.onValueChanged.AddListener(delegate { OnValueChange(); });
	}
	public virtual void OnValueChange()
	{
		if (m_Player == null || m_Toggle == null)        //OnValueChange gets called on startup, prevent exeptions
			return;

		UpdateColor();
	}
	public void UpdateColor()
	{
		if (m_Toggle.isOn)
		{
			m_Toggle.targetGraphic.color = Color.green;
		}
		else
		{
			m_Toggle.targetGraphic.color = Color.red;
		}
	}
}
