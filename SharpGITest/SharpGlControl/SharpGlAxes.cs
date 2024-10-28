using System;
using System.Xml.Serialization;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;

namespace SharpGlControl;

public class SharpGlAxes : SceneElement, IRenderable
{
    [XmlIgnore]
    private DisplayList displayList;

    public SharpGlAxes()
    {
        Name = "Design Time SharpGlAxes";
    }

    public float AxisLengthX { get; set; } = 30f;
    public float AxisLengthY { get; set; } = 30f;
    public float AxisLengthZ { get; set; } = 30f;
    
    public void Render(OpenGL gl, RenderMode renderMode)
    {
        if (renderMode != RenderMode.Design)
        {
            return;
        }
        
        if (displayList == null)
        {
            CreateDisplayList(gl);
        }
        else
        {
            displayList.Call(gl);
        }
    }

    private void CreateDisplayList(OpenGL gl)
    {
        displayList = new DisplayList();
        displayList.Generate(gl);
        displayList.New(gl, DisplayList.DisplayListMode.CompileAndExecute);
        gl.PushAttrib(8453U);
        gl.Disable(2896U);
        gl.Disable(3553U);
        gl.DepthFunc(519U);
        gl.LineWidth(1.5f);
        gl.Begin(1U);

        gl.Color(1f, 0.0f, 0.0f, 1f);
        gl.Vertex(-AxisLengthX, 0, 0);
        gl.Vertex(AxisLengthX, 0, 0);

        gl.Color(0.0f, 1f, 0.0f, 1f);
        gl.Vertex(0, -AxisLengthY, 0);
        gl.Vertex(0, AxisLengthY, 0);

        gl.Color(0.0f, 0.0f, 1f, 1f);
        gl.Vertex(0, 0, -AxisLengthZ);
        gl.Vertex(0, 0, AxisLengthZ);
        
        gl.End();

        DrawArrows(gl);

        gl.PopAttrib();
        displayList.End(gl);
    }

    private void DrawArrows(OpenGL gl)
    {
        var arrowLength = GetArrowLength();
        var arrowWidth = GetArrowWidth();

        DrawArrowX(gl, arrowLength, arrowWidth);
        DrawArrowY(gl, arrowLength, arrowWidth);
        DrawArrowZ(gl, arrowLength, arrowWidth);
    }

    private void DrawArrowX(OpenGL gl, float arrowLength, float arrowWidth)
    {
        gl.Begin(BeginMode.Triangles);
        gl.Color(1f, 0.0f, 0.0f, 1f);

        var count = 10;
        for (var i = 1; i <= count; i++)
        {
            var y0 = Math.Sin(2 * Math.PI * (i - 1) / count) * arrowWidth;
            var z0 = Math.Cos(2 * Math.PI * (i - 1) / count) * arrowWidth;
            var y1 = Math.Sin(2 * Math.PI * i / count) * arrowWidth;
            var z1 = Math.Cos(2 * Math.PI * i / count) * arrowWidth;
            
            gl.Vertex(AxisLengthX - arrowLength, y0, z0);
            gl.Vertex(AxisLengthX, 0, 0);
            gl.Vertex(AxisLengthX - arrowLength, y1, z1);

            gl.Vertex(AxisLengthX - arrowLength, y0, z0);
            gl.Vertex(AxisLengthX - arrowLength, 0, 0);
            gl.Vertex(AxisLengthX - arrowLength, y1, z1);
        }

        gl.End();
    }

    private void DrawArrowY(OpenGL gl, float arrowLength, float arrowWidth)
    {
        gl.Begin(BeginMode.Triangles);
        gl.Color(0f, 1.0f, 0.0f, 1f);

        var count = 10;
        for (var i = 1; i <= count; i++)
        {
            var x0 = Math.Sin(2 * Math.PI * (i - 1) / count) * arrowWidth;
            var z0 = Math.Cos(2 * Math.PI * (i - 1) / count) * arrowWidth;
            var x1 = Math.Sin(2 * Math.PI * i / count) * arrowWidth;
            var z1 = Math.Cos(2 * Math.PI * i / count) * arrowWidth;

            gl.Vertex(x0, AxisLengthX - arrowLength, z0);
            gl.Vertex(0, AxisLengthX, 0);
            gl.Vertex(x1, AxisLengthX - arrowLength, z1);

            gl.Vertex(x0, AxisLengthX - arrowLength, z0);
            gl.Vertex(0, AxisLengthX - arrowLength, 0);
            gl.Vertex(x1, AxisLengthX - arrowLength, z1);
        }

        gl.End();
    }

    private void DrawArrowZ(OpenGL gl, float arrowLength, float arrowWidth)
    {
        gl.Begin(BeginMode.Triangles);
        gl.Color(0f, 0.0f, 1.0f, 1f);

        var count = 10;
        for (var i = 1; i <= count; i++)
        {
            var x0 = Math.Sin(2 * Math.PI * (i - 1) / count) * arrowWidth;
            var y0 = Math.Cos(2 * Math.PI * (i - 1) / count) * arrowWidth;
            var x1 = Math.Sin(2 * Math.PI * i / count) * arrowWidth;
            var y1 = Math.Cos(2 * Math.PI * i / count) * arrowWidth;

            gl.Vertex(x0, y0, AxisLengthX - arrowLength);
            gl.Vertex(0, 0, AxisLengthX);
            gl.Vertex(x1, y1, AxisLengthX - arrowLength);

            gl.Vertex(x0, y0, AxisLengthX - arrowLength);
            gl.Vertex(0, 0,AxisLengthX - arrowLength);
            gl.Vertex(x1, y1, AxisLengthX - arrowLength);
        }

        gl.End();
    }

    private float GetArrowLength()
    {
        return AxisLengthX / 10;
    }

    private float GetArrowWidth()
    {
        return GetArrowLength() / 5;
    }
}