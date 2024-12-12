using System;
using Assimp;


namespace SharpGLExample
{
    internal class Voxelization
    {
        public static void VoxelizeModel(SharpGL.OpenGL gl, Assimp.Scene model, float voxelSize)
        {
            // Step 1: Compute the bounding box of the model
            Vector3D min = new Vector3D(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3D max = new Vector3D(float.MinValue, float.MinValue, float.MinValue);

            foreach (var mesh in model.Meshes)
            {
                foreach (var vertex in mesh.Vertices)
                {
                    min.X = Math.Min(min.X, vertex.X);
                    min.Y = Math.Min(min.Y, vertex.Y);
                    min.Z = Math.Min(min.Z, vertex.Z);

                    max.X = Math.Max(max.X, vertex.X);
                    max.Y = Math.Max(max.Y, vertex.Y);
                    max.Z = Math.Max(max.Z, vertex.Z);
                }
            }

            // Step 2: Iterate over the model's faces to fill the voxel grid
            foreach (var mesh in model.Meshes)
            {
                foreach (var face in mesh.Faces)
                {
                    // Extract the vertices of the face
                    var v1 = mesh.Vertices[face.Indices[0]];
                    var v2 = mesh.Vertices[face.Indices[1]];
                    var v3 = mesh.Vertices[face.Indices[2]];

                    // Compute the voxel indices that intersect with this face
                    VoxelizeTriangle(gl, v1, v2, v3, voxelSize, min, max);
                }
            }
        }

        // Helper method to voxelize a single triangle
        private static void VoxelizeTriangle(SharpGL.OpenGL gl, Vector3D v1, Vector3D v2, Vector3D v3,
                                       float voxelSize, Vector3D min, Vector3D max)
        {
            // Define the voxel grid bounds
            int gridX = (int)Math.Ceiling((max.X - min.X) / voxelSize);
            int gridY = (int)Math.Ceiling((max.Y - min.Y) / voxelSize);
            int gridZ = (int)Math.Ceiling((max.Z - min.Z) / voxelSize);

            // Iterate through the voxel grid and check if each voxel intersects the triangle
            for (int x = 0; x < gridX; x++)
            {
                for (int y = 0; y < gridY; y++)
                {
                    for (int z = 0; z < gridZ; z++)
                    {
                        // Convert voxel index to world space
                        float voxelCenterX = min.X + x * voxelSize + voxelSize / 2;
                        float voxelCenterY = min.Y + y * voxelSize + voxelSize / 2;
                        float voxelCenterZ = min.Z + z * voxelSize + voxelSize / 2;

                        // Check if voxel intersects the triangle
                        if (AABBTest.TriangleIntersectsVoxel(v1, v2, v3, voxelCenterX, voxelCenterY, voxelCenterZ, voxelSize))
                        {
                            // Render the voxel
                            DrawVoxel(gl, voxelCenterX, voxelCenterY, voxelCenterZ, voxelSize);
                        }
                    }
                }
            }
        }

        // Method to draw a single voxel
        public static void DrawVoxel(SharpGL.OpenGL gl, float x, float y, float z, float size)
        {
            gl.Begin(SharpGL.OpenGL.GL_QUADS);

            float halfSize = size / 2;

            // Front face
            gl.Vertex(x - halfSize, y - halfSize, z + halfSize);
            gl.Vertex(x + halfSize, y - halfSize, z + halfSize);
            gl.Vertex(x + halfSize, y + halfSize, z + halfSize);
            gl.Vertex(x - halfSize, y + halfSize, z + halfSize);

            // Back face
            gl.Vertex(x - halfSize, y - halfSize, z - halfSize);
            gl.Vertex(x + halfSize, y - halfSize, z - halfSize);
            gl.Vertex(x + halfSize, y + halfSize, z - halfSize);
            gl.Vertex(x - halfSize, y + halfSize, z - halfSize);

            // Left face
            gl.Vertex(x - halfSize, y - halfSize, z - halfSize);
            gl.Vertex(x - halfSize, y - halfSize, z + halfSize);
            gl.Vertex(x - halfSize, y + halfSize, z + halfSize);
            gl.Vertex(x - halfSize, y + halfSize, z - halfSize);

            // Right face
            gl.Vertex(x + halfSize, y - halfSize, z - halfSize);
            gl.Vertex(x + halfSize, y - halfSize, z + halfSize);
            gl.Vertex(x + halfSize, y + halfSize, z + halfSize);
            gl.Vertex(x + halfSize, y + halfSize, z - halfSize);

            // Top face
            gl.Vertex(x - halfSize, y + halfSize, z - halfSize);
            gl.Vertex(x + halfSize, y + halfSize, z - halfSize);
            gl.Vertex(x + halfSize, y + halfSize, z + halfSize);
            gl.Vertex(x - halfSize, y + halfSize, z + halfSize);

            // Bottom face
            gl.Vertex(x - halfSize, y - halfSize, z - halfSize);
            gl.Vertex(x + halfSize, y - halfSize, z - halfSize);
            gl.Vertex(x + halfSize, y - halfSize, z + halfSize);
            gl.Vertex(x - halfSize, y - halfSize, z + halfSize);

            gl.End();
        }
    }
}
