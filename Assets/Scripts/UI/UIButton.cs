using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class UIButton : CustomUI
{
	public TextMeshProUGUI DisplayText { get; private set; }
	protected void Start()
	{
		base.Init();
		DisplayText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
	}

	public virtual void OnClick() { }
}