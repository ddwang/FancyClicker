using Gma.System.MouseKeyHook;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace FancyClicker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
		//Mouse actions
		private const int MOUSEEVENTF_LEFTDOWN = 0x02;
		private const int MOUSEEVENTF_LEFTUP = 0x04;
		private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
		private const int MOUSEEVENTF_RIGHTUP = 0x10;

		#region Members
		private ClickerStateModel _clickerStateModel;
		private uint _lastMouseX, _lastMouseY;

		private IKeyboardMouseEvents _globalHook;
		private DispatcherTimer _dispatcherTimer;
		#endregion


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public MainWindow()
		{
			InitializeComponent();

			_globalHook = Hook.GlobalEvents();
			_globalHook.MouseDownExt += GlobalHookMouseDownExt;
			_globalHook.MouseUpExt += GlobalHook_MouseUpExt;

			_dispatcherTimer = new DispatcherTimer();
			_dispatcherTimer.Interval = TimeSpan.FromMilliseconds(25);  // (1000ms / 25ms = 40 cps)
			_dispatcherTimer.Tick += DispatcherTimer_Tick;

			_clickerStateModel = (ClickerStateModel?)DataContext;

			Closing += MainWindow_Closing;
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Unsubscribe();
		}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		public void Unsubscribe()
		{
			_globalHook.MouseDownExt -= GlobalHookMouseDownExt;
			_globalHook.MouseUpExt -= GlobalHook_MouseUpExt;
			_dispatcherTimer.Tick -= DispatcherTimer_Tick;

			_globalHook.Dispose();
		}

		private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
		{
			if (e.Button == MouseButtons.Right
				&& (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Windows)) > 0
				&& !_dispatcherTimer.IsEnabled)
			{
				Trace.WriteLine(String.Format("MouseDown: {0}; X={1}, Y={2}; Timestamp={3}", e.Button, e.X, e.Y, e.Timestamp));
				_dispatcherTimer.Start();

				_lastMouseX = (uint)e.X;
				_lastMouseY = (uint)e.Y;

				e.Handled = true;
			}
		}

		private void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			Trace.WriteLine(String.Format("auto clicking at ({0}, {1})", _lastMouseX, _lastMouseY));
			mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, _lastMouseX, _lastMouseY, 0, 0);

			_clickerStateModel.incrementClickCount();
		}

		private void GlobalHook_MouseUpExt(object sender, MouseEventExtArgs e)
		{
			if (e.Button == MouseButtons.Right
				&& _dispatcherTimer.IsEnabled)
			{
				Trace.WriteLine(String.Format("MouseUp: {0}; X={1}, Y={2}", e.Button, e.X, e.Y, e.Timestamp));
				_dispatcherTimer.Stop();

				e.Handled = true;
			}
		}
	}
}
