using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DataLayer;
using SQLiteClassLibrary;
using System.Linq;

namespace WindowsFormsApplication1
{
    public partial class DataLayerForm : Form
    {
        private const int MaxPointCount = 10;

        private readonly DataPullerThread _dataPullerThread;
        private readonly UpdaterThread _updaterThread;

        private void InitializeLabels()
        {
            NameLabel.Text = "Name";
            UserLabel.Text = "User";
            CpuLabel.Text = "CPU";
            RamLabel.Text = "RAM";
            VideoCardLabel.Text = "Video Card";
            IpLabel.Text = "IP";
        }

        private void InitializeTextBoxes(ComputerDetail computerDetail)
        {
            NameTextBox.Text = computerDetail.ComputerName;
            UserTextBox.Text = computerDetail.UserName;
            CpuTextBox.Text = computerDetail.Cpu;
            RamTextBox.Text = $"{computerDetail.Ram}GB";
            VideoCardTexBox.Text = computerDetail.VideoCard;
            IpTextBox.Text = computerDetail.Ip;
        }

        private void InitializeCpuRamChart()
        {
            CpuRamChart.Series.Clear();
            var cpuSeries = new Series("Cpu Usage")
            {
                YValuesPerPoint = 1,
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 3,
                XValueType = ChartValueType.DateTime,
                YAxisType = AxisType.Primary
            };
            var ramSeries = new Series("Ram Usage")
            {
                YValuesPerPoint = 1,
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 3,
                XValueType = ChartValueType.DateTime,
                YAxisType = AxisType.Primary
            };
            CpuRamChart.Series.Add(cpuSeries);
            CpuRamChart.Series.Add(ramSeries);
            CpuRamChart.ChartAreas[0].AxisX.LabelStyle.Format = "hh:mm:ss";
            CpuRamChart.ChartAreas[0].AxisX.Interval = 1;
            CpuRamChart.ChartAreas[0].AxisX.IntervalOffset = 1;
        }

        private void InitializeHddChart()
        {
            HDDChart.Series.Clear();
            var hddSeries = new Series("HDD space")
            {
                YValuesPerPoint = 1,
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 3,
                XValueType = ChartValueType.DateTime,
                YAxisType = AxisType.Primary
            };
            var queueLengthSeries = new Series("Avr. Queue length")
            {
                YValuesPerPoint = 1,
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 3,
                XValueType = ChartValueType.DateTime,
                YAxisType = AxisType.Secondary
            };
            HDDChart.Series.Add(hddSeries);
            HDDChart.Series.Add(queueLengthSeries);
            HDDChart.ChartAreas[0].AxisX.LabelStyle.Format = "hh:mm:ss";
            HDDChart.ChartAreas[0].AxisX.Interval = 1;
            HDDChart.ChartAreas[0].AxisX.IntervalOffset = 1;
        }

        public DataLayerForm()
        {
            InitializeComponent();
            var dataManager = new FullDataManager();
            _dataPullerThread = new DataPullerThread();
            _dataPullerThread.Start();
            ComputerDetail computerDetail;
            using (var context = new MetricsContext())
            {
                context.Database.EnsureCreated();
                computerDetail = MetricsContextSupport.GetComputerDetail(context, dataManager.GetName()) ??
                                 MetricsContextSupport.AddComputerDetail(context, dataManager.GetComputerSummary());
            }
            InitializeLabels();
            InitializeTextBoxes(computerDetail);
            InitializeCpuRamChart();
            InitializeHddChart();
            _updaterThread = new UpdaterThread(computerDetail.ComputerName);
            _updaterThread.UpdateFinished += UpdateCharts;
            _updaterThread.Start();
        }

        private void DataLayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _dataPullerThread.Abort();
                _updaterThread.Abort();
                Close();
            }
        }

        private void UpdateCharts(object sender, UpdateEventArgs e)
        {
            if (e.GetComputerDetail() == null)
            {
                return;
            }
            var usageDatas = e.GetComputerDetail().UsageDataCollection.ToList();
            if (usageDatas.Count > MaxPointCount)
            {
                usageDatas.RemoveRange(0, usageDatas.Count - MaxPointCount);
            }
            CpuRamChart.Series[0].Points.Clear();
            CpuRamChart.Series[1].Points.Clear();
            HDDChart.Series[0].Points.Clear();
            HDDChart.Series[1].Points.Clear();
            foreach (var usageData in usageDatas)
            {
                var timeMark = usageData.Time?.ToString("hh:mm:ss");
                CpuRamChart.Series[0].Points.AddXY(timeMark, usageData.CpuUsage);
                CpuRamChart.Series[1].Points.AddXY(timeMark, usageData.RamUsage);
                //ClearOldPoints(CpuRamChart);
                HDDChart.Series[0].Points.AddXY(timeMark, usageData.AvailableDiskSpaceGb);
                HDDChart.Series[1].Points.AddXY(timeMark, usageData.AverageQueueLength);
                //ClearOldPoints(HDDChart);
            }
        }
    }
}
