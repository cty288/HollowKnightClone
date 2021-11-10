using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Examples;
using MikroFramework.IOC;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class PointGame:Architecture<PointGame> {

        protected override void Init()
        {
            RegisterModel<IGameModel>(new GameModel());
            RegisterExtensibleUtility<IStorage>(new PlayerPrefsStorage());
            RegisterSystem<IScoreSystem>(new ScoreSystem());
            RegisterSystem<IAchievementSystem>(new AchievementSystem());
            RegisterSystem<ICountDownSystem>(new CountDownSystem());
        }

    }
}
