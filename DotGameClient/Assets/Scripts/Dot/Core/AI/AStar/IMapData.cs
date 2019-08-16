using UnityEngine;

namespace Dot.Core.AI.AStar
{
    public interface IMapData
    {
        float GetTileSize();
        int GetWidth();
        int GetHeight();
        int GetIndex(int x, int y);
        void GetXY(int index, out int x, out int y);
        bool IsBlock(int x, int y);
        float GetCost(int x, int y);
        bool IsGridValid(int x, int y);

        Vector3 GetPosition(int x, int y);
    }
}
