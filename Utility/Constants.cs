namespace WinFormsApp1.Utility
{
  public class Constants
  {
    // 이 기준으로 좌표를 설정했음
    public const int XWinSize = 1024;
    public const int YWinSize = 606;

    public const int RefreshXSize = 200;
    public const int RefreshYSize = 520;

    public const int DetermineXSize = 600;
    public const int DetermineYSize = 350;

    // 테스트를 완벽하게 할 때까진 적게..
    public const int MaxTrial = 10;
  }

  public class MouseOperations
  {
    public const int Move = 0x0200;
    public const int LeftDown = 0x0201;
    public const int LeftUp = 0x0202;
    public const int Wheel = 0x020A;
  }
}
