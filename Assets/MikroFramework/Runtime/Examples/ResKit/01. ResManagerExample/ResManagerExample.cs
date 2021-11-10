using System.Collections;
using System.Collections.Generic;
using MikroFramework.Examples;
using MikroFramework.ResKit;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class ResManagerExample : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/ResLoader/ResourcesExample/02. ResourceManagerExample", false, 2)]
        private static void MenuItem()
        {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject("ResManagerExample").AddComponent<ResManagerExample>();

        }
#endif
        private ResLoader resLoader;

        void Start() {
            ResLoader.Create((loader) => {
                resLoader = loader;
                StartCoroutine(Load());
            });
            
        }

        IEnumerator Load() {
            yield return new WaitForSeconds(2.0f);
            resLoader.LoadAsync<AudioClip>("resources://War loop", clip => {
                Debug.Log("Async loaded: " + clip.name);
                Debug.Log(Time.time);
            });
            Debug.Log(Time.time);
            yield return new WaitForSeconds(2.0f);
            resLoader.LoadSync<GameObject>("resources://HomePanel");
            yield return new WaitForSeconds(2.0f);
            resLoader.LoadSync<AudioClip>("resources://Lose");
            yield return new WaitForSeconds(2.0f);
            resLoader.LoadSync<GameObject>("resources://HomePanel");
            yield return new WaitForSeconds(2.0f);
            resLoader.ReleaseAllAssets();
        }




    }
}
