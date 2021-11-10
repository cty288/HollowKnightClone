using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.DataStructures;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MikroFramework.Examples
{
    public class TestAdvancedDataTable : Table<TestDataItem>
    {

        public TableIndex<string, TestDataItem> NameIndex { get; private set; }
        public TableIndex<int, TestDataItem> AgeIndex { get; private set; }

        public TestAdvancedDataTable()
        {
            NameIndex = new TableIndex<string, TestDataItem>(item => item.Name);
            AgeIndex = new TableIndex<int, TestDataItem>(item => item.Age);
        }

        protected override void OnClear() {
            NameIndex.Clear();
            AgeIndex.Clear();
        }

        public override void OnAdd(TestDataItem item)
        {
            NameIndex.Add(item);
            AgeIndex.Add(item);
        }

        public override void OnRemove(TestDataItem item)
        {
            NameIndex.Remove(item);
            AgeIndex.Remove(item);
        }
    }


    public class IndexSearchExample : MonoBehaviour {
        //search with index (strongly recommended)

        private TestAdvancedDataTable table = new TestAdvancedDataTable();

        private void Start() {
            for (int i = 0; i < 3000; i++)
            {
                table.Add(new TestDataItem()
                {
                    Name = "Name" + i,
                    Age = Random.Range(0, 100)
                });
            }

            foreach (var t in table.AgeIndex.Get(age => { return age > 50 && age < 80;})
                .Where(dataItem =>  dataItem.Name.Length == 5))
            {
                Debug.Log(t.ToString());
            }
        }
    }
}
