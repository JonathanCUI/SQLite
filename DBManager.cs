using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections;

namespace SqliteDB
{
    public class DBManager
    {

        //data members
        private SqliteConnection _dbConnection;
        private SqliteCommand _dbCommand;
        private SqliteDataReader _dbReader;

        //singlton
        private static DBManager _instanse;

        public static void Initialize(string pDBFilePath)
        {
            _instanse = new DBManager(pDBFilePath);
        }

        public static DBManager Instance
        {
            get
            {
                return _instanse;
            }
        }

        private DBManager(string pDBFilePath)
        {
            OpenDB(pDBFilePath);
        }

        private void OpenDB(string pDBFileName)
        {
            try
            {
                TextAsset txt = Resources.Load(pDBFileName, typeof(TextAsset)) as TextAsset;
                string dbFilePath = Application.persistentDataPath + "/" + Path.GetFileNameWithoutExtension(pDBFileName);
                File.WriteAllBytes(dbFilePath, txt.bytes);
                string connStr = "data source=" + dbFilePath;
                _dbConnection = new SqliteConnection(connStr);
                _dbConnection.Open();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Open DB Failed:" + ex.ToString());
            }
        }
        private void CloseDB()
        {
            //Debug.Log("CloseDB instance");
            if (_dbCommand != null)
            {
                _dbCommand.Dispose();
                _dbCommand = null;
            }
            if (_dbReader != null)
            {
                _dbReader.Close();
                _dbReader = null;
            }
            if (_dbConnection != null)
            {
                _dbConnection.Close();
                _dbConnection = null;
            }
            _instanse = null;
        }

        public static SqliteDataReader ExecuteQuery(string pSQL)
        {
            return _instanse.ExecuteSQL(pSQL);
        }

        private SqliteDataReader ExecuteSQL(string pSQL)
        {
            if (_dbCommand != null)
            {
                _dbCommand.Dispose();
                _dbCommand = null;
            }
            _dbCommand = _dbConnection.CreateCommand();
            _dbCommand.CommandText = pSQL;
            if (_dbReader != null)
            {
                _dbReader.Close();
                _dbReader = null;
            }
            _dbReader = _dbCommand.ExecuteReader();
            return _dbReader;
        }

        public static void Close()
        {
            _instanse.CloseDB();
        }
    }
}
