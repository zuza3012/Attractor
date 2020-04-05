using DotNumerics.ODE;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace Lorentz_Attractor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged {
        public MainWindow() {
            
            DataContext = this;
            InitializeComponent();
        }
        private OdeExplicitRungeKutta45 odeRK = new OdeExplicitRungeKutta45();
        double[] yprime = new double[3];
        Random rand = new Random();

        public event PropertyChangedEventHandler PropertyChanged; //interfejs
        private float _num;
        public float num {
            get { return _num; }
            set {
                if(_num != value) {
                    _num = (float)Math.Round(value,3);
                    OnPropertyChanged();
                }
            }
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        private void MakeAPlot(double[,] data, bool lorentz) {
            var plt = new ScottPlot.Plot(1000, 800);
            double[] x, y;
            int linewidth = 2, markersize = 0;
            if (lorentz) {
                double[] t = GetColumn(data, 0);
                Console.WriteLine("t0:" + t[0]);
                x = GetColumn(data, 1);
                y = GetColumn(data, 3);
                plt.Title("Lorentz Attractor");

            } else {
                x = GetColumn(data, 0);
                y = GetColumn(data, 1);
                Console.WriteLine("Length");
                Console.WriteLine(x.Length);
                Console.WriteLine(y.Length);
                linewidth = 0;
                markersize = 2;
                plt.Title("De jong Attractor");
            }

            plt.PlotScatter(x, y, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red, label: "x + y");
            plt.Legend();

            
            plt.YLabel("y");
            plt.XLabel("x");

           // Image image = new Image();

            Console.WriteLine(plt.GetBitmap().Width);
            Console.WriteLine(plt.GetBitmap().Height);


            image1.Source = CreateBitmapSourceFromGdiBitmap(plt.GetBitmap());
            //canvas.Children.Add(image);
        }
        public double[] GetColumn(double[,] matrix, int columnNumber) {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }
        private double[,] Solve() {
            OdeFunction fun = new OdeFunction(ODEs);
            double[] y0 = new double[3];
            y0[0] = 0;
            y0[1] = 1;
            y0[2] = 1;
            odeRK.InitializeODEs(fun, 3);
            double[,] sol = odeRK.Solve(y0, 0, 0.003, 15);


            for (int i = 0; i < sol.GetLength(0); i++) {
                for (int j = 0; j < sol.GetLength(1); j++) {
                    Console.Write(sol[i, j] + "\t");
                }
                Console.WriteLine();
            }


            Console.WriteLine("l0:" + sol.GetLength(0));
            Console.WriteLine("l1:" + sol.GetLength(1));
            return sol;

        }



        private double[] ODEs(double t, double[] y) {
            double sigma = 10, r = 99.96, b = 10 / 3;

            yprime[0] = sigma * (y[1] - y[0]);
            yprime[1] = -y[0] * y[2] + r * y[0] - y[1];
            yprime[2] = y[0] * y[1] - b * y[2];
            return yprime;
        }

        private double[,] JongCalculate(double a, double b, double c, double d) {
            double x0 = rand.NextDouble();
            double y0 = rand.NextDouble();
            Console.WriteLine("losowanie: " + x0 + " " + y0);

            double[,] data = new double[50001, 2];
            data[0, 0] = 1;
            data[0, 1] = 1;
            Console.WriteLine("Jong");
            int[,] array2D = new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            Console.WriteLine("test:" + array2D.GetLength(0));
            Console.WriteLine("test:" + array2D.GetLength(1));

            for (int i = 1; i < 50001; i++) {
                data[i, 0] = Math.Sin(a * data[i - 1, 1]) - Math.Cos(b * data[i - 1, 0]);
                data[i, 1] = Math.Sin(c * data[i - 1, 0]) - Math.Cos(d * data[i - 1, 1]);
            }

            Console.WriteLine("test:" + data.GetLength(0));
            Console.WriteLine("test:" + data.GetLength(1));


            for (int i = 0; i < data.GetLength(1); i++) {
                Console.Write(data[i, 0] + "\t" + data[i, 1]);

                Console.WriteLine();
            }


            return data;
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap) {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            } finally {
                bitmap.UnlockBits(bitmapData);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (lorentzBtn.IsChecked == true) {
                MakeAPlot(Solve(), true);
            } else if (jongBtn.IsChecked == true) {
                //MakeAPlot(JongCalculate(-2.7, -0.09, -0.86, -2.2), false);
                // MakeAPlot(JongCalculate(-0.709, 1.638, 0.452, 1.740), false);
                MakeAPlot(JongCalculate(sliderA.Value, sliderB.Value, sliderC.Value, sliderD.Value), false);


            } else if (rooslerBtn.IsChecked == true) {

            } else {

            }


        }

        private void SliderA_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            //abelA.Content = "a: " + sliderA.Value;
            //Console.WriteLine(labelA.Content);
        }
    }
}
