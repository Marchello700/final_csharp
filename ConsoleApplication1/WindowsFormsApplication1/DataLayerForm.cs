using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DataLayer;

namespace WindowsFormsApplication1
{
    public partial class DataLayerForm : Form
    {
        private event EventHandler someEvent;

        protected void OnSomeEvent()
        {
            someEvent?.Invoke(this, EventArgs.Empty);
        }

        private readonly FullDataManager _dataManager;

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

        public DataLayerForm()
        {
            InitializeComponent();
            _dataManager = new FullDataManager();
            InitializeLabels();
            InitializeTextBoxes();
            DataManagerChart.Series.Clear();
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
                YAxisType = AxisType.Secondary
            };
            DataManagerChart.Series.Add(cpuSeries);
            DataManagerChart.Series.Add(ramSeries);
            DataManagerChart.ChartAreas[0].AxisX.LabelStyle.Format = "hh:mm:ss";
            DataManagerChart.ChartAreas[0].AxisX.Interval = 1;
//            DataManagerChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            DataManagerChart.ChartAreas[0].AxisX.IntervalOffset = 1;
            //DateTime minDate = new DateTime(2013, 01, 01);
            //DateTime maxDate = DateTime.Now;
            //DataManagerChart.ChartAreas[0].AxisX.Minimum = minDate.ToOADate();
            //DataManagerChart.ChartAreas[0].AxisX.Maximum = maxDate.ToOADate();
            timer1.Interval = 2000;
            timer1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitializeLabels();
            InitializeTextBoxes();
        }

        private void DataLayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int yValue = _dataManager.GetCpuUsage();

            DataManagerChart.Series[0].Points.AddXY(DateTime.Now.ToString("hh:mm:ss"), yValue);
            yValue = _dataManager.GetRamUsage();
            DataManagerChart.Series[1].Points.AddXY(DateTime.Now.ToString("hh:mm:ss"), yValue);

            while (DataManagerChart.Series[0].Points.Count > 10)
            {
                DataManagerChart.Series[0].Points.RemoveAt(0);
            }
            while (DataManagerChart.Series[1].Points.Count > 10)
            {
                DataManagerChart.Series[1].Points.RemoveAt(0);
            }
        }
    }
}
