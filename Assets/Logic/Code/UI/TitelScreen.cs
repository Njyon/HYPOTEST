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

    [SerializeField] MMF_Player fadeIn;
    [SerializeField] MMF_Player fadeOut;

    public MMF_Player FadeOut { get { return fadeOut; } }

    async void Awake()
    {
        LoadedUI();

		titel.alpha = 0f;
        pressText.alpha = 0f;
        fadeIn.PlayFeedbacks();

        await new WaitForSeconds(0.1f);

        InputSystem.onAnyButtonPress.CallOnce(ButtonPressed);
    }

    void ButtonPressed(InputControl e)
	{
		UIManager.Instance.LoadMainMenu();
	}

	private void StartRemovingTitelScreen()
	{
		fadeIn.StopFeedbacks();
		fadeOut.PlayFeedbacks();
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
