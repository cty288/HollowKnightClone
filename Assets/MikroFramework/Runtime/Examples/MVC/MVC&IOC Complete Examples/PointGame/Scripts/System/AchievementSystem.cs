using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework.Examples
{
    public interface IAchievementSystem:ISystem {
    }

    public class AchievementItem {
        public string Name { get; set; }
        public Func<bool> CheckComplete { get; set; }
        public bool Unlocked { get; set; }

    }

    public class AchievementSystem : AbstractSystem, IAchievementSystem {
        private List<AchievementItem> items = new List<AchievementItem>();

        private bool missed = false;

        protected override void OnInit() {
            this.RegisterEvent<OnMissEvent>(e => {
                missed = true;
            });

            this.RegisterEvent<GameStartEvent>(e => {
                missed = false;
            });

            items.Add(new AchievementItem() {
                Name="100 score",
                CheckComplete = () => this.GetModel<IGameModel>().BestScore.Value > 100
            });

            items.Add(new AchievementItem() {
                Name = "Missed!",
                CheckComplete = ()=>this.GetModel<IGameModel>().Score.Value<0
            });

            items.Add(new AchievementItem() {
                Name = "No Missed!",
                CheckComplete = ()=>!missed
            });

            this.RegisterEvent<GameWInEvent>(async e => {
                await Task.Delay(TimeSpan.FromSeconds(0.1f));

                foreach (AchievementItem achievementItem in items) {
                    if (!achievementItem.Unlocked && achievementItem.CheckComplete()) {
                        achievementItem.Unlocked = true;
                        Debug.Log("Unlocked achievement: " + achievementItem.Name);
                    }
                }
            });
        }
    }
}
