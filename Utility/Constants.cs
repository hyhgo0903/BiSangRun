namespace BiSangRun.Utility;

public class Constants
{
  // 이 기준으로 좌표를 설정했음
  public const int XWinSize = 1024;
  public const int YWinSize = 606;

  public const int RefreshXSize = 200;
  public const int RefreshYSize = 520;

  public const int DetermineXSize = 600;
  public const int DetermineYSize = 350;

  public const int ShopSearchLeft = 425;
  public const int ShopSearchTop = 75;
  public const int ShopBuyButtonRightPadding = 94;
  public static int ShopBuyButtonCenterX => XWinSize - ShopBuyButtonRightPadding;
  public const int PurchaseConfirmX = 645;
  public const int PurchaseConfirmY = 422;
  public const int PurchasePopupDelayMs = 800;
  public const int PurchaseCompleteDelayMs = 3000;
  public const int ShopRefreshSettleDelayMs = 1000;
  public const int ShopScrollSettleDelayMs = 1000;
  public const int MatchDeduplicateYThreshold = 18;
  public const int MaxPurchaseScanCount = 10;

  public const IntPtr WParam = (-400 & 0xFFFF) << 16;

  // 항상 위로
  public const IntPtr HWndTopmost = -1;
  public const IntPtr HWndNoTopmost = -2;
  public const uint NoSize = 0x0001;
  public const uint NoMove = 0x0002;
}

public class MouseOperations
{
  public const int Move = 0x0200;
  public const int LeftDown = 0x0201;
  public const int LeftUp = 0x0202;
  public const int Wheel = 0x020A;
}
