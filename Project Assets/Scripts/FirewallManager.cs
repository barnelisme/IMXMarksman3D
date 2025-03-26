using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FirewallManager : MonoBehaviour
{
    void Start()
    {
        AddFirewallRule();
    }

    void AddFirewallRule()
    {
        string exePath = Process.GetCurrentProcess().MainModule.FileName;
        RunCommand($"netsh advfirewall firewall add rule name=\"{Application.productName}\" dir=in action=allow program=\"{exePath}\" enable=yes");
        RunCommand($"netsh advfirewall firewall add rule name=\"{Application.productName}\" dir=out action=allow program=\"{exePath}\" enable=yes");
    }

    void RunCommand(string command)
    {
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + command)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Verb = "runas" // Requests Admin Privileges
        };

        Process process = new Process { StartInfo = psi };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        Debug.Log($"Firewall Command Output: {output}");
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError($"Firewall Command Error: {error}");
        }
    }
}
