using System;
using Assimp;

namespace SharpGLExample
{
    internal class AABBTest
    {
        private const float Epsilon = 1e-3f;

        public static bool AABBTriangleIntersect(Vector3D voxelMin, Vector3D voxelMax, Vector3D v1, Vector3D v2, Vector3D v3)
        {
            // 1. Проверяем, находится ли какая-либо вершина треугольника внутри AABB
            if (IsPointInsideAABB(voxelMin, voxelMax, v1) ||
                IsPointInsideAABB(voxelMin, voxelMax, v2) ||
                IsPointInsideAABB(voxelMin, voxelMax, v3))
            {
                return true;
            }

            // 2. Проверяем, пересекает ли плоскость треугольника AABB
            Vector3D normal = CrossProduct(v2 - v1, v3 - v1);
            float d = -DotProduct(normal, v1);

            if (!IsPlaneIntersectingAABB(voxelMin, voxelMax, normal, d))
            {
                return false;
            }

            // 3. Проверяем пересечение рёбер треугольника с рёбрами AABB
            Vector3D[] triangleEdges = { v2 - v1, v3 - v2, v1 - v3 };
            Vector3D[] aabbEdges =
            {
            new Vector3D(1, 0, 0),
            new Vector3D(0, 1, 0),
            new Vector3D(0, 0, 1)
        };

            foreach (var edge in triangleEdges)
            {
                foreach (var axis in aabbEdges)
                {
                    Vector3D testAxis = CrossProduct(edge, axis);
                    if (!IsSeparatingAxis(testAxis, voxelMin, voxelMax, v1, v2, v3))
                    {
                        return false;
                    }
                }
            }

            // Если все тесты пройдены, треугольник пересекает AABB
            return true;
        }

        private static bool IsPointInsideAABB(Vector3D min, Vector3D max, Vector3D point)
        {
            return point.X > min.X && point.X < max.X &&
                   point.Y > min.Y && point.Y < max.Y &&
                   point.Z > min.Z && point.Z < max.Z;
        }

        private static bool IsPlaneIntersectingAABB(Vector3D min, Vector3D max, Vector3D normal, float d)
        {
            Vector3D positive = new Vector3D(
                normal.X > 0 ? max.X : min.X,
                normal.Y > 0 ? max.Y : min.Y,
                normal.Z > 0 ? max.Z : min.Z
            );

            Vector3D negative = new Vector3D(
                normal.X > 0 ? min.X : max.X,
                normal.Y > 0 ? min.Y : max.Y,
                normal.Z > 0 ? min.Z : max.Z
            );

            float positiveDistance = DotProduct(normal, positive) + d;
            float negativeDistance = DotProduct(normal, negative) + d;

            return positiveDistance >= -Epsilon && negativeDistance <= Epsilon;
        }

        private static bool IsSeparatingAxis(Vector3D axis, Vector3D min, Vector3D max, Vector3D v1, Vector3D v2, Vector3D v3)
        {
            if (Math.Abs(axis.X) < Epsilon && Math.Abs(axis.Y) < Epsilon && Math.Abs(axis.Z) < Epsilon)
            {
                return true; // Нулевой вектор, нет разделяющей оси
            }

            float[] triangleProjections =
            {
            DotProduct(axis, v1),
            DotProduct(axis, v2),
            DotProduct(axis, v3)
            };

            float triangleMin = Math.Min(triangleProjections[0], Math.Min(triangleProjections[1], triangleProjections[2]));
            float triangleMax = Math.Max(triangleProjections[0], Math.Max(triangleProjections[1], triangleProjections[2]));

            float[] aabbProjections =
            {
            DotProduct(axis, new Vector3D(min.X, min.Y, min.Z)),
            DotProduct(axis, new Vector3D(max.X, min.Y, min.Z)),
            DotProduct(axis, new Vector3D(min.X, max.Y, min.Z)),
            DotProduct(axis, new Vector3D(min.X, min.Y, max.Z)),
            DotProduct(axis, new Vector3D(max.X, max.Y, min.Z)),
            DotProduct(axis, new Vector3D(max.X, min.Y, max.Z)),
            DotProduct(axis, new Vector3D(min.X, max.Y, max.Z)),
            DotProduct(axis, new Vector3D(max.X, max.Y, max.Z))
            };

            float aabbMin = Math.Min(aabbProjections[0], Math.Min(aabbProjections[1], aabbProjections[2]));
            float aabbMax = Math.Max(aabbProjections[0], Math.Max(aabbProjections[1], aabbProjections[2]));

            return triangleMax >= aabbMin && triangleMin <= aabbMax;
        }

        private static float DotProduct(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        private static Vector3D CrossProduct(Vector3D a, Vector3D b)
        {
            return new Vector3D(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }
    }
}
