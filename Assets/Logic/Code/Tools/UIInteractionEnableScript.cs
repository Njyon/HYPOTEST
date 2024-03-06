using Michsky.UI.Reach;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteractionEnableScript : MonoBehaviour
{
    [SerializeField] List<ButtonManager> Buttons = new List<ButtonManager>();
    [SerializeField] bool executeOnStart = false;
	[ConditionalField("executeOnStart")]
	[SerializeField] bool startEnabledState = true;

	void Awake()
	{
		if (executeOnStart)
        {
			foreach (ButtonManager button in Buttons)
			{
				button.Interactable(startEnabledState);
			}
		}
	}

	public void EnableButtons(bool enabled)
    {
        foreach (ButtonManager button in Buttons)
        {
            button.Interactable(enabled);
        }
    }
}
