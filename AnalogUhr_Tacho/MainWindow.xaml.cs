using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AnalogUhr_Tacho
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Line? Needle { get; set; } = null;
        public DispatcherTimer? ClockTimer { get; set; } = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ClockTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1),
                IsEnabled = false
            };

            ClockTimer.Tick += ClockTimer_Tick;

            ClockTimer.Start();
        }

        private void sldTachoValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotateNeedle(sldTachoValue.Value);
        }

        private void canTacho_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // == Tacho ==
            //Invalidate / Repaint Canvas
            canTacho.Children.Clear();

            RedrawNeedle();
            RedrawSkala();
            RotateNeedle(sldTachoValue.Value);


            // == Clock ==
            RedrawClock();
        }

        private void ClockTimer_Tick(object? sender, EventArgs e)
        {
            RedrawClock();
        }

        private void RedrawClock()
        {
            //Invalidate / Repaint Canvas
            canClock.Children.Clear();

            Point clockCenter = new Point(canClock.ActualWidth / 2, canClock.ActualHeight / 2);

            double radius = (canClock.ActualHeight / 2) > (canClock.ActualWidth / 2)
                            ? (canClock.ActualWidth / 2)
                            : (canClock.ActualHeight / 2);

            for (int angle = 0; angle <= 360; angle += 30)
            {
                //Skalenline
                Line skale = new Line
                {
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1
                };

                skale.Y1 = canClock.ActualHeight / 2 - (radius * 0.78);
                skale.X1 = skale.X2 = canClock.ActualWidth / 2;
                skale.Y2 = (canClock.ActualHeight / 2) - (radius * 0.83);

                skale.RenderTransform = new RotateTransform(angle, clockCenter.X, clockCenter.Y);

                canClock.Children.Add(skale);

                //Don't print label with value 0 - it overwrites label 12
                if(angle == 0)
                {
                    continue;
                }

                //Skalenbeschriftung
                TextBlock textBlock = new TextBlock
                {
                    Text = (angle / 30).ToString(),
                    Width = 30,
                    Height = 20,
                    FontSize = 16,
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Yellow),
                    //Background = new SolidColorBrush(Colors.Red)
                };

                //Achtung: Transformreihenfolge umgekehrt zur Code-Reihenfolge! (Von unten nach oben lesen)
                TransformGroup transformGroupTB = new TransformGroup();
                transformGroupTB.Children.Add(new TranslateTransform(-textBlock.Width / 2, -textBlock.Height / 2));
                transformGroupTB.Children.Add(new RotateTransform(90 - angle));
                transformGroupTB.Children.Add(new TranslateTransform(radius * 0.9, 0));
                transformGroupTB.Children.Add(new RotateTransform(angle - 90));
                transformGroupTB.Children.Add(new TranslateTransform(canClock.ActualWidth / 2, canClock.ActualHeight / 2));

                textBlock.RenderTransform = transformGroupTB;

                canClock.Children.Add(textBlock);
            }

            //Hands
            Line secondHand = new Line
            {
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 2
            };
            Line minuteHand = new Line
            {
                Stroke = new SolidColorBrush(Colors.Green),
                StrokeThickness = 4
            };
            Line hourHand = new Line
            {
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 6
            };

            secondHand.X1 = secondHand.X2 = clockCenter.X;
            secondHand.Y1 = clockCenter.Y;
            secondHand.Y2 = (canClock.ActualHeight / 2) - (radius * 0.75);

            minuteHand.X1 = minuteHand.X2 = clockCenter.X;
            minuteHand.Y1 = clockCenter.Y;
            minuteHand.Y2 = (canClock.ActualHeight / 2) - (radius * 0.65);

            hourHand.X1 = hourHand.X2 = clockCenter.X;
            hourHand.Y1 = clockCenter.Y;
            hourHand.Y2 = (canClock.ActualHeight / 2) - (radius * 0.55);

            canClock.Children.Add(secondHand);
            canClock.Children.Add(minuteHand);
            canClock.Children.Add(hourHand);

            var currentTime = DateTime.Now;
            int secondAngle = currentTime.Second * 6;
            int minuteAngle = currentTime.Minute * 6;
            int hourAngle = currentTime.Hour * 30;

            secondHand.RenderTransform = new RotateTransform(secondAngle, secondHand.X1, secondHand.Y1);
            minuteHand.RenderTransform = new RotateTransform(minuteAngle, minuteHand.X1, minuteHand.Y1);
            hourHand.RenderTransform = new RotateTransform(hourAngle, hourHand.X1, hourHand.Y1);
        }

        private void RedrawSkala()
        {
            if (Needle == null) return;

            double radius = (canTacho.ActualHeight * 0.9) > (canTacho.ActualWidth / 2)
                            ? (canTacho.ActualWidth / 2)
                            : (canTacho.ActualHeight * 0.9);

            for (int angle = 0; angle <= 180; angle += 10)
            {
                //Skalenline
                Line skale = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Yellow),
                    StrokeThickness = 2
                };

                skale.X1 = canTacho.ActualWidth / 2 - (radius * 0.8);
                skale.Y1 = canTacho.ActualHeight * 0.9;
                skale.X2 = (canTacho.ActualWidth / 2) - (radius * 0.85);
                skale.Y2 = canTacho.ActualHeight * 0.9;

                RotateTransform rotateSkale = new RotateTransform(angle, Needle.X1, Needle.Y1);
                skale.RenderTransform = rotateSkale;

                canTacho.Children.Add(skale);


                //Skalenbeschriftung
                TextBlock textBlock = new TextBlock
                {
                    Text = angle.ToString(),
                    Width = 30,
                    Height = 20,
                    FontSize = 16,
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Yellow),
                    //Background = new SolidColorBrush(Colors.Red)
                };

                //Canvas.SetTop(textBlock, angle);
                //Canvas.SetLeft(textBlock, angle);

                //Achtung: Transformreihenfolge umgekehrt zur Code-Reihenfolge! (Von unten nach oben lesen)
                TransformGroup transformGroupTB = new TransformGroup();
                transformGroupTB.Children.Add(new TranslateTransform(-textBlock.Width / 2, -textBlock.Height / 2));
                transformGroupTB.Children.Add(new RotateTransform(180 - angle));
                transformGroupTB.Children.Add(new TranslateTransform(radius * 0.9, 0));
                transformGroupTB.Children.Add(new RotateTransform(-180 + angle));
                transformGroupTB.Children.Add(new TranslateTransform(canTacho.ActualWidth / 2, canTacho.ActualHeight * 0.9));

                textBlock.RenderTransform = transformGroupTB;

                canTacho.Children.Add(textBlock);
            }
        }

        private void RedrawNeedle()
        {
            double radius = (canTacho.ActualHeight * 0.9) > (canTacho.ActualWidth / 2)
                            ? (canTacho.ActualWidth / 2)
                            : (canTacho.ActualHeight * 0.9);

            Needle = new Line
            {
                Stroke = new SolidColorBrush(Colors.Yellow),
                StrokeThickness = 2
            };

            Needle.X1 = canTacho.ActualWidth / 2;
            Needle.Y1 = canTacho.ActualHeight * 0.9;
            Needle.X2 = (canTacho.ActualWidth / 2) - (radius * 0.75);
            Needle.Y2 = canTacho.ActualHeight * 0.9;

            canTacho.Children.Add(Needle);
        }

        private void RotateNeedle(double angle)
        {
            if (Needle == null) return;

            RotateTransform rotateNeedle = new RotateTransform(angle, Needle.X1, Needle.Y1);

            Needle.RenderTransform = rotateNeedle;
        }
    }
}
