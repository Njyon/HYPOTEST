using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterAttackUI : MonoBehaviour
{
    [SerializeField] Image counterAttackBar;
	[SerializeField] float speed = 10;
    float fillamountTarget;

    public void SetBarFillValue(float value)
    {
		fillamountTarget = value;
    }

	private void Update()
	{
		if (fillamountTarget != counterAttackBar.fillAmount)
		{
			counterAttackBar.fillAmount = Mathf.Lerp(counterAttackBar.fillAmount, fillamountTarget, Time.deltaTime * speed);
		}
	}
}