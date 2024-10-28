using System.ComponentModel;
using SharpGL;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Effects;

namespace SharpGlControl;

public class SharpGlArcBallEffect : Effect
{
    private SharpGlArcBall _arcBall = new();

    [Description("The SharpGlArcBall.")]
    [Category("Effect")]
    public SharpGlArcBall ArcBall
    {
        get => _arcBall;
        set => _arcBall = value;
    }

    public override void Push(OpenGL gl, SceneElement parentElement)
    {
        gl.PushMatrix();
        _arcBall.TransformMatrix(gl);
    }

    public override void Pop(OpenGL gl, SceneElement parentElement) => gl.PopMatrix();
}