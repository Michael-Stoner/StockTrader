using StockTrader.Dtos;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace StockTrader
{
    public static class Utility
    {
        public static Object LockObject = new Object();
        public static int ThreadsRunning = 0;

        public static ConcurrentQueue<string> OutputQueue { get; set; } = new ConcurrentQueue<string>();

        public static double CalculatePercent(double buy, double sell)
        {
            if (buy == 0) return 0;

            return (sell - buy) / buy;
        }

        public static void WriteToConsole()
        {
            while (true)
            {
                if (OutputQueue.TryDequeue(out string output))
                {
                    Console.WriteLine(output);
                }
                if (OutputQueue.Count == 0)
                {
                    Thread.Sleep(10);
                }
            }
        }

        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }

    public static class DoubleExtension
    {
        public static bool AlmostEqualTo(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.0000001;
        }

        public static bool GreaterThanAlmostEqualTo(this double value1, double value2)
        {
            var equal = AlmostEqualTo(value1, value2);

            if (equal) return true;

            return value1 > value2;
        }

        public static bool LessThanAlmostEqualTo(this double value1, double value2)
        {
            var equal = AlmostEqualTo(value1, value2);

            if (equal) return true;

            return value1 < value2;
        }
    }
}