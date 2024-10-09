using TMPro;
using UnityEngine;

public abstract class Dropdown : CustomUI
{
	protected TMP_Dropdown m_Dropdown;
	protected override void Init()
	{
		base.Init();
		m_Dropdown = GetComponent<TMP_Dropdown>();
		m_Dropdown.ClearOptions();

		m_Dropdown.onValueChanged.AddListener(delegate { OnValueChange(); });
	}
	public virtual void OnValueChange()
	{
		if (m_Player == null || m_Dropdown == null)        //OnValueChange gets called on startup, prevent exeptions
			return;
	}
}
