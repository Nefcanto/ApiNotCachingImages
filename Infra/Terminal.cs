namespace Infra;

public class Terminal
{
    public static TerminalResult Run(string command, string app = "/bin/bash")
    {
        var process = new ProcessStartInfo(app);
        string result = "";
        process.Arguments = (app == "/bin/bash" ? $@" -c ""{command.Trim()}""" : command.Trim());
        process.RedirectStandardInput = true;
        process.UseShellExecute = false;
        process.WindowStyle = ProcessWindowStyle.Hidden;
        process.CreateNoWindow = true;
        var shell = Process.Start(process);
        process.RedirectStandardOutput = true;
        process.RedirectStandardError = true;
        shell.OutputDataReceived += (sender, e) => result += e.Data + "\n";
        shell.ErrorDataReceived += (sender, e) => result += e.Data + "\n";
        shell.Start();
        shell.BeginOutputReadLine();
        shell.BeginErrorReadLine();
        shell.WaitForExit();
        var terminalResult = new TerminalResult();
        terminalResult.Log = result;
        terminalResult.ExitCode = shell.ExitCode;
        return terminalResult;
    }
}
