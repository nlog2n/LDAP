namespace SynchronousAD
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblDomainName = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblPwd = new System.Windows.Forms.Label();
            this.lblRootOU = new System.Windows.Forms.Label();
            this.txtDomainName = new System.Windows.Forms.TextBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtRootOU = new System.Windows.Forms.TextBox();
            this.btnSyns = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDomainName
            // 
            this.lblDomainName.AutoSize = true;
            this.lblDomainName.Location = new System.Drawing.Point(105, 58);
            this.lblDomainName.Name = "lblDomainName";
            this.lblDomainName.Size = new System.Drawing.Size(41, 12);
            this.lblDomainName.TabIndex = 0;
            this.lblDomainName.Text = "域名：";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(93, 96);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(53, 12);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "用户名：";
            // 
            // lblPwd
            // 
            this.lblPwd.AutoSize = true;
            this.lblPwd.Location = new System.Drawing.Point(105, 132);
            this.lblPwd.Name = "lblPwd";
            this.lblPwd.Size = new System.Drawing.Size(41, 12);
            this.lblPwd.TabIndex = 2;
            this.lblPwd.Text = "密码：";
            // 
            // lblRootOU
            // 
            this.lblRootOU.AutoSize = true;
            this.lblRootOU.Location = new System.Drawing.Point(69, 171);
            this.lblRootOU.Name = "lblRootOU";
            this.lblRootOU.Size = new System.Drawing.Size(77, 12);
            this.lblRootOU.TabIndex = 3;
            this.lblRootOU.Text = "根组织单位：";
            // 
            // txtDomainName
            // 
            this.txtDomainName.Location = new System.Drawing.Point(152, 55);
            this.txtDomainName.Name = "txtDomainName";
            this.txtDomainName.Size = new System.Drawing.Size(200, 21);
            this.txtDomainName.TabIndex = 1;
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(152, 129);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '•';
            this.txtPwd.Size = new System.Drawing.Size(200, 21);
            this.txtPwd.TabIndex = 3;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(152, 93);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(200, 21);
            this.txtUserName.TabIndex = 2;
            // 
            // txtRootOU
            // 
            this.txtRootOU.Location = new System.Drawing.Point(152, 168);
            this.txtRootOU.Name = "txtRootOU";
            this.txtRootOU.Size = new System.Drawing.Size(200, 21);
            this.txtRootOU.TabIndex = 4;
            // 
            // btnSyns
            // 
            this.btnSyns.Location = new System.Drawing.Point(277, 226);
            this.btnSyns.Name = "btnSyns";
            this.btnSyns.Size = new System.Drawing.Size(75, 23);
            this.btnSyns.TabIndex = 8;
            this.btnSyns.Text = "同步";
            this.btnSyns.UseVisualStyleBackColor = true;
            this.btnSyns.Click += new System.EventHandler(this.btnSyns_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 282);
            this.Controls.Add(this.btnSyns);
            this.Controls.Add(this.txtRootOU);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.txtDomainName);
            this.Controls.Add(this.lblRootOU);
            this.Controls.Add(this.lblPwd);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.lblDomainName);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AD域同步";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDomainName;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblPwd;
        private System.Windows.Forms.Label lblRootOU;
        private System.Windows.Forms.TextBox txtDomainName;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtRootOU;
        private System.Windows.Forms.Button btnSyns;
    }
}

