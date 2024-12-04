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
      this.mainPictureBox.Image = Image.FromFile("Resources/대기화면.PNG");

      var imageList = new[]
      {
        new ImageGameData("Resources/성약.PNG", "성약", 0.8f, false),
        new ImageGameData("Resources/신비.PNG", "신비", 0.8f, false),
        // 이거 하나 갖고 제대로 안 찾아준다 일단은 표본 2개로 해봄
        new ImageGameData("Resources/85제장비.PNG", "85제장비", 0.94f, true),
        new ImageGameData("Resources/85제장비2.PNG", "85제장비", 0.94f, true),
      };

      this.imageGameDataList = imageList;

      this.speechSynthesizer = new SpeechSynthesizer();
      speechSynthesizer.SetOutputToDefaultAudioDevice();
      speechSynthesizer.Volume = 100;
      // 한국어 지원은 Heami 만 가능, 없어도 됨, 있으면 오류 나는경우가 있는거 같음
      speechSynthesizer.SelectVoice("Microsoft Heami Desktop");
    }

    private void button1_Click(object sender, EventArgs e)
    {
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
      var processRect = Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);

      var checkImage = Image.FromFile("Resources/비상화면체크용.PNG");
      ImageFinder.SetSource(ImageFinder.MakeScreenshot(processRect));
      var finds = ImageFinder.Find(checkImage, 0.8f);
      if (finds.Count < 1)
      {
        this.label1.Text = @"초기화 실패. 비밀상점 화면이 아님";
        return;
      }

      this.initialize = true;
      this.label1.Text = @"초기화 완료. 에픽세븐 창 크기를 바꾸지 말 것 (다시 초기화 필요)";
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

        this.SetLabel2TextSafe(@"실행 중... 에픽세븐 창을 옮기거나 마우스오버하지 말 것!");
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

    private bool FindImage()
    {
      GetWindowRect(this.processWindow, out var rect);
      if (rect.Right - rect.Left != Constants.XWinSize)
      {
        this.SetLabel2TextSafe("창 크기가 변경되었음. 중지합니다", false);
        this.initialize = false;
        return true;
      }

      // 성능을 위해 필요 없는 부분 더 줄일 예정
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
          var text = @$"{gameData.Name} 발견!";
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
        this.SetLabel2TextSafe(@"[Debug] 이미지 검색 되지 않음", false);
      }
    }
  }
}
