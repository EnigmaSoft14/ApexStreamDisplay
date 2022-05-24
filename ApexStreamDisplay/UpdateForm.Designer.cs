
namespace ApexStreamDisplay
{
    partial class UpdateForm
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
            this.changesTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.versionLB = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.downloadBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // changesTB
            // 
            this.changesTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.changesTB.BackColor = System.Drawing.SystemColors.Window;
            this.changesTB.Location = new System.Drawing.Point(12, 86);
            this.changesTB.Multiline = true;
            this.changesTB.Name = "changesTB";
            this.changesTB.ReadOnly = true;
            this.changesTB.Size = new System.Drawing.Size(388, 145);
            this.changesTB.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "A new version is available:";
            // 
            // versionLB
            // 
            this.versionLB.AutoSize = true;
            this.versionLB.Location = new System.Drawing.Point(12, 31);
            this.versionLB.Name = "versionLB";
            this.versionLB.Size = new System.Drawing.Size(0, 13);
            this.versionLB.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "What\'s new:";
            // 
            // downloadBtn
            // 
            this.downloadBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadBtn.Location = new System.Drawing.Point(137, 244);
            this.downloadBtn.MaximumSize = new System.Drawing.Size(140, 23);
            this.downloadBtn.MinimumSize = new System.Drawing.Size(140, 23);
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.Size = new System.Drawing.Size(140, 23);
            this.downloadBtn.TabIndex = 4;
            this.downloadBtn.Text = "Download new version";
            this.downloadBtn.UseVisualStyleBackColor = true;
            this.downloadBtn.Click += new System.EventHandler(this.downloadBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 218);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 5;
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 279);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.downloadBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.versionLB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.changesTB);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(428, 318);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(428, 318);
            this.Name = "UpdateForm";
            this.ShowIcon = false;
            this.Text = "Check for Updates";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox changesTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label versionLB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button downloadBtn;
        private System.Windows.Forms.Label label3;
    }
}