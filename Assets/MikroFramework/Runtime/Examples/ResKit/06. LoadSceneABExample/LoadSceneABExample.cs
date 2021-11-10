using System.Collections;
using System.Collections.Generic;
using MikroFramework.ResKit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MikroFramework
{
    public class LoadSceneABExample : MonoBehaviour {
        private ResLoader resLoader;
        
        void Start() {
            resLoader = new ResLoader();
            StartCoroutine(LoadAB());
        }

        IEnumerator LoadAB() {
            yield return new WaitForSeconds(3f);
            resLoader.LoadSync<AssetBundle>("test/scene_test.ab");
            SceneManager.LoadScene("TestABScene");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
