using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class PlayerUI : UIBase
{
	[Header("Healthbar")]
	[SerializeField] Image healthbar;
	[SerializeField] Image healthbarBackGround;
	float healthbarFillTarget;
	float healthbarBackGroundFillTarget;
	[SerializeField] float healthbarSpeed = 2f;
	[SerializeField] float healthbarBackGroundSpeed = 1f;

	PlayerGameCharacter gameCharacter;

	public void Init(PlayerGameCharacter playerCharacter)
	{
		gameCharacter = playerCharacter;

		InitHealthbar();
	}

	void InitHealthbar()
	{
		healthbarFillTarget = 1;
		healthbarBackGroundFillTarget = 1;
		SetHealthBarTargetValue(gameCharacter.Health.CurrentValue);
		gameCharacter.Health.onCurrentValueChange += OnHealthValueChanged;
		SetHealthBarSize(gameCharacter.Health.MaxValue);
		gameCharacter.Health.onMaxValueChange += OnHealthMaxValueChanged;
	}

	void SetHealthBarSize(float value)
	{
		healthbar.rectTransform.sizeDelta = new Vector2(value, healthbar.rectTransform.sizeDelta.y);
		healthbarBackGround.rectTransform.sizeDelta = new Vector2(value, healthbar.rectTransform.sizeDelta.y);
	}

	void Awake()
	{
		LoadedUI();	
	}

	void Update()
	{
		UpdateHealthbar();
	}

	void UpdateHealthbar()
	{
		healthbar.fillAmount = Mathf.Lerp(healthbar.fillAmount, healthbarFillTarget, Time.deltaTime * healthbarSpeed);

		if (healthbar.fillAmount >= healthbarBackGround.fillAmount)
		{
			healthbarBackGround.fillAmount = healthbar.fillAmount;
		}
		else
		{
			healthbarBackGround.fillAmount = Mathf.Lerp(healthbarBackGround.fillAmount, healthbarBackGroundFillTarget, Time.deltaTime * healthbarBackGroundSpeed);
		}
	}

	public override void StartRemovingUI()
	{
		RemoveUI();
	}

	void OnHealthValueChanged(float newValue, float oldValue)
	{
		SetHealthBarTargetValue(newValue);
	}

	void OnHealthMaxValueChanged(float newValue, float oldValue)
	{
		SetHealthBarSize(newValue);
	}

	void SetHealthBarTargetValue(float newValue)
	{
		healthbarFillTarget = newValue / gameCharacter.Health.MaxValue;
		healthbarBackGroundFillTarget = healthbarFillTarget;
	}
}
