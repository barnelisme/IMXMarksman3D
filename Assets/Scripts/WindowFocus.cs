using UnityEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class ForceWindowFocus : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(System.IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern bool ShowWindow(System.IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    const int SW_RESTORE = 9;
    const int KEYEVENTF_KEYUP = 0x0002;
    const byte VK_MENU = 0x12; // ALT key

    float timer = 2f;
    bool isFocused = false;

    void Update()
    {
        if (!isFocused)
        {
            timer -= Time.deltaTime;

            BringToFront();

            if (timer <= 0f)
                isFocused = true;
        }
    }

    void BringToFront()
    {
        var handle = Process.GetCurrentProcess().MainWindowHandle;
        ShowWindow(handle, SW_RESTORE);

        // Simulate ALT key press to trick Windows focus system
        keybd_event(VK_MENU, 0, 0, 0);
        keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, 0);

        SetForegroundWindow(handle);

        UnityEngine.Debug.Log("Trying to bring Unity to front...");
    }
}
