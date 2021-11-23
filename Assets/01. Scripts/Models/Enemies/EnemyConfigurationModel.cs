using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public interface IEnemyConfigurationModel : IModel {
        T GetEnemyConfigurationItemByType<T>() where T : EnemyConfigurationItem, new();
    }

    public class EnemyConfigurationModel : AbstractModel, IEnemyConfigurationModel {
        
        public T GetEnemyConfigurationItemByType<T>() where T : EnemyConfigurationItem, new() {
            return new T();
        }

        protected override void OnInit() {
            
        }
    }
}
