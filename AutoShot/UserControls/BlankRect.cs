using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using d = System.Drawing;

namespace AutoShot.UserControls
{
    public class BlankRect : Shape
    {
        static BlankRect()
        {
            WidthProperty.OverrideMetadata(typeof(BlankRect),
                new FrameworkPropertyMetadata((o, e) => ((BlankRect)o).UpdateGeometry()));

            HeightProperty.OverrideMetadata(typeof(BlankRect),
                new FrameworkPropertyMetadata((o, e) => ((BlankRect)o).UpdateGeometry()));

            StrokeLineJoinProperty.OverrideMetadata(typeof(BlankRect),
                new FrameworkPropertyMetadata(PenLineJoin.Round));
        }

        public BlankRect()
        {
            this.Fill = Brushes.Black;
        }
        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register(nameof(Rect), typeof(d.Rectangle), typeof(BlankRect),
                new FrameworkPropertyMetadata((o, e) => ((BlankRect)o).UpdateGeometry()));

        public d.Rectangle Rect
        {
            get { return (d.Rectangle)GetValue(RectProperty); }
            set { SetValue(RectProperty, value); }
        }

        private readonly StreamGeometry geometry = new StreamGeometry();

        protected override Geometry DefiningGeometry
        {
            get { return geometry; }
        }

        public void UpdateGeometry()
        {
            if (!double.IsNaN(ActualWidth) && !double.IsNaN(ActualHeight))
            {
                Point p1 = new Point(Rect.Left, Rect.Top);
                Point p2 = new Point(Rect.Left + Rect.Width, Rect.Top);
                Point p3 = new Point(Rect.Left + Rect.Width, Rect.Top + Rect.Height);
                Point p4 = new Point(Rect.Left, Rect.Top + Rect.Height);

                using (var context = geometry.Open())
                {

                    context.BeginFigure(new Point(0, 0), true, true);

                    if (Rect != new d.Rectangle(0, 0, 0, 0))
                    {
                        context.LineTo(new Point(Rect.Left, 0), true, true);
                        context.LineTo(p1, true, true);
                        context.LineTo(p2, true, true);
                        context.LineTo(p3, true, true);
                        context.LineTo(p4, true, true);
                        context.LineTo(p1, true, true);
                        context.LineTo(new Point(Rect.Left, 0), true, true);
                    }


                    context.LineTo(new Point(ActualWidth, 0), true, true);
                    context.LineTo(new Point(ActualWidth, ActualHeight), true, true);
                    context.LineTo(new Point(0, ActualHeight), true, true);
                }
            }
        }
    }
}
