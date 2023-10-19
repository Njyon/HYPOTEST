using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ActionHolder
{
	public ClassInstance<ActionBase> action = new ClassInstance<ActionBase>();

	//public BaseAttack attack;
	//[HideInInspector]
	//public BaseAttack attackInstance;
	//[SerializeReference]
	//public AttackData attackDataRef;

	public ActionBase Action { 
		get 
		{
			return action.instance;
//#if UNITY_EDITOR
//			if (!EditorApplication.isUpdating)
//			{
//#endif
//				if (attackInstance == null && attack != null)
//				{
//					attackInstance = (BaseAttack)Activator.CreateInstance(attack.GetType());
//					if (attackDataRef != null && attackInstance != null) attackInstance.SetData(attackDataRef);
//				}
//
//#if UNITY_EDITOR
//			}
//#endif
//			return attackInstance; 
		} 
	}

//	public void SetAttackRef()
//	{
//#if UNITY_EDITOR
//		if (EditorApplication.isUpdating) return;
//#endif
//		if (attack != null)
//		{
//			if (attackInstance == null || attack.GetType() != attackInstance.GetType())
//				attackInstance = (BaseAttack)Activator.CreateInstance(attack.GetType());
//			if (attackDataRef != null)
//			{
//				if (attackDataRef.GetType() != attackInstance.GetAttackDataType())
//				{
//					attackDataRef = (AttackData)Activator.CreateInstance(attackInstance.GetAttackDataType());
//				}
//			}
//			else
//			{
//				attackDataRef = (AttackData)Activator.CreateInstance(attackInstance.GetAttackDataType());
//			}
//			
//			if (attackInstance != null)
//				attackInstance.SetData(attackDataRef);
//		}
//	}
}

//[ExecuteInEditMode]
//public class AttackHolderTest : MonoBehaviour
//{
//    public BaseAttack attack;
//    [SerializeReference]
//    public AttackData attackRef;
//
//	public void Start()
//	{
//#if UNITY_EDITOR 
//		SetAttackRef();
//#endif
//		if (Application.isPlaying)
//			attack.SetData(attackRef);
//	}
//
//	public void Update()
//	{
//#if UNITY_EDITOR
//		SetAttackRef();
//#endif
//	}
//
//	private void SetAttackRef()
//	{
//		if (attack != null)
//		{
//			if (attackRef != null)
//			{
//				if (attackRef.GetType() != attack.GetAttackDataType())
//				{
//					attackRef = (AttackData)Activator.CreateInstance(attack.GetAttackDataType());
//				}
//			}
//			else
//			{
//				attackRef = (AttackData)Activator.CreateInstance(attack.GetAttackDataType());
//			}
//		}
//	}
//}
