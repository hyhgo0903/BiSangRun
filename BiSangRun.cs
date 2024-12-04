using BiSangRun.GameData;
using BiSangRun.Utility;
using ImageFinderNS;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;

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
    private readonly IReadOnlyList<ImageGameData> imageGameDataList;
    private bool initialize;
    private CancellationTokenSource token = new();
    private readonly SpeechSynthesizer speechSynthesizer;

    public BiSangRun()
    {
      this.InitializeComponent();
      this.maxTrialCount = this.maximumNumericUpDown.Value;
      this.mainPictureBox.Image = Image.FromFile("Resources/���ȭ��.PNG");

      var imageList = new[]
      {
        new ImageGameData("Resources/����.PNG", "����", 0.8f, false),
        new ImageGameData("Resources/�ź�.PNG", "�ź�", 0.8f, false),
        // �̰� �ϳ� ���� ����� �� ã���ش� �ϴ��� ǥ�� 2���� �غ�
        new ImageGameData("Resources/85�����.PNG", "85�����", 0.94f, true),
        new ImageGameData("Resources/85�����2.PNG", "85�����", 0.94f, true),
      };

      this.imageGameDataList = imageList;

      this.speechSynthesizer = new SpeechSynthesizer();
      speechSynthesizer.SetOutputToDefaultAudioDevice();
      speechSynthesizer.Volume = 100;
      // �ѱ��� ������ Heami �� ����, ��� ��, ������ ���� ���°�찡 �ִ°� ����
      speechSynthesizer.SelectVoice("Microsoft Heami Desktop");
    }

    private void button1_Click(object sender, EventArgs e)
    {
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
      var processRect = Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);

      var checkImage = Image.FromFile("Resources/���ȭ��üũ��.PNG");
      ImageFinder.SetSource(ImageFinder.MakeScreenshot(processRect));
      var finds = ImageFinder.Find(checkImage, 0.8f);
      if (finds.Count < 1)
      {
        this.label1.Text = @"�ʱ�ȭ ����. ��л��� ȭ���� �ƴ�";
        return;
      }

      this.initialize = true;
      this.label1.Text = @"�ʱ�ȭ �Ϸ�. ���ȼ��� â ũ�⸦ �ٲ��� �� �� (�ٽ� �ʱ�ȭ �ʿ�)";
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

        this.SetLabel2TextSafe(@"���� ��... ���ȼ��� â�� �ű�ų� ���콺�������� �� ��!");
        Thread.Sleep(10);
        this.SendMouseClick(Constants.RefreshXSize, Constants.RefreshYSize);

        Thread.Sleep(400);
        this.SendMouseClick(Constants.DetermineXSize, Constants.DetermineYSize);

        Thread.Sleep(600);
        if (this.FindImage())
        {
          return;
        }

        SendMessage(this.processWindow, MouseOperations.Wheel, Constants.WParam, IntPtr.Zero);
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

    private bool FindImage()
    {
      GetWindowRect(this.processWindow, out var rect);
      if (rect.Right - rect.Left != Constants.XWinSize)
      {
        this.SetLabel2TextSafe("â ũ�Ⱑ ����Ǿ���. �����մϴ�", false);
        this.initialize = false;
        return true;
      }

      // ������ ���� �ʿ� ���� �κ� �� ���� ����
      var rectangle = Rectangle.FromLTRB(rect.Left + 400, rect.Top, rect.Right, rect.Bottom);
      ImageFinder.SetSource(ImageFinder.MakeScreenshot(rectangle));

      foreach (var gameData in this.imageGameDataList)
      {
        if (this.includeCheckBox.Checked is false && gameData.CanSkip)
        {
          continue;
        }

        var finds = ImageFinder.Find(gameData.Image, gameData.Similarity);
        if (finds.Count > 0)
        {
          var text = @$"{gameData.Name} �߰�!";
          this.SetLabel2TextSafe(text);

          if (this.soundCheckBox.Checked)
          {
            this.speechSynthesizer.Speak(text);
          }

          return true;
        }
      }

      return false;
    }

    private void button3_Click(object sender, EventArgs e)
    {
      this.token.Cancel();
    }

    private void SetLabel2TextSafe(string txt, bool append = true)
    {
      var resultTxt = append ? $"{txt} {this.trialCount} / {this.maxTrialCount}" : txt;

      if (this.label2.InvokeRequired)
      {
        this.label2.Invoke(new Action(() => this.label2.Text = resultTxt));
      }
      else
      {
        this.label2.Text = resultTxt;
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
      this.maxTrialCount = this.maximumNumericUpDown.Value;
    }

    private void mainPictureBox_Click(object sender, EventArgs e)
    {
      if (this.initialize is false)
      {
        return;
      }

      if (this.FindImage() is false)
      {
        this.SetLabel2TextSafe(@"[Debug] �̹��� �˻� ���� ����", false);
      }
    }
  }
}
