using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatRatingComponent : RecourceBase
{
	public delegate void StyleRankingChanged(int newRankIndex, int oldRankIndex);
	public StyleRankingChanged onStyleRankingChanged;

	PlayerGameCharacter gameCharacter; 
	List<StyleRankingScriptableObject> styleRanks = new List<StyleRankingScriptableObject>();
	float nextStyleRankLevelUp = 0;
	float nextStyleRankLeveldown = 0;
	int currentStyleRankIndex = 0;
	float limiter = 0.2f;

	public List<StyleRankingScriptableObject> StyleRanks { get { return styleRanks; } }
	public float NextStyleRankLevelUp { get { return nextStyleRankLevelUp; } }
	public float NextStyleRankLeveldown { get { return nextStyleRankLeveldown; } }
	public int CurrentStyleRankIndex { 
		get { return currentStyleRankIndex; } 
		protected set
		{
			if (styleRanks == null || styleRanks.Count <= 0) return;
			value = Mathf.Clamp(value, 0, styleRanks.Count - 1);
			if (currentStyleRankIndex != value)
			{
				int oldValue = currentStyleRankIndex;
				currentStyleRankIndex = value;


				if (oldValue > currentStyleRankIndex)
				{
					// RankDown
					nextStyleRankLeveldown -= styleRanks[currentStyleRankIndex].PointsToLevelUp;
					nextStyleRankLevelUp -= styleRanks[oldValue].PointsToLevelUp;
					if (onStyleRankingChanged != null) onStyleRankingChanged(currentStyleRankIndex, oldValue);
				}
				else
				{
					// RankUp
					nextStyleRankLeveldown += styleRanks[oldValue].PointsToLevelUp;
					nextStyleRankLevelUp += styleRanks[currentStyleRankIndex].PointsToLevelUp;
					if (onStyleRankingChanged != null) onStyleRankingChanged(currentStyleRankIndex, oldValue);
				}
				ChangeValueChangePerSecond(0, styleRanks[currentStyleRankIndex].PointDecreaseStopTime, -styleRanks[currentStyleRankIndex].PointDecreasePerSecond);

				UpdateStyleRank();
			}
		}	
	}


	public CombatRatingComponent(float startValue, float maxValue = float.PositiveInfinity, float minValue = 0, float defaultValueChangePerSecond = 0) : base(startValue, maxValue, minValue, defaultValueChangePerSecond)
	{ }

	public void Init(PlayerGameCharacter gameCharacter)
	{
		this.gameCharacter = gameCharacter;
		styleRanks = GameAssets.Instance.styleRanks;
		if (styleRanks != null && styleRanks.Count > 0)
			nextStyleRankLevelUp = styleRanks[0].PointsToLevelUp;
	}

	public override void Update(float deltaTime)
	{
		if (gameCharacter == null) return;

		if (gameCharacter.CharacterHasAggro)
		{
			base.Update(deltaTime);
		}
	}

	public override void AddCurrentValue(float value)
	{
		base.AddCurrentValue(value);
		UpdateStyleRank();
	}

	private void UpdateStyleRank()
	{
		if (CurrentValue >= nextStyleRankLevelUp)
		{
			CurrentStyleRankIndex++;
		}
		else if (CurrentValue < nextStyleRankLeveldown)
		{
			CurrentStyleRankIndex--;
		}
	}

	public void AddRatingOnHit(float damage)
	{
		AttackAnimationData newestAttack = gameCharacter.CombatComponent.PreviousAttacks[0];
		int numberOfLastAttackInList = gameCharacter.CombatComponent.PreviousAttacks.ContainedItemNum(newestAttack);
		//float rating = newestAttack.extraData.Rating / numberOfLastAttackInList;
		float rating = Mathf.Clamp(((gameCharacter.CombatComponent.CurrentWeapon.CurrentAction.Action.GetActionRanting() * gameCharacter.CombatComponent.ComboCount) / numberOfLastAttackInList) / 5, 10, int.MaxValue);


		gameCharacter.CombatComponent.CurrentWeapon.Charge -= newestAttack.Action.GetActionDischarge();
		AddCurrentValue(rating);
		foreach (ScriptableWeapon weapon in gameCharacter.CombatComponent.Weapons)
		{
			if (weapon == null || weapon.Weapon == null) continue;
			if (weapon.Weapon == gameCharacter.CombatComponent.CurrentWeapon) continue;
			// was X in drawing, limits the Value of craking up to hard, Clamp tries to cap low and highs
			float chargeDelta = Mathf.Clamp((CurrentValue * limiter) / (gameCharacter.CombatComponent.EquipedWeapons - 1), 50, 200);
			//Ultra.Utilities.Instance.DebugLogOnScreen("ChargeDelta => " + chargeDelta, 1f, StringColor.Random());
			weapon.Weapon.Charge += chargeDelta;
		}
	}

	public void OnGotHit(GameCharacter damageInitiator, float damage, bool removeCharge)
	{
		if (damage > 0)
		{
			AddCurrentValue(-(CurrentValue / 2));
			if (removeCharge)
			{
				foreach (ScriptableWeapon sWeapon in gameCharacter.CombatComponent.Weapons)
				{
					if (sWeapon == null || sWeapon.Weapon == null) continue;
					sWeapon.Weapon.Charge -= 200;
				}
			}
			UpdateStyleRank();
		}
	}
}
