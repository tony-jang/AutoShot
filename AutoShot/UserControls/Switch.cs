using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AutoShot.UserControls
{
    [TemplatePart(Name = "PART_Holder", Type = typeof(Ellipse))]
    [TemplatePart(Name = "PART_Border", Type = typeof(Border))]
    public class Switch : CheckBox
    {
        public static DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(Switch), new PropertyMetadata(Brushes.Black));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        private Ellipse holder;
        private Border border;
        private Storyboard lastStoryboard;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            holder = GetTemplateChild("PART_Holder") as Ellipse;
            border = GetTemplateChild("PART_Border") as Border;

            this.InvalidateVisual();

            
            while (SavingRequest.Count != 0)
            {
                Tuple<bool, RoutedEventArgs> check = SavingRequest.Dequeue();
                if (check.Item1) OnChecked(check.Item2);
                if (!check.Item1) OnUnchecked(check.Item2);
            }
            
        }

        public Queue<Tuple<bool, RoutedEventArgs>> SavingRequest = new Queue<Tuple<bool, RoutedEventArgs>>();
        protected override void OnChecked(RoutedEventArgs e)
        {
            
            if (holder == null) SavingRequest.Enqueue(new Tuple<bool,RoutedEventArgs> (true, e));
            else
            {
                base.OnChecked(e);

                if (holder.ActualWidth == 0)
                {
                    Animate(
                        new Thickness(0),
                        new Thickness(Width - holder.Width, 0, 0, 0));
                }
                else
                {
                    Animate(
                        new Thickness(0),
                        new Thickness(ActualWidth - holder.ActualWidth, 0, 0, 0));
                }
                
            }
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            if (holder == null) SavingRequest.Enqueue(new Tuple<bool, RoutedEventArgs>(false, e));
            else
            {
                base.OnUnchecked(e);

                Animate(
                    new Thickness(ActualWidth - holder.ActualWidth, 0, 0, 0),
                    new Thickness(0));
            }
            
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (holder != null)
            {
                float radius = (float)Math.Min(constraint.Width, constraint.Height) / 2f;

                border.CornerRadius = new CornerRadius(radius);

                holder.Width = radius * 2;
                holder.Height = radius * 2;
            }

            return base.MeasureOverride(constraint);
        }

        void Animate(Thickness fromThickness, Thickness toThickness)
        {
            lastStoryboard?.Stop();

            ThicknessAnimation thicknessAnim;

            var sb = new Storyboard()
            {
                FillBehavior = FillBehavior.HoldEnd
            };

            sb.Children.Add(thicknessAnim = new ThicknessAnimation()
            {
                From = fromThickness,
                To = toThickness,
                Duration = TimeSpan.FromMilliseconds(100)
            });

            Storyboard.SetTarget(thicknessAnim, holder);
            Storyboard.SetTargetProperty(thicknessAnim, new PropertyPath("Margin"));

            lastStoryboard = sb;
            sb.Begin();
        }
    }
}