using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using f = System.Windows.Forms;
using d = System.Drawing;
using System.Windows;
using System.Windows.Controls;
using AutoShot.UserControls;
using System.Windows.Media;
using System.Windows.Input;
using static AutoShot.Interop.NativeMethods;
using static AutoShot.Sounds.NotificationSounds;
using static AutoShot.Interop.UnsafeNativeMethods;
using static AutoShot.Globals.Globals;
using static AutoShot.Globals.ImageUtilities;

namespace AutoShot.Windows
{
    class CropWindow : Window
    {
        d.Rectangle fullBound = d.Rectangle.Empty;
        f.Timer tmr;
        BlankRect blankRect;
        Grid grid;
        Border dragGrid;
        POINT StartLoc;
        public CropWindow()
        {

            // 선택 캡처

            foreach (var scr in f.Screen.AllScreens)
            {
                fullBound = d.Rectangle.Union(fullBound, scr.Bounds);
            }

            tmr = new f.Timer()
            {
                Interval = 1
            };
            tmr.Tick += Dragging;

            grid = new Grid();

            PlayNotificationSound(SoundType.Captured);
            
            blankRect = new BlankRect();

            blankRect.Fill = (Brush)(new BrushConverter()).ConvertFromString("#7FFFFFFF");

            grid.Children.Add(blankRect);

            dragGrid = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                BorderBrush = Brushes.Red,
                BorderThickness = new Thickness(1),
                Width = 100,
                Height = 100,
                Visibility = Visibility.Hidden
            };
            grid.Children.Add(blankRect);

            blankRect.MouseDown += DragStart;
            this.AllowsTransparency = true;
            this.Topmost = true;
            this.ResizeMode = ResizeMode.NoResize;
            this.Left = fullBound.Left;
            this.Top = fullBound.Top;
            this.Width = fullBound.Width;
            this.Height = fullBound.Height;
            this.WindowStyle = WindowStyle.None;

            this.MouseDown += DragStart;

            this.Background = new ImageBrush(CopyScreen());

            this.Content = grid;
            this.Loaded += WdwInit;

            grid.KeyDown += KeyDownClose;
            dragGrid.KeyDown += KeyDownClose;
            this.KeyDown += KeyDownClose;
            blankRect.KeyDown += KeyDownClose;  
        }

        bool CaptureCompFlag = false;

        private void DragStart(object sender, MouseButtonEventArgs e)
        {
            POINT pt;
            GetCursorPos(out pt);
            dragGrid.Visibility = Visibility.Visible;
            StartLoc = pt;
            tmr.Start();
        }

        private void WdwInit(object sender, RoutedEventArgs e)
        {
            blankRect.Rect = new d.Rectangle(0, 0, 0, 0);
            blankRect.UpdateGeometry();
        }

        private void KeyDownClose(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Dragging(object sender, EventArgs e)
        {
            POINT NowLoc;
            GetCursorPos(out NowLoc);

            if (StartLoc.X < NowLoc.X)
            {
                dragGrid.Margin = GetMargin(dragGrid.Margin, ThickPosition.Left, StartLoc.X - fullBound.Left, fullBound.Width);
                dragGrid.Width = (NowLoc.X - StartLoc.X);
            }
            else
            {
                dragGrid.Margin = GetMargin(dragGrid.Margin, ThickPosition.Left, NowLoc.X - fullBound.Left, fullBound.Width);
                dragGrid.Width = (StartLoc.X - NowLoc.X);
            }
            if (StartLoc.Y > NowLoc.Y)
            {
                dragGrid.Margin = GetMargin(dragGrid.Margin, ThickPosition.Top, NowLoc.Y - fullBound.Top, fullBound.Width);
                dragGrid.Height = (StartLoc.Y - NowLoc.Y);
            }
            else
            {
                dragGrid.Margin = GetMargin(dragGrid.Margin, ThickPosition.Top, StartLoc.Y - fullBound.Top, fullBound.Width);
                dragGrid.Height = (NowLoc.Y - StartLoc.Y);
            }

            blankRect.Rect = new d.Rectangle((int)dragGrid.Margin.Left, (int)dragGrid.Margin.Top, (int)dragGrid.Width, (int)dragGrid.Height);
            
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                tmr.Stop();

                if (dragGrid.Width == 0 || dragGrid.Height == 0)
                {
                    dragGrid.Visibility = Visibility.Hidden;
                    dragGrid.Width = 0;
                    dragGrid.Height = 0;
                }
                else
                {
                    CaptureCompFlag = true;
                    this.Close();
                }
            }
        }
    }
}
