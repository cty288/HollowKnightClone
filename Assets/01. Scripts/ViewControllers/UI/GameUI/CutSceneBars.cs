using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
using UnityEngine;
using Action = Antlr.Runtime.Misc.Action;

namespace HollowKnight
{
    public class CutSceneBars : AbstractMikroController<HollowKnight>, ISingleton {
        [SerializeField] private GameObject upperBar;
        [SerializeField] private GameObject lowerBar;

        private float upperInitialY;
        private float lowerInitialY;

        [SerializeField] private float upperTargetY;
        [SerializeField] private float lowerTargetY;
        public static CutSceneBars Singleton {
            get {
                return SingletonProperty<CutSceneBars>.Singleton;
            }
        }

        private void Awake() {
            upperInitialY = upperBar.transform.position.y;
            lowerInitialY = lowerBar.transform.position.y;
        }

        public void StartBars(float time, Action onFinished) {
            upperBar.transform.DOLocalMoveX(upperTargetY, time);
            lowerBar.transform.DOLocalMoveX(lowerTargetY, time).OnComplete(() => { onFinished?.Invoke(); });
        }

        public void EndBars(float time, Action onFinished) {
            upperBar.transform.DOLocalMoveX(upperInitialY, time);
            lowerBar.transform.DOLocalMoveX(lowerInitialY, time).OnComplete(() => { onFinished?.Invoke(); });
        }

        public void OnSingletonInit() {
            
        }
    }
}
