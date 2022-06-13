using AWR.Control;
using AWR.Model;
using Bayz.FrameWork;
using DevExpress.XtraCharts;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            this.Text += string.Format(" V{0}.{1}.{2}",
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major,
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor,
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build);

            InitializeLogSetting();

            InitializeControl();
        }

        private void InitializeLogSetting()
        {
            //Log설정
            BayzLogManager.ConfigureLogger(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "l4n.xml"));
        }

        /// <summary>
        /// 변수 초기화
        /// </summary>
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

        /// <summary>
        /// 강우, 증발산 입력자료 읽기함수
        /// </summary>
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
                SetGrid_Input(this.ultraGrid1);

                _global.listAWRInput = listInput;
            }
        }

        /// <summary>
        /// 입력자료 그리드 세팅
        /// </summary>
        /// <param name="ultraGrid"></param>
        private void SetGrid_Input(UltraGrid ultraGrid)
        {
            UltraGridBand ultraGridBand = ultraGrid.DisplayLayout.Bands[0];

            ultraGridBand.Columns["curDate"].Header.Caption = "일자";
            ultraGridBand.Columns["Rainfall"].Header.Caption = "강우(mm)";
            ultraGridBand.Columns["Evaporation"].Header.Caption = "증발산(mm)";
        }

        /// <summary>
        /// 농업용수 연산 설정자료 읽기함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRead_Click(object sender, EventArgs e)
        {
            if (BenefitAreaValidation() == true)
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

                    if (listSet.Count > 0)
                    {
                        double benefitArea = double.Parse(this.txtBenefitArea.Text);
                        _global.BenefitArea = benefitArea;

                        BayzCalc.CalcRequiredQuantity(listSet, benefitArea);

                        this.ultraGrid3.DataSource = listSet;
                        SetGrid_Set(this.ultraGrid3);

                        _global.listAWRSet = listSet;
                    }                    
                }
            }
            else
            {
                MessageBox.Show("수해면적 항목을 확인하세요.");
            }
        }

        

        private void SetGrid_Set(UltraGrid ultraGrid)
        {
            UltraGridBand ultraGridBand = ultraGrid.DisplayLayout.Bands[0];

            ultraGrid.DisplayLayout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
            ultraGrid.DisplayLayout.Override.AllowRowLayoutColMoving = Infragistics.Win.Layout.GridBagLayoutAllowMoving.AllowAll;

            UltraGridGroup period_GridGroup = ultraGridBand.Groups.Add("Period", "기간");
            UltraGridGroup requiredDepth_GridGroup = ultraGridBand.Groups.Add("RequiredDepth", "필요수심(m)");

            ultraGridBand.Columns["StartDate"].RowLayoutColumnInfo.ParentGroup = period_GridGroup;
            ultraGridBand.Columns["endDate"].RowLayoutColumnInfo.ParentGroup = period_GridGroup;

            ultraGridBand.Columns["RequiredDepth_PaddyUnder"].RowLayoutColumnInfo.ParentGroup = requiredDepth_GridGroup;
            ultraGridBand.Columns["RequiredDepth_PaddyAbove"].RowLayoutColumnInfo.ParentGroup = requiredDepth_GridGroup;
            ultraGridBand.Columns["RequiredDepth_Sum"].RowLayoutColumnInfo.ParentGroup = requiredDepth_GridGroup;

            ultraGridBand.Columns["StartDate"].Header.Caption = "시작";
            ultraGridBand.Columns["endDate"].Header.Caption = "종료";
            ultraGridBand.Columns["GrowingType"].Header.Caption = "구분";
            ultraGridBand.Columns["RequiredDepth_PaddyUnder"].Header.Caption = "논바닥 밑";
            ultraGridBand.Columns["RequiredDepth_PaddyAbove"].Header.Caption = "논바닥 위";
            ultraGridBand.Columns["RequiredDepth_Sum"].Header.Caption = "합계";
            ultraGridBand.Columns["RequiredArea"].Header.Caption = "필요면적(%)";
            ultraGridBand.Columns["RequiredQuantity"].Header.Caption = "필요수량(K㎥)";
            ultraGridBand.Columns["SupplyQuantity"].Header.Caption = "공급량(㎥)";

            ultraGridBand.Columns["Flag"].RowLayoutColumnInfo.OriginX = 0;
            ultraGridBand.Columns["Flag"].RowLayoutColumnInfo.OriginY = 0;
            ultraGridBand.Columns["Flag"].RowLayoutColumnInfo.SpanX = 2;
            ultraGridBand.Columns["Flag"].RowLayoutColumnInfo.SpanY = 4;

            ultraGridBand.Columns["GrowingType"].RowLayoutColumnInfo.OriginX = 2;
            ultraGridBand.Columns["GrowingType"].RowLayoutColumnInfo.OriginY = 0;
            ultraGridBand.Columns["GrowingType"].RowLayoutColumnInfo.SpanX = 2;
            ultraGridBand.Columns["GrowingType"].RowLayoutColumnInfo.SpanY = 4;

            period_GridGroup.RowLayoutGroupInfo.OriginX = 6;
            period_GridGroup.RowLayoutGroupInfo.OriginY = 0;

            requiredDepth_GridGroup.RowLayoutGroupInfo.OriginX = 10;
            requiredDepth_GridGroup.RowLayoutGroupInfo.OriginY = 0;

            ultraGridBand.Columns["RequiredArea"].RowLayoutColumnInfo.OriginX = 14;
            ultraGridBand.Columns["RequiredArea"].RowLayoutColumnInfo.OriginY = 0;
            ultraGridBand.Columns["RequiredArea"].RowLayoutColumnInfo.SpanX = 2;
            ultraGridBand.Columns["RequiredArea"].RowLayoutColumnInfo.SpanY = 4;

            ultraGridBand.Columns["RequiredQuantity"].RowLayoutColumnInfo.OriginX = 16;
            ultraGridBand.Columns["RequiredQuantity"].RowLayoutColumnInfo.OriginY = 0;
            ultraGridBand.Columns["RequiredQuantity"].RowLayoutColumnInfo.SpanX = 2;
            ultraGridBand.Columns["RequiredQuantity"].RowLayoutColumnInfo.SpanY = 4;

            ultraGridBand.Columns["SupplyQuantity"].RowLayoutColumnInfo.OriginX = 18;
            ultraGridBand.Columns["SupplyQuantity"].RowLayoutColumnInfo.OriginY = 0;
            ultraGridBand.Columns["SupplyQuantity"].RowLayoutColumnInfo.SpanX = 2;
            ultraGridBand.Columns["SupplyQuantity"].RowLayoutColumnInfo.SpanY = 4;
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
                        SetGrid_Result(this.ultraGrid2);

                        BindingChart(this.chartControl1, listResult);
                    }
                    
                    break;
                default:
                    break;
            }
        }

        private void BindingChart(ChartControl chart, List<AWR_Result> listResult)
        {
            XYDiagram diagram = (XYDiagram)chart.Diagram;
            diagram.AxisX.WholeRange.SideMarginsValue = 0;

            try
            {
                // 타이블
                chart.Titles.Clear();
                chart.Titles.Add(new ChartTitle() { Text = "모의결과", Alignment = StringAlignment.Center, Dock = ChartTitleDockStyle.Top, Font = new Font("맑은 고딕", 14) });

                // series                
                chart.Series.Clear();

                #region [강우]
                Series seriesRF_mm = new Series("Rainfall_mm", ViewType.Bar);
                chart.Series.Add(seriesRF_mm);

                //bind
                seriesRF_mm.DataSource = listResult;

                //Set
                seriesRF_mm.ArgumentScaleType = ScaleType.DateTime;
                seriesRF_mm.ArgumentDataMember = "curDate";
                seriesRF_mm.ValueScaleType = ScaleType.Numerical;
                seriesRF_mm.ValueDataMembers.AddRange("Rainfall_mm");   // DataTable 컬럼명
                seriesRF_mm.View.Color = Color.PowderBlue;
                seriesRF_mm.CrosshairLabelPattern = "{S} : {V:F2}";
                #endregion

                #region [Evaporation]
                Series seriesEv_mm = new Series("Evaporation_mm", ViewType.Line);
                chart.Series.Add(seriesEv_mm);

                //bind
                seriesEv_mm.DataSource = listResult;

                //Set
                seriesEv_mm.ArgumentScaleType = ScaleType.DateTime;
                seriesEv_mm.ArgumentDataMember = "curDate";
                seriesEv_mm.ValueScaleType = ScaleType.Numerical;
                seriesEv_mm.ValueDataMembers.AddRange("Evaporation_mm");   // DataTable 컬럼명
                seriesEv_mm.View.Color = Color.PeachPuff;
                seriesEv_mm.CrosshairLabelPattern = "{S} : {V:F2}";
                #endregion

                #region [EndStorage]
                Series seriesES_km = new Series("EndingStorage_km", ViewType.Line);
                chart.Series.Add(seriesES_km);

                //bind
                seriesES_km.DataSource = listResult;

                //Set
                seriesES_km.ArgumentScaleType = ScaleType.DateTime;
                seriesES_km.ArgumentDataMember = "curDate";
                seriesES_km.ValueScaleType = ScaleType.Numerical;
                seriesES_km.ValueDataMembers.AddRange("EndingStorage_km");   // DataTable 컬럼명
                seriesES_km.View.Color = Color.LightSteelBlue;
                seriesES_km.CrosshairLabelPattern = "{S} : {V:F2}";

                #endregion

                #region [Irrigation Inflow]
                Series seriesII_mm = new Series("IrrigationInflow_mm", ViewType.Line);
                chart.Series.Add(seriesII_mm);

                //bind
                seriesII_mm.DataSource = listResult;

                //Set
                seriesII_mm.ArgumentScaleType = ScaleType.DateTime;
                seriesII_mm.ArgumentDataMember = "curDate";
                seriesII_mm.ValueScaleType = ScaleType.Numerical;
                seriesII_mm.ValueDataMembers.AddRange("IrrigationInflow_mm");   // DataTable 컬럼명
                seriesII_mm.View.Color = Color.DeepSkyBlue;
                seriesII_mm.CrosshairLabelPattern = "{S} : {V:F2}";
                #endregion

                #region [Irrigation Demand]
                Series seriesID_km = new Series("IrrigationDemand_km", ViewType.Line);
                chart.Series.Add(seriesID_km);

                //bind
                seriesID_km.DataSource = listResult;

                //Set
                seriesID_km.ArgumentScaleType = ScaleType.DateTime;
                seriesID_km.ArgumentDataMember = "curDate";
                seriesID_km.ValueScaleType = ScaleType.Numerical;
                seriesID_km.ValueDataMembers.AddRange("IrrigationDemand_km");   // DataTable 컬럼명
                seriesID_km.View.Color = Color.LightPink;
                seriesID_km.CrosshairLabelPattern = "{S} : {V:F2}";
                #endregion

                #region [Outflow]
                Series seriesOF_km = new Series("Outflow_km", ViewType.Line);
                chart.Series.Add(seriesOF_km);

                //bind
                seriesOF_km.DataSource = listResult;

                //Set
                seriesOF_km.ArgumentScaleType = ScaleType.DateTime;
                seriesOF_km.ArgumentDataMember = "curDate";
                seriesOF_km.ValueScaleType = ScaleType.Numerical;
                seriesOF_km.ValueDataMembers.AddRange("Outflow_km");   // DataTable 컬럼명
                seriesOF_km.View.Color = Color.SteelBlue;
                seriesOF_km.CrosshairLabelPattern = "{S} : {V:F2}";
                #endregion

                #region [축관련]

                // X축
                diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Day;
                diagram.AxisX.Title.Font = new Font("맑은 고딕", 11);
                diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
                diagram.AxisX.Title.Text = "시간";
                diagram.AxisX.GridLines.Visible = true;
                diagram.AxisX.GridLines.LineStyle.DashStyle = DashStyle.Dash;
                diagram.AxisX.WholeRange.SideMarginsValue = 0;  // 양쪽 여백 없애기

                // 왼쪽 Y축
                diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
                diagram.AxisY.Title.Text = "mm";
                diagram.AxisY.Title.Font = new Font("맑은 고딕", 11);
                diagram.AxisY.GridLines.LineStyle.DashStyle = DashStyle.Dash;

                //오른쪽 Y축
                SecondaryAxisY secondY = new SecondaryAxisY();
                secondY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
                secondY.Title.Text = "K㎥";
                secondY.Title.Font = new Font("맑은 고딕", 11);
                //secondY.Reverse = true;
                secondY.Alignment = AxisAlignment.Far;
                secondY.GridLines.Visible = false;

                diagram.SecondaryAxesY.Clear();
                diagram.SecondaryAxesY.Add(secondY);

                LineSeriesView lineView = (LineSeriesView)chart.Series["EndingStorage_km"].View;
                lineView.AxisY = secondY;

                LineSeriesView lineView1 = (LineSeriesView)chart.Series["IrrigationDemand_km"].View;
                lineView1.AxisY = secondY;

                LineSeriesView lineView2 = (LineSeriesView)chart.Series["Outflow_km"].View;
                lineView2.AxisY = secondY;

                #endregion

                // legend
                this.chartControl1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
                this.chartControl1.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
                this.chartControl1.Legend.AlignmentVertical = LegendAlignmentVertical.BottomOutside;
                this.chartControl1.Legend.Direction = LegendDirection.LeftToRight;
                this.chartControl1.Legend.UseCheckBoxes = true;

                // zoom                
                ((XYDiagram)chartControl1.Diagram).EnableAxisXZooming = true;
                ((XYDiagram)chartControl1.Diagram).EnableAxisYZooming = false;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void SetGrid_Result(UltraGrid ultraGrid)
        {
            UltraGridBand ultraGridBand = ultraGrid.DisplayLayout.Bands[0];

            ultraGrid.DisplayLayout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
            ultraGrid.DisplayLayout.Override.AllowRowLayoutColMoving = Infragistics.Win.Layout.GridBagLayoutAllowMoving.AllowAll;

            #region [Group Setting]
            UltraGridGroup beginningStorage_GridGroup = ultraGridBand.Groups.Add("BeginningStorage", "Beginning Storage");
            UltraGridGroup irrigationInflow_GridGroup = ultraGridBand.Groups.Add("IrrigationInflow", "Irrigation Inflow");
            UltraGridGroup rainfall_GridGroup = ultraGridBand.Groups.Add("Rainfall", "Rainfall");
            UltraGridGroup evaporation_GridGroup = ultraGridBand.Groups.Add("Evaporation", "Evaporation");
            UltraGridGroup netEvaporation_GridGroup = ultraGridBand.Groups.Add("NetEvaporation", "Net Evaporation");
            UltraGridGroup endingStorage_GridGroup = ultraGridBand.Groups.Add("EndingStorage", "Ending Storage");
            UltraGridGroup outflow_GridGroup = ultraGridBand.Groups.Add("Outflow", "Outflow");

            ultraGridBand.Columns["BeginningStorage_Km"].RowLayoutColumnInfo.ParentGroup = beginningStorage_GridGroup;
            ultraGridBand.Columns["BeginningStorage_mm"].RowLayoutColumnInfo.ParentGroup = beginningStorage_GridGroup;

            ultraGridBand.Columns["IrrigationInflow_ms"].RowLayoutColumnInfo.ParentGroup = irrigationInflow_GridGroup;
            ultraGridBand.Columns["IrrigationInflow_km"].RowLayoutColumnInfo.ParentGroup = irrigationInflow_GridGroup;
            ultraGridBand.Columns["IrrigationInflow_mm"].RowLayoutColumnInfo.ParentGroup = irrigationInflow_GridGroup;

            ultraGridBand.Columns["Rainfall_mm"].RowLayoutColumnInfo.ParentGroup = rainfall_GridGroup;
            ultraGridBand.Columns["Rainfall_km"].RowLayoutColumnInfo.ParentGroup = rainfall_GridGroup;

            ultraGridBand.Columns["Evaporation_mm"].RowLayoutColumnInfo.ParentGroup = evaporation_GridGroup;
            ultraGridBand.Columns["Evaporation_km"].RowLayoutColumnInfo.ParentGroup = evaporation_GridGroup;

            ultraGridBand.Columns["NetEvaporation_mm"].RowLayoutColumnInfo.ParentGroup = netEvaporation_GridGroup;
            ultraGridBand.Columns["NetEvaporation_km"].RowLayoutColumnInfo.ParentGroup = netEvaporation_GridGroup;

            ultraGridBand.Columns["EndingStorage_km"].RowLayoutColumnInfo.ParentGroup = endingStorage_GridGroup;
            ultraGridBand.Columns["EndingStorage_mm"].RowLayoutColumnInfo.ParentGroup = endingStorage_GridGroup;

            ultraGridBand.Columns["Outflow_km"].RowLayoutColumnInfo.ParentGroup = outflow_GridGroup;
            ultraGridBand.Columns["Outflow_ms"].RowLayoutColumnInfo.ParentGroup = outflow_GridGroup;
            #endregion

            ultraGridBand.Columns["curDate"].Header.Caption = "일자";
            ultraGridBand.Columns["Period"].Header.Caption = "시기";
            ultraGridBand.Columns["Flag"].Header.Caption = "Flag";

            ultraGridBand.Columns["BeginningStorage_Km"].Header.Caption = "(K㎥)";
            ultraGridBand.Columns["BeginningStorage_Km"].Format = "0";
            ultraGridBand.Columns["BeginningStorage_mm"].Header.Caption = "(㎜)";
            ultraGridBand.Columns["BeginningStorage_mm"].Format = "0.00";

            ultraGridBand.Columns["IrrigationInflow_ms"].Header.Caption = "(㎥/s)";
            ultraGridBand.Columns["IrrigationInflow_ms"].Format = "0.000";
            ultraGridBand.Columns["IrrigationInflow_km"].Header.Caption = "(K㎥)";
            ultraGridBand.Columns["IrrigationInflow_km"].Format = "0";
            ultraGridBand.Columns["IrrigationInflow_mm"].Header.Caption = "(㎜)";
            ultraGridBand.Columns["IrrigationInflow_mm"].Format = "0.00";

            ultraGridBand.Columns["Rainfall_mm"].Header.Caption = "(㎜)";
            ultraGridBand.Columns["Rainfall_mm"].Format = "0.00";
            ultraGridBand.Columns["Rainfall_km"].Header.Caption = "(K㎥)";
            ultraGridBand.Columns["Rainfall_km"].Format = "0";

            ultraGridBand.Columns["Evaporation_mm"].Header.Caption = "(㎜)";
            ultraGridBand.Columns["Evaporation_mm"].Format = "0.00";
            ultraGridBand.Columns["Evaporation_km"].Header.Caption = "(K㎥)";
            ultraGridBand.Columns["Evaporation_km"].Format = "0";

            ultraGridBand.Columns["NetEvaporation_mm"].Header.Caption = "(㎜)";
            ultraGridBand.Columns["NetEvaporation_mm"].Format = "0.00";
            ultraGridBand.Columns["NetEvaporation_km"].Header.Caption = "(K㎥)";
            ultraGridBand.Columns["NetEvaporation_km"].Format = "0";

            ultraGridBand.Columns["IrrigationDemand_km"].Header.Caption = "Irrigation Demand (K㎥)";
            ultraGridBand.Columns["IrrigationDemand_km"].Format = "0";

            ultraGridBand.Columns["EndingStorage_km"].Header.Caption = "(K㎥)";
            ultraGridBand.Columns["EndingStorage_km"].Format = "0.0";
            ultraGridBand.Columns["EndingStorage_mm"].Header.Caption = "(㎜)";
            ultraGridBand.Columns["EndingStorage_mm"].Format = "0.00";

            ultraGridBand.Columns["Outflow_km"].Header.Caption = "(K㎥)";
            ultraGridBand.Columns["Outflow_km"].Format = "0";
            ultraGridBand.Columns["Outflow_ms"].Header.Caption = "(㎥/s)";
            ultraGridBand.Columns["Outflow_ms"].Format = "0.0000000";

            ultraGridBand.Columns["RateofReturn"].Header.Caption = "Rate of Return (%)";
            ultraGridBand.Columns["RateofReturn"].Format = "0.00";

            ultraGridBand.Columns["curDate"].RowLayoutColumnInfo.OriginX = 0;
            ultraGridBand.Columns["curDate"].RowLayoutColumnInfo.OriginY = 0;
            ultraGridBand.Columns["curDate"].RowLayoutColumnInfo.SpanX = 2;
            ultraGridBand.Columns["curDate"].RowLayoutColumnInfo.SpanY = 4;

            ultraGridBand.Columns["Period"].RowLayoutColumnInfo.OriginX = 2;
            ultraGridBand.Columns["Period"].RowLayoutColumnInfo.OriginY = 0;
            ultraGridBand.Columns["Period"].RowLayoutColumnInfo.SpanX = 2;
            ultraGridBand.Columns["Period"].RowLayoutColumnInfo.SpanY = 4;

            ultraGridBand.Columns["Flag"].RowLayoutColumnInfo.OriginX = 4;
            ultraGridBand.Columns["Flag"].RowLayoutColumnInfo.OriginY = 0;
            ultraGridBand.Columns["Flag"].RowLayoutColumnInfo.SpanX = 2;
            ultraGridBand.Columns["Flag"].RowLayoutColumnInfo.SpanY = 4;

            beginningStorage_GridGroup.RowLayoutGroupInfo.OriginX = 8;
            beginningStorage_GridGroup.RowLayoutGroupInfo.OriginY = 0;

            irrigationInflow_GridGroup.RowLayoutGroupInfo.OriginX = 12;
            irrigationInflow_GridGroup.RowLayoutGroupInfo.OriginY = 0;

            rainfall_GridGroup.RowLayoutGroupInfo.OriginX = 18;
            rainfall_GridGroup.RowLayoutGroupInfo.OriginY = 0;

            evaporation_GridGroup.RowLayoutGroupInfo.OriginX = 22;
            evaporation_GridGroup.RowLayoutGroupInfo.OriginY = 0;

            netEvaporation_GridGroup.RowLayoutGroupInfo.OriginX = 26;
            netEvaporation_GridGroup.RowLayoutGroupInfo.OriginY = 0;

            ultraGridBand.Columns["IrrigationDemand_km"].RowLayoutColumnInfo.OriginX = 30;
            ultraGridBand.Columns["IrrigationDemand_km"].RowLayoutColumnInfo.OriginY = 0;
            ultraGridBand.Columns["IrrigationDemand_km"].RowLayoutColumnInfo.SpanX = 2;
            ultraGridBand.Columns["IrrigationDemand_km"].RowLayoutColumnInfo.SpanY = 4;

            endingStorage_GridGroup.RowLayoutGroupInfo.OriginX = 34;
            endingStorage_GridGroup.RowLayoutGroupInfo.OriginY = 0;

            outflow_GridGroup.RowLayoutGroupInfo.OriginX = 38;
            outflow_GridGroup.RowLayoutGroupInfo.OriginY = 0;

            ultraGridBand.Columns["RateofReturn"].RowLayoutColumnInfo.OriginX = 42;
            ultraGridBand.Columns["RateofReturn"].RowLayoutColumnInfo.OriginY = 0;
            ultraGridBand.Columns["RateofReturn"].RowLayoutColumnInfo.SpanX = 2;
            ultraGridBand.Columns["RateofReturn"].RowLayoutColumnInfo.SpanY = 4;

            for (int i = 0; i < ultraGrid.Rows.Count; i++)
            {
                ultraGrid.Rows[i].Cells["curDate"].Appearance.BackColor = Color.Bisque;
                ultraGrid.Rows[i].Cells["Period"].Appearance.BackColor = Color.Bisque;
                //ultraGrid.Rows[i].Cells["Flag"].Appearance.BackColor = Color.Bisque;
                ultraGrid.Rows[i].Cells["Flag"].Appearance.ForeColor = Color.Red;

                ultraGrid.Rows[i].Cells["BeginningStorage_Km"].Appearance.BackColor = Color.LightSteelBlue;
                ultraGrid.Rows[i].Cells["BeginningStorage_mm"].Appearance.BackColor = Color.LightSteelBlue;

                ultraGrid.Rows[i].Cells["IrrigationInflow_ms"].Appearance.BackColor = Color.DeepSkyBlue;
                ultraGrid.Rows[i].Cells["IrrigationInflow_km"].Appearance.BackColor = Color.DeepSkyBlue;
                ultraGrid.Rows[i].Cells["IrrigationInflow_mm"].Appearance.BackColor = Color.DeepSkyBlue;

                ultraGrid.Rows[i].Cells["Rainfall_mm"].Appearance.BackColor = Color.PowderBlue;
                ultraGrid.Rows[i].Cells["Rainfall_km"].Appearance.BackColor = Color.PowderBlue;

                ultraGrid.Rows[i].Cells["Evaporation_mm"].Appearance.BackColor = Color.PeachPuff;
                ultraGrid.Rows[i].Cells["Evaporation_km"].Appearance.BackColor = Color.PeachPuff;
                ultraGrid.Rows[i].Cells["NetEvaporation_mm"].Appearance.BackColor = Color.PeachPuff;
                ultraGrid.Rows[i].Cells["NetEvaporation_km"].Appearance.BackColor = Color.PeachPuff;

                ultraGrid.Rows[i].Cells["IrrigationDemand_km"].Appearance.BackColor = Color.LightPink;

                ultraGrid.Rows[i].Cells["EndingStorage_km"].Appearance.BackColor = Color.LightSteelBlue;
                ultraGrid.Rows[i].Cells["EndingStorage_mm"].Appearance.BackColor = Color.LightSteelBlue;

                ultraGrid.Rows[i].Cells["Outflow_km"].Appearance.BackColor = Color.SteelBlue;
                ultraGrid.Rows[i].Cells["Outflow_ms"].Appearance.BackColor = Color.SteelBlue;

                ultraGrid.Rows[i].Cells["RateofReturn"].Appearance.BackColor = Color.Beige;
            }
        }

        private bool ValidationData()
        {
            if (InitVolumeValidation() == true)
            {
                double InitVolume = double.Parse(this.txtInitVolume.Text);
                _global.InitVolume = InitVolume;

                if (_global.listAWRInput.Count > 0 && _global.listAWRSet.Count > 0)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("입력자료를 확인하세요.");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool BenefitAreaValidation()
        {
            if (this.txtBenefitArea.Text == "")
            {
                MessageBox.Show("수혜면적을 확인하세요.");
                return false;
            }
            else
            {
                double checkNum = 0.0;
                bool isNum = double.TryParse(this.txtBenefitArea.Text, out checkNum);

                if (isNum == true)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("수혜면적을 숫자형으로 입력하세요.");
                    return false;
                }
            }
        }

        private bool InitVolumeValidation()
        {
            if (this.txtInitVolume.Text == "")
            {
                MessageBox.Show("초기수위를 확인하세요");
                return false;
            }
            else
            {
                double checkNum = 0.0;
                bool isNum = double.TryParse(this.txtInitVolume.Text, out checkNum);

                if (isNum == true)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("초기수위를 숫자형으로 입력하세요.");
                    return false;
                }
            }
        }
    }
}
