using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace Lesson6_HomeWork_Task2
{
/*
   2. *В некой директории лежат файлы. По структуре они содержат 3 числа, разделенные пробелами.
       Первое число - целое, обозначает действие, 1- умножение и 2- деление,
       остальные два - числа с плавающей точкой.
       
       Написать многопоточное приложение, выполняющее вышеуказанные действия над числами и
       сохраняющими результат в файл result.dat. Количество файлов 
       в директории заведомо много.
 */
    class Program
    {
       [ThreadStatic]
     private  double first;
     private  double second;
     private  double result;
     private  List<double> done=new List<double>();
     private  string[] tmp = new string[3];//Коллекция конкретного файла разбитого на элементы
     private  List<string> dirs = new List<string>(Directory.GetFiles(@"local"));//коллекция ссылок на файлы
     private  List<string> task = new List<string>();//Коллекция файлов
     private  string file;

        /// <summary>
        /// Непосредственное выполнение действий над файлами
        /// </summary>
        private void Do(string[] ComandConfig)
        {
             string command= ComandConfig[0];
             first = Convert.ToDouble(ComandConfig[1]);
             second = Convert.ToDouble(ComandConfig[2]);

            if (command == "1") Myltiply(first, second);
            else if (command == "2") Devide(first, second);
            else Console.WriteLine("Error");
            done.Add(result);
        }
        private double Myltiply(double a, double b) => result = (a * b);
        private double Devide(double a, double b) => result = a / b;

        /// <summary>
        /// Запись результата выполнения программы в файл
        /// </summary>
        private void Write()
        {
            if(done!=null &done.Count>0)
            {
                using (StreamWriter sw = new StreamWriter("result.dat"))
                {
                   TextWriter.Synchronized(sw);
                    /*
                     * используя паралельные потоки в записи и чтении файлов, выходило много ошибок (раз в 5 запусков)
                     * даже обертка не помогала, поэтому использую ForEach обычный
                     * 
                     */
                    #region Paralel ForeEach
                    //try
                    //{
                    //    Parallel.ForEach(done, sw.WriteLine);
                    //    sw.AutoFlush = true;
                    //}
                    //catch (Exception ex)
                    // {
                    //    Console.WriteLine(ex.Message);
                    // }
                    #endregion

                    foreach (var el in done)
                        sw.WriteLine(el);
                }
            }
        }
 
        /// <summary>
        /// Подготовка коллекция для работы, считывание
        /// и разделение на элементы
        /// </summary>
        private void Start()
        {
            if(dirs!=null & dirs.Count>0)
            {
                //Parallel.For(0, dirs.Count, i =>
                //{
                foreach(var el in dirs)
                    using (StreamReader sr = new StreamReader(el))
                    {
                        TextReader.Synchronized(sr);
                        task.Add(sr.ReadLine());
                    };
                //});

                Parallel.For(0, task.Count, i =>
                {
                    file = task[i];
                    tmp = file.Split(' ');
                    Do(tmp);
                });
            }     
        }

        static void Main(string[] args)
        {
            
            Program pr = new Program();
            Console.WriteLine("\tStart", Console.ForegroundColor = ConsoleColor.Blue);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 1
            };

            #region Paralel

            Parallel.Invoke(options, pr.Start, pr.Write);//когда файлов много и потоков больше 1го, незаписывает результаты 
            //изза конфликта потоков, первый неуспевает все прочесть и сделать, как запускается этот и не выполняется изза 
            //отсутсвия данных и доступа к ним, в этом случае таск лучше работает, изза указания чтоб ожидалось окончание потока
            #endregion

            #region Использование Task

            //Task task = Task.Factory.StartNew(pr.Start);
            //task.Wait();
            //task.Dispose();
            //task = Task.Factory.StartNew(pr.Write);
            //task.Wait();
            //task.Dispose();
            //stopwatch.Stop();

            #endregion

            foreach (var el in pr.done)
                Console.Write($"{el:0.00}\t" ,Console.ForegroundColor = ConsoleColor.Green);
            Console.WriteLine("\tStop", Console.ForegroundColor = ConsoleColor.Blue);
            Console.WriteLine($"\t stopwatch {stopwatch.Elapsed.TotalMilliseconds}", Console.ForegroundColor = ConsoleColor.Yellow);
            stopwatch.Reset();

            Console.ReadLine();
        }
    }
}
