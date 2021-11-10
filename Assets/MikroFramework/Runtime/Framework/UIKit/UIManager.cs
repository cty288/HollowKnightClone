using System.Collections;
using System.Collections.Generic;
using MikroFramework.Managers;
using MikroFramework.Singletons;
using UnityEngine;
using UnityEngine.UI;

namespace MikroFramework.UIKit {
    public enum UILayer
    {
        Bg,
        Common,
        Top,
    }

    [MonoSingletonPath("[FrameworkPersistent]/UIManager")]
    public class UIManager : ManagerBehavior, ISingleton {
        private Dictionary<string, GameObject> panelDictionary = new Dictionary<string, GameObject>();
        private static UIManager singleton {
            get {
                return SingletonProperty<UIManager>.Singleton;
            }
        }

        void ISingleton.OnSingletonInit() {
            if (!uiRoot) {
                uiRoot = Object.Instantiate(Resources.Load<GameObject>("UIRoot"));
                uiRoot.name = "UIRoot";
            }
        }


        private static GameObject uiRoot;
        public static GameObject UIRoot {
            get {
                if (uiRoot == null) {
                    uiRoot = Object.Instantiate(Resources.Load<GameObject>("UIRoot"));
                    uiRoot.name = "UIRoot";
                }

                return uiRoot;
            }
        }


        /// <summary>
        /// Set the resolution of the canvas ("Scale with Screen Size" mode)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="matchWidthOrHeight"></param>
        public static void SetResolution(float width, float height, float matchWidthOrHeight) {
            CanvasScaler canvasScaler = UIRoot.GetComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2(width, height);
            canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
        }

        public static GameObject LoadPanel(string panelName,UILayer uiLayer) {

            GameObject panelPrefab = Resources.Load<GameObject>(panelName);
            GameObject panelObj = Instantiate(panelPrefab);
            panelObj.name = panelName;
            //panelObj.transform.SetParent(canvasObj.transform);

            singleton. panelDictionary.Add(panelName,panelObj);

            switch (uiLayer) {
                case UILayer.Bg:
                    panelObj.transform.SetParent(UIRoot.transform.Find("Bg"));
                    break;
                case UILayer.Common:
                    panelObj.transform.SetParent(UIRoot.transform.Find("Common"));
                    break;
                case UILayer.Top:
                    panelObj.transform.SetParent(UIRoot.transform.Find("Top"));
                    break;
            }

            

            RectTransform panelRectTransform = panelObj.transform as RectTransform;

            panelRectTransform.offsetMin=Vector2.zero;
            panelRectTransform.offsetMax=Vector2.zero;
            panelRectTransform.anchoredPosition3D=Vector3.zero;
            panelRectTransform.anchorMin=Vector2.zero;
            panelRectTransform.anchorMax = Vector2.one;

            return panelObj;
        }

        public static void UnLoadPanel(string panelName) {
            if (singleton.panelDictionary.ContainsKey(panelName)) {
                Destroy(singleton.panelDictionary[panelName]);
            }
            else {
                Debug.LogError("Unable to find panel "+panelName+" when unloading it.");
            }
        }
        
    }

}
