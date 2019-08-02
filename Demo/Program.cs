using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace OldSystem
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            void WriteData(Action<string> action, ManualResetEventSlim resetEvent, DataReceivedEventArgs e)
            {
                try
                {
                    if (e.Data == null)
                    {
                        resetEvent.Set();
                        return;
                    }

                    action(e.Data);
                }
                catch (Exception ex)
                {
                    try
                    {
                        Console.WriteLine($"Error occured handling message: {ex}");
                    }
                    catch
                    {
                        // Ignore
                    }
                }
            }

            using (var outputResetEvent = new ManualResetEventSlim(false))
            using (var errorResetEvent = new ManualResetEventSlim(false))
            using (var process = new Process())
            {
                Console.WriteLine("Starting child process");

                process.StartInfo.FileName = @"C:\windows\System32\WindowsPowershell\v1.0\PowerShell.exe";
                process.StartInfo.Arguments = @"-Command ""$ErrorActionPreference = 'Stop'; & 'c:\windows\system32\curl.exe' -foo""";
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += (sender, e) => WriteData(Console.WriteLine, outputResetEvent, e);
                process.ErrorDataReceived += (sender, e) => WriteData(Console.Error.WriteLine, errorResetEvent, e);

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
                process.CancelErrorRead();
                process.CancelOutputRead();

                outputResetEvent.Wait(CancellationToken.None);
                errorResetEvent.Wait(CancellationToken.None);


                return process.ExitCode;
            }
        }
    }
}