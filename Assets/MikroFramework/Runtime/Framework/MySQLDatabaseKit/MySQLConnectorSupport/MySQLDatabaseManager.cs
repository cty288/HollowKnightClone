using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace MikroFramework.DatabaseKit.MySQLConnector
{
    public class MySQLDatabaseManager : MikroSingleton<MySQLDatabaseManager> {
        public void CreateTable(string name, string[] cols, string[] colType, Action<int> onFinished,
            Action<MySqlException> onError)
        {
            if (cols.Length != colType.Length)
            {
                throw new Exception("columns.Length != colType.Length");
            }

            string query = "CREATE TABLE " + name + " (" + cols[0] + " " + colType[0];

            for (int i = 1; i < cols.Length; ++i)
            {
                query += ", " + cols[i] + " " + colType[i];
            }

            query += ")";

            MySQLExecuter.Singleton.ExecuteNonQuery(query, onFinished, onError);
        }

        public void CreateTableAutoID(string name, string[] cols, string[] colType, Action<int> onFinished,
            Action<MySqlException> onError)
        {
            if (cols.Length != colType.Length)
            {
                throw new Exception("columns.Length != colType.Length");
            }

            string query = "CREATE TABLE " + name + " (" + cols[0] + " " + colType[0] + " NOT NULL AUTO_INCREMENT";

            for (int i = 1; i < cols.Length; ++i)
            {
                query += ", " + cols[i] + " " + colType[i];
            }

            query += ", PRIMARY KEY (" + cols[0] + ")" + ")";

            MySQLExecuter.Singleton.ExecuteNonQuery(query, onFinished, onError);
        }
    }
}
