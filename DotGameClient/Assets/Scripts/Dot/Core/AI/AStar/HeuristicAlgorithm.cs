using System;

namespace Dot.Core.AI.AStar
{
    public enum HeuristicFormula
    {
        Manhattan = 1,
        Chebyshev = 2,
        Euclidean = 3,
        EuclideanNoSQR = 4,
        Octile = 5,
    }

    public static class AlgorithmFactory
    {
        public static IHeuristicAlgorithm GetAlgorithm(HeuristicFormula formula)
        {
            switch (formula)
            {
                case HeuristicFormula.Manhattan:
                    return new Manhattan();
                case HeuristicFormula.Chebyshev:
                    return new Chebyshev();
                case HeuristicFormula.Euclidean:
                    return new Euclidean();
                case HeuristicFormula.EuclideanNoSQR:
                    return new EuclideanNoSQR();
                case HeuristicFormula.Octile:
                    return new Octile();
                default:
                    return null;
            }
        }
    }

    public interface IHeuristicAlgorithm
    {
        float GetH(int he, int x, int y, int ex, int ey);
    }

    public class Manhattan : IHeuristicAlgorithm
    {
        public float GetH(int he, int x, int y, int ex, int ey)
        {
            return he * (Math.Abs(x - ex) + Math.Abs(y - ey));
        }
    }

    public class Chebyshev : IHeuristicAlgorithm
    {
        public float GetH(int he, int x, int y, int ex, int ey)
        {
            return he * (Math.Max(Math.Abs(x - ex), Math.Abs(y - ey)));
        }
    }
    public class Euclidean : IHeuristicAlgorithm
    {
        public float GetH(int he, int x, int y, int ex, int ey)
        {
            return (float)(he * Math.Sqrt(Math.Pow(x - ex, 2) + Math.Pow(y - ey, 2)));
        }
    }

    public class EuclideanNoSQR : IHeuristicAlgorithm
    {
        public float GetH(int he, int x, int y, int ex, int ey)
        {
            return (float)(he * (Math.Pow(x - ex, 2) + Math.Pow(y - ey, 2)));
        }
    }

    public class Octile : IHeuristicAlgorithm
    {
        private float sqrt2 = 1.414f;

        public float GetH(int he, int x, int y, int ex, int ey)
        {
            int dx = Math.Abs(ex - x);
            int dy = Math.Abs(ey - y);
            return he * (Math.Max(dx, dy) + (sqrt2 - 1) * (Math.Min(dx, dy)));
        }
    }
}
