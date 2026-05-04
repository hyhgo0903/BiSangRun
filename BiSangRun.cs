using BiSangRun.GameData;
using BiSangRun.Utility;
using ImageFinderNS;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;

namespace BiSangRun;

public partial class BiSangRun : Form
{
  [DllImport("user32.dll")]
  private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

  [DllImport("user32.dll")]
  private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

  [DllImport("user32.dll")]
  private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

  private int trialCount;
  private int covenantPurchaseCount;
  private int mysticPurchaseCount;
  private decimal maxTrialCount;
  private IntPtr processWindow;
  private readonly IReadOnlyList<ImageGameData> imageGameDataList;
  private bool initialize;
  private bool isRunning;
  private CancellationTokenSource token = new();
  private readonly SpeechSynthesizer speechSynthesizer;

  public BiSangRun()
  {
    this.InitializeComponent();
    this.maxTrialCount = this.maximumNumericUpDown.Value;
    this.mainPictureBox.Image = Image.FromFile("Resources/대기화면.PNG");

    this.imageGameDataList =
    [
      new ImageGameData("Resources/성약.PNG", "성약", 0.8f, false, ShopItemType.CovenantBookmark),
      new ImageGameData("Resources/신비.PNG", "신비", 0.8f, false, ShopItemType.MysticMedal),
      new ImageGameData("Resources/85제장비.PNG", "85제장비", 0.94f, true, ShopItemType.Equipment85),
      new ImageGameData("Resources/85제장비2.PNG", "85제장비", 0.94f, true, ShopItemType.Equipment85),
    ];

    this.speechSynthesizer = new SpeechSynthesizer();
    this.speechSynthesizer.SetOutputToDefaultAudioDevice();
    this.speechSynthesizer.Volume = 100;
    this.speechSynthesizer.SelectVoice("Microsoft Heami Desktop");
  }

  private void initializeButton_Click(object sender, EventArgs e)
  {
    var processes = Process.GetProcessesByName("EpicSeven");
    if (processes.Length != 1)
    {
      this.label1.Text = @"프로세스 검색실패";
      return;
    }

    var process = processes[0];
    this.processWindow = process.MainWindowHandle;

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
    this.label1.Text = @"초기화 완료. 비밀상점 창 크기를 바꾸면 안 됨 (다시 초기화 필요)";
  }

  private void startButton_Click(object sender, EventArgs e)
  {
    if (this.initialize is false)
    {
      this.SetLabel2TextSafe(@"초기화 먼저 해야 함");
      return;
    }

    if (this.isRunning)
    {
      this.SetLabel2TextSafe(@"이미 실행 중", false);
      return;
    }

    var settings = this.CreateSearchSettings();
    Task.Run(() => this.StartWhile(settings));
  }

  private void StartWhile(SearchSettings settings)
  {
    this.SetRunningStateSafe(true);
    this.token = new CancellationTokenSource();
    this.trialCount = 0;
    this.covenantPurchaseCount = 0;
    this.mysticPurchaseCount = 0;

    try
    {
      while (this.trialCount < settings.MaxTrialCount)
      {
        ++this.trialCount;

        if (this.token.IsCancellationRequested)
        {
          this.SetLabel2TextSafe(@"중지 됨.");
          return;
        }

        this.SetLabel2TextSafe(@"새로고침 중... 비밀상점 창 마우스조작하면 안 됨!");
        Thread.Sleep(10);
        this.SendMouseClick(Constants.RefreshXSize, Constants.RefreshYSize, doubleClick: true);

        Thread.Sleep(400);
        this.SendMouseClick(Constants.DetermineXSize, Constants.DetermineYSize, doubleClick: true);

        Thread.Sleep(Constants.ShopRefreshSettleDelayMs);
        if (this.ProcessVisibleItems(settings))
        {
          return;
        }

        SendMessage(this.processWindow, MouseOperations.Wheel, Constants.WParam, IntPtr.Zero);
        Thread.Sleep(Constants.ShopScrollSettleDelayMs);

        if (this.ProcessVisibleItems(settings))
        {
          return;
        }
      }

      this.SetLabel2TextSafe(@"탐색 완료.");
    }
    finally
    {
      this.SetRunningStateSafe(false);
    }
  }

  private bool ProcessVisibleItems(SearchSettings settings)
  {
    for (var scanCount = 0; scanCount < Constants.MaxPurchaseScanCount; ++scanCount)
    {
      var foundItems = this.FindItems(settings);
      if (foundItems.ScanStopped)
      {
        return true;
      }

      if (foundItems.Items.Count == 0)
      {
        return false;
      }

      if (settings.AutoPurchase is false)
      {
        this.NotifyFound(foundItems.Items[0], settings);
        return true;
      }

      var equipment85 = foundItems.Items.FirstOrDefault(item => item.GameData.ItemType == ShopItemType.Equipment85);
      if (equipment85 is not null)
      {
        this.NotifyFound(equipment85, settings);
        return true;
      }

      var purchaseTargets = foundItems.Items
        .Where(item => this.ShouldPurchase(item.GameData, settings))
        .OrderBy(item => item.BuyY)
        .ToList();

      if (purchaseTargets.Count == 0)
      {
        return false;
      }

      var target = purchaseTargets[0];
      if (this.token.IsCancellationRequested)
      {
        return true;
      }

      this.PurchaseAndVerify(target, settings);
    }

    this.SetLabel2TextSafe(@"구매 후 재탐색 제한 도달. 안전을 위해 중지", false);
    return true;
  }

  private bool PurchaseAndVerify(FoundShopItem target, SearchSettings settings)
  {
    this.SetLabel2TextSafe($"{target.GameData.Name} 구매 시도");
    this.SendMouseClick(target.BuyX, target.BuyY, doubleClick: true);
    Thread.Sleep(Constants.PurchasePopupDelayMs);
    this.SendMouseClick(Constants.PurchaseConfirmX, Constants.PurchaseConfirmY, doubleClick: true);
    Thread.Sleep(Constants.PurchaseCompleteDelayMs);

    var verification = this.FindItems(settings);
    if (verification.ScanStopped)
    {
      return false;
    }

    var stillVisible = verification.Items.Any(item => this.IsSameVisibleTarget(item, target));
    if (stillVisible)
    {
      this.SetLabel2TextSafe($"{target.GameData.Name} 구매 재확인 필요");
      return false;
    }

    this.AddPurchaseCount(target.GameData.ItemType);
    return true;
  }

  private bool IsSameVisibleTarget(FoundShopItem item, FoundShopItem target)
  {
    return item.GameData.ItemType == target.GameData.ItemType
      && Math.Abs(item.BuyY - target.BuyY) <= Constants.MatchDeduplicateYThreshold;
  }

  private void SendMouseClick(int x, int y, bool doubleClick = false)
  {
    IntPtr lParam = ((y << 16) | (x & 0xFFFF));
    SendMessage(this.processWindow, MouseOperations.Move, IntPtr.Zero, lParam);
    SendMessage(this.processWindow, MouseOperations.LeftDown, IntPtr.Zero, lParam);
    SendMessage(this.processWindow, MouseOperations.LeftUp, IntPtr.Zero, lParam);

    if (doubleClick)
    {
      SendMessage(this.processWindow, MouseOperations.LeftDown, IntPtr.Zero, lParam);
      SendMessage(this.processWindow, MouseOperations.LeftUp, IntPtr.Zero, lParam);
    }
  }

  private SearchResult FindItems(SearchSettings settings, bool debug = false)
  {
    GetWindowRect(this.processWindow, out var rect);
    if ((rect.Right - rect.Left != Constants.XWinSize) || (rect.Bottom - rect.Top != Constants.YWinSize))
    {
      this.SetLabel2TextSafe("창 크기가 변경되었음. 중지합니다", false);
      this.initialize = false;
      return SearchResult.Stopped;
    }

    var rectangle = Rectangle.FromLTRB(
      rect.Left + Constants.ShopSearchLeft,
      rect.Top + Constants.ShopSearchTop,
      rect.Right,
      rect.Bottom);

    using var screenshot = ImageFinder.MakeScreenshot(rectangle);
    if (debug)
    {
      screenshot.Save("Resources/디버그용.BMP", ImageFormat.Bmp);
    }

    ImageFinder.SetSource(screenshot);
    var items = new List<FoundShopItem>();

    foreach (var gameData in this.imageGameDataList)
    {
      if (this.ShouldSearch(gameData, settings) is false)
      {
        continue;
      }

      var finds = ImageFinder.Find(gameData.Image, gameData.Similarity);
      foreach (var find in finds)
      {
        var centerY = Constants.ShopSearchTop + find.Zone.Top + (find.Zone.Height / 2);
        var item = new FoundShopItem(
          gameData,
          find.Zone,
          find.Similarity,
          Constants.ShopBuyButtonCenterX,
          centerY);

        this.AddDeduplicated(items, item);
      }
    }

    return new SearchResult(false, items.OrderBy(item => item.BuyY).ToList());
  }

  private void AddDeduplicated(List<FoundShopItem> items, FoundShopItem item)
  {
    var sameItemIndex = items.FindIndex(existing =>
      existing.GameData.ItemType == item.GameData.ItemType
      && Math.Abs(existing.BuyY - item.BuyY) <= Constants.MatchDeduplicateYThreshold);

    if (sameItemIndex < 0)
    {
      items.Add(item);
      return;
    }

    if (item.Similarity > items[sameItemIndex].Similarity)
    {
      items[sameItemIndex] = item;
    }
  }

  private bool ShouldSearch(ImageGameData gameData, SearchSettings settings)
  {
    return gameData.ItemType != ShopItemType.Equipment85 || settings.IncludeEquipment85;
  }

  private bool ShouldPurchase(ImageGameData gameData, SearchSettings settings)
  {
    return gameData.ItemType switch
    {
      ShopItemType.CovenantBookmark => true,
      ShopItemType.MysticMedal => true,
      ShopItemType.Equipment85 => false,
      _ => false,
    };
  }

  private void NotifyFound(FoundShopItem item, SearchSettings settings)
  {
    var text = @$"{item.GameData.Name} 발견!";
    this.SetLabel2TextSafe(text);

    if (settings.Sound)
    {
      this.speechSynthesizer.Speak(text);
    }
  }

  private void stopButton_Click(object sender, EventArgs e)
  {
    this.token.Cancel();
  }

  private void AddPurchaseCount(ShopItemType itemType)
  {
    switch (itemType)
    {
      case ShopItemType.CovenantBookmark:
        ++this.covenantPurchaseCount;
        break;
      case ShopItemType.MysticMedal:
        ++this.mysticPurchaseCount;
        break;
    }
  }

  private void SetLabel2TextSafe(string txt, bool append = true)
  {
    var purchaseCountText = $"신비 {this.mysticPurchaseCount}회 / 성약 {this.covenantPurchaseCount}회";
    var resultTxt = append
      ? $"{txt} {this.trialCount} / {this.maxTrialCount}{Environment.NewLine}{purchaseCountText}"
      : txt;

    if (this.label2.InvokeRequired)
    {
      this.label2.Invoke(new Action(() => this.label2.Text = resultTxt));
    }
    else
    {
      this.label2.Text = resultTxt;
    }
  }

  private void SetRunningStateSafe(bool running)
  {
    if (this.startButton.InvokeRequired)
    {
      this.startButton.Invoke(new Action(() => this.SetRunningStateSafe(running)));
      return;
    }

    this.isRunning = running;
    this.startButton.Enabled = running is false;
  }

  private SearchSettings CreateSearchSettings()
  {
    return new SearchSettings(
      this.maximumNumericUpDown.Value,
      this.includeCheckBox.Checked,
      this.soundCheckBox.Checked,
      this.autoPurchaseCheckBox.Checked);
  }

  private void releaseTopButton_Click(object sender, EventArgs e)
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

  private void maximumNumericUpDown_ValueChanged(object sender, EventArgs e)
  {
    this.maxTrialCount = this.maximumNumericUpDown.Value;
  }

  private void mainPictureBox_Click(object sender, EventArgs e)
  {
    if (this.initialize is false)
    {
      return;
    }

    var result = this.FindItems(this.CreateSearchSettings(), true);
    if (result.Items.Count == 0)
    {
      this.SetLabel2TextSafe(@"[Debug] 이미지 검색 결과 없음", false);
      return;
    }

    var text = string.Join(", ", result.Items.Select(item =>
      $"{item.GameData.Name}(Y:{item.BuyY}, {item.Similarity:0.00})"));
    this.SetLabel2TextSafe($"[Debug] {text}", false);
  }

  private sealed record SearchSettings(
    decimal MaxTrialCount,
    bool IncludeEquipment85,
    bool Sound,
    bool AutoPurchase);

  private sealed record FoundShopItem(
    ImageGameData GameData,
    Rectangle MatchZone,
    float Similarity,
    int BuyX,
    int BuyY);

  private sealed record SearchResult(bool ScanStopped, IReadOnlyList<FoundShopItem> Items)
  {
    public static SearchResult Stopped { get; } = new(true, []);
  }
}
