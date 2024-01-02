using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraingRoomButtonHelper : MonoBehaviour
{
    public void LoadTrainingRoom()
    {
		SceneLoaderManager.Instance.LoadTrainingsMap();
	}
}
