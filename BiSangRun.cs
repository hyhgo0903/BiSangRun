using System.Diagnostics;
using System.Runtime.InteropServices;
using BiSangRun.Utility;
using ImageFinderNS;

namespace BiSangRun
{
  public partial class BiSangRun : Form
  {
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    private int trialCount;
    private decimal maxTrialCount;
    private IntPtr processWindow;
    private Rectangle processRect;
    private IReadOnlyList<Image> images = Array.Empty<Image>();
    private bool initialize;
    private CancellationTokenSource token = new();

    public BiSangRun()
    {
      this.InitializeComponent();
      this.maxTrialCount = this.numericUpDown1.Value;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      var imageList = new[]
      {
        Image.FromFile("Resources/����.PNG"),
        Image.FromFile("Resources/�ź�.PNG")
      };

      this.images = imageList;

      // Pc��ó ������
      var processes = Process.GetProcessesByName("EpicSeven");
      if (processes.Length != 1)
      {
        this.label1.Text = @"���μ��� �˻�����";
        return;
      }

      var process = processes[0];
      this.processWindow = process.MainWindowHandle;

      // Ư�� �ػ󵵷� �����Ŵ
      SetWindowPos(
        this.processWindow,
        Constants.HWndTopmost,
        0,
        0,
        Constants.XWinSize,
        Constants.YWinSize,
        Constants.NoMove);

      GetWindowRect(this.processWindow, out var rect);
      this.processRect = Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);

      var checkImage = Image.FromFile("Resources/���ȭ��üũ��.PNG");
      ImageFinder.SetSource(ImageFinder.MakeScreenshot(this.processRect));
      var finds = ImageFinder.Find(checkImage, 0.8f);
      if (finds.Count < 1)
      {
        this.label1.Text = @"�ʱ�ȭ ����. ��л��� ȭ���� �ƴ�";
        return;
      }

      this.initialize = true;
      this.label1.Text = @"�ʱ�ȭ �Ϸ�. ���ȼ��� âũ�⸦ �ٲ��� �� �� (�ٽ� �ʱ�ȭ �ʿ�)";
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (this.initialize is false)
      {
        this.SetLabel2TextSafe(@"�ʱ�ȭ ���� �ؾ� ��");
        return;
      }

      Task.Run(this.StartWhile);
    }

    private void StartWhile()
    {
      this.token = new CancellationTokenSource();
      this.trialCount = 0;

      while (this.trialCount < this.maxTrialCount)
      {
        ++this.trialCount;

        if (this.token.IsCancellationRequested)
        {
          this.SetLabel2TextSafe(@"���� ��.");
          return;
        }

        this.SetLabel2TextSafe(@"���� ��... ���ȼ��� â�� ���콺�������� �� ��!");
        Thread.Sleep(10);
        this.SendMouseClick(Constants.RefreshXSize, Constants.RefreshYSize);

        Thread.Sleep(400);
        this.SendMouseClick(Constants.DetermineXSize, Constants.DetermineYSize);

        Thread.Sleep(600);
        if (this.FindImage())
        {
          return;
        }

        this.SendMouseWheel();
        Thread.Sleep(400);

        if (this.FindImage())
        {
          return;
        }
      }

      this.SetLabel2TextSafe(@"���� �Ϸ�.");
    }

    private void SendMouseClick(int x, int y)
    {
      // TODO ���콺�� ���� ������ ����Ŭ������ �ߴµ� �׷��� ���� ���� ���� ���� �ʿ�
      IntPtr lParam = ((y << 16) | (x & 0xFFFF));
      SendMessage(this.processWindow, MouseOperations.Move, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftDown, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftUp, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftDown, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftUp, IntPtr.Zero, lParam);
    }

    private void SendMouseWheel()
    {
      int wheelDelta = -360; // �� ������: ���� ��
      IntPtr wParam = ((wheelDelta & 0xFFFF) << 16);
      SendMessage(this.processWindow, MouseOperations.Wheel, wParam, IntPtr.Zero);
    }

    private bool FindImage()
    {
      ImageFinder.SetSource(ImageFinder.MakeScreenshot(this.processRect));

      if (this.images
        .Select(image => ImageFinder.Find(image, 0.8f))
        .Any(finds => finds.Count > 0))
      {
        this.SetLabel2TextSafe(@"�߰�!");
        return true;
      }

      return false;
    }

    private void button3_Click(object sender, EventArgs e)
    {
      this.token.Cancel();
    }

    private void SetLabel2TextSafe(string txt)
    {
      var appendedTxt = $"{txt} {this.trialCount} / {this.maxTrialCount}";
      if (this.label2.InvokeRequired)
      {
        this.label2.Invoke(new Action(() => this.label2.Text = appendedTxt));
      }
      else
      {
        this.label2.Text = appendedTxt;
      }
    }

    private void button4_Click(object sender, EventArgs e)
    {
      if (this.initialize is false)
      {
        return;
      }

      SetWindowPos(
        this.processWindow,
        Constants.HWndNoTopmost,
        0,
        0,
        0,
        0,
        Constants.NoMove | Constants.NoSize);
    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {
      this.maxTrialCount = this.numericUpDown1.Value;
    }
  }
}
