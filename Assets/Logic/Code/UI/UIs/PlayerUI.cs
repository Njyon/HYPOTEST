using MoreMountains.Feedbacks;
using MyBox;
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
	[SerializeField] CanvasGroup styleRankingGroup;
	[SerializeField] Image styleRankImage;
	[SerializeField] Image styleRankImageOutline;
	[SerializeField] MMF_Player styleRankUpFeedback;
	[SerializeField] MMF_Player styleRankDownFeedback;
	[SerializeField] MMF_Player showStyleRankFeedback;
	[SerializeField] MMF_Player hideStyleRankFeedback;

	[Header("Weapons")]
	[SerializeField] List<GameObject> weaponAnkers;
	List<WeaponVisualizer> weaponUIs = new List<WeaponVisualizer>();

	[Header("Combo")]
	[SerializeField] TMPro.TextMeshProUGUI comboText;
	[SerializeField] MMF_Player comboAddFeedback;
	[SerializeField] MMF_Player comboEndFeedback;
	[SerializeField] MMF_Player comboBreakFeedback;
	[SerializeField] MMF_Player comboShowFeedback;

	[Header("Dodge")]
	[SerializeField] RectTransform dodgeHolder;
	[SerializeField] DodgebarElemnt dodgeBarPrefab;
	List<Image> dodgeBars;

	[Header("Ult")]
	[SerializeField] Image ultUIElement;
	[SerializeField] Image ultUIElementBackground;
	[SerializeField] MMF_Player showUltUI;
	[SerializeField] MMF_Player hideUltUI;

	PlayerGameCharacter gameCharacter;

	void Awake()
	{
		ultUIElement.SetAlpha(0f);
		ultUIElementBackground.SetAlpha(0f);

		LoadedUI();
		styleRankingGroup.alpha = 0f;
	}

	public void Init(PlayerGameCharacter playerCharacter)
	{
		gameCharacter = playerCharacter;

		SetRankImage();
		gameCharacter.CombatRatingComponent.onStyleRankingChanged += OnStyleRankingChanged;
		gameCharacter.CombatRatingComponent.onCurrentValueChange += OnStyleValueChanged;
		gameCharacter.CombatComponent.onComboCountChanged += OnComboCountChanged;
		gameCharacter.onGameCharacterAggroChanged += OnAggroChanged;
		InitHealthbar();

		for (int i = 0; i < gameCharacter.CombatComponent.Weapons.Length; i++)
		{
			if (gameCharacter.CombatComponent.Weapons[i] == null) continue;
			GameObject uiElement = GameObject.Instantiate(gameCharacter.CombatComponent.Weapons[i].UIElement, weaponAnkers[i].transform);
			WeaponVisualizer weaponVisualizer = uiElement.GetComponent<WeaponVisualizer>();
			weaponVisualizer.InitWeaponVisualizer(gameCharacter.CombatComponent.Weapons[i].Weapon, gameCharacter);
			weaponUIs.Add(weaponVisualizer);
		}

		gameCharacter.CombatComponent.onWeaponChanged += OnWeaponChanged;
		OnWeaponChanged(gameCharacter.CombatComponent.CurrentWeapon, null, gameCharacter);

		comboText.alpha = 0f;

		dodgeBars = new List<Image>();
		for (int i = 0; i < gameCharacter.GameCharacterData.MaxDodgeAmount; i++)
		{
			dodgeBars.Add(GameObject.Instantiate(dodgeBarPrefab, dodgeHolder).dodgebar);
			dodgeBars[i].fillAmount = 1;
		}
		gameCharacter.CombatComponent.onDodgeLeftChanged += OnDodgeLeftChanged;
	}

	void OnDestroy()
	{
		if (gameCharacter != null)
		{
			gameCharacter.CombatRatingComponent.onStyleRankingChanged -= OnStyleRankingChanged;
			gameCharacter.CombatRatingComponent.onCurrentValueChange -= OnStyleValueChanged;
			gameCharacter.CombatComponent.onComboCountChanged -= OnComboCountChanged;
			gameCharacter.onGameCharacterAggroChanged -= OnAggroChanged;
			gameCharacter.CombatComponent.onWeaponChanged -= OnWeaponChanged;
			 if (gameCharacter.CombatComponent.CurrentWeapon != null) gameCharacter.CombatComponent.CurrentWeapon.onUltChargeValueChanged -= OnUltChargeValueChanged;
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

	void Update()
	{
		UpdateHealthbar();

		if (gameCharacter != null && gameCharacter.CombatComponent != null)
		{
			if (gameCharacter.CombatComponent.DodgeRecoveryTimer.IsRunning && dodgeBars.Count > gameCharacter.CombatComponent.DodgesLeft)
				dodgeBars[gameCharacter.CombatComponent.DodgesLeft].fillAmount = gameCharacter.CombatComponent.GetDodgeRecovertTimerProgress();
		}
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
			styleRankDownFeedback.PlayFeedbacks();

		}
	}

	void SetRankImage()
	{
		styleRankImageOutline.sprite = gameCharacter.CombatRatingComponent.StyleRanks[gameCharacter.CombatRatingComponent.CurrentStyleRankIndex].StyleImageOutline;
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

	void OnDodgeLeftChanged(int newDodgeAmount, int oldDodgeAmount)
	{
		if (newDodgeAmount < oldDodgeAmount)
		{
			dodgeBars[oldDodgeAmount - 1].fillAmount = 0f;
			if (dodgeBars.Count > oldDodgeAmount) dodgeBars[oldDodgeAmount].fillAmount = 0f;
		}
		if (newDodgeAmount > oldDodgeAmount)
		{
			dodgeBars[oldDodgeAmount].fillAmount = 1f;
		}
	}

	void OnAggroChanged()
	{
		if (gameCharacter != null)
		{
			if (gameCharacter.CharacterHasAggro)
			{
				// Show Ranking
				showStyleRankFeedback?.PlayFeedbacks();
			}
			else
			{
				// Hide Ranking
				hideStyleRankFeedback?.PlayFeedbacks();
			}
		}
	}

	void OnWeaponChanged(WeaponBase newWeapon, WeaponBase oldWeapon, GameCharacter gameCharacter)
	{
		if (oldWeapon != null)
		{
			oldWeapon.onUltChargeValueChanged -= OnUltChargeValueChanged;
		}

		if (newWeapon != null)
		{
			newWeapon.onUltChargeValueChanged += OnUltChargeValueChanged;
			ultUIElement.sprite = newWeapon.WeaponData.WeaponImage;
			OnUltChargeValueChanged(newWeapon.UltCharge, 0);
		}else
		{
			// Hide UI if weapon is null
			OnUltChargeValueChanged(0, 0);
		}
	}

	void OnUltChargeValueChanged(float newCharge, float oldCharge)
	{
		if (newCharge >= gameCharacter.CombatComponent.CurrentWeapon.WeaponData.MaxUltChargeAmount)
		{
			showUltUI.PlayFeedbacks();
		}
		else
		{
			hideUltUI.PlayFeedbacks();	
		}
	}
}
