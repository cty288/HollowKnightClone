using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.DataStructures
{
    public class TableIndex <TKeyType,TDataItem> {
        
        private Dictionary<TKeyType, List<TDataItem>> index = new Dictionary<TKeyType, List<TDataItem>>();

        private Func<TDataItem, TKeyType> getKeyByDataItem = null;

        public TableIndex(Func<TDataItem, TKeyType> keyGetter) {
            getKeyByDataItem = keyGetter;
        }

        /// <summary>
        /// Add or create a new data item to its corresponding key in the index
        /// </summary>
        /// <param name="dataItem"></param>
        public void Add(TDataItem dataItem) {
            TKeyType key = getKeyByDataItem.Invoke(dataItem);
            if (index.ContainsKey(key)) {
                index[key].Add(dataItem);
            }
            else {
                index.Add(key,new List<TDataItem>(){dataItem});
            }
        }


        /// <summary>
        /// Remove a dataitem from its index
        /// </summary>
        /// <param name="dataItem"></param>
        public void Remove(TDataItem dataItem) {
            TKeyType key = getKeyByDataItem(dataItem);
            index[key].Remove(dataItem);
        }

        /// <summary>
        /// Get a set of data items from the table's index, where their keys equals the input key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<TDataItem> Get(TKeyType key) {
            List<TDataItem> retResult=null;
            index.TryGetValue(key, out retResult);
            return retResult;
        }

        /// <summary>
        /// Get a set of data items from the table's index, where their keys matches a certain conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IEnumerable<TDataItem> Get(Func<TKeyType, bool> condition) {
            IEnumerator<KeyValuePair<TKeyType,List<TDataItem>>> kvp= index.Where(pair => condition(pair.Key)).GetEnumerator();

            List<TDataItem> retResult = new List<TDataItem>();

            while (kvp.MoveNext()) {
                retResult.AddRange(kvp.Current.Value);
            }

            return retResult;
        }

        public void Clear() {
            index.Clear();
        }
    }
}
