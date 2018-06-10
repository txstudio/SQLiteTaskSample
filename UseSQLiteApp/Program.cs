using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace UseSQLiteApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Task> _list;

            _list = new List<Task>();

            for (int i = 0; i < 10; i++)
                _list.Add(new Task(() => { RunLogger(); }));

            for (int i = 0; i < _list.Count; i++)
                _list[i].Start();

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static void RunLogger()
        {
            Logger _logger;
            string _message;

            _logger = new Logger();

            for (int i = 0; i < 30; i++)
            {
                _message = string.Format("event time {0:yyyy/MM/dd HH:mm:ss}", DateTime.Now);

                _logger.AddEvent(_message);
            }

            _logger.Delete();
        }
    }


    public sealed class Logger
    {
        private string fileName;
        private string connectionString;

        public Logger()
        {
            this.fileName = "sample.sqlite";
            this.connectionString = string.Format("Data Source=./{0}", this.fileName);
        }

        private void Init()
        {
            if (File.Exists(this.fileName) == true)
                return;

            string _sqlString = @"CREATE TABLE Logger
(
	No			INTEGER PRIMARY KEY AUTOINCREMENT,
	EventTime	INTEGER,
	Content		TEXT
)";

            using (SqliteConnection _conn = new SqliteConnection())
            {
                _conn.ConnectionString = this.connectionString;

                SqliteCommand _cmd = new SqliteCommand();
                _cmd.Connection = _conn;
                _cmd.CommandText = _sqlString;

                _conn.Open();
                try
                {
                    _cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }
                _conn.Close();
            }
        }

        public void AddEvent(string message)
        {
            this.Init();

            Stopwatch _stopWatch = new Stopwatch();

            string _sqlString;

            _sqlString = @"
INSERT INTO Logger (EventTime,Content) 
    VALUES(@EventTime,@Content)";

            _stopWatch.Start();

            //將事件紀錄到 SQLite
            using (SqliteConnection _conn = new SqliteConnection())
            {
                _conn.ConnectionString = this.connectionString;

                SqliteCommand _cmd = new SqliteCommand();
                _cmd.Connection = _conn;
                _cmd.CommandText = _sqlString;
                _cmd.Parameters.AddWithValue("@EventTime", DateTime.Now);
                _cmd.Parameters.AddWithValue("@Content", message);

                _conn.Open();
                _cmd.ExecuteNonQuery();
                _conn.Close();
            }


            _stopWatch.Stop();
            
            Console.WriteLine("{0}\telapsed {1} ms"
                            , this.fileName
                            , _stopWatch.ElapsedMilliseconds);
        }

        public void Delete()
        {
            //File.Delete(this.fileName);
        }
    }
}
