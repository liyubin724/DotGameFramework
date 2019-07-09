using System;
using System.Collections.Generic;
using System.IO;

namespace Dot.Core.IO
{
    internal class FileFragmentData
    {
        public int start;
        public int size;
    }
    
    internal class FileFragment
    {
        private List<FileFragmentData> sortedSizeList = new List<FileFragmentData>();
        private List<FileFragmentData> sortedStartList = new List<FileFragmentData>();

        internal FileFragment()
        {

        }

        internal unsafe void ReadFragment(string fragmentPath)
        {
            if (File.Exists(fragmentPath))
            {
                int fragmentSize = sizeof(uint) * 2;
                byte[] buffer = new byte[fragmentSize];
                FileStream fragmentFS = null;

                try
                {
                    fragmentFS = new FileStream(fragmentPath, FileMode.Open, FileAccess.Read);
                    while (true)
                    {
                        int size = fragmentFS.Read(buffer, 0, fragmentSize);
                        if (size <= 0)
                        {
                            break;
                        }
                        else if (size != fragmentSize)
                        {
                            //TODO:error
                            break;
                        }

                        FileFragmentData fragmentData = new FileFragmentData();
                        int byteOffset = 0;
                        fixed (byte* b = &buffer[byteOffset])
                        {
                            fragmentData.start = *((int*)b);
                        }
                        byteOffset += sizeof(int);
                        fixed (byte* b = &buffer[byteOffset])
                        {
                            fragmentData.size = *((int*)b);
                        }
                        sortedSizeList.Add(fragmentData);
                        sortedStartList.Add(fragmentData);
                    }
                    sortedStartList.Sort((x, y) =>
                    {
                        if(x.start>y.start)
                        {
                            return 1;
                        }else if(x.start<y.start)
                        {
                            return -1;
                        }else
                        {
                            return 0;
                        }
                    });
                }
                catch (Exception e)
                {
                    //TODO:error
                }
                finally
                {
                    if (fragmentFS != null)
                    {
                        fragmentFS.Close();
                        fragmentFS = null;
                    }
                }
            }
        }

        internal void AddFragment(FileIndex indexData)
        {
            int insertIndex = -1;
            for (int i = 0; i < sortedStartList.Count; i++)
            {
                if (sortedStartList[i].start > indexData.start)
                {
                    insertIndex = i;
                    break;
                }
            }
            if (insertIndex < 0)
            {
                insertIndex = sortedStartList.Count;
            }
            FileFragmentData preFragment = insertIndex == 0 ? null : sortedStartList[insertIndex - 1];
            FileFragmentData nextFragment = insertIndex == sortedStartList.Count ? null : sortedStartList[insertIndex];

            bool isConcatPre = false;
            if (preFragment!=null)
            {
                if (preFragment.start + preFragment.size == indexData.start)
                {
                    isConcatPre = true;
                }
            }
            bool isConcatNext = false;
            if (nextFragment != null)
            {
                if (nextFragment.start == indexData.start + indexData.size)
                {
                    isConcatNext = true;
                }
            }

            if (isConcatPre && isConcatNext)
            {
                preFragment.size += indexData.size + nextFragment.size;
                sortedStartList.Remove(nextFragment);
                sortedSizeList.Remove(nextFragment);
            }
            else if (isConcatPre)
            {
                preFragment.size += indexData.size;
            }
            else if (isConcatNext)
            {
                nextFragment.start = indexData.start;
                nextFragment.size += indexData.size;
            }
            else
            {
                FileFragmentData fragmentData = new FileFragmentData() { start = indexData.start, size = indexData.size };
                sortedStartList.Add(fragmentData);
                sortedSizeList.Add(fragmentData);
            }

            sortedSizeList.Sort((x, y) =>
            {
                if(x.size>y.size)
                {
                    return 1;
                }else if(x.size<y.size)
                {
                    return -1;
                }else
                {
                    return x.start.CompareTo(y.start);
                }
            });

            sortedStartList.Sort((x, y) =>
            {
                if (x.start > y.start)
                {
                    return 1;
                }
                else if (x.start < y.start)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
        }

        internal void DeleteFragment(FileFragmentData data)
        {
            sortedSizeList.Remove(data);
            sortedStartList.Remove(data);
        }

        internal bool SaveFragment(string fragmentPath)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(fragmentPath, FileMode.Create, FileAccess.Write);
                for (int i = 0; i < sortedSizeList.Count; i++)
                {
                    byte[] startBytes = BitConverter.GetBytes(sortedSizeList[i].start);
                    fs.Write(startBytes, 0, startBytes.Length);
                    byte[] sizeBytes = BitConverter.GetBytes(sortedSizeList[i].size);
                    fs.Write(sizeBytes, 0, sizeBytes.Length);
                }
                fs.Flush();
                fs.Close();
                fs = null;
            }
            catch
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                return false;
            }
            return true;
        }

        internal FileFragmentData GetFragment(int len)
        {
            for (int i = 0; i < sortedSizeList.Count; i++)
            {
                if (sortedSizeList[i].size >= len)
                {
                    return sortedSizeList[i];
                }
            }

            return null;
        }
    }
}
