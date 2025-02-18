namespace BiSangRun.GameData;

internal class ImageGameData(string path, string name, float similarity, bool canSkip)
{
  public Image Image { get; } = Image.FromFile(path);
  public string Name { get; } = name;
  public float Similarity { get; } = similarity;
  public bool CanSkip { get; } = canSkip;
}