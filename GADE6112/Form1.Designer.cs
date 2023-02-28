namespace GADE6112
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
            this.MapLbl = new System.Windows.Forms.Label();
            this.PlayerStatsLbl = new System.Windows.Forms.Label();
            this.heroLabel = new System.Windows.Forms.Label();
            this.enemyLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MapLbl
            // 
            this.MapLbl.AccessibleName = "label1";
            this.MapLbl.AutoSize = true;
            this.MapLbl.Location = new System.Drawing.Point(191, 89);
            this.MapLbl.Name = "MapLbl";
            this.MapLbl.Size = new System.Drawing.Size(31, 15);
            this.MapLbl.TabIndex = 0;
            this.MapLbl.Text = "Map";
            // 
            // PlayerStatsLbl
            // 
            this.PlayerStatsLbl.AutoSize = true;
            this.PlayerStatsLbl.Location = new System.Drawing.Point(321, 352);
            this.PlayerStatsLbl.Name = "PlayerStatsLbl";
            this.PlayerStatsLbl.Size = new System.Drawing.Size(32, 15);
            this.PlayerStatsLbl.TabIndex = 1;
            this.PlayerStatsLbl.Text = "Stats";
            // 
            // heroLabel
            // 
            this.heroLabel.AccessibleName = "heroLabel";
            this.heroLabel.AutoSize = true;
            this.heroLabel.Location = new System.Drawing.Point(640, 31);
            this.heroLabel.Name = "heroLabel";
            this.heroLabel.Size = new System.Drawing.Size(72, 15);
            this.heroLabel.TabIndex = 2;
            this.heroLabel.Text = "Hero output";
            // 
            // enemyLabel
            // 
            this.enemyLabel.AccessibleName = "enemyLabel";
            this.enemyLabel.AutoSize = true;
            this.enemyLabel.Location = new System.Drawing.Point(640, 129);
            this.enemyLabel.Name = "enemyLabel";
            this.enemyLabel.Size = new System.Drawing.Size(82, 15);
            this.enemyLabel.TabIndex = 3;
            this.enemyLabel.Text = "Enemy output";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(637, 348);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.enemyLabel);
            this.Controls.Add(this.heroLabel);
            this.Controls.Add(this.PlayerStatsLbl);
            this.Controls.Add(this.MapLbl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label MapLbl;
        private Label PlayerStatsLbl;
        private Label heroLabel;
        private Label enemyLabel;
        private Button button1;
    }
}