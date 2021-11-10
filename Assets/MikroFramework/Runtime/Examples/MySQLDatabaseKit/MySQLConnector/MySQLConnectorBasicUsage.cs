using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MikroFramework.DatabaseKit.MySQLConnector;
using MySql.Data.MySqlClient;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MikroFramework.Examples
{
    public class MySQLConnectorBasicUsage : MonoBehaviour {
        private MySQLTableManager userTable;

        private void Start() {
            userTable = MySQLTableManager.Create("users");
        }

        private void OnError(MySqlException ex) {
            Debug.Log(ex.Message);
        }

        public void Insert() {
            string[] cols = new[] {"username", "playfabid", "password"};
            string[] values = new[] {
                Random.Range(100, 500).ToString(), Random.Range(1000, 10000).ToString(),
                Random.Range(200, 2000).ToString()
            };
            userTable.InsertInto(cols,values,(num)=>{Debug.Log($"Affected {num} rows");} , OnError);
        }

        public void Delete() {
            userTable.Delete(new[] {"username"}, new[] {"cty8"}, (num) => {
                Debug.Log($"Affected {num} rows");
            }, OnError);
        }

        public void SelectIDUsername() {
            userTable.Select(new[] {"id", "username"}, dataset => {
                DataTableReader reader = dataset.CreateDataReader();

                while (reader.Read()) {
                    Debug.Log(reader[0] + "          " + reader[1]);
                }
            }, OnError);
        }

        public void SelectIDGreaterThan12() {
            userTable.SelectWhere(new []{"id","username"},new []{"id"},
                new []{">"},new []{"12"}, dataset => {
                    DataTableReader reader = dataset.CreateDataReader();

                    while (reader.Read())
                    {
                        Debug.Log(reader[0] + "          " + reader[1]);
                    }
                },OnError);
        }


        public void UpdateInfo() {
            userTable.UpdateInto(new []{"last_login_time"},new []{DateTime.Now.ToString()},
                "id", "15", result => {
                    Debug.Log($"Affected {result} rows");
                },OnError);
        }


    }
}
