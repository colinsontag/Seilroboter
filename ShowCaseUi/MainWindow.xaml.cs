using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HelixToolkit.Wpf;

namespace ShowCaseUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Vector3> redPoints;
        private int currentRedPointIndex;
        List<Vector3> drivePositions = new List<Vector3>();


        public MainWindow()
        {
            InitializeComponent();

            drivePositions.Add(new Vector3(0, 0, 100));
            drivePositions.Add(new Vector3(100, 100, 100));
            drivePositions.Add(new Vector3(0, 100, 100));
            foreach (var position in drivePositions)
            {
                var sphere = new SphereVisual3D { Center = new Point3D(position.X, position.Y, position.Z), Radius = 1 };
                ViewPort.Children.Add(sphere);
            }

            // List of points to be shown in red
            redPoints = new List<Vector3>();

            // Generate points in a circle

            // Generate points in a circle with varying Z value
            for (double i = 0; i < 2 * Math.PI; i += Math.PI / 100) // 200 points in a circle
            {
                redPoints.Add(new Vector3((float)Math.Cos(i) * 25, (float)Math.Sin(i) * 25, 50 + (float)Math.Sin(i) * 25)); // 25 is the radius of the circle, Z value changes with sin(i)
            }

            currentRedPointIndex = 0;
            
                ShowRedPoint();
            


            // Set up a timer to move to the next red point every 0.5 second
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(2);
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            currentRedPointIndex++;
            if (currentRedPointIndex >= redPoints.Count)
            {
                currentRedPointIndex = 0; // Loop back to the start
            }
            ShowRedPoint();
        }

        private void ShowRedPoint()
        {
            // Clear previous red point and lines
            for (int i = ViewPort.Children.Count - 1; i >= 0; i--)
            {
                if (ViewPort.Children[i] is SphereVisual3D sphere && sphere.Material is DiffuseMaterial material && material.Brush is SolidColorBrush brush && brush.Color == Colors.Red)
                {
                    ViewPort.Children.RemoveAt(i);
                }
                else if (ViewPort.Children[i] is LinesVisual3D)
                {
                    ViewPort.Children.RemoveAt(i);
                }
            }

            // Show new red point
            var point = redPoints[currentRedPointIndex];
            var redSphere = new SphereVisual3D
            {
                Center = new Point3D(point.X, point.Y, point.Z),
                Radius = 1,
                Material = new DiffuseMaterial(new SolidColorBrush(Colors.Red))
            };
            ViewPort.Children.Add(redSphere);

            // Draw lines from each drive to the current red point
            List<double> lineLengths = new List<double>();
            foreach (var drive in drivePositions)
            {
                var line = new LinesVisual3D
                {
                    Points = new Point3DCollection
                    {
                        new Point3D(drive.X, drive.Y, drive.Z),
                        new Point3D(point.X, point.Y, point.Z)
                    },
                    Color = Colors.Blue
                };
                ViewPort.Children.Add(line);

                // Calculate and store line lengths
                var length = Math.Sqrt(Math.Pow(drive.X - point.X, 2) + Math.Pow(drive.Y - point.Y, 2) + Math.Pow(drive.Z - point.Z, 2));
                lineLengths.Add(length);
            }

            // Display line lengths in the UI
            LineLengthsTextBlock.Text = string.Join(", ", lineLengths.Select((l, index) => $"CableLength {index + 1}: {l.ToString("F2")}"));
        }
    }
}