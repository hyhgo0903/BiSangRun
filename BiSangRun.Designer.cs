namespace BiSangRun
{
  partial class BiSangRun
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (this.components != null))
      {
        this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.button1 = new Button();
      this.button2 = new Button();
      this.label1 = new Label();
      this.label2 = new Label();
      this.button3 = new Button();
      this.button4 = new Button();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new Point(80, 34);
      this.button1.Name = "button1";
      this.button1.Size = new Size(75, 23);
      this.button1.TabIndex = 1;
      this.button1.Text = "초기화";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += this.button1_Click;
      // 
      // button2
      // 
      this.button2.Location = new Point(80, 93);
      this.button2.Name = "button2";
      this.button2.Size = new Size(75, 23);
      this.button2.TabIndex = 2;
      this.button2.Text = "실행";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += this.button2_Click;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new Point(161, 38);
      this.label1.Name = "label1";
      this.label1.Size = new Size(163, 15);
      this.label1.TabIndex = 3;
      this.label1.Text = "비밀상점 화면에서 시작할 것";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new Point(161, 97);
      this.label2.Name = "label2";
      this.label2.Size = new Size(0, 15);
      this.label2.TabIndex = 4;
      // 
      // button3
      // 
      this.button3.Location = new Point(511, 93);
      this.button3.Name = "button3";
      this.button3.Size = new Size(75, 23);
      this.button3.TabIndex = 5;
      this.button3.Text = "중지";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += this.button3_Click;
      // 
      // button4
      // 
      this.button4.Location = new Point(511, 34);
      this.button4.Name = "button4";
      this.button4.Size = new Size(106, 23);
      this.button4.TabIndex = 6;
      this.button4.Text = "항상 위 해제";
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += this.button4_Click;
      // 
      // BiSangRun
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(685, 149);
      Controls.Add(this.button4);
      Controls.Add(this.button3);
      Controls.Add(this.label2);
      Controls.Add(this.label1);
      Controls.Add(this.button2);
      Controls.Add(this.button1);
      Name = "BiSangRun";
      Text = "비이이이상!";
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion
    private Button button1;
    private Button button2;
    private Label label1;
    private Label label2;
    private Button button3;
    private Button button4;
  }
}
