namespace WinFormsApp1
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
      if (disposing && (components != null))
      {
        components.Dispose();
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
	  button1 = new Button();
	  button2 = new Button();
	  label1 = new Label();
	  label2 = new Label();
	  button3 = new Button();
	  SuspendLayout();
	  // 
	  // button1
	  // 
	  button1.Location = new Point(80, 34);
	  button1.Name = "button1";
	  button1.Size = new Size(75, 23);
	  button1.TabIndex = 1;
	  button1.Text = "초기화";
	  button1.UseVisualStyleBackColor = true;
	  button1.Click += button1_Click;
	  // 
	  // button2
	  // 
	  button2.Location = new Point(80, 93);
	  button2.Name = "button2";
	  button2.Size = new Size(75, 23);
	  button2.TabIndex = 2;
	  button2.Text = "실행";
	  button2.UseVisualStyleBackColor = true;
	  button2.Click += button2_Click;
	  // 
	  // label1
	  // 
	  label1.AutoSize = true;
	  label1.Location = new Point(161, 38);
	  label1.Name = "label1";
	  label1.Size = new Size(0, 15);
	  label1.TabIndex = 3;
	  // 
	  // label2
	  // 
	  label2.AutoSize = true;
	  label2.Location = new Point(161, 97);
	  label2.Name = "label2";
	  label2.Size = new Size(0, 15);
	  label2.TabIndex = 4;
	  // 
	  // button3
	  // 
	  button3.Location = new Point(511, 93);
	  button3.Name = "button3";
	  button3.Size = new Size(75, 23);
	  button3.TabIndex = 5;
	  button3.Text = "중지";
	  button3.UseVisualStyleBackColor = true;
	  button3.Click += button3_Click;
	  // 
	  // Form1
	  // 
	  AutoScaleDimensions = new SizeF(7F, 15F);
	  AutoScaleMode = AutoScaleMode.Font;
	  ClientSize = new Size(685, 149);
	  Controls.Add(button3);
	  Controls.Add(label2);
	  Controls.Add(label1);
	  Controls.Add(button2);
	  Controls.Add(button1);
	  Name = "Form1";
	  Text = "비이이이상!";
	  ResumeLayout(false);
	  PerformLayout();
	}

	#endregion
	private Button button1;
    private Button button2;
    private Label label1;
    private Label label2;
	private Button button3;
  }
}
