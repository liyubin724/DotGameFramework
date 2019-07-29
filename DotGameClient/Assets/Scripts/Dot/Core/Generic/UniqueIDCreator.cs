namespace Dot.Core.Generic
{
    public class UniqueIDCreator
    {
        private long id = 0;

        public long Next()
        {
            return ++id;
        }
    }
}
