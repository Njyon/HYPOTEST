using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	[Header("StyleRanking")]
	[SerializeField] Image styleRankImage;
	[SerializeField] MMF_Player styleRankUpFeedback;
	[SerializeField] MMF_Player styleRanDownFeedback;

	[Header("Weapons")]
	[SerializeField] List<GameObject> weaponAnkers;
	List<WeaponVisualizer> weaponUIs = new List<WeaponVisualizer>();

	[Header("Combo")]
	[SerializeField] TMPro.TextMeshProUGUI comboText;
	[SerializeField] MMF_Player comboAddFeedback;
	[SerializeField] MMF_Player comboEndFeedback;
	[SerializeField] MMF_Player comboBreakFeedback;
	[SerializeField] MMF_Player comboShowFeedback;

	PlayerGameCharacter gameCharacter;

	public void Init(PlayerGameCharacter playerCharacter)
	{
		gameCharacter = playerCharacter;

		SetRankImage();
		gameCharacter.CombatRatingComponent.onStyleRankingChanged += OnStyleRankingChanged;
		gameCharacter.CombatRatingComponent.onCurrentValueChange += OnStyleValueChanged;
		gameCharacter.CombatComponent.onComboCountChanged += OnComboCountChanged;
		InitHealthbar();

		for (int i = 0; i < gameCharacter.CombatComponent.Weapons.Length; i++)
		{
			if (gameCharacter.CombatComponent.Weapons[i] == null) continue;
			GameObject uiElement = GameObject.Instantiate(gameCharacter.CombatComponent.Weapons[i].UIElement, weaponAnkers[i].transform);
			WeaponVisualizer weaponVisualizer = uiElement.GetComponent<WeaponVisualizer>();
			weaponVisualizer.InitWeaponVisualizer(gameCharacter.CombatComponent.Weapons[i].Weapon, gameCharacter);
			weaponUIs.Add(weaponVisualizer);
		}

		comboText.alpha = 0f;
	}

	void OnDestroy()
	{
		if (gameCharacter != null)
		{
			gameCharacter.CombatRatingComponent.onStyleRankingChanged -= OnStyleRankingChanged;
		}
		weaponUIs.Clear();
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

	void OnStyleRankingChanged(int newRankIndex, int oldRankIndex)
	{
		if (oldRankIndex < newRankIndex)
		{
			// RankUp
			SetRankImage();
			styleRankUpFeedback.PlayFeedbacks();
		}
		else if (oldRankIndex > newRankIndex)
		{
			// RankDown
			SetRankImage();
			styleRanDownFeedback.PlayFeedbacks();

		}
	}

	void SetRankImage()
	{
		styleRankImage.sprite = gameCharacter.CombatRatingComponent.StyleRanks[gameCharacter.CombatRatingComponent.CurrentStyleRankIndex].StyleImage;
		styleRankImage.type = Image.Type.Filled;
		styleRankImage.fillMethod = Image.FillMethod.Vertical;
		SetRankImageFill();
	}

	void SetRankImageFill()
	{
		float fillValue = Ultra.Utilities.Remap(gameCharacter.CombatRatingComponent.CurrentValue, gameCharacter.CombatRatingComponent.NextStyleRankLeveldown, gameCharacter.CombatRatingComponent.NextStyleRankLevelUp, 0, 1); 
		fillValue = Mathf.Clamp(fillValue, 0, 1);
		styleRankImage.fillAmount = fillValue;
	}

	void OnStyleValueChanged(float newValue, float oldValue)
	{
		SetRankImageFill();
	}

	void OnComboCountChanged(int newComboCount, int oldComboCount, EComboChangeType type)
	{
		switch(type)
		{
			case EComboChangeType.Break:
				if (oldComboCount == 0) return;
				comboBreakFeedback?.PlayFeedbacks();
					break;
			case EComboChangeType.TimerEnds:
				if (oldComboCount == 0) return;
				comboEndFeedback?.PlayFeedbacks();
					break;
			default:
				comboText.text = newComboCount + "x";
				comboAddFeedback?.PlayFeedbacks();
				if (oldComboCount == 0) comboShowFeedback?.PlayFeedbacks();
				break;
		}
	}
}
