public class UIType
{
    public string assetName { get; private set; }
    public string bundleName { get; private set; }

    public UIType(string asset_Name, string AB_Name)
    {
        assetName = asset_Name;
        bundleName = AB_Name;
    }
}
