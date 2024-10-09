using UnityEngine;

public class OptionsButton : UIButton
{
	public GameObject OptionsMenu;
	override public void OnClick()
	{
		base.OnClick();
		OptionsMenu.SetActive(!OptionsMenu.activeInHierarchy);  //Toggle active state
	}
}