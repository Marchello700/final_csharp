using System;
using System.Collections.Generic;
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
        private const int maxPointCount = 10;

        private readonly FullDataManager _dataManager;
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

        //private void InitializeTextBoxes()
        //{
        //    NameTextBox.Text = _dataManager.GetName();
        //    UserTextBox.Text = _dataManager.GetUser();
        //    CpuTextBox.Text = _dataManager.GetCpu();
        //    RamTextBox.Text = $"Ram: {_dataManager.GetRamGb()}GB";
        //    VideoCardTexBox.Text = _dataManager.GetVideoCard();
        //    IpTextBox.Text = $"Ip Adress: {_dataManager.GetIp()}";
        //}

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
            while (series.Points.Count > maxPointCount)
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
            _dataPullerThread = new DataPullerThread();
            _dataPullerThread.Start();
            ComputerDetail computerDetail = null;
            using (var context = new MetricsContext())
            {
                context.Database.EnsureCreated();
                computerDetail = MetricsContextSupport.GetComputerDetail(context, _dataManager.GetName());
                if (computerDetail == null)
                {
                    computerDetail = MetricsContextSupport.AddComputerDetail(context, _dataManager.GetComputerSummary());
                }
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
                Close();
            }
        }

        private void UpdateCharts(object sender, UpdateEventArgs e)
        {
            if (e.GetComputerDetail() == null)
            {
                return;
            }
            var UsageDatas = e.GetComputerDetail().UsageDataCollection.ToList();
            if (UsageDatas.Count > maxPointCount)
            {
                UsageDatas.RemoveRange(0, UsageDatas.Count - maxPointCount);
            }
            CpuRamChart.Series[0].Points.Clear();
            CpuRamChart.Series[1].Points.Clear();
            HDDChart.Series[0].Points.Clear();
            HDDChart.Series[1].Points.Clear();
            foreach (var UsageData in UsageDatas)
            {
                
                var TimeMark = UsageData.Time?.ToString("hh:mm:ss");
                CpuRamChart.Series[0].Points.AddXY(TimeMark, UsageData.CpuUsage);
                CpuRamChart.Series[1].Points.AddXY(TimeMark, UsageData.RamUsage);
                //ClearOldPoints(CpuRamChart);
                HDDChart.Series[0].Points.AddXY(TimeMark, UsageData.AvailableDiskSpaceGb);
                HDDChart.Series[1].Points.AddXY(TimeMark, UsageData.AverageQueueLength);
                //ClearOldPoints(HDDChart);
            }
        }

        private void DataLayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dataPullerThread.Abort();
            _updaterThread.Abort();
        }
    }
}
