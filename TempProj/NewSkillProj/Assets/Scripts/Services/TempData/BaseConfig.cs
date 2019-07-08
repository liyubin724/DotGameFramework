using System.Collections.Generic;

public class BaseConfigData
{
    public int id;
}

public class BaseConfig<T> where T:BaseConfigData
{
    protected List<T> datas = new List<T>();
    public T GetData(int id)
    {
        foreach(var d in datas)
        {
            if(d.id == id)
            {
                return d;
            }
        }
        return null;
    }

    public void AddData(T data)
    {
        datas.Add(data);
    }
}