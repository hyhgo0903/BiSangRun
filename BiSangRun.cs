using System.Diagnostics;
using System.Runtime.InteropServices;
using WinFormsApp1.Utility;

namespace WinFormsApp1
{
  public partial class BiSangRun : Form
  {
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern IntPtr WindowFromPoint(Point point);

    // 항상 위로
    private const IntPtr HWndTopmost = -1;
    private const uint NoMove = 0x0002;

    private IntPtr processWindow;
    private Point processPoint = new(0, 0);
    private IReadOnlyList<Image> images = Array.Empty<Image>();
    private bool initialize;
    private CancellationTokenSource token = new();

    public BiSangRun()
    {
      this.InitializeComponent();
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
      SetWindowPos(this.processWindow, HWndTopmost, 0, 0, Constants.XWinSize, Constants.YWinSize, NoMove);
      GetWindowRect(this.processWindow, out var rect);
      this.processPoint = new Point(rect.Left, rect.Top);
      this.initialize = true;
      this.label1.Text = @"초기화 완료. 프로세스 크기나 위치를 옮기지 말 것 (다시 초기화 할 것)";

      // TODO 비상런 화면 아니면 초기화 실패하기 (귀찮아서 일단 뒀음)
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (initialize is false)
      {
        this.SetLabel2TextSafe(@"초기화 부터 해야 함");
        return;
      }

      this.SetLabel2TextSafe(@"실행 중... 에픽세븐 창에 마우스오버하지 말 것!");
      Task.Run(StartWhile);
    }

    private void StartWhile()
    {
      this.token = new();
      var count = 0;

      while (count < Constants.MaxTrial)
      {
        if (this.token.IsCancellationRequested)
        {
          this.SetLabel2TextSafe(@"중지 됨");
          return;
        }

        Thread.Sleep(10);
        SendMouseClick(Constants.RefreshXSize, Constants.RefreshYSize);

        Thread.Sleep(400);
        SendMouseClick(Constants.DetermineXSize, Constants.DetermineYSize);

        Thread.Sleep(600);
        if (FindImage()) return;

        SendMouseWheel();
        Thread.Sleep(400);

        if (FindImage()) return;

        ++count;
      }

      this.SetLabel2TextSafe(@"실행 완료");
    }

    private void SendMouseClick(int x, int y)
    {
      // TODO 마우스가 가끔 씹혀서 더블클릭으로 했는데 그래도 씹힐 때가 있음 개선 필요
      IntPtr lParam = (IntPtr)((y << 16) | (x & 0xFFFF));
      SendMessage(this.processWindow, MouseOperations.Move, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftDown, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftUp, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftDown, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftUp, IntPtr.Zero, lParam);
    }

    private void SendMouseWheel()
    {
      int wheelDelta = -360; // 휠 내리기: 음수 값
      IntPtr wParam = (IntPtr)((wheelDelta & 0xFFFF) << 16);
      SendMessage(this.processWindow, MouseOperations.Wheel, wParam, IntPtr.Zero);
    }

    private bool FindImage()
    {
      ImageFinderNS.ImageFinder.SetSource(
      ImageFinderNS.ImageFinder.MakeScreenshot(new Rectangle(
        processPoint.X,
        processPoint.Y,
        Constants.XWinSize,
        Constants.YWinSize)));

      if (images
        .Select(image => ImageFinderNS.ImageFinder.Find(image, 0.8f))
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
      if (this.label2.InvokeRequired)
      {
        this.label2.Invoke(new Action(() => this.label2.Text = txt));
      }
      else
      {
        this.label2.Text = txt;
      }
    }
  }
}
