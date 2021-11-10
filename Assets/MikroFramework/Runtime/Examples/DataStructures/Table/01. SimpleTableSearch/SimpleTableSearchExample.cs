using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.DataStructures;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MikroFramework.Examples
{
    public class TestDataItem {
        public string Name { get; set; }

        public int Age { get; set; }

        public override string ToString() {
            return $"Name: {Name}   Age: {Age}";
        }
    }

    public class TestSimpleDataTable : Table<TestDataItem>
    {
        protected override void OnClear() {
            
        }

        public override void OnAdd(TestDataItem item)
        {
        }

        public override void OnRemove(TestDataItem item)
        {
            
        }
    }

    public class SimpleTableSearchExample : MonoBehaviour {
        //search without using index. Less recommended
        private TestSimpleDataTable table = new TestSimpleDataTable();

        private void Start() {
            for (int i = 0; i < 3000; i++) {
                table.Add(new TestDataItem()
                {
                    Name = "Name " + i,
                    Age = Random.Range(0,100)
                });
            }

            foreach (var t in table.Get(item => item.Age > 50))
            {
                Debug.Log(t.ToString());
            }


        }
    }
}
