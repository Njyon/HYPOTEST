using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(AttackHolderTest))]
//public class AttackHolderTestInspector : Editor
//{
//	public override void OnInspectorGUI()
//	{
//		base.OnInspectorGUI();
//		AttackHolderTest attackHolder = (AttackHolderTest)target;
//		if (attackHolder.attack != null)
//		{
//
//			AttackData data = (AttackData)Activator.CreateInstance(attackHolder.attack.GetAttackData());
//			if (data.GetType() != attackHolder.attackRef.GetType()) 
//			{
//				attackHolder.attackRef = data;
//			}
//		}
//	}
//}
