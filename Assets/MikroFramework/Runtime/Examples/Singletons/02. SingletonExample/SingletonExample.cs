using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using MikroFramework.Utilities;
using UnityEngine;

namespace MikroFramework.Examples {
public class SingletonExample : MikroSingleton<SingletonExample> {
    private SingletonExample() {
            Debug.Log("Singleton example");
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("MikroFramework/Examples/Singletons/02. SingletonExample", false, 2)]
    private static void MenuClicked() {
        var instance = SingletonExample.Singleton;
        instance = SingletonExample.Singleton;
    }
#endif
    }

}
