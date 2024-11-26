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

    // �׻� ����
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
      SetWindowPos(this.processWindow, HWndTopmost, 0, 0, Constants.XWinSize, Constants.YWinSize, NoMove);
      GetWindowRect(this.processWindow, out var rect);
      this.processPoint = new Point(rect.Left, rect.Top);
      this.initialize = true;
      this.label1.Text = @"�ʱ�ȭ �Ϸ�. ���μ��� ũ�⳪ ��ġ�� �ű��� �� �� (�ٽ� �ʱ�ȭ �� ��)";

      // TODO ��� ȭ�� �ƴϸ� �ʱ�ȭ �����ϱ� (�����Ƽ� �ϴ� ����)
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (initialize is false)
      {
        this.SetLabel2TextSafe(@"�ʱ�ȭ ���� �ؾ� ��");
        return;
      }

      this.SetLabel2TextSafe(@"���� ��... ���ȼ��� â�� ���콺�������� �� ��!");
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
          this.SetLabel2TextSafe(@"���� ��");
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

      this.SetLabel2TextSafe(@"���� �Ϸ�");
    }

    private void SendMouseClick(int x, int y)
    {
      // TODO ���콺�� ���� ������ ����Ŭ������ �ߴµ� �׷��� ���� ���� ���� ���� �ʿ�
      IntPtr lParam = (IntPtr)((y << 16) | (x & 0xFFFF));
      SendMessage(this.processWindow, MouseOperations.Move, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftDown, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftUp, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftDown, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftUp, IntPtr.Zero, lParam);
    }

    private void SendMouseWheel()
    {
      int wheelDelta = -360; // �� ������: ���� ��
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
