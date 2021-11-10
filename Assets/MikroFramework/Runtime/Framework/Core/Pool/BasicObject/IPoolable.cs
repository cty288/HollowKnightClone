using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Pool
{
    public interface IPoolable {
        void OnRecycled();
        bool IsRecycled { get; set; }
        void RecycleToCache();
    }
}
