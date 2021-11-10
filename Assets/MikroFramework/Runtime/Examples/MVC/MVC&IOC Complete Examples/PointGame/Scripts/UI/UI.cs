using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Examples;
using UnityEngine;

namespace MikroFramework
{
    public class UI : AbstractMikroController<PointGame> {
        void Start() {
            this.RegisterEvent<GameWInEvent>(OnGameWin);

            this.RegisterEvent<OnCountdownEndEvent>(e => {
                transform.Find("Canvas/GamePanel").gameObject.SetActive(false);
                transform.Find("Canvas/GameoverPanel").gameObject.SetActive(true);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnGameWin(GameWInEvent e) {
            transform.Find("Canvas/GamePanel").gameObject.SetActive(false);
            transform.Find("Canvas/WinPanel").gameObject.SetActive(true);
        }

        private void OnDestroy() {
            this.UnRegisterEvent<GameWInEvent>(OnGameWin);
        }

    }
}
