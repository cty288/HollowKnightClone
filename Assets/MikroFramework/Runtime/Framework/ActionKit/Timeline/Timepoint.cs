using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public class Timepoint:IPoolable {
        public float StartSeconds;
        public MikroAction Action;
        public void OnRecycled() {
            StartSeconds = 0;
            Action = null;
        }

        public bool IsRecycled { get; set; }
        public void RecycleToCache() {
            SafeObjectPool<Timepoint>.Singleton.Recycle(this);
        }

        public static Timepoint Allocate() {
            return SafeObjectPool<Timepoint>.Singleton.Allocate();
        }
    }
}
