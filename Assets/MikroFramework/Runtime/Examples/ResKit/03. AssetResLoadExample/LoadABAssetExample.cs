using System.Collections;
using System.Collections.Generic;
using MikroFramework.Examples;
using MikroFramework.ResKit;
using UnityEngine;

namespace MikroFramework
{
    public class LoadABAssetExample : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/ResLoader/LoadAssetsFromABExample/LoadAssetsFromAB", false,1)]
        private static void MenuItemBuild()
        {
            UnityEditor.EditorApplication.isPlaying = true;

            new GameObject("Load Assets from AB Test").AddComponent<LoadABAssetExample>();
        }
#endif
        private ResLoader resLoader;

        void Start() {
            ResLoader.Create((loader) => {
                resLoader = loader;

                var texture = resLoader.LoadSync<Texture2D>("mftest", "Asteroid_1");

                Debug.Log(texture.name);
            });

            

            //resLoader.LoadAsync<GameObject>("mftest","TestABObj",
                //prefab => {
                  //  Instantiate(prefab);
              //  });
        }

        void OnDestroy() {
            resLoader.ReleaseAllAssets();
        }
    }
}
