using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		#region Members
		private ClickerStateModel? _clickerStateModel;
		private uint _lastMouseX, _lastMouseY;

		private IKeyboardMouseEvents _globalHook;
		private DispatcherTimer _dispatcherTimer;
		#endregion

		public MainWindow()
		{
			InitializeComponent();

			_globalHook = Hook.GlobalEvents();
			_globalHook.MouseUpExt += GlobalHook_MouseUpExt;
			_globalHook.MouseDownExt += GlobalHook_MouseDownExt;

			Action danceAction = () => { Trace.WriteLine("blah!"); };
			_globalHook.OnCombination(new Dictionary<Combination, Action> {
				{ Combination.FromString("Control+LWin+D"), danceAction},
			});

			_dispatcherTimer = new DispatcherTimer();
			_dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);  // (1000ms / 10ms = 100 cps)
			_dispatcherTimer.Tick += DispatcherTimer_Tick;

			_clickerStateModel = (ClickerStateModel?)DataContext;

			Closing += MainWindow_Closing;
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// unsubscribe handlers
			_dispatcherTimer.Tick -= DispatcherTimer_Tick;
			_globalHook.MouseUpExt -= GlobalHook_MouseUpExt;
			_globalHook.MouseDownExt -= GlobalHook_MouseDownExt;

			// dispose hook
			_globalHook.Dispose();
		}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		private void GlobalHook_MouseDownExt(object sender, MouseEventExtArgs e)
		{
			if (e.Button == MouseButtons.Right
				&& !_dispatcherTimer.IsEnabled
				&& (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Windows)) > 0)
			{
				_dispatcherTimer.Start();
				Trace.WriteLine(String.Format("{0} - MouseDown: {1}; ({2}, Y={3})", e.Timestamp, e.Button, e.X, e.Y));

				_lastMouseX = (uint)e.X;
				_lastMouseY = (uint)e.Y;

				e.Handled = true;
			}
		}

		private void GlobalHook_MouseUpExt(object sender, MouseEventExtArgs e)
		{
			if (e.Button == MouseButtons.Right
				&& _dispatcherTimer.IsEnabled)
			{
				_dispatcherTimer.Stop();
				Trace.WriteLine(String.Format("{0} - MouseUp: {1}, ({2}, {3})", e.Timestamp, e.Button, e.X, e.Y));

				e.Handled = true;
			}
		}
		private void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			Trace.WriteLine(String.Format("{0} - auto clicking at ({1}, {2})", Environment.TickCount, _lastMouseX, _lastMouseY));

			User32_DLL.mouse_event(
				User32_DLL.MOUSEEVENTF_LEFTDOWN | User32_DLL.MOUSEEVENTF_LEFTUP,
				_lastMouseX,
				_lastMouseY,
				0,
				0);

			if (_clickerStateModel != null)
				_clickerStateModel.incrementClickCount();
		}
	}
}
