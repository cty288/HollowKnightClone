using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace MikroFramework.DatabaseKit.MySQLConnector
{
    public class MySQLExecuter : MikroSingleton<MySQLExecuter> {
        private string connectionString;

        private MySQLExecuter() {
            connectionString= string.Format("Server = {0}; port = {1}; Database = {2}; User ID = {3}; Password = {4}; Pooling=true; " +
                                            "Charset = {5};", MySQLConnectorConfig.IP, MySQLConnectorConfig.Port, 
                MySQLConnectorConfig.DatabaseName, MySQLConnectorConfig.UserID, MySQLConnectorConfig.Password,
                MySQLConnectorConfig.Charset);
        }

        


        #region Simple command (no parameters)

        public async void ExecuteNonQuery(string SQLString, Action<int> onFinished, Action<MySqlException> onError)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        onFinished.Invoke(rows);
                    }
                    catch (MySqlException e)
                    {
                        onError.Invoke(e);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }


        public async void ExecuteNonQueryTransaction(ArrayList SQLStringList,Action onFinished, Action <MySqlException> onError)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    tx.Commit();
                    onFinished.Invoke();

                }
                catch (MySqlException E)
                {
                    tx.Rollback();
                    onError.Invoke(E);
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                }
            }
        }


        public async void ExecuteScalar(string SQLString, Action<object> onFinished, Action<MySqlException> onError)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(SQLString, connection);
                
                try
                {
                    connection.Open();
                    object obj = await cmd.ExecuteScalarAsync();
                    if ((System.Object.Equals(obj, null)) || (System.Object.Equals(obj, System.DBNull.Value)))
                    {
                        onFinished.Invoke(null);
                    }
                    else
                    {
                        onFinished.Invoke(obj);
                    }
                }
                catch (MySqlException E)
                {
                    onError.Invoke(E);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }


        public async void ExecuteReader(string strSQL, Action<List<object[]>> onFinished, Action<MySqlException> onError)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(strSQL, connection);
            try
            {
                connection.Open();
                DbDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows) {
                    List<object[]> results = new List<object[]>();
                    while (reader.Read()) {
                        object[] rowDatas = new object[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++) {
                            rowDatas[i] = reader[i];
                        }
                        results.Add(rowDatas);
                    }
                    onFinished?.Invoke(results);
                }
                else {
                    onFinished?.Invoke(null);
                }
                
            }
            catch (MySqlException e)
            {
                onError.Invoke(e);
            }
            finally
            {
             cmd.Dispose();
             connection.Close();
            }
        }


        public async void ExecuteQuery(string SQLString, Action<DataSet> onFinished, Action<MySqlException> onError)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(SQLString, connection);
                    await da.FillAsync(ds);
                    onFinished.Invoke(ds);
                }
                catch (MySqlException ex)
                {
                    connection.Close();
                    onError(ex);
                }
                finally
                {
                    connection.Close();
                }
                
            }
        }

        #endregion

        #region Command with parameters
        public async void ExecuteNonQuery(string SQLString, Action<int> onFinished, Action<MySqlException> onError,
            params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = await cmd.ExecuteNonQueryAsync();
                        cmd.Parameters.Clear();
                        onFinished.Invoke(rows);
                    }
                    catch (MySqlException E)
                    {
                        onError.Invoke(E);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }


        public async void ExecuteNonQueryTran(Dictionary<string, MySqlParameter[]> SQLStringList,
            Action onFinished, Action<MySqlException> onError) {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        foreach (KeyValuePair<string, MySqlParameter[]> keyValuePair in SQLStringList) {
                            string cmdText = keyValuePair.Key.ToString();
                            MySqlParameter[] cmdParms = keyValuePair.Value;

                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = await cmd.ExecuteNonQueryAsync();
                            cmd.Parameters.Clear();

                        }

                        trans.Commit();
                        onFinished.Invoke();

                    }
                    catch(MySqlException e)
                    {
                        trans.Rollback();
                        onError.Invoke(e);
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
        }


        public async void GetSingle(string SQLString, Action<object> onFinished,
            Action<MySqlException> onError, params MySqlParameter[] cmdParms) {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = await cmd.ExecuteScalarAsync();
                        cmd.Parameters.Clear();
                        if ((System.Object.Equals(obj, null)) || (System.Object.Equals(obj, System.DBNull.Value)))
                        {
                            onFinished?.Invoke(null);
                        }
                        else
                        {
                            onFinished?.Invoke(obj);
                        }
                    }
                    catch (MySqlException e)
                    {
                        onError.Invoke(e);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }


        public async void ExecuteReader(string SQLString, Action<List<object[]>> onFinished,
            Action<MySqlException> onError, params MySqlParameter[] cmdParms) {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                DbDataReader myReader = await cmd.ExecuteReaderAsync();

                if (myReader.HasRows)
                {
                    List<object[]> results = new List<object[]>();
                    while (myReader.Read())
                    {
                        object[] rowDatas = new object[myReader.FieldCount];
                        for (int i = 0; i < myReader.FieldCount; i++)
                        {
                            rowDatas[i] = myReader[i];
                        }
                        results.Add(rowDatas);
                    }
                    onFinished?.Invoke(results);
                }
                else
                {
                    onFinished?.Invoke(null);
                }

                cmd.Parameters.Clear();
            }
            catch (MySqlException e)
            {
                onError.Invoke(e);
            }
            finally
            {
             cmd.Dispose();
             connection.Close();
            }
        }


        public async void ExcuteQuery(string SQLString, Action<DataSet> onFinished,
            Action<MySqlException> onError, params MySqlParameter[] cmdParms){
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        await da.FillAsync(ds, "ds");
                        cmd.Parameters.Clear();
                        onFinished.Invoke(ds);
                    }
                    catch (MySqlException ex)
                    {
                        onError.Invoke(ex);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        #endregion




        private void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = CommandType.Text;//cmdType;

            if (cmdParms != null)
            {
                foreach (MySqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null)) {
                        parameter.Value = DBNull.Value;
                    }
                    
                    cmd.Parameters.Add(parameter);
                }
            }
        }
    }
}
