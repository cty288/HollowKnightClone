using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Examples;
using UnityEngine;

namespace MikroFramework.Examples
{
    public interface IGameModel:IModel {
        public BindableProperty<int> KillCount { get; }
        public BindableProperty<int> Gold { get; }
        public BindableProperty<int> Score { get; }
        public BindableProperty<int> BestScore { get; }

        public BindableProperty<int> Life { get; }
    }
    public class GameModel:AbstractModel,IGameModel{
       

        public  BindableProperty<int> KillCount { get; } = new BindableProperty<int>() {
            Value = 0
        };
        public  BindableProperty<int> Gold { get; } = new BindableProperty<int>()
        {
            Value = 0
        };
        public  BindableProperty<int> Score { get; } = new BindableProperty<int>()
        {
            Value = 0
        };
        public  BindableProperty<int> BestScore { get; } = new BindableProperty<int>()
        {
            Value = 0
        };
        public BindableProperty<int> Life { get; } = new BindableProperty<int>();

        protected override void OnInit() {
            IStorage storage = this.GetUtility<IStorage>();

            BestScore.Value = storage.LoadInt(nameof(BestScore), 0);
            BestScore.RegisterOnValueChaned(v => { storage.SaveInt(nameof(BestScore), v);});

            Life.Value = storage.LoadInt(nameof(Life), 1);
            Life.RegisterOnValueChaned(v => { storage.SaveInt(nameof(Life), v); });

            Gold.Value = storage.LoadInt(nameof(Gold), 1);
            Gold.RegisterOnValueChaned(v => { storage.SaveInt(nameof(Gold), v); });

        }
    }
}
