using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Schema;

namespace RNG
{
    class Program
    {
        private static List<int> _values = new List<int>();
        private static int _iterations = 0;
        static void Main(string[] args)
        {
            var warmupIterations = 1000000;
            while (_iterations < warmupIterations)
            {
                GenerateRandomNumber(false);
                _iterations += 1;
            }
            
            Console.WriteLine("Done warming up");

            _values.Clear();
            for (var i = 0; i < 100; i++)
            {
                GenerateRandomNumber(true);
            }

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    Console.Write($"{_values[(i * 10) + j]:000}  ");
                }
                
                Console.WriteLine();
            }
            
            Console.WriteLine("Done output");
        }

        static void GenerateRandomNumber(bool addToList)
        {
            var rng = new RandomGenerator();
            var value = rng.Next(1, 401);
            while (_values.Contains(value))
            {
                value = rng.Next(1, 401);
            }

            if (addToList)
            {
                _values.Add(value);
            }
        }
    }
    
    public class RandomGenerator
    {
        readonly RNGCryptoServiceProvider csp;

        public RandomGenerator()
        {
            csp = new RNGCryptoServiceProvider();
        }

        public int Next(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue)
            {
                throw new ArgumentOutOfRangeException("minValue must be lower than maxExclusiveValue");
            }

            long diff = (long)maxExclusiveValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        private uint GetRandomUInt()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            csp.GetBytes(buffer);
            return buffer;
        }
    }
} 