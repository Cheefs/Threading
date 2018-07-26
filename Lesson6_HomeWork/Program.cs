using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson6_HomeWork
{
/*
     1. Даны 2 двумерных матрицы. Размерность 100х100 каждая. Напишите приложение,
       производящее параллельное умножение матриц. Матрицы заполняются случайными целыми
       числами от 0 до10.
*/
    class Program
    {
        const int X = 100;
        const int Y = 100;
        
      int[,] first = new int[Y, X];
      int[,] second = new int[Y, X];
      int[,] r = new int[Y, X];
      List<int[,]> Arrays;
        Random rnd = new Random();

        /// <summary>
        /// Заполнение матриц рандомными данными
        /// </summary>
        private void FillArray(int[,] Array)
        {
            for (int i=0;i<Array.GetLength(0);i++)
            {
              
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    Array[i, j] = rnd.Next(0, 10);
                }
            }
        }
        /// <summary>
        /// Отображение матриц (для тестов)
        /// </summary>
        private void Print(int [,] Array)
        {

            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                    Console.Write($"{Array[i, j]} ");
                    Console.WriteLine();
            }
            Console.WriteLine("\n");
        }

       /// <summary>
       /// Умножение матриц
       /// </summary>
        private void Myltiply(int [,] a, int [,] b)
        {
            Parallel.For(0, a.GetLength(0), i =>
            {
                 Parallel.For(0, b.GetLength(1), j =>
                 {
                      Parallel.For(0, r.GetLength(0), k => r[i, j] += first[i, k] * second[k, j]);
                 });
            });     
        }

        static void Main(string[] args)
        {
        
            Program pr = new Program();
            pr.Arrays=  new List<int[,]>() { pr.first, pr.second };
            Parallel.ForEach(pr.Arrays, pr.FillArray);
            foreach (var el in pr.Arrays)
                pr.Print(el);

            pr.Myltiply(pr.first, pr.second);
            pr.Print(pr.r);
            Console.ReadLine();
        }
    }
}
