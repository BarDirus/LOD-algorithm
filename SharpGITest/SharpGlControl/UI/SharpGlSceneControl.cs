using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SharpGL;
using SharpGL.SceneGraph.Effects;
using SharpGL.SceneGraph.Primitives;
using SharpGL.Version;
using SharpGL.WPF;

namespace SharpGlControl.UI;

public partial class SharpGlSceneControl : UserControl
{
    private readonly SharpGlArcBallEffect _arcBallEffect = new();

    public SharpGlSceneControl()
    {
        InitializeComponent();

        SharpGlSceneHelper.InitialiseModelingScene(Scene);
        MouseDown += SceneControl_MouseDown;
        MouseMove += SceneControl_MouseMove;
        MouseUp += SceneControl_MouseUp;
        MouseWheel += OnMouseWheel;

        _gl = _sceneControl.OpenGL;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SharpGlScene Scene { get; set; } = new();

    private readonly OpenGL _gl;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public OpenGL OpenGl => _gl;

    private List<Polygon> _polygons;

    public List<Polygon> Polygons
    {
        get => _polygons;
        set
        {
            var axis = new SharpGlAxes();
            Scene.SceneContainer.AddChild(axis);
            axis.AddEffect(new OpenGLAttributesEffect());
            axis.AddEffect(_arcBallEffect);

            // Add each polygon (There is only one in ducky.obj)
            foreach (var polygon in value)
            {
                polygon.Name = "Ducky";
                //polygon.Transformation.RotateX = 90f; // So that Ducky appears in the right orientation

                //  Get the bounds of the polygon.
                var extent = new float[3];
                polygon.BoundingVolume.GetBoundDimensions(out extent[0], out extent[1], out extent[2]);

                // Get the max extent.
                var maxExtent = extent.Max();

                // Scale so that we are at most 10 units in size.
                var scaleFactor = maxExtent > 10 ? 10.0f / maxExtent : 1;

                polygon.Parent.RemoveChild(polygon);
                polygon.Transformation.ScaleX = scaleFactor;
                polygon.Transformation.ScaleY = scaleFactor;
                polygon.Transformation.ScaleZ = scaleFactor;
                //polygon.Freeze(OpenGl);
                Scene.SceneContainer.AddChild(polygon);

                // Add effects.
                polygon.AddEffect(new OpenGLAttributesEffect());
                polygon.AddEffect(_arcBallEffect);
            }

            
            _polygons = value;
        }
    }

    private void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
    {
        Scene.CreateInContext(OpenGl);
    }

    private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
    {
        OpenGl.MakeCurrent();
        Scene.Draw();
    }

    protected void OnSizeChanged(EventArgs e)
    {
        OpenGl.SetDimensions((int)ActualWidth, (int)ActualHeight);
        Scene.Resize((int)ActualWidth, (int)ActualHeight);
    }

    private void SceneControl_OnResized(object sender, OpenGLRoutedEventArgs args)
    {
        OnSizeChanged(args);
    }

    private void SceneControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e?.Source is not UIElement selectedObject)
        {
            return;
        }

        _arcBallEffect.ArcBall?.SetBounds((float)ActualWidth, (float)ActualHeight);
        var x = (int)e.GetPosition(selectedObject).X;
        var y = (int)e.GetPosition(selectedObject).Y;
        _arcBallEffect.ArcBall?.MouseDown(x, y);
    }

    private void SceneControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (e?.Source is not UIElement selectedObject)
        {
            return;
        }

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var x = (int)e.GetPosition(selectedObject).X;
            var y = (int)e.GetPosition(selectedObject).Y;
            _arcBallEffect.ArcBall?.MouseMove(x, y);
            Scene.Render = true;
        }
    }

    private void SceneControl_MouseUp(object sender, MouseEventArgs e)
    {
        if (e?.Source is not UIElement selectedObject)
        {
            return;
        }

        if (e.LeftButton == MouseButtonState.Released)
        {
            var x = (int)e.GetPosition(selectedObject).X;
            var y = (int)e.GetPosition(selectedObject).Y;
            _arcBallEffect.ArcBall?.MouseUp(x, y);
        }
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e == null)
        {
            return;
        }

        Scene.OnMouseWheel(e.Delta);
    }
}