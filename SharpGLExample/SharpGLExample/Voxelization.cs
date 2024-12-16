using System;
using System.Collections.Generic;
using SharpGL;
using SharpGL.SceneGraph;
using Assimp;
using SharpGLExample;

public class Voxelization
{
    private const float Epsilon = 1e-2f;

    public static void VoxelizeModel(SharpGL.OpenGL gl, Assimp.Scene model, float voxelSize)
    {
        // 1. Определяем границы модели
        Vector3D modelMin = new Vector3D(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3D modelMax = new Vector3D(float.MinValue, float.MinValue, float.MinValue);

        foreach (var mesh in model.Meshes)
        {
            foreach (var vertex in mesh.Vertices)
            {
                modelMin.X = Math.Min(modelMin.X, vertex.X);
                modelMin.Y = Math.Min(modelMin.Y, vertex.Y);
                modelMin.Z = Math.Min(modelMin.Z, vertex.Z);

                modelMax.X = Math.Max(modelMax.X, vertex.X);
                modelMax.Y = Math.Max(modelMax.Y, vertex.Y);
                modelMax.Z = Math.Max(modelMax.Z, vertex.Z);
            }
        }

        // Расширяем границы
        modelMin.X -= Epsilon;
        modelMin.Y -= Epsilon;
        modelMin.Z -= Epsilon;
        modelMax.X += Epsilon;
        modelMax.Y += Epsilon;
        modelMax.Z += Epsilon;

        // 2. Генерируем воксельную сетку
        for (float x = modelMin.X; x <= modelMax.X; x += voxelSize)
        {
            for (float y = modelMin.Y; y <= modelMax.Y; y += voxelSize)
            {
                for (float z = modelMin.Z; z <= modelMax.Z; z += voxelSize)
                {
                    // Проверяем пересечение треугольников с текущим вокселем
                    if (IsVoxelIntersectingModel(new Vector3D(x, y, z), voxelSize, model))
                    {
                        DrawVoxel(gl, x, y, z, voxelSize);
                    }
                }
            }
        }
    }

    private static bool IsVoxelIntersectingModel(Vector3D voxelCenter, float voxelSize, Assimp.Scene model)
    {
        Vector3D voxelMin = new Vector3D(
            voxelCenter.X - voxelSize / 2,
            voxelCenter.Y - voxelSize / 2,
            voxelCenter.Z - voxelSize / 2);
        Vector3D voxelMax = new Vector3D(
            voxelCenter.X + voxelSize / 2,
            voxelCenter.Y + voxelSize / 2,
            voxelCenter.Z + voxelSize / 2);

        foreach (var mesh in model.Meshes)
        {
            foreach (var face in mesh.Faces)
            {
                // Получаем вершины треугольника
                Vector3D v1 = mesh.Vertices[face.Indices[0]];
                Vector3D v2 = mesh.Vertices[face.Indices[1]];
                Vector3D v3 = mesh.Vertices[face.Indices[2]];

                // Проверяем пересечение треугольника с вокселем
                if (AABBTest.AABBTriangleIntersect(voxelMin, voxelMax, v1, v2, v3))
                {
                    return true;
                }
            }
        }
        return false;
    }


    private static void DrawVoxel(SharpGL.OpenGL gl, float x, float y, float z, float size)
    {
        gl.Begin(SharpGL.OpenGL.GL_QUADS);

        // Front face
        gl.Vertex(x - size / 2, y - size / 2, z + size / 2);
        gl.Vertex(x + size / 2, y - size / 2, z + size / 2);
        gl.Vertex(x + size / 2, y + size / 2, z + size / 2);
        gl.Vertex(x - size / 2, y + size / 2, z + size / 2);

        // Back face
        gl.Vertex(x - size / 2, y - size / 2, z - size / 2);
        gl.Vertex(x + size / 2, y - size / 2, z - size / 2);
        gl.Vertex(x + size / 2, y + size / 2, z - size / 2);
        gl.Vertex(x - size / 2, y + size / 2, z - size / 2);

        // Left face
        gl.Vertex(x - size / 2, y - size / 2, z - size / 2);
        gl.Vertex(x - size / 2, y - size / 2, z + size / 2);
        gl.Vertex(x - size / 2, y + size / 2, z + size / 2);
        gl.Vertex(x - size / 2, y + size / 2, z - size / 2);

        // Right face
        gl.Vertex(x + size / 2, y - size / 2, z - size / 2);
        gl.Vertex(x + size / 2, y - size / 2, z + size / 2);
        gl.Vertex(x + size / 2, y + size / 2, z + size / 2);
        gl.Vertex(x + size / 2, y + size / 2, z - size / 2);

        // Top face
        gl.Vertex(x - size / 2, y + size / 2, z - size / 2);
        gl.Vertex(x - size / 2, y + size / 2, z + size / 2);
        gl.Vertex(x + size / 2, y + size / 2, z + size / 2);
        gl.Vertex(x + size / 2, y + size / 2, z - size / 2);

        // Bottom face
        gl.Vertex(x - size / 2, y - size / 2, z - size / 2);
        gl.Vertex(x - size / 2, y - size / 2, z + size / 2);
        gl.Vertex(x + size / 2, y - size / 2, z + size / 2);
        gl.Vertex(x + size / 2, y - size / 2, z - size / 2);

        gl.End();
    }
}
