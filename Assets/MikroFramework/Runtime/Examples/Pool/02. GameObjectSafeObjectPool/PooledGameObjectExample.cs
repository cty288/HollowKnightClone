using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class PooledGameObjectExample : PoolableGameObject
    {
        public static int globalId = 0;

        public int Id;

        public override void OnRecycled()
        {
            Debug.Log($"{gameObject.name} - ID: {Id} recycled!");
        }

        private void Start() {
            Id = globalId++;
        }
    }
}
