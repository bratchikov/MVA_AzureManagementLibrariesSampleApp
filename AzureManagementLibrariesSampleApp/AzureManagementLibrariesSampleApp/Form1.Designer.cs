namespace AzureManagementLibrariesSampleApp
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
            this.siteNameTextBox = new System.Windows.Forms.TextBox();
            this.urlPrefixLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.NewDemoWebSiteBtn = new System.Windows.Forms.Button();
            this.RemoveDemoWebSiteBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // siteNameTextBox
            // 
            this.siteNameTextBox.Location = new System.Drawing.Point(62, 12);
            this.siteNameTextBox.Name = "siteNameTextBox";
            this.siteNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.siteNameTextBox.TabIndex = 1;
            this.siteNameTextBox.Text = "Atlas";
            // 
            // urlPrefixLabel
            // 
            this.urlPrefixLabel.AutoSize = true;
            this.urlPrefixLabel.Location = new System.Drawing.Point(8, 15);
            this.urlPrefixLabel.Name = "urlPrefixLabel";
            this.urlPrefixLabel.Size = new System.Drawing.Size(48, 13);
            this.urlPrefixLabel.TabIndex = 4;
            this.urlPrefixLabel.Text = "contoso-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(168, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = ".azurewebsites.net";
            // 
            // NewDemoWebSiteBtn
            // 
            this.NewDemoWebSiteBtn.Location = new System.Drawing.Point(11, 46);
            this.NewDemoWebSiteBtn.Name = "NewDemoWebSiteBtn";
            this.NewDemoWebSiteBtn.Size = new System.Drawing.Size(114, 23);
            this.NewDemoWebSiteBtn.TabIndex = 5;
            this.NewDemoWebSiteBtn.Text = "Создать сайт";
            this.NewDemoWebSiteBtn.UseVisualStyleBackColor = true;
            this.NewDemoWebSiteBtn.Click += new System.EventHandler(this.NewDemoWebSiteBtn_Click);
            // 
            // RemoveDemoWebSiteBtn
            // 
            this.RemoveDemoWebSiteBtn.Location = new System.Drawing.Point(140, 46);
            this.RemoveDemoWebSiteBtn.Name = "RemoveDemoWebSiteBtn";
            this.RemoveDemoWebSiteBtn.Size = new System.Drawing.Size(123, 23);
            this.RemoveDemoWebSiteBtn.TabIndex = 6;
            this.RemoveDemoWebSiteBtn.Text = "Удалить сайт";
            this.RemoveDemoWebSiteBtn.UseVisualStyleBackColor = true;
            this.RemoveDemoWebSiteBtn.Click += new System.EventHandler(this.RemoveDemoWebSiteBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.RemoveDemoWebSiteBtn);
            this.Controls.Add(this.NewDemoWebSiteBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.urlPrefixLabel);
            this.Controls.Add(this.siteNameTextBox);
            this.Name = "Form1";
            this.Text = "MVA Sample";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox siteNameTextBox;
        private System.Windows.Forms.Label urlPrefixLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button NewDemoWebSiteBtn;
        private System.Windows.Forms.Button RemoveDemoWebSiteBtn;
    }
}

