using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace wowantiafk
{
    internal class Program
    {
        private const int MinMovementTimeout = 45000;

        private const int ShortTimeout = 10000;

        private const string WorldOfWarcraftWindowName = "World of Warcraft";

        private const int WM_KEYDOWN = 0x0100;

        private const int WM_KEYUP = 0x0101;

        private const int WM_CHAR = 0x0102;

        [DllImport("user32.dll")]
        private static extern bool PostMessage(uint hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(int uCode, int uMapType);

        private static void Main()
        {
            var wowHandlers = GetWowWindowHandlers().ToArray();

            if (!wowHandlers.Any())
            {
                Console.WriteLine("Wow process not founded");
            }
            else
            {
                foreach (var wowWindow in wowHandlers)
                {
                    AntiAfk(wowWindow);
                }
            }
            
            Console.ReadKey();
        }

        private static IEnumerable<uint> GetWowWindowHandlers()
        {
            return from process in Process.GetProcesses()
                where process.MainWindowTitle == WorldOfWarcraftWindowName
                select (uint) process.MainWindowHandle;
        }

        private static void AntiAfk(uint windowHandler)
        {
            new Thread(() =>
            {
                var randomizer = new Random();
                /*
P The message was posted to the queue with the PostMessage function. No information is available concerning the ultimate disposition of the message.  
S The message was sent with the SendMessage function. This means that the sender doesn’t regain control until the receiver processes and returns the message. The receiver can, therefore, pass a return value back to the sender.  
s The message was sent, but security prevents access to the return value. 
R Each ‘S’ line has a corresponding ‘R’ (return) line that lists the message return value. Sometimes message calls are nested, which means that one message handler sends another message.  

<001492> 0003043E R WM_CAPTURECHANGED
<001493> 0003043E S WM_NCHITTEST xPos:-76 yPos:286
<001494> 0003043E R WM_NCHITTEST nHittest:HTCLIENT
<001495> 0003043E S WM_SETCURSOR hwnd:0003043E nHittest:HTCLIENT wMouseMsg:WM_MOUSEMOVE
<001496> 0003043E R WM_SETCURSOR fHaltProcessing:False
<001497> 0003043E P WM_MOUSEMOVE fwKeys:0000 xPos:1309 yPos:227
>
<001498> 0003043E P WM_KEYDOWN nVirtKey:'W' cRepeat:1 ScanCode:11 fExtended:0 fAltDown:0 fRepeat:0 fUp:0
<001499> 0003043E P WM_CHAR chCharCode:'119' (119) cRepeat:1 ScanCode:11 fExtended:0 fAltDown:0 fRepeat:0 fUp:0
<001500> 0003043E P WM_KEYUP nVirtKey:'W' cRepeat:1 ScanCode:11 fExtended:0 fAltDown:0 fRepeat:1 fUp:1
<001501> 0003043E P WM_KEYDOWN nVirtKey:'S' cRepeat:1 ScanCode:1F fExtended:0 fAltDown:0 fRepeat:0 fUp:0
<001502> 0003043E P WM_CHAR chCharCode:'115' (115) cRepeat:1 ScanCode:1F fExtended:0 fAltDown:0 fRepeat:0 fUp:0
<001503> 0003043E P WM_KEYUP nVirtKey:'S' cRepeat:1 ScanCode:1F fExtended:0 fAltDown:0 fRepeat:1 fUp:1
>
<001504> 0003043E P WM_SYSKEYDOWN nVirtKey:VK_MENU cRepeat:1 ScanCode:38 fExtended:0 fAltDown:1 fRepeat:0 fUp:0
<001505> 0003043E S WM_NCACTIVATE fActive:False
<001506> 0003043E R WM_NCACTIVATE fDeactivateOK:True
                 */
                 
                while (true)
                {
                    var dtNow = DateTime.Now;
                    Console.WriteLine($"-> Movement [{$"{dtNow.Hour:D2}h {dtNow.Minute:D2}m {dtNow.Second:D2}s"}]");

                    var pressTime = 50 + randomizer.Next(50);

                    PressKey(windowHandler, 'W', pressTime);

                    Thread.Sleep(randomizer.Next(200));

                    PressKey(windowHandler, 'S', pressTime);

                    var sleep = MinMovementTimeout + randomizer.Next(0, ShortTimeout);
                    Thread.Sleep(sleep);
                }
            }).Start();
        }

        private static void PressKey(uint handler, char chr, int timeout)
        {
            var virtualKey = MapVirtualKey(chr, (int)MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR);

            /*
                1. WM_KEYDOWN nVirtKey:'W' 
                2. WM_CHAR     chCharCode:'119' (119) 
                3. WM_KEYUP nVirtKey:'W' 
            */

            // 1. WM_KEYDOWN nVirtKey:'W' 
            PostMessage(handler, WM_KEYDOWN, virtualKey, 0);

            // 2. WM_CHAR     chCharCode:'119' (119) 
            PostMessage(handler, WM_CHAR, chr, 0);

            // Press time
            Thread.Sleep(timeout);

            // 3. WM_KEYUP nVirtKey:'W' 
            PostMessage(handler, WM_KEYUP, virtualKey, 0);
        }

        /// <summary>
        ///     The set of valid MapTypes used in MapVirtualKey
        /// </summary>
        private enum MapVirtualKeyMapTypes : uint
        {
            /// <summary>
            ///     uCode is a virtual-key code and is translated into a scan code.
            ///     If it is a virtual-key code that does not distinguish between left- and
            ///     right-hand keys, the left-hand scan code is returned.
            ///     If there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_VSC = 0x00,

            /// <summary>
            ///     uCode is a scan code and is translated into a virtual-key code that
            ///     does not distinguish between left- and right-hand keys. If there is no
            ///     translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK = 0x01,

            /// <summary>
            ///     uCode is a virtual-key code and is translated into an unshifted
            ///     character value in the low-order word of the return value. Dead keys (diacritics)
            ///     are indicated by setting the top bit of the return value. If there is no
            ///     translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_CHAR = 0x02,

            /// <summary>
            ///     Windows NT/2000/XP: uCode is a scan code and is translated into a
            ///     virtual-key code that distinguishes between left- and right-hand keys. If
            ///     there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK_EX = 0x03,

            /// <summary>
            ///     Not currently documented
            /// </summary>
            MAPVK_VK_TO_VSC_EX = 0x04
        }
    }
}