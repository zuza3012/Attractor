using DotNumerics.ODE;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lorentz_Attractor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
        private OdeExplicitRungeKutta45 odeRK = new OdeExplicitRungeKutta45();
        double[] yprime = new double[3];
        Random rand = new Random();
        string xlabel = "x", ylabel = "y";
        int counter = 0;

        private void MakeAPlot(double[,] data) {
            var plt = new ScottPlot.Plot(1000, 800);
            double[] x, y;
            int linewidth = 2, markersize = 0;
            if (lorentzBtn.IsChecked == true) {
                double[] t = GetColumn(data, 0);
                x = GetColumn(data, 1);
                y = GetColumn(data, 3);
                plt.Title("Lorentz Attractor");
                plt.PlotScatter(x, y, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red);

            } else if (jongBtn.IsChecked == true) {
                x = GetColumn(data, 0);
                y = GetColumn(data, 1);
                linewidth = 0;
                markersize = 2;
                plt.Title("De jong Attractor");
                plt.PlotScatter(x, y, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red);
            } else if (standardBtn.IsChecked == true) {
                x = GetColumn(data, 0);
                y = GetColumn(data, 1);
                linewidth = 1;
                markersize = 0;
                plt.Title("Standard");
                plt.PlotScatter(x, y, markerSize: markersize, lineWidth: linewidth, color: System.Drawing.Color.Red);
            }


            plt.Legend();

            plt.XLabel(xlabel);
            plt.YLabel(ylabel);

            image1.Source = CreateBitmapSourceFromGdiBitmap(plt.GetBitmap());

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

            double[,] data = new double[50001, 2];
            data[0, 0] = 1;
            data[0, 1] = 1;

            for (int i = 1; i < 50001; i++) {
                data[i, 0] = Math.Sin(a * data[i - 1, 1]) - Math.Cos(b * data[i - 1, 0]);
                data[i, 1] = Math.Sin(c * data[i - 1, 0]) - Math.Cos(d * data[i - 1, 1]);
            }
            return data;
        }

        private double[,] Standard() {
            double[,] data = new double[100, 2];
            data[0, 0] = Convert.ToDouble(tbX.Text);
            data[0, 1] = data[0, 0];
            for (int i = 1; i < counter; i++) {
                decimal fraction = (decimal)data[i - 1, 1];
                decimal dPart = (2 * fraction) % 1.0m;

                data[i, 1] = decimal.ToDouble(dPart);
                data[i, 0] = data[i - 1, 1];
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

        private void OnKeyDownHandler(object sender, KeyEventArgs e) {
            if (standardBtn.IsChecked == true) {
                counter++;
                if (e.Key == Key.Return) {
                    tbN.Clear();
                    tbN.Text = Convert.ToString(counter);
                    image1.Source = null;
                    MakeAPlot(Standard());
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e) {
            counter = 0;
            image1.Source = null;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e) {

            if(standardBtn.IsChecked == true) {
                tbA.IsEnabled = false;
                tbB.IsEnabled = false;
                tbC.IsEnabled = false;
                tbD.IsEnabled = false;

                sliderA.IsEnabled = false;
                sliderB.IsEnabled = false;
                sliderC.IsEnabled = false;
                sliderD.IsEnabled = false;

                tbX.IsEnabled = true;
                tbN.IsEnabled = true;
            }else if(jongBtn.IsChecked == true) {
                tbX.IsEnabled = false;
                tbN.IsEnabled = false;

                tbA.IsEnabled = true;
                tbB.IsEnabled = true;
                tbC.IsEnabled = true;
                tbD.IsEnabled = true;

                sliderA.IsEnabled = true;
                sliderB.IsEnabled = true;
                sliderC.IsEnabled = true;
                sliderD.IsEnabled = true;
            }
            

        }

        private void Draw_Click(object sender, RoutedEventArgs e) {
            if (lorentzBtn.IsChecked == true) {
                MakeAPlot(Solve());
            } else if (jongBtn.IsChecked == true) {
                //MakeAPlot(JongCalculate(-2.7, -0.09, -0.86, -2.2), false);
                // MakeAPlot(JongCalculate(-0.709, 1.638, 0.452, 1.740), false);
                MakeAPlot(JongCalculate(sliderA.Value, sliderB.Value, sliderC.Value, sliderD.Value));


            } else if (rooslerBtn.IsChecked == true) {

            } else {
                MakeAPlot(Standard());
            }


        }


    }
}
