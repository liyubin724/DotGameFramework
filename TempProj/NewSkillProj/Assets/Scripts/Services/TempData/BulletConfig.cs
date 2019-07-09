using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BulletConfig : BaseConfig<BulletConfigData>
{
    public BulletConfig()
    {
        AddData(new BulletConfigData()
        {
            id = 1,
            assetPath = "Bullet/bullet_1",
            maxTime = 10.0f,
            timeLineConfig = "Bullet/bullet_1001",
        });
        AddData(new BulletConfigData()
        {
            id = 2,
            assetPath = "Bullet/bullet_1",
            maxTime = 10.0f,
            timeLineConfig = "Bullet/bullet_1002",
        });
    }
}
public class BulletConfigData : BaseConfigData
{
    public string assetPath;
    public float maxTime;
    public string timeLineConfig;
}