using System;
using System.IO;
using System.Diagnostics;

namespace video_dur
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string[] files = Directory.GetFiles(@"q:\"); // путь к папке

            // ############################################################################
            // проверка ffmpeg
            // ############################################################################
            //string file = @"d:\test_parce_video\PogChamps.mp4";
            
            double getDurationFromFile(string file)
            {
                Process process = new Process();

                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = @"/c ffprobe -v quiet -show_entries format=duration -of csv=""p=0"" " + "\"" + file + "\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                StreamReader reader = process.StandardOutput;
                string output = reader.ReadToEnd();

                double celTimeSecond = Convert.ToDouble(output.Split(".")[0]);
                process.WaitForExit();
                process.Close();

                return celTimeSecond;
            }

            double allTime = 0;
            for (int i = 0; i < files.Length; i++)
            {
                double seconds = getDurationFromFile(files[i]);
                
                allTime += seconds;
                TimeSpan interval = TimeSpan.FromSeconds(seconds);
                string time = $"{interval.Hours}h:{interval.Minutes}m:{interval.Seconds}s";

                Console.WriteLine($"{files[i],-90} {time}");
            }
            
            // ############################################################################
            // вывод общего времени всех файлов
            // ############################################################################
            TimeSpan allInterval = TimeSpan.FromSeconds(allTime);
            string timeAll = $"{allInterval.Days}d:{allInterval.Hours}h:{allInterval.Minutes}m:{allInterval.Seconds}s";
            Console.WriteLine(timeAll);
            // ############################################################################
            sw.Stop();
            TimeSpan interval2 = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            string time2 = $"{interval2.Seconds}s:{interval2.Milliseconds}ms";
            Console.WriteLine(time2);
        }
    }
}
