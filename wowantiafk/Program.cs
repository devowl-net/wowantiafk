using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace wowantiafk
{
    class Program
    {

        /// <summary>
        /// The set of valid MapTypes used in MapVirtualKey
        /// </summary>
        public enum MapVirtualKeyMapTypes : uint
        {
            /// <summary>
            /// uCode is a virtual-key code and is translated into a scan code.
            /// If it is a virtual-key code that does not distinguish between left- and
            /// right-hand keys, the left-hand scan code is returned.
            /// If there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_VSC = 0x00,

            /// <summary>
            /// uCode is a scan code and is translated into a virtual-key code that
            /// does not distinguish between left- and right-hand keys. If there is no
            /// translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK = 0x01,

            /// <summary>
            /// uCode is a virtual-key code and is translated into an unshifted
            /// character value in the low-order word of the return value. Dead keys (diacritics)
            /// are indicated by setting the top bit of the return value. If there is no
            /// translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_CHAR = 0x02,

            /// <summary>
            /// Windows NT/2000/XP: uCode is a scan code and is translated into a
            /// virtual-key code that distinguishes between left- and right-hand keys. If
            /// there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK_EX = 0x03,

            /// <summary>
            /// Not currently documented
            /// </summary>
            MAPVK_VK_TO_VSC_EX = 0x04
        }


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern int MapVirtualKey(uint uCode, uint uMapType);

        /*
            /click MiniMapBattlefieldFrame
            /click DropDownList1Button2
         * //---> JOINT
            /run for i=1,GetNumBattlegroundTypes()do local _,_,_,iR,_=GetBattlegroundInfo(i)if iR then JoinBattlefield(i)end end
         */
        private static string JoinToRandomBG = "/run for i=1,GetNumBattlegroundTypes()do local _,_,_,iR,_=GetBattlegroundInfo(i)if iR then JoinBattlefield(i)end end";
        private static int WM_KEYDOWN = 0x0100;
        private static int WM_KEYUP = 0x0101;
        private static int WM_PASTE = 0x0302;

        static void Main(string[] args)
        {
            var wowWindow = FindWindow(null, "World of Warcraft");

            if (wowWindow != IntPtr.Zero)
            {
                GoFwdBck(wowWindow);
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private static void GoFwdBck(IntPtr wowWindow)
        {
            new Thread(() =>
            {
                Random r = new Random();

                while (true)
                {
                   

                    var dtNow = DateTime.Now;
                    Console.WriteLine(string.Format("-> Go [{0}:{1}:{2}]", dtNow.Hour, dtNow.Minute, dtNow.Second));

                    int rndGo = r.Next(200);

                    SendMessage(wowWindow, WM_KEYDOWN, MapVirtualKey('W', (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR), 0);
                    Thread.Sleep(rndGo);
                    SendMessage(wowWindow, WM_KEYUP, MapVirtualKey('W', (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR), 0);

                    Thread.Sleep(rndGo);

                    SendMessage(wowWindow, WM_KEYDOWN, MapVirtualKey('S', (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR), 0);
                    Thread.Sleep(rndGo);
                    SendMessage(wowWindow, WM_KEYUP, MapVirtualKey('S', (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR), 0);

                    int sleep = 25000 + r.Next(0, 10000);
                    Thread.Sleep(sleep);
                    
                }
            }).Start();
        }

        private static void GoFwdBckLftRght(IntPtr wowWindow)
        {
            new Thread(new ThreadStart(() =>
            {
                Random r = new Random();
                List<Char> ReverseKeys = new List<char>() { 'A', 'D' };

                while (true)
                {
                    Thread.Sleep(10000);
                    char nextKey = ReverseKeys[r.Next(0, ReverseKeys.Count)];
                    Console.WriteLine(string.Format("<- Key [{0}]", nextKey));
                    SendMessage(wowWindow, WM_KEYDOWN, MapVirtualKey(nextKey, (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR), 0);

                    Thread.Sleep(1300);

                    SendMessage(wowWindow, WM_KEYUP, MapVirtualKey(nextKey, (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR), 0);
                }
            })).Start();
            new Thread(new ThreadStart(() =>
            {
                Random r = new Random();
                List<char> GoKeys = new List<char>() { 'W', 'S', };

                while (true)
                {
                    char nextKey = GoKeys[r.Next(0, GoKeys.Count)];
                    Console.WriteLine(string.Format("-> Key [{0}]", nextKey));
                    SendMessage(wowWindow, WM_KEYDOWN, MapVirtualKey(nextKey, (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR), 0);

                    Thread.Sleep(2000);

                    SendMessage(wowWindow, WM_KEYUP, MapVirtualKey(nextKey, (uint)MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR), 0);
                }
            })).Start();
        }
    }
}
