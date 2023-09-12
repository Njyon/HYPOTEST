using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CombatRatingComponent : RecourceBase
{
	public delegate void StyleRankingChanged(int newRankIndex, int oldRankIndex);
	public StyleRankingChanged onStyleRankingChanged;

	PlayerGameCharacter gameCharacter; 
	List<StyleRankingScriptableObject> styleRanks = new List<StyleRankingScriptableObject>();
	float nextStyleRankLevelUp = 0;
	float nextStyleRankLeveldown = 0;
	int currentStyleRankIndex = 0;

	public List<StyleRankingScriptableObject> StyleRanks { get { return styleRanks; } }
	public float NextStyleRankLevelUp { get { return nextStyleRankLevelUp; } }
	public float NextStyleRankLeveldown { get { return nextStyleRankLeveldown; } }
	public int CurrentStyleRankIndex { 
		get { return currentStyleRankIndex; } 
		protected set
		{
			if (styleRanks == null || styleRanks.Count <= 0) return;
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

		base.Update(deltaTime);

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

	public void AddRatingOnHit()
	{
		AttackAnimationData newestAttack = gameCharacter.CombatComponent.PreviousAttacks[0];
		int numberOfLastAttackInList = gameCharacter.CombatComponent.PreviousAttacks.ContainedItemNum(newestAttack);
		float rating = newestAttack.extraData.Rating / numberOfLastAttackInList;

		gameCharacter.CombatComponent.CurrentWeapon.Charge -= newestAttack.extraData.discharge;
		AddCurrentValue(rating);
		foreach (ScriptableWeapon weapon in gameCharacter.CombatComponent.Weapons)
		{
			if (weapon == null || weapon.Weapon == null) continue;
			if (weapon.Weapon == gameCharacter.CombatComponent.CurrentWeapon) continue;
			weapon.Weapon.Charge = newestAttack.extraData.Rating / gameCharacter.CombatComponent.EquipedWeapons;
		}
	}

	public void OnGotHit(GameCharacter damageInitiator, float damage)
	{
		if (damage > 0)
		{
			CurrentValue = CurrentValue / 2;
			foreach (ScriptableWeapon sWeapon in gameCharacter.CombatComponent.Weapons)
			{
				sWeapon.Weapon.Charge /= 2;
			}
			UpdateStyleRank();
		}
	}
}
