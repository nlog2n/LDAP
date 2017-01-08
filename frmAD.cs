using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Security.Principal;

using LDAP.ActiveDirectory;

namespace SynchronousAD
{
    public partial class Form1 : Form
    {
        private AD ad = new AD();

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// buttong for synchronization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSyns_Click(object sender, EventArgs e)
        {
            if (ValidationInput())
            {
                if (ad.Connect(txtDomainName.Text.Trim(), txtUserName.Text.Trim(), txtPwd.Text.Trim(), txtRootOU.Text.Trim()))
                {
                    MessageBox.Show("连接AD成功,继续同步中...", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (ad.ExchangeDomain != null)
                    {
                        /*
                        List<AdNode> list = ad.GetAllUsers(); //同步所有

                        StringBuilder sb = new StringBuilder();
                        sb.Append("DomainName=" + txtDomainName.Text + "\n");
                        sb.Append("UserName=" + txtUserName.Text + "\n");
                        sb.Append("RootOU=" + txtRootOU.Text + "\n");
                        sb.Append("同步成功" + list.Count.ToString() + "项\n");
                        sb.Append("\r\n帐号\t姓名\t类型\t电邮\t路径\r\n");

                        foreach (var item in list)
                        {
                            sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\r\n", item.Name, item.DisplayName, item.Type, item.Email, item.DistinguishedName);
                        }

                        LogRecord.WriteLog(sb.ToString());
                        MessageBox.Show(sb.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        */

                        AdGroup group = ad.FindGroup("ICS");
                        string sss = "ICS\n";
                        foreach (AdNode item in group.Members)
                        {
                            if (!string.IsNullOrEmpty(item.Email))
                                sss += item.Email + ",";
                        }
                        MessageBox.Show(sss, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Application.Exit();
                    }
                    else
                    {
                        MessageBox.Show("域中不存在此组织结构!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("不能连接到域,请确认输入是否正确!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }



        /// <summary>
        /// form loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            txtDomainName.Text = "teak.local.net"; // "10.217.253.161";
            txtUserName.Text = "hfang"; // "administrator";
            txtPwd.Text = "P@ssw0rd";
            txtRootOU.Text = "Exchange"; // "company";
        }

        /// <summary>
        /// input verification
        /// </summary>
        /// <returns></returns>
        private bool ValidationInput()
        {
            if (txtDomainName.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入域名!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDomainName.Focus();
                return false;
            }

            if (txtUserName.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入用户名!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserName.Focus();
                return false;
            }

            if (txtPwd.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入密码!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPwd.Focus();
                return false;
            }

            if (txtRootOU.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入根组织单位!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRootOU.Focus();
                return false;
            }
            return true;
        }
    }
}
