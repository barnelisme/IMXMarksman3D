using UnityEngine;
using System.Diagnostics;
using System.IO;

public class HardwareCaller : MonoBehaviour
{
    void Start()
    {
        string output = RunHardwareReader();
        UnityEngine.Debug.Log("CPU ID: " + output);
    }

    string RunHardwareReader()
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = Application.dataPath + "/Plugins/HardwareInfoReader.exe",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(psi))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                return reader.ReadToEnd().Trim();
            }
        }
    }
}
