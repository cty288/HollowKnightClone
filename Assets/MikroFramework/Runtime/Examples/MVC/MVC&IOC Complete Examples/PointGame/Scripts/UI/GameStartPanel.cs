using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Examples;
using UnityEngine;
using UnityEngine.UI;

namespace MikroFramework
{
    public class GameStartPanel : AbstractMikroController<PointGame> {

        private IGameModel gameModel;
        void OnEnable() {
            gameModel = this.GetModel<IGameModel>();

            transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(() => {
                gameObject.SetActive(false);
                this.SendCommand<StartGameCommand>();
            });

            transform.Find("BuyLifeButton").GetComponent<Button>().onClick.AddListener(() => {
                this.SendCommand<BuyLifeCommand>();
            });

            gameModel.Score.Value = 0;
            gameModel.KillCount.Value = 0;
            gameModel.Gold.RegisterOnValueChaned(OnGoldValueChanged);
            gameModel.Life.RegisterOnValueChaned(OnLifeValueChanged);

            OnGoldValueChanged(gameModel.Gold.Value);
            OnLifeValueChanged(gameModel.Life.Value);

            transform.Find("BestScoreText").GetComponent<Text>().text = "Best Score: " + gameModel.BestScore.Value;
        }

        private void OnLifeValueChanged(int newLife) {
            transform.Find("LifeText").GetComponent<Text>().text = "Life: " + gameModel.Life.Value;
        }

        private void OnGoldValueChanged(int newGold) {
            transform.Find("GoldText").GetComponent<Text>().text = "Gold: " + gameModel.Gold.Value;
        }

        private void OnDestroy() {
            gameModel.Gold.UnRegisterOnValueChanged(OnGoldValueChanged);
            gameModel.Life.UnRegisterOnValueChanged(OnLifeValueChanged);
            gameModel = null;
        }


    }
}
