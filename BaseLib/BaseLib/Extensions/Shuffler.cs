using System;
using System.Collections.Generic;

namespace BaseLib.Extensions
{
    public static class Shuffler
    {
        private static readonly Random Rnd;

        static Shuffler()
        {
            Rnd = new Random();
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
