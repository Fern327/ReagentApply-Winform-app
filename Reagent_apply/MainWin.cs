using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Drawing.Printing;
using Microsoft.Reporting.WinForms;

namespace 入库文件
{

    public partial class ReForm : Form
    {
        public int ReForm_PageNumber;
        public int ReForm_PageSize = 20;
        public int ReForm_CurrentPageNumber = 1;
        public List<Emp> emps = new List<Emp>();
        public Emp GiveEmp { get; set; }
        public const string BindDatasql = "select Name,Manufacturer,Scale,Count,Scale,Applyer,Appdate from rea_app where Appdate like @appdate";
        public ReForm()
        {
            InitializeComponent();
        }
        //把list转为datatable
        public static DataTable ToDataTableTow(List<Emp> list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();

                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow (array, true);
                }
            }
            return result;
        }
        //分页显示
        public DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)//PageIndex表示第几页，PageSize表示每页的记录数
        {
            if (PageIndex == 0)
                return dt;//0页代表每页数据，直接返回

            DataTable newdt = dt.Copy();
            newdt.Clear();//copy dt的框架

            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;//源数据记录数小于等于要显示的记录，直接返回dt

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <rowend; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }
        //将数据库信息显示到表格
        public void BindData(string date,string sql,SqlParameter ReaName)
        {
            emps.Clear();
            bool getvalue = false;
            SqlParameter appdate = new SqlParameter("@appdate", SqlDbType.NVarChar, 10);
            appdate.Value = date;
            //执行查询获得结果集
            SqlDataReader sdr;
            //如果没有名称参数传入，只查询指定日期
            if (ReaName == null) { sdr = SqlHelper.Reader(sql, appdate); }
            //如果有名称参数传入，查询指定名称
            else { sdr = SqlHelper.Reader(sql, appdate,ReaName); }
            int i = 0;
            while (sdr.Read())
            {
                //判断是否有行数返回
                getvalue = true;
                Emp emp = new Emp();
                emp.name = sdr["Name"] + "";
                emp.manufacturer = sdr["Manufacturer"] + "";
                emp.scale = sdr["Scale"] + "";
                emp.count = Convert.ToInt32(sdr["Count"]);
                emp.applyer = sdr["Applyer"] + "";
                emp.appdate = sdr["Appdate"] + "";
                emps.Add(emp);
            }
            sdr.Close();
            if (getvalue == true) 
            {
                //算出记录的页数
                ReForm_PageSize = dataGridView1.Height / 28;
                ReForm_PageNumber = emps.Count / ReForm_PageSize + (emps.Count % ReForm_PageSize > 0 ? 1 : 0);
                //显示出总页数
                label10.Text = "/" + ReForm_PageNumber + "";
                textBox5.Text = ReForm_CurrentPageNumber+"";
                emps = emps.OrderBy(a => a.appdate).ToList();
                for (i = 0; i < emps.Count; i++) 
                {
                    emps[i].id = i + 1;
                }               
                DataTable dt = ToDataTableTow(emps);
                DataTable showdt = GetPagedTable(dt, 1, ReForm_PageSize);
                dataGridView1.DataSource = showdt;
                label11.Text = "共" + Convert.ToInt32(i ) + "条记录";
            }
            else
            {
                dataGridView1.DataSource = emps;
                label11.Text = "无数据";
                MessageBox.Show("无可显示数据! error:5", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
        }
        //得到有效日期存入数据库
        public string GetDate() 
        {
            string year = comboBox2.Text;
            string month = comboBox3.Text;
            string day = comboBox4.Text;
            //如果未输入年或月等信息，则按当前时间计算
            if (comboBox2.Text == "" || comboBox2.Text == "/") { year = DateTime.Now.Year.ToString(); }
            if (comboBox3.Text == "" || comboBox3.Text == "/") { month = DateTime.Now.Month.ToString(); }
            if (comboBox4.Text == "" || comboBox4.Text == "/") { day = DateTime.Now.Day.ToString(); }
            return year + "." + month + "." + day;
        }
        //得到要查询显示的日期
        public string SearchDate()
        {
            string year = comboBox2.Text;
            string month = comboBox3.Text;
            string day = comboBox4.Text;
            //如果未输入年或月等信息，则按当前时间计算
            if (comboBox2.Text == "") { year = DateTime.Now.Year.ToString(); }
            if (comboBox3.Text == "") { month = DateTime.Now.Month.ToString(); }
            if (comboBox4.Text == "") { day = DateTime.Now.Day.ToString(); }
            string date;
            if (year == "/") { date = "%"; }
            else if (month == "/") { date = year + "%"; }
            else if (day == "/") { date = year + "." + month + "%"; }
            else { date = year + "." + month + "." + day; }
            return date;
        }
        //检查指定日期是否正确
        public bool RangeCheck()
        {
            bool rangeout = false;
            if (comboBox3.Text != "" && comboBox3.Text != "/")
            {
                int monthrange = Convert.ToInt32(comboBox3.Text);
                if (monthrange < 1 || monthrange > 12) { rangeout = true; }
            }
            if (comboBox4.Text != "" && comboBox4.Text != "/")
            {
                int dayrange = Convert.ToInt32(comboBox4.Text);
                if (dayrange < 1 || dayrange > 31) { rangeout = true; }
            }
            return rangeout;
        }
        //查询按钮
        private void button1_Click(object sender, EventArgs e)
        {
            //简写框里未输入信息，因此无法查询
            if (textBox1.Text == "")
            {
                MessageBox.Show("请输入试剂简写! error:1", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                textBox2.Text = textBox3.Text = textBox4.Text = null;
            }
            else
            {
                //bool值检测是否简写有结果
                bool valuble = false;
                SqlParameter key = new SqlParameter("@key", SqlDbType.NChar, 6);
                key.Value = textBox1.Text;
                //执行查询获得结果集
                SqlDataReader sdr = SqlHelper.Reader("select name,manufacturer,scale from rea_info where inkey=@key ", key);
                //如果sdr返回false,则说明无查询结果
                while (sdr.Read())
                {
                    valuble = true;
                    //在左侧文本框里显示名称
                    textBox2.Text = sdr["name"] + "";
                    //在左侧文本框里显示厂家
                    textBox3.Text = sdr["manufacturer"] + "";
                    //在左侧文本框里显示规模
                    textBox4.Text = sdr["scale"] + "";
                }
                sdr.Close();
                if (valuble==false)
                {
                    MessageBox.Show("无法查询到试剂，请输入正确的试剂名称! error:2", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    textBox2.Text = textBox3.Text = textBox4.Text = null;
                }
            }
  
        }
        //增加申请按钮
        private void button2_Click(object sender, EventArgs e)
        {
            if (RangeCheck()) 
            {
                MessageBox.Show("请输入正确的日期! error:7", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || numericUpDown1.Text == "0" || comboBox1.Text == "" )
            {
                MessageBox.Show("请输入完整的试剂申请信息! error:3", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else 
            {
                string date = GetDate();
                SqlParameter rename = new SqlParameter("@rename", SqlDbType.NVarChar, 30);
                rename.Value = textBox2.Text;
                SqlParameter adate = new SqlParameter("@adate", SqlDbType.NVarChar, 10);
                adate.Value = date;
                SqlDataReader sdr = SqlHelper.Reader("select name from rea_app where Name=@rename and Appdate like @adate", rename,adate);
                if (sdr.Read())
                {
                    MessageBox.Show("今天已输入该试剂信息，请勿重复输入申请试剂! error:7", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
                else {
                    //传入试剂名称参数
                    SqlParameter Name = new SqlParameter("@Name", SqlDbType.NVarChar, 30);
                    Name.Value = textBox2.Text;
                    //传入生产厂家参数
                    SqlParameter Manu = new SqlParameter("@Manu", SqlDbType.NVarChar, 20);
                    Manu.Value = textBox3.Text;
                    //传入规格参数
                    SqlParameter Scale = new SqlParameter("@Scale", SqlDbType.NVarChar, 20);
                    Scale.Value = textBox4.Text;
                    //传入申请数量参数
                    SqlParameter Count = new SqlParameter("@Count", SqlDbType.Int);
                    Count.Value = Convert.ToInt32(numericUpDown1.Text);
                    //传入申请人参数
                    SqlParameter Applyer = new SqlParameter("@Applyer", SqlDbType.NVarChar, 10);
                    Applyer.Value = comboBox1.Text;
                    //传入申请日期参数
                    SqlParameter Appdate = new SqlParameter("@Appdate", SqlDbType.NVarChar, 10);
                    Appdate.Value = date;
                    //传入sql命令，在数据库中增加记录
                    string sql = "insert into rea_app(Name,Manufacturer,Scale,Count,Applyer,Appdate) values(@Name,@Manu,@Scale,@Count,@Applyer,@Appdate);";
                    int row = SqlHelper.Execute(sql, Name, Manu, Scale, Count, Applyer, Appdate);
                    //如果插入零行，则插入失败
                    if (row == 0)
                    {
                        MessageBox.Show("增加信息失败!error:4", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    }
                    //显示指定年、月、日的记录
                    BindData(date,BindDatasql,null);
                }
                textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = null;
            }
        }
        //查询记录按钮
        private void button3_Click(object sender, EventArgs e)
        {
            string date = SearchDate();
            BindData(date, BindDatasql,null);
            ReForm_CurrentPageNumber = 1;
            textBox5.Text = ReForm_CurrentPageNumber + "";
        }
        //删除按钮
        private void button5_Click(object sender, EventArgs e)
        {
            //如果没有选择行只选择了一格
            if (dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请选择一行数据！error:10", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else
            {
                String nameStr = dataGridView1.SelectedRows[0].Cells[1].Value + "";
                SqlParameter Str = new SqlParameter("@Str", SqlDbType.NVarChar, 30);
                Str.Value = nameStr;
                int rows = SqlHelper.Execute("delete from rea_app where Name=@Str", Str);
                if (rows == 0)
                {
                    MessageBox.Show("删除记录失败!error:6", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
                string date = SearchDate();
                BindData(date, BindDatasql, null);
                // MessageBox.Show("删除成功" + rows + "行"); 
            }
        }
        //上一页按钮
        private void button7_Click(object sender, EventArgs e)
        {
            if (ReForm_CurrentPageNumber == 1)
            {
                MessageBox.Show("已经是第一页! error:8", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else
            {
                ReForm_CurrentPageNumber--;
                textBox5.Text = ReForm_CurrentPageNumber + "";
                DataTable dt = ToDataTableTow(emps);
                DataTable showdt = GetPagedTable(dt, ReForm_CurrentPageNumber, ReForm_PageSize);
                dataGridView1.DataSource = showdt;
            }
        }
        //下一页按钮
        private void button6_Click(object sender, EventArgs e)
        {
            if (ReForm_CurrentPageNumber == ReForm_PageNumber)
            {
                MessageBox.Show("已经是最后一页! error:8", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else
            {
                ReForm_CurrentPageNumber++;
                textBox5.Text = ReForm_CurrentPageNumber + "";
                DataTable dt = ToDataTableTow(emps);
                DataTable showdt = GetPagedTable(dt, ReForm_CurrentPageNumber, ReForm_PageSize);
                dataGridView1.DataSource = showdt;
            }
        }
        //查询指定试剂信息
        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox6.Text == "")
            {
                MessageBox.Show("请输入要查询试剂名称! error:9", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else
            {
                string date = SearchDate();
                SqlParameter ReaName = new SqlParameter("@ReaName", SqlDbType.NVarChar, 30);
                ReaName.Value = textBox6.Text;
                string sql = "select Name,Manufacturer,Scale,Count,Scale,Applyer,Appdate from rea_app where Appdate like @appdate and Name = @ReaName";
                BindData(date, sql,ReaName);
            }
        }
        //修改记录
        private void button9_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1 )
            {
                MessageBox.Show("请选择一行数据！error:11", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else
            {
                FormEdit edit = new FormEdit();
                Emp EditEmp=new Emp();
                EditEmp.name = dataGridView1.SelectedRows[0].Cells[1].Value+"";
                edit.Editemp = EditEmp;
                edit.ShowDialog();  //打开模式窗口
                string date = SearchDate();
                BindData(date, BindDatasql, null);
            }
        }
        //打开表格时候加载本月记录
        private void Form1_Load(object sender, EventArgs e)
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string date = year+"."+month+"%";
            BindData(date, BindDatasql, null);
        }
        //打印
        private void button4_Click(object sender, EventArgs e)
        {
            PrintView Pview = new PrintView();
            Pview.Emps = emps;
            Pview.report_Load();
            Pview.Show();
            //LocalReport report = new LocalReport();
            //设置需要打印的报表的文件名称。
            //report.ReportPath = @"c:\PrintMe.rdlc";
            //创建要打印的数据源 
            //DataTable dt = ToDataTableTow(emps);
            //ReportDataSource source = new ReportDataSource(dt.TableName, dt);
            //report.DataSources.Add(source);
            //刷新报表中的需要呈现的数据
            //report.Refresh();
            
            //将报表的内容按照deviceInfo指定的格式输出到CreateStream函数提供的Stream中。
            //report.Render("Image", deviceInfo, CreateStream, out warnings);
        }
        //改变行数
        private void Form1_Resize(object sender, EventArgs e)
        {
            ReForm_PageSize = dataGridView1.Height / 28;
            DataTable dt = ToDataTableTow(emps);
            DataTable showdt = GetPagedTable(dt, ReForm_CurrentPageNumber, ReForm_PageSize);
            dataGridView1.DataSource = showdt;
        }

    }
}
