using SharpGL;

public static class RubikCubeRenderer
{
    /// <summary>
    /// Рисует собранный кубик Рубика без центрального кубика.
    /// </summary>
    public static void DrawRubikCube(SharpGL.OpenGL gl, float voxelSize, float gap)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    // Пропускаем центральный кубик
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    // Рисуем воксель с правильной раскраской
                    DrawVoxel(gl, x * gap, y * gap, z * gap, voxelSize, x, y, z);
                }
            }
        }
    }

    /// <summary>
    /// Рисует отдельный кубик (воксель) с правильной раскраской граней.
    /// </summary>
    private static void DrawVoxel(SharpGL.OpenGL gl, float x, float y, float z, float size, int gridX, int gridY, int gridZ)
    {
        float halfSize = size / 2;

        // Сдвиг к позиции кубика
        gl.Translate(x, y, z);

        gl.Begin(SharpGL.OpenGL.GL_QUADS);

        // Верхняя грань (белая), рисуется всегда
        gl.Color(gridY == 1 ? 1.0f : 0f, gridY == 1 ? 1.0f : 0f, gridY == 1 ? 1.0f : 0f);
        gl.Vertex(-halfSize, halfSize, -halfSize);
        gl.Vertex(-halfSize, halfSize, halfSize);
        gl.Vertex(halfSize, halfSize, halfSize);
        gl.Vertex(halfSize, halfSize, -halfSize);

        // Нижняя грань (жёлтая), рисуется всегда
        gl.Color(gridY == -1 ? 1.0f : 0f, gridY == -1 ? 1.0f : 0f, 0.0f);
        gl.Vertex(-halfSize, -halfSize, -halfSize);
        gl.Vertex(halfSize, -halfSize, -halfSize);
        gl.Vertex(halfSize, -halfSize, halfSize);
        gl.Vertex(-halfSize, -halfSize, halfSize);

        // Левая грань (синяя), рисуется всегда
        gl.Color(0.0f, 0.0f, gridX == -1 ? 1.0f : 0f);
        gl.Vertex(-halfSize, -halfSize, -halfSize);
        gl.Vertex(-halfSize, halfSize, -halfSize);
        gl.Vertex(-halfSize, halfSize, halfSize);
        gl.Vertex(-halfSize, -halfSize, halfSize);

        // Правая грань (зелёная), рисуется всегда
        gl.Color(0.0f, gridX == 1 ? 1.0f : 0f, 0.0f);
        gl.Vertex(halfSize, -halfSize, -halfSize);
        gl.Vertex(halfSize, -halfSize, halfSize);
        gl.Vertex(halfSize, halfSize, halfSize);
        gl.Vertex(halfSize, halfSize, -halfSize);

        // Передняя грань (красная), рисуется всегда
        gl.Color(gridZ == 1 ? 1.0f : 0f, 0.0f, 0.0f);
        gl.Vertex(-halfSize, -halfSize, halfSize);
        gl.Vertex(halfSize, -halfSize, halfSize);
        gl.Vertex(halfSize, halfSize, halfSize);
        gl.Vertex(-halfSize, halfSize, halfSize);

        // Задняя грань (оранжевая), рисуется всегда
        gl.Color(gridZ == -1 ? 1.0f : 0f, 0f, 0.0f);
        gl.Vertex(-halfSize, -halfSize, -halfSize);
        gl.Vertex(-halfSize, halfSize, -halfSize);
        gl.Vertex(halfSize, halfSize, -halfSize);
        gl.Vertex(halfSize, -halfSize, -halfSize);

        gl.End();

        // Возвращаем матрицу к исходному состоянию
        gl.Translate(-x, -y, -z);
    }
}
