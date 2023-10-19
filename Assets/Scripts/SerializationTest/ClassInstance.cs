using UnityEngine;

[System.Serializable]
public class ClassInstance<T>
{
    [SerializeReference] public T instance;
}