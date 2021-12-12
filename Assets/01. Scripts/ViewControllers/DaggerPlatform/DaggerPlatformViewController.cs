using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Utilities;
using UnityEngine;

namespace HollowKnight
{

    public struct OnDaggerGet { }
    public class DaggerPlatformViewController : AbstractMikroController<HollowKnight>, ICanSendEvent {

        public void GetDagger() {
            Debug.Log("233333");
            this.SendEvent<OnDaggerGet>(new OnDaggerGet());
        }
    }
}
