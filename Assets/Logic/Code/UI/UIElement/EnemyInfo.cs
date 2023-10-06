using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfo : MonoBehaviour
{
	[SerializeField] TMPro.TextMeshProUGUI damageText;
	[SerializeField] Image healthbar;
	[SerializeField] Image staggerBar;
	[SerializeField] float damageTime = 2f;

	GameCharacter gameCharacter;
	Ultra.Timer damageTimer;
	float currentDamage = 0;

	public void Init(GameCharacter character)
	{
		gameCharacter = character;

		if (damageTimer == null) 
			damageTimer = new Ultra.Timer(damageTime, true);

		gameCharacter.Health.onCurrentValueChange += OnHealthValueChange;
		gameCharacter.StaggerComponent.onCurrentValueChange += OnStaggerValueChange;
		damageTimer.onTimerFinished += OnTimerFinished;

		OnHealthValueChange(gameCharacter.Health.CurrentValue, 0);
		damageText.text = 0.ToString("F3");
		OnStaggerValueChange(gameCharacter.StaggerComponent.CurrentValue, 0);
	}

	private void OnDisable()
	{
		if (gameCharacter != null)
		{
			gameCharacter.Health.onCurrentValueChange -= OnHealthValueChange;
			gameCharacter.StaggerComponent.onCurrentValueChange -= OnStaggerValueChange;
		}
	
		if (damageTimer != null)
		{
			damageTimer.onTimerFinished -= OnTimerFinished;
		}

		gameCharacter = null;
	}

	private void Update()
	{
		if (gameCharacter == null) return;
		if (damageTimer != null) damageTimer.Update(Time.deltaTime);
		

		Vector3 screenPos = Camera.main.WorldToScreenPoint(gameCharacter.MovementComponent.CharacterCenter);
		screenPos += gameCharacter.GameCharacterData.EnemyInfoOffset;
		transform.position = screenPos;
	}

	void OnHealthValueChange(float newValue, float oldValue)
	{
		if (gameCharacter == null) return;

		healthbar.fillAmount = newValue / gameCharacter.Health.MaxValue;
		if (newValue < oldValue)
		{
			// Life Subtraction
			currentDamage += Mathf.Abs(newValue);
			damageText.text = currentDamage.ToString("F3");
			damageTimer.Start();
		}
	}

	void OnTimerFinished()
	{
		currentDamage = 0;
		damageText.text = 0.ToString("F3");
	}

	void OnStaggerValueChange(float newValue, float oldValue)
	{
		if (gameCharacter.StaggerComponent.MaxValue <= 0)
		{
			staggerBar.fillAmount = 0;
			return;
        }
		staggerBar.fillAmount = newValue / gameCharacter.StaggerComponent.MaxValue;
	}
}
