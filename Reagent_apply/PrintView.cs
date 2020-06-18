using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace 入库文件
{
    public partial class PrintView : Form
    {
        public List<Emp> Emps { get; set; }
        public PrintView()
        {
            InitializeComponent();
        }

        private void PrintView_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }
        private void PrintView_FormClosed(object sender, FormClosedEventArgs e)
        {
            reportViewer1.LocalReport.ReleaseSandboxAppDomain();
        }
        public void report_Load()
        {
             DataTable dt = new DataTable();
            //定义本地数据表的列，名称应跟之前所建的testDataTable表中列相同。
            dt.Columns.Add("名称", typeof(string));
            dt.Columns.Add("生产厂家", typeof(string));
            dt.Columns.Add("规格", typeof(string));
            dt.Columns.Add("数量", typeof(string));
            dt.Columns.Add("申请人", typeof(string));
            //动态生成一些测试用数据
            int NULL_Count= (21 - (Emps.Count + 1) % 21)%21;;
            for (int i = 0; i < Emps.Count; i++)
            {
                DataRow row = dt.NewRow();
                row[0] = Emps[i].name ;
                row[1] = Emps[i].manufacturer;
                row[2] = Emps[i].scale;
                row[3] = Emps[i].count+""+"盒";
                row[4] = Emps[i].applyer;
                dt.Rows.Add(row);
            }
            for (int i = 0; i < NULL_Count; i++)
            {
                DataRow row = dt.NewRow();
                row[0] = null;
                row[1] = null;
                row[2] = null;
                row[3] = null;
                row[4] = null;
                dt.Rows.Add(row);
            }

                //设置本地报表，使程序与之前所建的testReport.rdlc报表文件进行绑定。
            this.reportViewer1.LocalReport.ReportPath = "Report1.rdlc";
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet2",dt));

        }
    }
}
