using DotNumerics.ODE;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
using Image = System.Windows.Controls.Image;

namespace Lorentz_Attractor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            MakeAPlot(Solve());    

        }
        private OdeExplicitRungeKutta45 odeRK = new OdeExplicitRungeKutta45(); 
        double[] yprime = new double[3];

        private void MakeAPlot (double[,] data) {
             var plt = new ScottPlot.Plot(800, 450);

            double[] t = GetColumn(data, 0);
            Console.WriteLine("t0:" + t[0]);
            double[] x = GetColumn(data, 1);
            double[] y = GetColumn(data, 3);

            plt.PlotScatter(x, y, markerSize: 0, lineWidth: 2, color: System.Drawing.Color.Red, label: "x + y");          
            plt.Legend();

            plt.Title("Lorentz Attractor");
            plt.YLabel("y");
            plt.XLabel("x");

            Image image = new Image();

            Console.WriteLine(plt.GetBitmap().Width);
            Console.WriteLine(plt.GetBitmap().Height);


            image.Source = CreateBitmapSourceFromGdiBitmap(plt.GetBitmap());
            canvas.Children.Add(image);
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

            int[,] array2D = new int[,] {   { 1, 2 }, 
                                            { 3, 4 }, 
                                            { 5, 6 }, 
                                            { 7, 8 } };
            

            for (int i = 0; i < sol.GetLength(0); i++) {
                for (int j = 0; j < sol.GetLength(1); j++) {
                    Console.Write(sol[i,j] + "\t");
                }
                Console.WriteLine();
            }
            

            Console.WriteLine("l0:" + sol.GetLength(0));
            Console.WriteLine("l1:" + sol.GetLength(1));
            return sol;

        }

  
        
        private double[] ODEs(double t, double[] y) {
            double sigma = 10, r = 99.96, b = 10 / 3;

            yprime[0] = sigma*(y[1]-y[0]);
            yprime[1] = -y[0] * y[2] + r*y[0] - y[1];
            yprime[2] = y[0] * y[1] - b*y[2];
            return yprime;
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

    }
}
