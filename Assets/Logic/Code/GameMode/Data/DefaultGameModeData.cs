using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultGameModeData", menuName = "Assets/GameMode/DefaultData")]
public class DefaultGameModeData : ScriptableObject
{
    public float AfterCombatNoGravityTime = 1f;

    [Header("Hit Freez Data")]
    public float defaultHitFreezTime = 0.08f;
    public float defaultHitFreezTimeManipulation = 0.20f;
    public float heavyHitFreezTime = 0.13f;
    public float heavyHitFreezTimeManipulation = 0.1f;
    [HideInInspector] public string defaultHitFreezString = "DefaultFreez";
    [HideInInspector] public string heavytHitFreezString = "HeavyFreez";

    [Header("Hit Wiggle Data")]
    public float hitWiggleTime = 0.5f;
    public Vector3 hitWiggleHalfLenghtRange = Vector3.zero;
    public float hitWiggleFrequency = 1f;

    [Header("Hit Shader Effect Data")]
    public float shaderEffectTime = 0.2f;
    public Color hitShaderColor = Color.white;
    public float hitShaderIntensity = 1f;

    [Header("Hit Data")]
    public float ignoreGravityAfterHit = 0.5f;

    [Header("Enemy Desolve")]
    public float desolveLenght = 1f;
}
