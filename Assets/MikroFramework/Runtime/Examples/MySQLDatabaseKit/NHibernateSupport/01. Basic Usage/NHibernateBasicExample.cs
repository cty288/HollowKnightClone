using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MikroFramework.DatabaseKit.NHibernate;
using MikroNHibernateCore;
using UnityEngine;
using Action = Antlr.Runtime.Misc.Action;
using Random = UnityEngine.Random;

namespace MikroFramework.NHibernate.Examples
{

    public class NHibernateBasicExample : MonoBehaviour
    {
        /*BEFORE read the code below, please make sure:
        1. You need to create the MAPPING and MODEL files of your database tables on a seperate MikroNHibernateCore project
           The project's VS template is saved in the framework folder MySQLDatabaseKit/NHibernateSupport/MikroNHibernateCore.zip
                - Model is a C# class that represents a specific table of your MySQL database
                - Mapping is a config file that actually "maps" your model to the database. You can auto-generate mapping file using some generators

        2. Read NHibernateTableManager.cs and UserTableManagerExtension.cs. They manage insert/delete/search for specific tables

        3. To run this example, make sure you have a MySQL database running on your computer and it has a table called "users".

        4. Columns in your table have the same name specified in Users.hbm.xml in MikroNHibernateCore project

        5. Go to "hibernate.cfg.xml" in your StreamingAssets folder, change its "connection.connection_string"
           to your own database's username, password, etc.

        6. also change its "mapping assembly" to "MikroFramework". 
        
         */


        private NHibernateTableManager<User> userTable;
        void Start() {
            userTable = NHibernateTableManager<User>.Singleton;
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.A)) {
                AddUser(() => {
                    Debug.Log("Add success!");
                });
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                SearchUsername(user => {
                    Debug.Log(user.LastLoginTime);
                });
            }
        }

        private async void AddUser(Action onFinished) {
            //insert a new User Model to the MySQL database using table.add()
            await userTable.Add(new User()
            {
                Username = Random.Range(0, 10000).ToString(),
                Password = Random.Range(50000, 100000).ToString(),
                Playfabid = Random.Range(100000, 500000).ToString()
            });

            onFinished.Invoke();
        }


        private async void SearchUsername(Action<User> onFinished) {
            //Let userTable search a username. userTable will connect to the MySQL server and search. It will
            //return a User model containing all info of the searched user. (like id, login time, etc.)
            User result= await userTable.SearchUsername("cty5");
            onFinished.Invoke(result);
        }
    }
}
