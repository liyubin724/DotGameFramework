public class SoundConfig : BaseConfig<SoundConfigData>
{
    public SoundConfig()
    {
        AddData(new SoundConfigData()
        {
            id = 1,
            assetPath = "AudioClip/G_001_L",
            soundType = SoundType.Camera,
        });
        AddData(new SoundConfigData()
        {
            id = 2,
            assetPath = "AudioClip/G_011_L",
            soundType = SoundType.Camera,
        });
        AddData(new SoundConfigData()
        {
            id = 3,
            assetPath = "AudioClip/G_018_L",
            soundType = SoundType.Camera,
        });
        AddData(new SoundConfigData()
        {
            id = 4,
            assetPath = "AudioClip/G_021_L",
            soundType = SoundType.Camera,
        });
        AddData(new SoundConfigData()
        {
            id = 5,
            assetPath = "AudioClip/G_028_L",
            soundType = SoundType.Camera,
        });
        AddData(new SoundConfigData()
        {
            id = 6,
            assetPath = "AudioClip/G_029_L",
            soundType = SoundType.Camera,
        });
    }
}

public class SoundConfigData : BaseConfigData
{
    public string assetPath;
    public SoundType soundType;
}