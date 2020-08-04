using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            string head = $"{"NAME",-90} {"TIME",-17} | {"COUNT"}";
            string LL = new string('#', head.Length);
            string LS = new string('-', head.Length);
            Console.WriteLine(LL);
            Console.WriteLine(head);
            Console.WriteLine(LL);

            Parallel.ForEach(dirInRootDir, (currentDir) =>
            {
                List<FileInfo> allFileFromDirAndSubDir = GetAllFileFromDirAndSubDir(currentDir);
                double allTimeDir = 0;

                Parallel.ForEach(allFileFromDirAndSubDir, (currentFile) =>
                {
                    double seconds = getDurationFromFile(currentFile.FullName);
                    allTimeDir += seconds;
                });

                allTime += allTimeDir;
                string time = SecondsInTime(allTimeDir);
                string outData = $"{currentDir,-90}| {time,-17}| {allFileFromDirAndSubDir.Count}";
                lineLength = outData.Length;
                
                Console.WriteLine(outData);
                Console.WriteLine(LS);
            });

            //string LL = new string('#', lineLength);
            Console.WriteLine($"\n{LL}");

            Parallel.ForEach(filesInRootDir, (currentFile) =>
            {
                double seconds = getDurationFromFile(currentFile);
                allTime += seconds;
                string time = SecondsInTime(seconds);
                
                Console.WriteLine($"{currentFile,-90}| {time,-15}");
                Console.WriteLine(LS);
            });

            Console.WriteLine($"\n{LL}");
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
            Console.ReadLine();
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