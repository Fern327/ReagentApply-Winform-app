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

namespace 入库文件
{
    public partial class FormEdit : Form
    {
        public FormEdit()
        {
            InitializeComponent();
            
        }
        public Emp Editemp { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text=="")
            {
                MessageBox.Show("请输入正确的数量! error:12", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else if(Convert.ToInt32(textBox1.Text) < 0)
            {
                MessageBox.Show("请输入正确的数量! error:12", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else
            {
                SqlParameter EditName = new SqlParameter("@EditName", SqlDbType.NVarChar, 30);
                EditName.Value = Editemp.name;
                SqlParameter EditCount = new SqlParameter("@EditCount", SqlDbType.Int);
                EditCount.Value = Convert.ToInt32(textBox1.Text);
                string sql = "update  rea_app set Count=@EditCount where Name= @EditName;";
                int row = SqlHelper.Execute(sql, EditCount, EditName);
                //如果插入零行，则插入失败
                if (row == 0)
                {
                    MessageBox.Show("修改信息失败!error:12", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                } 
            }
        }
        public int location { get; set; }
    }
}
