using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JasonManager 
{
	public static string ToJason<T>(T obj)
	{
		return JsonUtility.ToJson(obj);
	}

	public static T FromJason<T>(string json)
	{
		return JsonUtility.FromJson<T>(json);
	}

	public static void FromJsonOverwrite<T>(string json, T obj)
	{
		JsonUtility.FromJsonOverwrite(json, obj);
	}

	public static void SaveIntoJason<T>(T obj, string fileName)
	{
		string data = ToJason(obj);
		System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".json", data);
	}

	public static string GetJasonString(string fileName)
	{
		return System.IO.File.ReadAllText(Application.persistentDataPath + "/" + fileName + ".json");
	}

	public static T ReadFromJason<T>(string fileName)
	{
		string data = GetJasonString(fileName);
		return FromJason<T>(data);
	}

	public static bool DoesPathExist(string fileName)
	{
		string path = Application.persistentDataPath + "/" + fileName + ".json";
		return System.IO.File.Exists(path);
	}

	public static void DeleteData(string fileName)
	{
		System.IO.File.Delete(Application.persistentDataPath + "/" + fileName + ".json");
	}
}
