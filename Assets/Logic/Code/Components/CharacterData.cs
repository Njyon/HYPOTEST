using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CharacterData : MonoBehaviour
{
	[Header("MovementData")]
	[SerializeField] float maxMovementSpeed = 5f;
	[SerializeField] float movmentGravity = 9.81f;
	[SerializeField] float acceleration = 5f;
	[SerializeField] float drag = 1f;
	[SerializeField] float inAirControll = 1f;

	[Header("JumpData")]
	[SerializeField] int maxJumps = 3;
	[SerializeField] float jumpForce = 100f;
	[SerializeField] float gravityMultiplier = 1f;

	public float MaxMovementSpeed { get { return maxMovementSpeed; } }
	public float MovmentGravity { get { return movmentGravity; } }
	public float Acceleration { get { return acceleration; } }	
	public float Drag { get { return drag; } }	
	public float InAirControll { get { return inAirControll; } }

	public int MaxJumps { get { return maxJumps; } }
	public float JumpForce { get { return jumpForce; } }
	public float GravityMultiplier { get { return gravityMultiplier; } }
}
