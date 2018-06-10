using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UseFileApp
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

        public Logger()
        {
            this.fileName = string.Format("{0}.txt", Guid.NewGuid());

            //把檔案名稱改成固定: 會出現檔案在使用中的 exception
            //this.fileName = "test.txt";
        }

        private void Init()
        {
            if(File.Exists(this.fileName) == false)
            {
                string _initMessage
                    = string.Format("init at {0:yyyy/MM/dd HH:mm:ss}", DateTime.Now);

                File.WriteAllText(this.fileName, _initMessage);
            }
        }
        
        public void AddEvent(string message)
        {
            this.Init();

            Stopwatch _stopWatch = new Stopwatch();
            StringBuilder _builder = new StringBuilder();

            _stopWatch.Start();

            _builder.AppendLine();
            _builder.Append(message);

            File.AppendAllText(this.fileName, _builder.ToString());
            _stopWatch.Stop();

            Console.WriteLine("{0}\telapsed {1} ms"
                            , this.fileName
                            , _stopWatch.ElapsedMilliseconds);
        }

        public void Delete()
        {
            File.Delete(this.fileName);
        }
    }

}
