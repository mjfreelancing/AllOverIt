namespace ParallelEvaluation
{
  partial class FrmMain
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
      this.pnlTop = new System.Windows.Forms.Panel();
      this.lblCalcRate = new System.Windows.Forms.Label();
      this.btnGenerate = new System.Windows.Forms.Button();
      this.pnlImage = new System.Windows.Forms.Panel();
      this.pnlTop.SuspendLayout();
      this.SuspendLayout();
      // 
      // pnlTop
      // 
      this.pnlTop.Controls.Add(this.lblCalcRate);
      this.pnlTop.Controls.Add(this.btnGenerate);
      this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
      this.pnlTop.Location = new System.Drawing.Point(0, 0);
      this.pnlTop.Name = "pnlTop";
      this.pnlTop.Size = new System.Drawing.Size(567, 71);
      this.pnlTop.TabIndex = 0;
      // 
      // lblCalcRate
      // 
      this.lblCalcRate.AutoSize = true;
      this.lblCalcRate.Location = new System.Drawing.Point(12, 45);
      this.lblCalcRate.Name = "lblCalcRate";
      this.lblCalcRate.Size = new System.Drawing.Size(40, 13);
      this.lblCalcRate.TabIndex = 1;
      this.lblCalcRate.Text = "Status:";
      this.lblCalcRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // btnGenerate
      // 
      this.btnGenerate.Location = new System.Drawing.Point(12, 12);
      this.btnGenerate.Name = "btnGenerate";
      this.btnGenerate.Size = new System.Drawing.Size(75, 23);
      this.btnGenerate.TabIndex = 0;
      this.btnGenerate.Text = "Generate";
      this.btnGenerate.UseVisualStyleBackColor = true;
      this.btnGenerate.Click += new System.EventHandler(this.BtnGenerate_Click);
      // 
      // pnlImage
      // 
      this.pnlImage.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pnlImage.Location = new System.Drawing.Point(0, 71);
      this.pnlImage.Name = "pnlImage";
      this.pnlImage.Size = new System.Drawing.Size(567, 389);
      this.pnlImage.TabIndex = 1;
      // 
      // FrmMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(567, 460);
      this.Controls.Add(this.pnlImage);
      this.Controls.Add(this.pnlTop);
      this.Name = "FrmMain";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Parallel Expression Evaluation";
      this.pnlTop.ResumeLayout(false);
      this.pnlTop.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel pnlTop;
    private System.Windows.Forms.Button btnGenerate;
    private System.Windows.Forms.Panel pnlImage;
    private System.Windows.Forms.Label lblCalcRate;
  }
}
