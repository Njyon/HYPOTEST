using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndbossDeadVisuallHack : MonoBehaviour
{
    public void OnEndbossDead()
    {
        Ultra.HypoUttilies.GameMode.PlayerGameCharacter?.PlayerUI?.showThxForPlayingPanel.PlayFeedbacks();
    }
}
