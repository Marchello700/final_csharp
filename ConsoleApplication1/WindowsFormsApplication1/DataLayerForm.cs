using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DataLayer;
using SQLiteClassLibrary;

namespace WindowsFormsApplication1
{
    public partial class DataLayerForm : Form
    {
        private readonly FullDataManager _dataManager;
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

        private void InitializeTextBoxes()
        {
            NameTextBox.Text = _dataManager.GetName();
            UserTextBox.Text = _dataManager.GetUser();
            CpuTextBox.Text = _dataManager.GetCpu();
            RamTextBox.Text = $"Ram: {_dataManager.GetRamGb()}GB";
            VideoCardTexBox.Text = _dataManager.GetVideoCard();
            IpTextBox.Text = $"Ip Adress: {_dataManager.GetIp()}";
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

        private void ClearOldPoints(Series series)
        {
            while (series.Points.Count > 10)
            {
                series.Points.RemoveAt(0);
            }
        }

        private void ClearOldPoints(Chart chart)
        {
            foreach(var series in chart.Series)
            {
                ClearOldPoints(series);
            }
        }

        public DataLayerForm()
        {
            InitializeComponent();
            _dataManager = new FullDataManager();
            _updaterThread = new UpdaterThread(_dataManager);
            InitializeLabels();
            InitializeTextBoxes();

            InitializeCpuRamChart();
            InitializeHddChart();

            _updaterThread.UpdateFinished += UpdateCharts;
            _updaterThread.Start();

            using (var context = new MetricsContext())
            {
                context.Database.EnsureCreated();
            }

            //var computerDetail = new ComputerDetail();
            //computerDetail.ComputerName = _dataManager.GetName();
            //computerDetail.UserName = _dataManager.GetUser();
            //computerDetail.Cpu = _dataManager.GetCpu();
            //computerDetail.Ram = _dataManager.GetRamGb().ToString();
            //computerDetail.VideoCard = _dataManager.GetVideoCard();
            //computerDetail.Ip = _dataManager.GetIp().ToString();

            //var metricsContext = new MetricsContext();
            //metricsContext.Add(computerDetail);
            //metricsContext.SaveChanges();

            //            public int ComputerDetailId { get; set; }
            //public string ComputerName { get; set; }
            //public string UserName { get; set; }
            //public string Cpu { get; set; }
            //public string Ram { get; set; }
            //public string VideoCard { get; set; }
            //public string Ip { get; set; }
            //public ICollection<UsageData> UsageDataCollection { get; set; }

        }

        private void DataLayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void UpdateCharts(object sender, EventArgs e)
        {
            var TimeMark = _updaterThread.TimeMark;
            CpuRamChart.Series[0].Points.AddXY(TimeMark, _updaterThread.CpuUsage);
            CpuRamChart.Series[1].Points.AddXY(TimeMark, _updaterThread.RamUsage);
            ClearOldPoints(CpuRamChart);
            HDDChart.Series[0].Points.AddXY(TimeMark, _updaterThread.AvailableDiskSpaceGb);
            HDDChart.Series[1].Points.AddXY(TimeMark, _updaterThread.AverageQueueLength);
            ClearOldPoints(HDDChart);
        }

        private void DataLayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _updaterThread.Abort();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
