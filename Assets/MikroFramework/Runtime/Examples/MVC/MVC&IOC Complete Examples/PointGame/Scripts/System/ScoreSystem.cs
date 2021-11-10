using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework.Examples
{
    public interface IScoreSystem : ISystem {

    }
    
    public class ScoreSystem : AbstractSystem, IScoreSystem
    {
        protected override void OnInit() {
            IGameModel gameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<GameWInEvent>(e => {
                ICountDownSystem countDownSystem = this.GetSystem<ICountDownSystem>();
                var timeScore = countDownSystem.CurrentRemainSeconds * 10;

                gameModel.Score.Value += timeScore;


                if (gameModel.Score.Value > gameModel.BestScore.Value) {
                    Debug.Log("New record: "+gameModel.Score.Value);
                    gameModel.BestScore.Value = gameModel.Score.Value;
                }
            });

            this.RegisterEvent<OnKillEvent>(e => {
                gameModel.Score.Value += 10;
                Debug.Log("Got 10 score");
                Debug.Log("Current score: "+gameModel.Score.Value);
            });

            this.RegisterEvent<OnMissEvent>(e => {
                gameModel.Score.Value -= 5;

                Debug.Log("Lost 10 score");
                Debug.Log("Current score: " + gameModel.Score.Value);
            });
        }
    }
}
