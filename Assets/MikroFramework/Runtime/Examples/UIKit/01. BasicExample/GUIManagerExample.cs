using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Managers;
using MikroFramework.UIKit;
using UnityEngine;

namespace MikroFramework.Examples {


    public class GUIManagerExample : MikroBehavior
    {
        protected override void OnBeforeDestroy()
        {
        }

        void Start() {
            GameObject panel= UIKit.UIManager.LoadPanel("HomePanel",UILayer.Common);
            Delay(3f,()=>{UIKit.UIManager.UnLoadPanel("HomePanel");});
        }

    }
}

