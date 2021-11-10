using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MikroFramework.DataStructures
{
    //TODO: add NHibernate support extension
    public abstract class Table<TDataItem> : IEnumerable<TDataItem> where TDataItem : class {
        private List<TDataItem> items = new List<TDataItem>();
        public List<TDataItem> Items => items;


        public void Add(TDataItem item) {
            items.Add(item);
            OnAdd(item);
        }

        public void Add(IEnumerable<TDataItem> items) {
            this.items.AddRange(items);
            foreach (TDataItem dataItem in items) {
                OnAdd(dataItem);
            }
        }

        public void Remove(TDataItem item) {
            items.Remove(item);
            OnRemove(item);
        }

        public void Update() {

        }

        public void Clear() {
            items.Clear();
            OnClear();
        }

        /// <summary>
        /// Triggered when the table is cleared
        /// </summary>
        protected abstract void OnClear();


        /// <summary>
        /// Add your logic for adding your customized index here
        /// </summary>
        /// <param name="item"></param>
        public abstract void OnAdd(TDataItem item);

        /// <summary>
        /// Add your logic for removing your customized index here
        /// </summary>
        /// <param name="item"></param>
        public abstract void OnRemove(TDataItem item);

        public IEnumerable<TDataItem> Get(Func<TDataItem, bool> condition) {
            return items.Where(condition);
        }

        public IEnumerator<TDataItem> GetEnumerator() {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
