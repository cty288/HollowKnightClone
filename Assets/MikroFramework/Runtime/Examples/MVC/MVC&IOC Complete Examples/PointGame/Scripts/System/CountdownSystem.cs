using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;

namespace MikroFramework.Examples
{
    public interface ICountDownSystem : ISystem{
        int CurrentRemainSeconds { get; }
        void Update();
    }

    public class CountDownSystem : AbstractSystem, ICountDownSystem {
        private bool started = false;
        private DateTime gameStartTime { get; set; }
        protected override void OnInit() {
            this.RegisterEvent<GameStartEvent>(e => {
                started = true;
                gameStartTime = DateTime.Now;
            });

            this.RegisterEvent<GameWInEvent>(e => {
                started = false;
            });
        }

        

        public int CurrentRemainSeconds { get; protected set; }
        public void Update() {
            if (started) {

                CurrentRemainSeconds =10- (DateTime.Now - gameStartTime).Seconds;

                if ((DateTime.Now - gameStartTime)> TimeSpan.FromSeconds(10)) {
                    this.SendEvent<OnCountdownEndEvent>();
                    started = false;
                }
            }
        }

        
    }
}
