using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;
using System.Windows.Threading;

namespace TestProjectD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double MinPulse = 40;
        private const double MaxPulse = 200;
        private const double MinChange = -5;
        private const double MaxChange = 5;
        private double _lastPulse = (MinPulse + MaxPulse) / 2;

        public PlotModel MyModel { get; private set; } = null!;
        private AreaSeries _pulseSeries = null!;
        private DispatcherTimer _timer =  null!;
        private readonly Random _random = new();
        private TextAnnotation _pulseAnnotation = null!;


        public MainWindow()
        {
            InitializeComponent();
            InitializeModel();
            InitializeTimer();
            PrepopulateHistoricalData();
        }

        private void InitializeModel()
        {
            MyModel = new PlotModel();

            var softGrey = OxyColor.FromRgb(211, 211, 211);
            var axisTextColor = OxyColor.FromRgb(119, 119, 119);
            var neonGreen = OxyColor.FromRgb(15, 210, 8);
            var lighterNeonGreenTransparent = OxyColor.FromAColor(120, neonGreen);

            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddMinutes(-7)),
                Maximum = DateTimeAxis.ToDouble(DateTime.Now),
                IntervalType = DateTimeIntervalType.Minutes,
                IntervalLength = 80,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                TextColor = axisTextColor,
                TicklineColor = softGrey,
                MajorGridlineColor = softGrey,
                MinorGridlineColor = softGrey,
            };

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 40,
                Maximum = 200,
                MajorStep = 20,
                MinorStep = 10,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                TextColor = axisTextColor,
                TicklineColor = softGrey,
                MajorGridlineColor = softGrey,
                MinorGridlineColor = softGrey,
            };

            MyModel.Axes.Add(dateAxis);
            MyModel.Axes.Add(valueAxis);

            _pulseSeries = new AreaSeries
            {
                Color = neonGreen,
                Fill = lighterNeonGreenTransparent,
                StrokeThickness = 2,
            };

            _pulseAnnotation = new TextAnnotation
            {
                TextPosition = new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), valueAxis.Minimum + 5),
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Top,
                Stroke = OxyColors.Transparent
            };

            MyModel.Annotations.Add(_pulseAnnotation);
            MyModel.Series.Add(_pulseSeries);
            MyModel.PlotAreaBorderColor = softGrey;
            MyModel.PlotAreaBorderThickness = new OxyThickness(1);
            DataContext = this;
        }


        private void InitializeTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick!;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var change = _random.NextDouble() * (MaxChange - MinChange) + MinChange;
            var nextValue = Math.Max(MinPulse, Math.Min(MaxPulse, _lastPulse + change));
            _lastPulse = nextValue;

            CurrentPulseTextBlock.Text = $"{Math.Round(nextValue)}";

            var now = DateTime.Now;

            _pulseSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(now), nextValue));
            _pulseSeries.Fill = OxyColor.FromAColor(120, OxyColors.Green);
            _pulseAnnotation.Text = $"Current Pulse: {nextValue}";
            _pulseAnnotation.TextPosition = new DataPoint(DateTimeAxis.ToDouble(now), 40);

            while (_pulseSeries.Points.Count > 0 &&
                   _pulseSeries.Points[0].X < DateTimeAxis.ToDouble(now.AddMinutes(-7)))
            {
                _pulseSeries.Points.RemoveAt(0);
            }


            MyModel.Axes[0].Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddMinutes(-7));
            MyModel.Axes[0].Maximum = DateTimeAxis.ToDouble(DateTime.Now);

            MyModel.InvalidatePlot(true);
        }

        private void PrepopulateHistoricalData()
        {
            var startTime = DateTime.Now.AddMinutes(-7);


            for (var time = startTime; time <= DateTime.Now; time = time.AddSeconds(1))
            {
                var change = _random.NextDouble() * (MaxChange - MinChange) + MinChange;

                var pulse = Math.Max(MinPulse, Math.Min(MaxPulse, _lastPulse + change));

                _pulseSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time), pulse));
            }
        }
    }
}