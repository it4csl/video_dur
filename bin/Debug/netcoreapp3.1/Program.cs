using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace video_dur
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string rootDir = @"q:\";

            string[] filesInRootDir = Directory.GetFiles(rootDir); // путь к папке
            string[] dirInRootDir = Directory.GetDirectories(rootDir);

            double allTime = 0;
            int lineLength = 0;
            foreach (var path in dirInRootDir)
            {
                List<FileInfo> allFileFromDirAndSubDir = GetAllFileFromDirAndSubDir(path);
                double allTimeDir = 0;
                foreach (var f in allFileFromDirAndSubDir)
                {
                    double seconds = getDurationFromFile(f.FullName);
                    allTimeDir += seconds;
                }
                allTime += allTimeDir;
                string time = SecondsInTime(allTimeDir);
                string outData = $"{path,-90} {time,-17} | {allFileFromDirAndSubDir.Count}";
                lineLength = outData.Length;
                Console.WriteLine(outData);
            }
            Console.WriteLine("");
            string LL = new string('#', lineLength);
            Console.WriteLine(LL);
            foreach (string file in filesInRootDir)
            {
                double seconds = getDurationFromFile(file);
                allTime += seconds;
                string time = SecondsInTime(seconds);
                Console.WriteLine($"{file,-90} {time,-15}");
            }

            Console.WriteLine("");
            Console.WriteLine(LL);
            // ############################################################################
            // вывод общего времени всех файлов
            // ############################################################################
            string timeAll = SecondsInTime(allTime);
            List<FileInfo> countAllFile = GetAllFileFromDirAndSubDir(rootDir);
            Console.WriteLine($"File count: {countAllFile.Count}\nAll time: {timeAll}");
            // ############################################################################
            sw.Stop();
            TimeSpan interval2 = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            string time2 = $"{interval2.Seconds}s:{interval2.Milliseconds}ms";
            Console.WriteLine(time2);           
        }

        static string SecondsInTime(double seconds)
        {
            TimeSpan interval = TimeSpan.FromSeconds(seconds);
            string time = $"{interval.Days}d:{interval.Hours}h:{interval.Minutes}m:{interval.Seconds}s";
            return time;
        }
        private static double getDurationFromFile(string file)
        {
            Process process = new Process();

            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = @"/c ffprobe -v quiet -show_entries format=duration -of csv=""p=0"" " + $"\"{file}\"";
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
        private static List<FileInfo> GetAllFileFromDirAndSubDir(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            string[] filters = { "*.mkv", "*.avi", "*.mp4", "*.ts", "*.mpg" };
            List<FileInfo> files = filters.SelectMany(filter => dir.EnumerateFiles(filter, SearchOption.AllDirectories)).ToList();
            return files;
        }
    }
}
