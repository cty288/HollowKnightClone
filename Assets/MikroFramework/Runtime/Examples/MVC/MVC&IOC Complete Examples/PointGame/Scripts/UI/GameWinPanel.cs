using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;
using UnityEngine.UI;

namespace MikroFramework.Examples
{
    public class GameWinPanel: AbstractMikroController<PointGame> {
        private void Start() {
            transform.Find("RemainingTimeText").GetComponent<Text>().text = "Remaining Time: " +
                                                                            this.GetSystem<ICountDownSystem>()
                                                                                .CurrentRemainSeconds + "s";
            IGameModel gameModel = this.GetModel<IGameModel>();
            transform.Find("BestScoreText").GetComponent<Text>().text = "Best Score: " + gameModel.BestScore.Value;

            transform.Find("ScoreText").GetComponent<Text>().text = "Score: " + gameModel.Score.Value;
        }
    }
}
