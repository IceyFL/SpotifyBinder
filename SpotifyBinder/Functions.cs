using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SpotifyBinder.Functions
{
    public static class MediaFunctions
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private const int KEYEVENTF_KEYUP = 0x0002;

        public static void PlayPause()
        {
            SendMediaKey(Keys.MediaPlayPause);
        }

        public static void Skip()
        {
            SendMediaKey(Keys.MediaNextTrack);
        }

        public static void Previous()
        {
            SendMediaKey(Keys.MediaPreviousTrack);
        }

        private static void SendMediaKey(Keys mediaKey)
        {
            byte vk = (byte)mediaKey;

            keybd_event(vk, 0, 0, 0);
            keybd_event(vk, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}
