using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class EnemyAI_TypeB : MonoBehaviour
    {
        public enum EnemyState
        {
            Patrolling,
            Engaging,
            Attacking,
            dodging,
        }
    }
}
