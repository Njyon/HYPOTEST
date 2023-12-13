using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class TitelScreen : UIBase
{
    [SerializeField] TMPro.TextMeshProUGUI titel;
    [SerializeField] TMPro.TextMeshProUGUI pressText;
	[SerializeField] Animator animController;

    async void Awake()
    {
        LoadedUI();

        await new WaitForSeconds(0.1f);

        InputSystem.onAnyButtonPress.CallOnce(ButtonPressed);
    }

	void Start()
	{

	}

	void ButtonPressed(InputControl e)
	{
		UIManager.Instance.LoadMainMenu();
	}

	private void StartRemovingTitelScreen()
	{
		animController?.SetTrigger("Trigger");
	}

	public void RemoveTitelScreen()
    {
        RemoveUI();
    }

	public override void StartRemovingUI()
	{
		StartRemovingTitelScreen();
	}
}
