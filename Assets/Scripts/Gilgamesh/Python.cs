using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum EnvironmentState
{
    NoPython,
    NoVirtualEnv,
    AllGood
}

public static class Python
{
    public static string PythonPath = "python";

    public static string WorkspacePath
    {
        get => PlayerPrefs.GetString("AGENCE_WORKSPACE_PATH", null);
        set => PlayerPrefs.SetString("AGENCE_WORKSPACE_PATH", value);
    }

    public static string SubmissionsFolder => WorkspacePath + "/Submissions";
    public static string TrainingsFolder => WorkspacePath + "/Trainings";
    public static string VirtualEnvPath => WorkspacePath + "/VirtualEnv";

    public static EnvironmentState CheckEnvironment()
    {
        var exePath = Path.Combine(VirtualEnvPath, "Scripts/python.exe");
        var isEnvironmentValid = File.Exists(exePath);
        if (isEnvironmentValid) return EnvironmentState.AllGood;
        return IsPythonInstalled() ? EnvironmentState.NoVirtualEnv : EnvironmentState.NoPython;
    }

    public static bool IsPythonInstalled()
    {
        var pyPath = PythonPath;
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pyPath,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };

            process.Start();

            // e.g. "Python 2.7.17"
            var output = process.StandardOutput.ReadToEnd();
            if (output == "") output = process.StandardError.ReadToEnd();
            var split = output.Split(' ');
            if (split.Length < 2)
            {
                Debug.LogError($"Bad output for `{pyPath} --version`: {output}");

                return false;
            }
            var version = new Version(split[1]);
            if (version < new Version(3, 6))
            {
                Debug.LogError($"Wrong Version of Python ({version}). Please upgrade to 3.6 or later.");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error opening python at {pyPath}");
            Debug.LogException(e);
            return false;
        }

        return true;
    }

    public static void InstallWorkspace(string workspacePath)
    {
        WorkspacePath = workspacePath;
        Directory.CreateDirectory(WorkspacePath);

        Directory.CreateDirectory(SubmissionsFolder);
        Directory.CreateDirectory(TrainingsFolder);
        Directory.CreateDirectory(VirtualEnvPath);
    }

    public static async void InstallEnv(Action<string> onProgress = null, Action<int> onComplete = null)
    {
        var pyPath = PythonPath;
        var envPath = Path.GetFullPath(VirtualEnvPath);
        var scriptName = "install.bat";
        var builder = new StringBuilder();

        builder.AppendLine($"\"{pyPath}\" -m venv {envPath} --clear");
        builder.AppendLine($@"call {envPath}\Scripts\activate");
        builder.AppendLine(@"pip3 install mlagents");


        var commandPath = Path.Combine(WorkspacePath, scriptName);
        File.WriteAllText(commandPath, builder.ToString());

        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {scriptName}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = WorkspacePath
            }
        })
        {


            var sb = new StringBuilder();

            process.OutputDataReceived += (sender, args) => sb.AppendLine(args.Data);
            process.ErrorDataReceived += (sender, args) => sb.AppendLine(args.Data);
            process.Start();
            var res = await Task.Run(() =>
            {
                try
                {

                    while (true)
                    {
                        var line = process.StandardOutput.ReadLine();
                        if (line == null) break;
                        onProgress.Invoke(line);
                    }

                    Debug.Log(sb.ToString());

                    process.WaitForExit();

                    return process.ExitCode;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);

                    return -1;
                }
            });

            onComplete?.Invoke(res);
        }
    }

    public static void LaunchWindowsScript(string hyperPath)
    {
        var envPath = Path.GetFullPath(VirtualEnvPath);

        var builder = new StringBuilder();

        builder.AppendLine($@"call {envPath}\Scripts\activate");

        hyperPath = Path.GetFullPath(hyperPath);
        builder.AppendLine($"start cmd.exe /c mlagents-learn {hyperPath} --train");

        var scriptName = "command.bat";

        var commandPath = Path.Combine(WorkspacePath, scriptName);
        File.WriteAllText(commandPath, builder.ToString());

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {scriptName}",
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = WorkspacePath
            }
        };
        process.Start();
        process.WaitForExit();
    }
}
