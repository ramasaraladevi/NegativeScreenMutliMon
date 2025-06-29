//Copyright 2011-2012 Melvyn Laily
//http://arcanesanctum.net

//This file is part of NegativeScreen.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NegativeScreen
{
    /// <summary>
    /// Overlay that inverts colours of a single window.
    /// </summary>
    public class WindowOverlay : Form
    {
        private IntPtr hwndMag;
        private IntPtr targetWindow;
        /// <summary>
        /// used when refreshing the control
        /// </summary>
        public IntPtr HwndMag { get { return hwndMag; } }

        public WindowOverlay(IntPtr hwnd)
            : base()
        {
            targetWindow = hwnd;
            RECT rect;
            if (!NativeMethods.GetWindowRect(hwnd, out rect))
            {
                throw new Exception("GetWindowRect()", NativeMethods.GetExceptionForLastError());
            }

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(rect.left, rect.top);
            this.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = false;

            IntPtr hInst = NativeMethods.GetModuleHandle(null);
            if (hInst == IntPtr.Zero)
            {
                throw new Exception("GetModuleHandle()", NativeMethods.GetExceptionForLastError());
            }

            if (NativeMethods.SetWindowLong(this.Handle, NativeMethods.GWL_EXSTYLE, (int)ExtendedWindowStyles.WS_EX_LAYERED | (int)ExtendedWindowStyles.WS_EX_TRANSPARENT) == 0)
            {
                throw new Exception("SetWindowLong()", NativeMethods.GetExceptionForLastError());
            }

            if (!NativeMethods.SetLayeredWindowAttributes(this.Handle, 0, 255, LayeredWindowAttributeFlags.LWA_ALPHA))
            {
                throw new Exception("SetLayeredWindowAttributes()", NativeMethods.GetExceptionForLastError());
            }

            hwndMag = NativeMethods.CreateWindowEx(0,
                    NativeMethods.WC_MAGNIFIER,
                    "MagnifierWindow",
                    (int)WindowStyles.WS_CHILD |
                    (int)WindowStyles.WS_VISIBLE,
                    0, 0, this.Width, this.Height,
                    this.Handle, IntPtr.Zero, hInst, IntPtr.Zero);

            if (hwndMag == IntPtr.Zero)
            {
                throw new Exception("CreateWindowEx()", NativeMethods.GetExceptionForLastError());
            }

            BuiltinMatrices.ChangeColorEffect(hwndMag, BuiltinMatrices.Negative);

            if (!NativeMethods.MagSetWindowSource(this.hwndMag, rect))
            {
                throw new Exception("MagSetWindowSource()", NativeMethods.GetExceptionForLastError());
            }

            Transformation transformation = new Transformation(1.0f);
            if (!NativeMethods.MagSetWindowTransform(this.hwndMag, ref transformation))
            {
                throw new Exception("MagSetWindowTransform()", NativeMethods.GetExceptionForLastError());
            }

            this.Show();
        }
    }
}
