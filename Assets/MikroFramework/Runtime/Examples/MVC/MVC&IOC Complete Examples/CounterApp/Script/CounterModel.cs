using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UniRx;

namespace MikroFramework.Examples.CounterApp
{
    public interface ICounterModel : IModel
    {
        ReactiveProperty<int> Count { get; }
    }

    public class CounterModel : AbstractModel, ICounterModel {

        public ReactiveProperty<int> Count { get; } = new ReactiveProperty<int>(0);


        protected override void OnInit()
        {
            IStorage storage = this.GetUtility<IStorage>();

            Count.Value = storage.LoadInt("Counter_Count");

            Count.Subscribe(count => { storage.SaveInt("Counter_Count", count); });
        }
    }




    
}
