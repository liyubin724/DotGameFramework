using System.Text.RegularExpressions;

namespace Dot.Core.Util
{
    public static class StringUtil
    {
        public static string RemoveSpecialCharacter(string input,string replacement)
        {
            return Regex.Replace(input, "[ \\[ \\] \\^ \\-_*×――(^)|'$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]", replacement);
        }

        public unsafe static uint GetHashByTime33(string path)
        {
            uint hash = 5381;
            fixed (char* str = path)
            {
                int index = 0;
                while (*(str + index) != 0)
                {
                    hash += (hash << 5) + *(str + index);
                    index++;
                }
            }
            return (hash & 0x7FFFFFFF);
        }
    }
}
