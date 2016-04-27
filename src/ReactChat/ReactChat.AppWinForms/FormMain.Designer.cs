namespace ReactChat.AppWinForms
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.splashPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // splashPanel
            // 
            this.splashPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splashPanel.BackgroundImage")));
            this.splashPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.splashPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splashPanel.Location = new System.Drawing.Point(0, 0);
            this.splashPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splashPanel.Name = "splashPanel";
            this.splashPanel.Size = new System.Drawing.Size(740, 454);
            this.splashPanel.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(740, 454);
            this.Controls.Add(this.splashPanel);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormMain";
            this.Text = "ReactChat";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel splashPanel;
    }
}

