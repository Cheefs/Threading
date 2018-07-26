using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;


namespace Task2
{/// <summary>
/// 2. *Написать приложение, 
///      выполняющее парсинг CSV-файла, произвольной структуры и 
///      сохраняющего его в обычный TXT-файл. Все операции проходят в потоках.
///      CSV-файл заведомо имеет большой объём.
/// </summary>
    class Program
    {
        #region Для красоты
        private static void Print(string msg, ConsoleColor color)
        {
           
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
        }


        #endregion

        private static object lockObject = new object();

        private static string inPath = "test.csv";
        private static string outPath = "test.txt";
        [ThreadStatic]
        public List<string> db = new List<string>();
        public List<string> tmp = new List<string>();

        /// <summary>
        /// Чтение файла
        /// </summary>
        /// <param name="state"></param>
        public void Read(object state)
        {

            Print("\tRead Thread Start", ConsoleColor.Yellow);
            StreamReader sr = new StreamReader(inPath);

            while (!(sr.EndOfStream))
                db.Add(sr.ReadLine());

            Print("\tRead Thread Completed", ConsoleColor.Yellow);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Replace));
        }
        /// <summary>
        /// Замена разделителей 
        /// </summary>
        public void Replace(object state)
        {

            Print($"\tReplace Thread Start.", ConsoleColor.Green);

            Char delimiter = ';';
            if (db.Count != 0)
                for (int i = 0; i < db.Count; i++)
                {
                    tmp.Add(db[i].Replace(delimiter, '|'));
                }
            Print($"\tReplace Thread Completed.", ConsoleColor.Green);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Write));
        }
        /// <summary>
        /// Запись в файл
        /// </summary>
        public void Write(object state)
        {
            StreamWriter sw = new StreamWriter(outPath);
            Print($"\tWriteToTxt Thread Start.", ConsoleColor.Blue);
            for (int i = 0; i < tmp.Count; i++)
            {
                sw.AutoFlush = true;
                sw.WriteLine("\t"+tmp[i]);
            }
            Print("\tWriteToTxt Thread Completed.", ConsoleColor.Blue);
        }


        static void Main(string[] args)
        {
            Program pr = new Program();
            ThreadPool.QueueUserWorkItem(new WaitCallback(pr.Read));
            Console.ReadLine();
        }
    }
}
