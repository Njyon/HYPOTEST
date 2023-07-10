using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableCharacterDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue>, ISerializationCallbackReceiver
{

}