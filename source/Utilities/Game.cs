using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Drawing;

namespace r6recoilv2.Utilities
{
    class Game
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        // Token: 0x06000012 RID: 18
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(ushort virtualKeyCode);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
        private const uint MOUSEEVENTF_MOVE = 0x0001; // Flag for relative mouse movement

        private static string? GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }

            return null;
        }

        public static bool bGetAsyncKeyState(Keys vKey)
        {
            int asyncKeyState = (int)GetAsyncKeyState(vKey);
            return asyncKeyState == 1 || asyncKeyState == -32768;
        }
        /*
        public static bool IsSiegeActive()
        {
            if (GetActiveWindowTitle() == "Rainbow Six")
            {
            return true;
            }

            return false;
        }*/

        public static void MoveMouse(int DeltaX, int DeltaY)
        {
            mouse_event(MOUSEEVENTF_MOVE, DeltaX, DeltaY, 0, UIntPtr.Zero);
        }


        public static async void RecoilLoop()
        {
            while (true)
            {

                if (MainWindow.RecoilControl & !MainWindow.MouseOverProgram)
                {
                    bool DidPass = false;

                    if (MainWindow.FireMode == 1 && bGetAsyncKeyState(Keys.LButton) && bGetAsyncKeyState(Keys.RButton)) // ADS
                    {
                        DidPass = true;
                    }
                    else if (MainWindow.FireMode == 2 && bGetAsyncKeyState(Keys.LButton) && !bGetAsyncKeyState(Keys.RButton)) // Hipfire
                    {
                        DidPass = true;
                    }

                    if (DidPass)
                    {
                        // Debug.WriteLine(MainWindow.XAxisValue + " " + MainWindow.YAxisValue);
                        MoveMouse(MainWindow.XAxisValue, MainWindow.YAxisValue);
                    }
                }

                await Task.Delay(1);
            }
        }
    }
}
