using System;
using System.Windows.Forms;
using DataLayer;

namespace WindowsFormsApplication1
{
    public partial class DataLayerForm : Form
    {
        private FullDataManager dataManager;

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
            NameTextBox.Text = dataManager.GetName();
            UserTextBox.Text = dataManager.GetUser();
            CpuTextBox.Text = dataManager.GetCpu();
            RamTextBox.Text = $"Ram: {dataManager.GetRamGb()}GB";
            VideoCardTexBox.Text = dataManager.GetVideoCard();
            IpTextBox.Text = $"Ip Adress: {dataManager.GetIp()}";
        }

        public DataLayerForm()
        {
            InitializeComponent();
            dataManager = new FullDataManager();
            InitializeLabels();
            InitializeTextBoxes();
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
    }
}
