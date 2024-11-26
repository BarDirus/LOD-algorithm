using SharpGL;

public static class RubikCubeRenderer
{
    /// <summary>
    /// Рисует собранный кубик Рубика без центрального кубика.
    /// </summary>
    public static void DrawRubikCube(OpenGL gl, float voxelSize, float gap)
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

    private static void DrawVoxel(OpenGL gl, float x, float y, float z, float size, int gridX, int gridY, int gridZ)
    {
        float halfSize = size / 2;

        // Цвета граней
        float[] white = { 1.0f, 1.0f, 1.0f }; // Верхняя грань (W)
        float[] yellow = { 1.0f, 1.0f, 0.0f }; // Нижняя грань (Y)
        float[] blue = { 0.0f, 0.0f, 1.0f };   // Левая грань (B)
        float[] green = { 0.0f, 1.0f, 0.0f };  // Правая грань (G)
        float[] red = { 1.0f, 0.0f, 0.0f };    // Передняя грань (R)
        float[] orange = { 1.0f, 0.5f, 0.0f }; // Задняя грань (O)

        // Сдвиг к позиции кубика
        gl.Translate(x, y, z);

        gl.Begin(OpenGL.GL_QUADS);

        // Верхняя грань (белый) только если gridY == 1
        if (gridY == 1)
        {
            gl.Color(white);
            gl.Vertex(-halfSize, halfSize, -halfSize);
            gl.Vertex(-halfSize, halfSize, halfSize);
            gl.Vertex(halfSize, halfSize, halfSize);
            gl.Vertex(halfSize, halfSize, -halfSize);
        }

        // Нижняя грань (желтый) только если gridY == -1
        if (gridY == -1)
        {
            gl.Color(yellow);
            gl.Vertex(-halfSize, -halfSize, -halfSize);
            gl.Vertex(halfSize, -halfSize, -halfSize);
            gl.Vertex(halfSize, -halfSize, halfSize);
            gl.Vertex(-halfSize, -halfSize, halfSize);
        }

        // Левая грань (синий) только если gridX == -1
        if (gridX == -1)
        {
            gl.Color(blue);
            gl.Vertex(-halfSize, -halfSize, -halfSize);
            gl.Vertex(-halfSize, -halfSize, halfSize);
            gl.Vertex(-halfSize, halfSize, halfSize);
            gl.Vertex(-halfSize, halfSize, -halfSize);
        }

        // Правая грань (зеленый) только если gridX == 1
        if (gridX == 1)
        {
            gl.Color(green);
            gl.Vertex(halfSize, -halfSize, -halfSize);
            gl.Vertex(halfSize, halfSize, -halfSize);
            gl.Vertex(halfSize, halfSize, halfSize);
            gl.Vertex(halfSize, -halfSize, halfSize);
        }

        // Передняя грань (красный) только если gridZ == 1
        if (gridZ == 1)
        {
            gl.Color(red);
            gl.Vertex(-halfSize, -halfSize, halfSize);
            gl.Vertex(halfSize, -halfSize, halfSize);
            gl.Vertex(halfSize, halfSize, halfSize);
            gl.Vertex(-halfSize, halfSize, halfSize);
        }

        // Задняя грань (оранжевый) только если gridZ == -1
        if (gridZ == -1)
        {
            gl.Color(orange);
            gl.Vertex(-halfSize, -halfSize, -halfSize);
            gl.Vertex(-halfSize, halfSize, -halfSize);
            gl.Vertex(halfSize, halfSize, -halfSize);
            gl.Vertex(halfSize, -halfSize, -halfSize);
        }

        gl.End();

        // Возвращаем матрицу к исходному состоянию
        gl.Translate(-x, -y, -z);
    }
}
