using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class ErrorArea : AbstractMikroController<PointGame>
    {
        private void OnMouseDown() {
            Debug.Log("Error");
            this.SendCommand<MissCommand>();
        }
    }
}
