using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerBase : MonoBehaviour
{
	GameObject pawn;

	public virtual void BeginPosses(GameObject pawn)
	{
		this.pawn = pawn;
	}
	public virtual void EndPosses()
	{
		this.pawn = null;
	}
}
