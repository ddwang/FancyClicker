using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FancyClicker
{
	//
	// https://social.msdn.microsoft.com/Forums/vstudio/en-US/80bb4cc8-0b4d-469b-a106-923c870157c0/help-performing-keyboard-events?forum=vbgeneral
	//

	internal class User32_DLL
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

		//Mouse actions
		public static uint MOUSEEVENTF_LEFTDOWN = 0x02;
		public static uint MOUSEEVENTF_LEFTUP = 0x04;
		public static uint MOUSEEVENTF_RIGHTDOWN = 0x08;
		public static uint MOUSEEVENTF_RIGHTUP = 0x10;
	}
}
