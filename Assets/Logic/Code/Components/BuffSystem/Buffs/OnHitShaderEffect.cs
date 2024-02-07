using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitShaderEffect : ABuff
{
	public OnHitShaderEffect(GameCharacter gameCharacter, float duration) : base(gameCharacter, duration)
	{
		var data = Ultra.HypoUttilies.GameMode.GetDefaultGameModeData();
		TurnOnMaterialEffect(data.hitShaderColor, data.hitShaderIntensity);
	}

	public override void Update(float deltaTime)
	{
		base.Update(Time.unscaledDeltaTime);
	}

	public override void BuffEnds()
	{
		TurnOnMaterialEffect(Color.white, 0);
	}

	void TurnOnMaterialEffect(Color color, float intensity)
	{
		foreach (SkinnedMeshRenderer smr in GameCharacter.SkinnedMeshRenderers)
		{
			foreach (Material mat in smr.materials)
			{
				mat.SetColor(GameCharacter.MaterialHitColorIndex, color);
				mat.SetFloat(GameCharacter.MaterialHitIntensityIndex, intensity);
			}
		}
	}

	public override EBuff GetBuffType()
	{
		return EBuff.OnHitShaderEffect;
	}
}
