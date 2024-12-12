using System;
using Assimp;

namespace SharpGLExample
{
    internal class AABBTest
    {
        public static bool TriangleIntersectsVoxel(Vector3D v1, Vector3D v2, Vector3D v3,
                                     float voxelCenterX, float voxelCenterY, float voxelCenterZ,
                                     float voxelSize)
        {
            // Step 1: Define the voxel as an AABB (Axis-Aligned Bounding Box)
            float halfSize = voxelSize / 2;

            Vector3D voxelMin = new Vector3D(
                voxelCenterX - halfSize,
                voxelCenterY - halfSize,
                voxelCenterZ - halfSize
            );

            Vector3D voxelMax = new Vector3D(
                voxelCenterX + halfSize,
                voxelCenterY + halfSize,
                voxelCenterZ + halfSize
            );

            // Step 2: Perform an AABB-Triangle Intersection Test
            return AABBTriangleIntersect(voxelMin, voxelMax, v1, v2, v3);
        }

        // Helper method to check intersection between an AABB and a triangle
        private static bool AABBTriangleIntersect(Vector3D boxMin, Vector3D boxMax,
                                           Vector3D v1, Vector3D v2, Vector3D v3)
        {
            // Step 1: Test the AABB's overlap with the triangle's bounding box
            Vector3D triMin = new Vector3D(
                Math.Min(v1.X, Math.Min(v2.X, v3.X)),
                Math.Min(v1.Y, Math.Min(v2.Y, v3.Y)),
                Math.Min(v1.Z, Math.Min(v2.Z, v3.Z))
            );

            Vector3D triMax = new Vector3D(
                Math.Max(v1.X, Math.Max(v2.X, v3.X)),
                Math.Max(v1.Y, Math.Max(v2.Y, v3.Y)),
                Math.Max(v1.Z, Math.Max(v2.Z, v3.Z))
            );

            // If the triangle's bounding box doesn't overlap the AABB, no intersection
            if (triMax.X < boxMin.X || triMin.X > boxMax.X ||
                triMax.Y < boxMin.Y || triMin.Y > boxMax.Y ||
                triMax.Z < boxMin.Z || triMin.Z > boxMax.Z)
            {
                return false;
            }

            // Step 2: Test the triangle's vertices against the AABB
            if (PointInsideAABB(v1, boxMin, boxMax) ||
                PointInsideAABB(v2, boxMin, boxMax) ||
                PointInsideAABB(v3, boxMin, boxMax))
            {
                return true;
            }

            // Step 3: Test the triangle edges against the AABB faces
            Vector3D[] edges = { v2 - v1, v3 - v2, v1 - v3 };
            Vector3D[] boxAxes = { new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1) };

            foreach (var edge in edges)
            {
                foreach (var axis in boxAxes)
                {
                    if (!OverlapOnAxis(boxMin, boxMax, v1, v2, v3, Vector3D.Cross(edge, axis)))
                    {
                        return false;
                    }
                }
            }

            // Step 4: Test the triangle's plane against the AABB
            Vector3D normal = Vector3D.Cross(v2 - v1, v3 - v1);
            if (!PlaneAABBOverlap(normal, v1, boxMin, boxMax))
            {
                return false;
            }

            // If all tests pass, there is an intersection
            return true;
        }

        // Check if a point is inside an AABB
        private static bool PointInsideAABB(Vector3D point, Vector3D boxMin, Vector3D boxMax)
        {
            return (point.X >= boxMin.X && point.X <= boxMax.X &&
                    point.Y >= boxMin.Y && point.Y <= boxMax.Y &&
                    point.Z >= boxMin.Z && point.Z <= boxMax.Z);
        }

        // Check if projections overlap on a given axis
        private static bool OverlapOnAxis(Vector3D boxMin, Vector3D boxMax, Vector3D v1, Vector3D v2, Vector3D v3, Vector3D axis)
        {
            // Project the AABB onto the axis
            float boxProjection = Math.Abs(axis.X * (boxMax.X - boxMin.X) / 2) +
                                  Math.Abs(axis.Y * (boxMax.Y - boxMin.Y) / 2) +
                                  Math.Abs(axis.Z * (boxMax.Z - boxMin.Z) / 2);

            // Project the triangle onto the axis
            float triProjectionMin = Vector3D.Dot(axis, v1);
            float triProjectionMax = triProjectionMin;

            triProjectionMin = Math.Min(triProjectionMin, Vector3D.Dot(axis, v2));
            triProjectionMin = Math.Min(triProjectionMin, Vector3D.Dot(axis, v3));

            triProjectionMax = Math.Max(triProjectionMax, Vector3D.Dot(axis, v2));
            triProjectionMax = Math.Max(triProjectionMax, Vector3D.Dot(axis, v3));

            // Check for overlap
            return !(triProjectionMin > boxProjection || triProjectionMax < -boxProjection);
        }

        // Check for AABB-plane overlap
        private static bool PlaneAABBOverlap(Vector3D normal, Vector3D pointOnPlane, Vector3D boxMin, Vector3D boxMax)
        {
            // Compute the projection interval of the AABB onto the plane normal
            float boxProjection = Math.Abs(normal.X * (boxMax.X - boxMin.X) / 2) +
                                  Math.Abs(normal.Y * (boxMax.Y - boxMin.Y) / 2) +
                                  Math.Abs(normal.Z * (boxMax.Z - boxMin.Z) / 2);

            // Compute the distance from the AABB center to the plane
            float planeDistance = Vector3D.Dot(normal, (boxMax + boxMin) / 2 - pointOnPlane);

            // Check for overlap
            return Math.Abs(planeDistance) <= boxProjection;
        }
    }
}
