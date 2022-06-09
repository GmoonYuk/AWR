using AWR.Control;
using AWR.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AWR
{
    public partial class frmMain : Form
    {
        public Global _global { get; set; }

        public frmMain()
        {
            InitializeComponent();
            _global = Global.GetInstance();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            InitializeControl();
        }

        private void InitializeControl()
        {
            //임시로 기본세팅
            this.txtBenefitArea.Text = "800000";
            this.txtInitVolume.Text = "0";
        }

        private void ultraToolbarsManager1_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "btnReadInput":
                    ReadAWRInput();
                    break;
                default:
                    break;
            }
        }

        private void ReadAWRInput()
        {
            List<AWR_InputTemplate> listInput = new List<AWR_InputTemplate>();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                listInput = BayzFileIO.ReadAWR_InputTemplate(ofd.FileName);
                this.ultraGrid1.DataSource = listInput;
                _global.listAWRInput = listInput;
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            List<RequiredDepth> listSet = new List<RequiredDepth>();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                listSet = BayzFileIO.ReadAWR_Set(ofd.FileName);
                this.ultraGrid3.DataSource = listSet;
                _global.listAWRSet = listSet;
            }
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            if (_global.listAWRSet.Count > 0 && this.txtBenefitArea.Text != "")
            {
                double BenefitArea = double.Parse(this.txtBenefitArea.Text);
                BayzCalc.CalcRequiredQuantity(_global.listAWRSet, BenefitArea);

                this.ultraGrid3.DataSource = _global.listAWRSet;
                this.ultraGrid3.Refresh();
            }
        }

        private void btnSet_Save_Click(object sender, EventArgs e)
        {
            _global.BenefitArea = double.Parse(this.txtBenefitArea.Text);
            _global.InitVolume = double.Parse(this.txtInitVolume.Text);
        }

        private void ultraToolbarsManager2_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "btnCalcAWR":
                    if (ValidationData() == true)
                    {
                        List<AWR_Result> listResult = new List<AWR_Result>();
                        listResult = BayzCalc.CalcAWR(_global.listAWRInput, _global.listAWRSet, _global.BenefitArea, _global.InitVolume);

                        this.ultraGrid2.DataSource = listResult;
                    }
                    
                    break;
                default:
                    break;
            }
        }

        private bool ValidationData()
        {
            //임시로
            return true;
        }
    }
}
