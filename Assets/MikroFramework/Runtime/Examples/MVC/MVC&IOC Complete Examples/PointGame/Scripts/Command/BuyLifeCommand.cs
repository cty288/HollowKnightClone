using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;

namespace MikroFramework.Examples
{
    public class BuyLifeCommand:AbstractCommand<BuyLifeCommand>
    {
        protected override void OnExecute() {
            IGameModel gameModel = this.GetModel<IGameModel>();
            gameModel.Gold.Value--;
            gameModel.Life.Value++;
        }
    }
}
