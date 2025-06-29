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

namespace NegativeScreen
{
    /// <summary>
    /// Provides brightness measurement for a specific window.
    /// </summary>
    internal static class WindowBrightnessWatcher
    {
        /// <summary>
        /// Returns the average brightness of the specified window as a value between 0 and 1.
        /// </summary>
        public static float GetAverageBrightness(IntPtr hwnd)
        {
            RECT rect;
            if (!NativeMethods.GetWindowRect(hwnd, out rect))
            {
                return 0f;
            }
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;
            if (width <= 0 || height <= 0)
            {
                return 0f;
            }

            using (Bitmap bmp = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height));
                long sum = 0;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color c = bmp.GetPixel(x, y);
                        sum += c.R + c.G + c.B;
                    }
                }
                return sum / (3f * width * height * 255f);
            }
        }
    }
}
