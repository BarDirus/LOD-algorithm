using System;
using SharpGL;
using SharpGL.SceneGraph;

namespace SharpGlControl;

[Serializable]
public class SharpGlArcBall
{
    private float _adjustWidth = 1f;
    private float _adjustHeight = 1f;
    public Vertex StartVector = new(0.0f, 0.0f, 0.0f);
    public Vertex CurrentVector = new(0.0f, 0.0f, 0.0f);
    private Matrix _transformMatrix = new(4, 4);
    private Matrix _lastRotationMatrix = new(3, 3);
    private Matrix _thisRotationMatrix = new(3, 3);

    public SharpGlArcBall()
    {
        _transformMatrix.SetIdentity();
        _lastRotationMatrix.SetIdentity();
        _thisRotationMatrix.SetIdentity();
    }

    public void TransformMatrix(OpenGL gl) => gl.MultMatrix(_transformMatrix.AsColumnMajorArray);

    public void MouseDown(int x, int y) => StartVector = MapToSphere(x, y);

    public void MouseMove(int x, int y)
    {
        CurrentVector = MapToSphere(x, y);
        _thisRotationMatrix = Matrix3fSetRotationFromQuat4f(CalculateQuaternion());
        _thisRotationMatrix *= _lastRotationMatrix;
        Matrix4fSetRotationFromMatrix3f(ref _transformMatrix, _thisRotationMatrix);
    }

    public void MouseUp(int x, int y)
    {
        _lastRotationMatrix.FromOtherMatrix(_thisRotationMatrix, 3, 3);
        _thisRotationMatrix.SetIdentity();
        StartVector = new Vertex(0.0f, 0.0f, 0.0f);
    }

    private Matrix Matrix3fSetRotationFromQuat4f(float[] q1)
    {
        var num1 = (float)((double)q1[0] * (double)q1[0] + (double)q1[1] * (double)q1[1] + (double)q1[2] * (double)q1[2] + (double)q1[3] * (double)q1[3]);
        var num2 = (double)num1 > 0.0 ? 2f / num1 : 0.0f;
        var num3 = q1[0] * num2;
        var num4 = q1[1] * num2;
        var num5 = q1[2] * num2;
        var num6 = q1[3] * num3;
        var num7 = q1[3] * num4;
        var num8 = q1[3] * num5;
        var num9 = q1[0] * num3;
        var num10 = q1[0] * num4;
        var num11 = q1[0] * num5;
        var num12 = q1[1] * num4;
        var num13 = q1[1] * num5;
        var num14 = q1[2] * num5;
        return new Matrix(3, 3)
        {
            [0, 0] = 1.0 - ((double)num12 + (double)num14),
            [1, 0] = (double)num10 - (double)num8,
            [2, 0] = (double)num11 + (double)num7,
            [0, 1] = (double)num10 + (double)num8,
            [1, 1] = 1.0 - ((double)num9 + (double)num14),
            [2, 1] = (double)num13 - (double)num6,
            [0, 2] = (double)num11 - (double)num7,
            [1, 2] = (double)num13 + (double)num6,
            [2, 2] = 1.0 - ((double)num9 + (double)num12)
        };
    }

    private void Matrix4fSetRotationFromMatrix3f(ref Matrix transform, Matrix matrix)
    {
        var scale = transform.TempSVD();
        transform.FromOtherMatrix(matrix, 3, 3);
        transform.Multiply(scale, 3, 3);
    }

    private float[] CalculateQuaternion()
    {
        var vertex = StartVector.VectorProduct(CurrentVector);
        if (vertex.Magnitude() <= 1E-05)
        {
            return new float[4];
        }

        return new float[4]
        {
            vertex.X,
            vertex.Y,
            vertex.Z,
            StartVector.ScalarProduct(CurrentVector)
        };
    }

    public Vertex MapToSphere(float x, float y)
    {
        var sphere = new Vertex((float)((double)x * (double)_adjustWidth - 1.0), -(float)((double)y * (double)_adjustHeight - 1.0), 0.0f);
        var d = (float)((double)sphere.X * (double)sphere.X + (double)sphere.Y * (double)sphere.Y);
        sphere.Z = (double)d > 0.125 ? 0.125f / (float)Math.Sqrt((double)d) : (float)Math.Sqrt(0.25 - (double)d);
        return sphere;
    }

    public void SetBounds(float width, float height) => SetBounds(width, height, 1f);

    public void SetBounds(float width, float height, float sphereRadius)
    {
        _adjustWidth = 2f / width;
        _adjustHeight = 2f / height;
    }
}