using System.Collections;
using System.Collections.Generic;
using MikroFramework.ResKit;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class UIXXXPanelExample : MonoBehaviour {
        private AudioClip audioClip = null;

        private ResLoader resLoader;
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/ResLoader/ResourcesExample/01. Load&Unload Resources", false, 1)]
        private static void MenuItem()
        {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject("LoadUnloadExample").AddComponent<UIXXXPanelExample>().gameObject.AddComponent<UIYYYPanel>();

        }
#endif

        // Start is called before the first frame update
        void Start() {
            ResLoader.Create(loader => {
                resLoader = loader;
                resLoader.LoadSync<AudioClip>("resources://War loop");
                resLoader.LoadSync<AudioClip>("resources://Lose");
            });
      
        }

        void OtherFunction() {
            resLoader.LoadSync<GameObject>("resources://War loop");
        }

        void OnDestroy() {
            resLoader.ReleaseAllAssets();
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }


    public class UIYYYPanel : MonoBehaviour {
        private ResLoader resLoader;
        // Start is called before the first frame update
        void Start() {
            ResLoader.Create(loader => {
                resLoader = loader;
                resLoader.LoadSync<AudioClip>("resources://War loop");
                resLoader.LoadSync<AudioClip>("resources://Lose");
            });
        }

        void OtherFunction()
        {
            resLoader.LoadSync<GameObject>("resources://War loop");
        }

        void OnDestroy()
        {
            resLoader.ReleaseAllAssets();
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
