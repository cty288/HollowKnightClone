using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework.Examples {
    public class Enemy : AbstractMikroController<PointGame>
    {
        public GameObject winPanel;


        private void OnMouseDown() {
            gameObject.SetActive(false);
            this.SendCommand<KillEnemyCommand>();
        }

    }

}
