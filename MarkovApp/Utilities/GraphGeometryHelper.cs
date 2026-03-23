using MarkovApp.Models;
using System.Windows;

namespace MarkovApp.Utilities
{
    public static class GraphGeometryHelper
    {
        public static bool IsPositionValid(
            Point pos,
            IEnumerable<Node> nodes,
            IEnumerable<Edge> edges,
            double nodeMinDistance,
            double edgeMinDistance)
        {
            foreach (var node in nodes)
            {
                double dx = (node.X + Node.Radius) - pos.X;
                double dy = (node.Y + Node.Radius) - pos.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance < nodeMinDistance)
                {
                    return false;
                }
            }

            foreach (var edge in edges)
            {
                double distanceToEdge = DistancePointToEdge(pos, edge);
                if (distanceToEdge < edgeMinDistance)
                {
                    return false;
                }
            }

            return true;
        }

        public static double DistancePointToEdge(Point p, Edge edge)
        {
            var a = new Vector(edge.FromNode.X + Node.Radius, edge.FromNode.Y + Node.Radius);
            var b = new Vector(edge.ToNode.X + Node.Radius, edge.ToNode.Y + Node.Radius);
            var ap = new Vector(p.X, p.Y) - a;
            var ab = b - a;

            double abLengthSquared = ab.LengthSquared;

            if (abLengthSquared == 0)
            {
                return ap.Length;
            }

            double t = Vector.Multiply(ap, ab) / abLengthSquared;
            t = Math.Clamp(t, 0, 1);

            Vector closest = a + t * ab;
            return (new Vector(p.X, p.Y) - closest).Length;
        }
    }
}