using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
         {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void Add(TKey key, TValue value, bool allowDuplicateKeys)
    {
        if (allowDuplicateKeys || !ContainsKey(key))
        {
            base[key] = value;
        }
        else
        {
            Debug.LogWarningFormat("Unable to add item with duplicate key '{0}' to SerializableDictionary", key.ToString());
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}

[Serializable]
public class BetterSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
	[SerializeField]
	private List<TKey> keys = new List<TKey>();

	[SerializeField]
	private List<TValue> values = new List<TValue>();

	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		foreach (KeyValuePair<TKey, TValue> pair in this)
		{
			keys.Add(pair.Key);
			values.Add(pair.Value);
		}
	}

	public void Add(TKey key, TValue value, bool allowDuplicateKeys)
	{
		if (allowDuplicateKeys || !ContainsKey(key))
		{
			base[key] = value;
		}
		else
		{
			Debug.LogWarningFormat("Unable to add item with duplicate key '{0}' to SerializableDictionary", key.ToString());
		}
	}

	// load dictionary from lists
	public void OnAfterDeserialize()
	{
		this.Clear();

		if (keys.Count != values.Count)
			throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

		for (int i = 0; i < keys.Count; i++)
			this.Add(keys[i], values[i]);
	}
}