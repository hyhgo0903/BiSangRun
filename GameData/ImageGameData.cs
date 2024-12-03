namespace BiSangRun.GameData
{
  internal class ImageGameData
  {
    public ImageGameData(string path, string name, float similarity, bool canSkip)
    {
      this.Image = Image.FromFile(path);
      this.Name = name;
      this.Similarity = similarity;
      this.CanSkip = canSkip;
    }

    public Image Image { get; }
    public string Name { get; }
    public float Similarity { get; }
    public bool CanSkip { get; }
  }
}
