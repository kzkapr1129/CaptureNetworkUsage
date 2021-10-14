using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using SharpPcap;
using PacketDotNet;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        public static void ThreadProcAsync()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                while (true)
                {
                    get(client);

                    Thread.Sleep(500);
                }
            }
        }

        private static async void get(System.Net.Http.HttpClient client)
        {
            var result1 = await client.GetAsync(@"http://www.google.com");
            Console.Write(".");
        }

        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "connect") {
                Thread t = new Thread(new ThreadStart(ThreadProcAsync));
                t.Start();
            }

            // PC上で動いているプロセス一覧を取得
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();
            int i = 0;
            foreach (Process p in ps)
            {
                Console.WriteLine("{0}: {1}", i++, p.ProcessName);
            }

            // 通信量を監視したいアプリを選択
            Console.Write("通信量を見たいプロセスの番号を選択してください：");
            var line = Console.ReadLine();
            var pos = Int32.Parse(line);

            string pn = ps[pos].ProcessName;
            Console.WriteLine("target process: {0}");

            var readOpSec = new PerformanceCounter("Process", "IO Read Operations/sec", pn);
            var writeOpSec = new PerformanceCounter("Process", "IO Write Operations/sec", pn);
            var dataOpSec = new PerformanceCounter("Process", "IO Data Operations/sec", pn);
            var readBytesSec = new PerformanceCounter("Process", "IO Read Bytes/sec", pn);
            var writeByteSec = new PerformanceCounter("Process", "IO Write Bytes/sec", pn);
            var dataBytesSec = new PerformanceCounter("Process", "IO Data Bytes/sec", pn);

            var counters = new List<PerformanceCounter>
                {
                readOpSec,
                writeOpSec,
                dataOpSec,
                readBytesSec,
                writeByteSec,
                dataBytesSec
                };

            while (true)
            {
                // get current value
                foreach (PerformanceCounter counter in counters)
                {

                    float rawValue = counter.NextValue();
                    if (0 < rawValue) {
                        // display the value
                        Console.WriteLine("{0} : {1} Bytes/sec", counter.CounterName, rawValue);
                    }
                }

                System.Threading.Thread.Sleep(1);
            }
        }
    }
}
