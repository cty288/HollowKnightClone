using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace MikroFramework.DatabaseKit.MySQLConnector
{
    public class MySQLTableManager {
        private string tableName;

        private MySQLTableManager(string tableName) {
            this.tableName = tableName;
        }

        public static MySQLTableManager Create(string tableName) {
            return new MySQLTableManager(tableName);
        }

        public void InsertInto(string[] cols, string[] values, Action<int> onFinished, Action<MySqlException> onError)
        {
            if (cols.Length != values.Length)
            {
                throw new Exception("columns.Length != colType.Length");
            }

            string query = "INSERT INTO " + tableName + " (" + cols[0];
            for (int i = 1; i < cols.Length; ++i)
            {
                query += ", " + cols[i];
            }

            query += ") VALUES (" + "'" + values[0] + "'";
            for (int i = 1; i < values.Length; ++i)
            {
                query += ", " + "'" + values[i] + "'";
            }

            query += ")";

            MySQLExecuter.Singleton.ExecuteNonQuery(query,onFinished,onError);
        }

        /// <summary>
        /// Update table, set columns to new colsValues where selectkey=selectvalue
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="colsValues"></param>
        /// <param name="selectKey"></param>
        /// <param name="selectValue"></param>
        /// <returns></returns>
        public void UpdateInto(string[] cols, string[] colsValues, string selectKey, string selectValue,
            Action<int> onFinished, Action<MySqlException> onError)
        {
            string query = "UPDATE " + tableName + " SET " + cols[0] + " = " + "'" + colsValues[0] + "'";

            for (int i = 1; i < colsValues.Length; ++i)
            {
                query += ", " + cols[i] + " =" + "'" + colsValues[i] + "'";
            }

            query += " WHERE " + selectKey + " = " + selectValue;

            MySQLExecuter.Singleton.ExecuteNonQuery(query,onFinished,onError);
        }

        /// <summary>
        /// Delete from tableName where col[0] = colsValues[0] OR cols[1]=colValues[1]
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="colsValues"></param>
        /// <param name="onFinished"></param>
        /// <param name="onError"></param>
        public void Delete(string[] cols, string[] colsValues, Action<int> onFinished, Action<MySqlException> onError)
        {
            string query = "DELETE FROM " + tableName + " WHERE " + cols[0] + " = '" + colsValues[0]+"'";
           

            for (int i = 1; i < colsValues.Length; ++i)
            {
                query += " or " + cols[i] + " = '" + colsValues[i]+"'";
            }
            Debug.Log(query);
            MySQLExecuter.Singleton.ExecuteNonQuery(query,onFinished,onError);
        }



        /// <summary>
        /// E.g.: Select selectedItems... from tableName where conditioncol[0] = values[0] and conditioncol[1] > values[1]. 
        /// Code: SelectWhere(selectedItems, new string[]{"conditioncol0", "conditioncol1"}, new string[]{"=",">"},
        /// new string[]{"value0", "value1"))
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <param name="conditionCols"></param>
        /// <param name="operations"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public void SelectWhere(string[] selectedItems, string[] conditionCols, 
            string[] operations, string[] values, Action<DataSet> onFinished, Action<MySqlException> onError)
        {
            if (conditionCols.Length != operations.Length || operations.Length != values.Length)
            {
                throw new Exception("col.Length != operation.Length != values.Length");
            }

            string query = "SELECT " + selectedItems[0];

            for (int i = 1; i < selectedItems.Length; ++i)
            {
                query += ", " + selectedItems[i];
            }

            query += " FROM " + tableName + " WHERE " + conditionCols[0] + operations[0] + "'" + values[0] + "' ";

            for (int i = 1; i < conditionCols.Length; ++i)
            {
                query += " AND " + conditionCols[i] + operations[i] + "'" + values[i] + "' ";
            }

            MySQLExecuter.Singleton.ExecuteQuery(query,onFinished,onError);
        }


        public void Select(string[] items, Action<DataSet> onFinished, Action<MySqlException> onError)
        {
            string query = "SELECT " + items[0];

            for (int i = 1; i < items.Length; ++i)
            {
                query += ", " + items[i];
            }

            query += " FROM " + tableName;

            MySQLExecuter.Singleton.ExecuteQuery(query,onFinished,onError);
        }


        public void Select(string tableName,Action<DataSet> onFinished, Action<MySqlException> onError)
        {
            string query = "SELECT * FROM " + tableName;
            MySQLExecuter.Singleton.ExcuteQuery(query,onFinished,onError);
        }
    }
}
