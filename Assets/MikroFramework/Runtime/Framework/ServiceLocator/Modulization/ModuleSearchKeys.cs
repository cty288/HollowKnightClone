using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework
{
    public class ModuleSearchKeys : IPoolable{
       public string Name { get; set; }
       public Type Type { get; set; }

       public ModuleSearchKeys() { }
       public void OnRecycled() {
           Type = null;
           Name = null;
       }

       public bool IsRecycled { get; set; }
       public void RecycleToCache() {
           SafeObjectPool<ModuleSearchKeys>.Singleton.Recycle(this);
       }

       public static ModuleSearchKeys Allocate(Type type, string name) {
           ModuleSearchKeys keys= SafeObjectPool<ModuleSearchKeys>.Singleton.Allocate();
           keys.Type = type;
           keys.Name = name;
           return keys;
       }
    }
}
