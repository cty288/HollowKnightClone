using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.UI;

namespace MikroFramework.Examples
{
    public class GamePanel: AbstractMikroController<PointGame> {
        private ICountDownSystem countDownSystem;
        private IGameModel gameModel;
        private void Awake() {
            countDownSystem = this.GetSystem<ICountDownSystem>();
            gameModel = this.GetModel<IGameModel>();

            gameModel.Gold.RegisterOnValueChaned(OnGoldValueChanged);
            gameModel.Life.RegisterOnValueChaned(OnLifeValueChanged);
            gameModel.Score.RegisterOnValueChaned(OnScoreValueChanged);

            OnGoldValueChanged(gameModel.Gold.Value);
            OnLifeValueChanged(gameModel.Life.Value);
            OnScoreValueChanged(gameModel.Score.Value);
        }

        private void OnGoldValueChanged(int gold) {
            transform.Find("GoldText").GetComponent<Text>().text = "Gold: " + gold;
        }

        private void OnLifeValueChanged(int life) {
            transform.Find("LifeText").GetComponent<Text>().text = "Life: " + life;
        }

        private void OnScoreValueChanged(int score) {
            transform.Find("ScoreText").GetComponent<Text>().text = "Score: " + score;
        }

        private void Update() {
            if (Time.frameCount % 20 == 0) {
                transform.Find("CountDownText").GetComponent<Text>().text = countDownSystem.CurrentRemainSeconds + "s";
                countDownSystem.Update();
            }
        }

        private void OnDestroy() {
            gameModel.Gold.UnRegisterOnValueChanged(OnGoldValueChanged);
            gameModel.Life.UnRegisterOnValueChanged(OnLifeValueChanged);
            gameModel.Score.UnRegisterOnValueChanged(OnScoreValueChanged);
        }
    }
}
