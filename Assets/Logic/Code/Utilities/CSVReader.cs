using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[Serializable]
//public class DifficultyLevel
//{
//	public int level;
//	public int colors;
//	public int cubes;
//	public float time;
//}
//
//[Serializable]
//public class DifficultyList
//{
//	public DifficultyLevel[] list;
//}
//
//public static class CSVReader
//{
//	public static DifficultyList list = new DifficultyList();
//   
//
//	public static void ReadCSV(TextAsset textData)
//	{
//		string[] data = textData.text.Split(new string[] { ",","\n" }, StringSplitOptions.None);
//		int tableSize = data.Length / 4;
//		list.list = new DifficultyLevel[tableSize];
//
//		for (int i = 0; i< tableSize; i++)
//		{
//			list.list[i] = new DifficultyLevel();
//			list.list[i].level = int.Parse(data[i]);
//			list.list[i].colors = int.Parse(data[tableSize + i]);
//			list.list[i].cubes = int.Parse(data[(tableSize * 2) + i]);
//			list.list[i].time = float.Parse(data[(tableSize * 3) + i]);
//		}
//	}
//}
