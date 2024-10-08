using UnityEngine;
using UnityEngine.UI;

public class SettingsTabSelector : MonoBehaviour
{
	[SerializeField] private GameObject TabCollection;
	private GameObject[] Tabs;
	private GameObject[] TabSelectorButtons;
	private UnityEngine.UI.Image[] ButtonBorders;
	private int ActiveTabIndex;

	void Start()
	{
		int tabCount = TabCollection.transform.childCount;
		int buttonCount = this.transform.childCount;

#if UNITY_EDITOR
		if (tabCount != buttonCount)
		{
			Debug.LogError("Settings: Tabs.Length != TabSelectorButtons.Length");
			return;
		}
#endif

		//Load Tabs
		Tabs = new GameObject[tabCount];
		for (int i = 0; i < tabCount; i++)
		{
			Tabs[i] = TabCollection.transform.GetChild(i).gameObject;
		}

		//Load TabSelectorButtons
		TabSelectorButtons = new GameObject[buttonCount];
		for (int i = 0; i < buttonCount; i++)
		{
			TabSelectorButtons[i] = this.transform.GetChild(i).gameObject;
		}

		//Load ButtonBorders
		ButtonBorders = new UnityEngine.UI.Image[buttonCount];
		for (int i = 0; i < buttonCount; i++)
		{
			ButtonBorders[i] = TabSelectorButtons[i].transform.Find("SelectionBorder").GetComponent<UnityEngine.UI.Image>();
		}

		//Set default tab
		ActiveTabIndex = 0;
		SelectTab(TabSelectorButtons[ActiveTabIndex]);

		SubAllButtons();
	}

	public void SelectTab(GameObject triggerButton) //Set active tab and selection border
	{
		Tabs[ActiveTabIndex].SetActive(false);
		ButtonBorders[ActiveTabIndex].enabled = false;

		ActiveTabIndex = triggerButton.transform.GetSiblingIndex();

		Tabs[ActiveTabIndex].SetActive(true);
		ButtonBorders[ActiveTabIndex].enabled = true;
	}
	public void SubAllButtons()
	{
		for (int i = 0; i < Tabs.Length; i++)
		{
			int index = i;	//!Important
			TabSelectorButtons[i].GetComponent<Button>().onClick.AddListener(() => SelectTab(TabSelectorButtons[index]));
		}
	}
}
