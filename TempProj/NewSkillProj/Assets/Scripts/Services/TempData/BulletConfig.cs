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
            id = 100,
            assetPath = "Bullet/Prefab/missile_dandao_02_fire",
            maxTime = 10.0f,
            timeLineConfig = "Bullet/Data/bullet_100",
            maxSpeed = 5,
        });
    }
}
public class BulletConfigData : BaseConfigData
{
    public string assetPath;
    public float maxTime;
    public string timeLineConfig;
    public float maxSpeed;
}