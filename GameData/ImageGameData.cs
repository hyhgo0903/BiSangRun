namespace BiSangRun.GameData;

internal enum ShopItemType
{
  CovenantBookmark,
  MysticMedal,
  Equipment85,
}

internal class ImageGameData(string path, string name, float similarity, bool canSkip, ShopItemType itemType)
{
  public Image Image { get; } = Image.FromFile(path);
  public string Name { get; } = name;
  public float Similarity { get; } = similarity;
  public bool CanSkip { get; } = canSkip;
  public ShopItemType ItemType { get; } = itemType;
}
