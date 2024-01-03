using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMainMenuHelper : MonoBehaviour
{
    public void BackToMainMenu()
    {
        UIManager.Instance.LoadMainMenu();
    }
}
