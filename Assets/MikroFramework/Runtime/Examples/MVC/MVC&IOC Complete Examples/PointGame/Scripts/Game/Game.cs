using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Examples;
using UnityEngine;

namespace MikroFramework
{
    public class Game : AbstractMikroController<PointGame>
    {

        void Start() {
            this.RegisterEvent<GameStartEvent>(OnGameStart);

            this.RegisterEvent<OnCountdownEndEvent>(e => { transform.Find("Enemies").gameObject.SetActive(false); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<GameWInEvent>(e => { transform.Find("Enemies").gameObject.SetActive(false); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

        }

        private void OnGameStart(GameStartEvent e) {
            Transform enemyRoot = transform.Find("Enemies");
            enemyRoot.gameObject.SetActive(true);

            foreach (Transform tran in enemyRoot) {
                tran.gameObject.SetActive(true);
            }
        }

        private void OnDestroy() {
            this.UnRegisterEvent<GameStartEvent>(OnGameStart);
        }

    }
}
