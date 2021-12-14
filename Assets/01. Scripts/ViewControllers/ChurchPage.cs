using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class ChurchPage : MonoBehaviour
    {
        public void Triggered() {
            GameManager.Singleton.ChurchChargeEnemyActivate();
        }
    }
}
