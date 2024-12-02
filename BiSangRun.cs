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
        Image.FromFile("Resources/성약.PNG"),
        Image.FromFile("Resources/신비.PNG")
      };

      this.images = imageList;

      // Pc런처 기준임
      var processes = Process.GetProcessesByName("EpicSeven");
      if (processes.Length != 1)
      {
        this.label1.Text = @"프로세스 검색실패";
        return;
      }

      var process = processes[0];
      this.processWindow = process.MainWindowHandle;

      // 특정 해상도로 변경시킴
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

      var checkImage = Image.FromFile("Resources/비상화면체크용.PNG");
      ImageFinder.SetSource(ImageFinder.MakeScreenshot(this.processRect));
      var finds = ImageFinder.Find(checkImage, 0.8f);
      if (finds.Count < 1)
      {
        this.label1.Text = @"초기화 실패. 비밀상점 화면이 아님";
        return;
      }

      this.initialize = true;
      this.label1.Text = @"초기화 완료. 에픽세븐 창크기를 바꾸지 말 것 (다시 초기화 필요)";
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (this.initialize is false)
      {
        this.SetLabel2TextSafe(@"초기화 부터 해야 함");
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
          this.SetLabel2TextSafe(@"중지 됨.");
          return;
        }

        this.SetLabel2TextSafe(@"실행 중... 에픽세븐 창에 마우스오버하지 말 것!");
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

      this.SetLabel2TextSafe(@"실행 완료.");
    }

    private void SendMouseClick(int x, int y)
    {
      // TODO 마우스가 가끔 씹혀서 더블클릭으로 했는데 그래도 씹힐 때가 있음 개선 필요
      IntPtr lParam = ((y << 16) | (x & 0xFFFF));
      SendMessage(this.processWindow, MouseOperations.Move, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftDown, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftUp, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftDown, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftUp, IntPtr.Zero, lParam);
    }

    private void SendMouseWheel()
    {
      int wheelDelta = -360; // 휠 내리기: 음수 값
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
        this.SetLabel2TextSafe(@"발견!");
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
