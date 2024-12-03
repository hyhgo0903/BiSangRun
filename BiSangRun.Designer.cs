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
      this.initializeButton = new Button();
      this.startButton = new Button();
      this.label1 = new Label();
      this.label2 = new Label();
      this.stopButton = new Button();
      this.releaseTopButton = new Button();
      this.maximumNumericUpDown = new NumericUpDown();
      this.maximumLabel = new Label();
      this.mainPictureBox = new PictureBox();
      this.includeCheckBox = new CheckBox();
      this.soundCheckBox = new CheckBox();
      ((System.ComponentModel.ISupportInitialize)this.maximumNumericUpDown).BeginInit();
      ((System.ComponentModel.ISupportInitialize)this.mainPictureBox).BeginInit();
      this.SuspendLayout();
      // 
      // initializeButton
      // 
      this.initializeButton.Location = new Point(80, 34);
      this.initializeButton.Name = "initializeButton";
      this.initializeButton.Size = new Size(75, 23);
      this.initializeButton.TabIndex = 1;
      this.initializeButton.Text = "초기화";
      this.initializeButton.UseVisualStyleBackColor = true;
      this.initializeButton.Click += this.button1_Click;
      // 
      // startButton
      // 
      this.startButton.Location = new Point(80, 93);
      this.startButton.Name = "startButton";
      this.startButton.Size = new Size(75, 23);
      this.startButton.TabIndex = 2;
      this.startButton.Text = "실행";
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += this.button2_Click;
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
      // stopButton
      // 
      this.stopButton.Location = new Point(573, 93);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new Size(75, 23);
      this.stopButton.TabIndex = 5;
      this.stopButton.Text = "중지";
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += this.button3_Click;
      // 
      // releaseTopButton
      // 
      this.releaseTopButton.Location = new Point(559, 34);
      this.releaseTopButton.Name = "releaseTopButton";
      this.releaseTopButton.Size = new Size(106, 23);
      this.releaseTopButton.TabIndex = 6;
      this.releaseTopButton.Text = "항상 위 해제";
      this.releaseTopButton.UseVisualStyleBackColor = true;
      this.releaseTopButton.Click += this.button4_Click;
      // 
      // maximumNumericUpDown
      // 
      this.maximumNumericUpDown.Increment = new decimal(new int[] { 10, 0, 0, 0 });
      this.maximumNumericUpDown.Location = new Point(167, 153);
      this.maximumNumericUpDown.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
      this.maximumNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
      this.maximumNumericUpDown.Name = "maximumNumericUpDown";
      this.maximumNumericUpDown.Size = new Size(75, 23);
      this.maximumNumericUpDown.TabIndex = 7;
      this.maximumNumericUpDown.Value = new decimal(new int[] { 30, 0, 0, 0 });
      this.maximumNumericUpDown.ValueChanged += this.numericUpDown1_ValueChanged;
      // 
      // maximumLabel
      // 
      this.maximumLabel.AutoSize = true;
      this.maximumLabel.Location = new Point(50, 155);
      this.maximumLabel.Name = "maximumLabel";
      this.maximumLabel.Size = new Size(111, 15);
      this.maximumLabel.TabIndex = 8;
      this.maximumLabel.Text = "최대 실행횟수 제한";
      // 
      // mainPictureBox
      // 
      this.mainPictureBox.Location = new Point(507, 151);
      this.mainPictureBox.Name = "mainPictureBox";
      this.mainPictureBox.Size = new Size(175, 50);
      this.mainPictureBox.TabIndex = 9;
      this.mainPictureBox.TabStop = false;
      // 
      // includeCheckBox
      // 
      this.includeCheckBox.AutoSize = true;
      this.includeCheckBox.Checked = true;
      this.includeCheckBox.CheckState = CheckState.Checked;
      this.includeCheckBox.Location = new Point(285, 157);
      this.includeCheckBox.Name = "includeCheckBox";
      this.includeCheckBox.Size = new Size(80, 19);
      this.includeCheckBox.TabIndex = 10;
      this.includeCheckBox.Text = "85제 검색";
      this.includeCheckBox.UseVisualStyleBackColor = true;
      // 
      // soundCheckBox
      // 
      this.soundCheckBox.AutoSize = true;
      this.soundCheckBox.Checked = true;
      this.soundCheckBox.CheckState = CheckState.Checked;
      this.soundCheckBox.Location = new Point(285, 182);
      this.soundCheckBox.Name = "soundCheckBox";
      this.soundCheckBox.Size = new Size(62, 19);
      this.soundCheckBox.TabIndex = 11;
      this.soundCheckBox.Text = "사운드";
      this.soundCheckBox.UseVisualStyleBackColor = true;
      // 
      // BiSangRun
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(690, 213);
      Controls.Add(this.soundCheckBox);
      Controls.Add(this.includeCheckBox);
      Controls.Add(this.mainPictureBox);
      Controls.Add(this.maximumLabel);
      Controls.Add(this.maximumNumericUpDown);
      Controls.Add(this.releaseTopButton);
      Controls.Add(this.stopButton);
      Controls.Add(this.label2);
      Controls.Add(this.label1);
      Controls.Add(this.startButton);
      Controls.Add(this.initializeButton);
      Name = "BiSangRun";
      Text = "비이이이상!";
      ((System.ComponentModel.ISupportInitialize)this.maximumNumericUpDown).EndInit();
      ((System.ComponentModel.ISupportInitialize)this.mainPictureBox).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion
    private Button initializeButton;
    private Button startButton;
    private Label label1;
    private Label label2;
    private Button stopButton;
    private Button releaseTopButton;
    private NumericUpDown maximumNumericUpDown;
    private Label maximumLabel;
    private PictureBox mainPictureBox;
    private CheckBox includeCheckBox;
    private CheckBox soundCheckBox;
  }
}
