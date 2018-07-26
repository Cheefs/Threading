using System;
using System.Threading;
using System.Numerics;


namespace Task1
{
    /*1. Написать приложение, считающее в раздельных потоках:
        a.факториал числа N, которое вводится с клавиатуру;
        b.сумму целых чисел до N, которое также вводится с клавиатуры.
    */
    public class Task1
    {
        private BigInteger sum = 0;
        private int N;
        private static object _locker = new object();

        #region Для красоты
        private static void Print(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg, color);
        }

        private static string Read()
        {
            return Console.ReadLine();
        }
        #endregion

        static void Main()
        {
            Task1 hw = new Task1();
            Print("Введите число больше нуля", ConsoleColor.White);

            try
            {
                hw.N = Convert.ToInt32(Read());
            }
            catch (FormatException ex)
            {
                Print($"\t\tНекоректный вовод, исколючения типа {ex}", ConsoleColor.Red);
            }

            if (hw.N != 0)
            {
                Thread threadFaktorial = new Thread(new ThreadStart(hw.Faktorial))
                {
                    Name = "Faktorial Thread",
                    Priority = ThreadPriority.Normal
                };
                threadFaktorial.Start();

                Thread threadSummOfAll = new Thread(new ThreadStart(hw.Summ))
                {
                    Name = "Summ Thread",
                    Priority = ThreadPriority.Normal
                };
                threadSummOfAll.Start();


                Read();
            }
            else Main();
        }

        public void Faktorial()
        {
            lock (_locker)
            {
                sum = 1;
                for (int i = 1; i <= N; i++)
                    sum *= i;

                Print($"Faktorial of {N} is: {sum}", ConsoleColor.Green);
                Print($"\t{Thread.CurrentThread.Name} Completed.", ConsoleColor.Blue);
            }
        }

        private void Summ()
        {
            lock (_locker)
            {
                int sum = 0;
                for (int i = 1; i <= N; i++)
                    sum += i;
                Print($"Summ of all numbers in {N} is: {sum}", ConsoleColor.Yellow);
                Print($"\t{Thread.CurrentThread.Name} Completed.", ConsoleColor.Cyan);
            }
        }
    }
}