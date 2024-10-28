using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Core;

namespace SharpGlControl;

[TypeConverter(typeof(ExpandableObjectConverter))]
[XmlInclude(typeof(PerspectiveCamera))]
[XmlInclude(typeof(OrthographicCamera))]
[XmlInclude(typeof(FrustumCamera))]
[XmlInclude(typeof(LookAtCamera))]
public class SharpGlScene : IHasOpenGLContext
{
    private OpenGL _gl;

    private GLColor _clearColor = new(1.0f, 1.0f, 1.0f, 1.0f);

    public SharpGlScene()
    {
        RenderBoundingVolumes = false;
        SceneContainer.ParentScene = this;
    }

    public bool Render { get; set; } = true;

    public void SetClearColor(GLColor color)
    {
        _clearColor = color;
    }

    public virtual IEnumerable<SceneElement> DoHitTest(int x, int y)
    {
        var sceneElementList = new List<SceneElement>();
        var hitMap = new Dictionary<uint, SceneElement>();

        if (CurrentCamera == null)
        {
            return sceneElementList;
        }

        var numArray1 = new int[4];
        _gl.GetInteger(2978U, numArray1);
        y = numArray1[3] - y;
        var buffer = new uint[512];
        _gl.SelectBuffer(512, buffer);
        _gl.RenderMode(7170U);
        _gl.InitNames();
        _gl.PushName(0U);
        _gl.MatrixMode(5889U);
        _gl.PushMatrix();
        _gl.LoadIdentity();
        _gl.PickMatrix((double)x, (double)y, 4.0, 4.0, numArray1);
        CurrentCamera.TransformProjectionMatrix(_gl);
        _gl.MatrixMode(5888U);
        _gl.LoadIdentity();
        uint currentName = 1;
        RenderElementForHitTest(SceneContainer, hitMap, ref currentName);
        _gl.MatrixMode(5889U);
        _gl.PopMatrix();
        _gl.MatrixMode(5888U);
        _gl.Flush();
        var num1 = _gl.RenderMode(7168U);
        uint num2 = 0;

        for (var index1 = 0; index1 < num1; ++index1)
        {
            var numArray2 = buffer;
            var index2 = (int)num2;
            var num3 = (uint)(index2 + 1);
            var num4 = numArray2[index2];
            var index3 = (int)num3;
            var num5 = (uint)(index3 + 1);
            var index4 = (int)num5;
            num2 = (uint)(index4 + 1);
            if (num4 != 0U)
            {
                for (var index5 = 0; (long)index5 < (long)num4; ++index5)
                {
                    var key = buffer[(int)num2++];
                    sceneElementList.Add(hitMap[key]);
                }
            }
        }
        return sceneElementList;
    }

    public virtual void Draw(Camera camera = null)
    {
        if (!Render)
        {
            return;
        }
        
        camera ??= CurrentCamera;

        var clearColor = (float[])_clearColor;
        _gl.ClearColor(clearColor[0], clearColor[1], clearColor[2], clearColor[3]);

        camera?.Project(_gl);
        
        _gl.Clear(17664U);



        if (_gl.RenderContextProvider != null)
        {
            _gl.DrawText(10, 10, 0.5f, 0.5f, 0.5f, "Text", 14, "Test text");
        }
        


        RenderElement(SceneContainer, RenderMode.Design);
        _gl.Flush();
        
        Render = false;
    }

    public void RenderElement(SceneElement sceneElement, RenderMode renderMode)
    {
        if (!sceneElement.IsEnabled)
        {
            return;
        }

        foreach (var effect in sceneElement.Effects)
        {
            if (effect.IsEnabled)
            {
                effect.Push(_gl, sceneElement);
            }
        }

        if (sceneElement is IBindable bindableElement)
        {
            bindableElement.Bind(_gl);
        }

        if (sceneElement is IHasObjectSpace objectSpaceElement1)
        {
            objectSpaceElement1.PushObjectSpace(_gl);
        }

        if (sceneElement is IHasMaterial { Material: not null } materialElement1)
        {
            materialElement1.Material.Push(_gl);
        }

        if (sceneElement is IRenderable renderableElement)
        {
            renderableElement.Render(_gl, renderMode);
        }

        if (sceneElement is IHasMaterial { Material: not null } materialElement2)
        {
            materialElement2.Material.Pop(_gl);
        }

        if (RenderBoundingVolumes && sceneElement is IVolumeBound volumeBound)
        {
            volumeBound.BoundingVolume.Render(_gl, renderMode);
        }

        foreach (var child in sceneElement.Children)
        {
            RenderElement(child, renderMode);
        }

        if (sceneElement is IHasObjectSpace objectSpaceElement2)
        {
            objectSpaceElement2.PopObjectSpace(_gl);
        }

        for (var index = sceneElement.Effects.Count - 1; index >= 0; --index)
        {
            if (sceneElement.Effects[index].IsEnabled)
            {
                sceneElement.Effects[index].Pop(_gl, sceneElement);
            }
        }
    }

    private void RenderElementForHitTest(SceneElement sceneElement, Dictionary<uint, SceneElement> hitMap, ref uint currentName)
    {
        if (!sceneElement.IsEnabled || sceneElement == CurrentCamera)
        {
            return;
        }

        foreach (var effect in sceneElement.Effects)
        {
            if (effect.IsEnabled)
            {
                effect.Push(_gl, sceneElement);
            }
        }

        if (sceneElement is IHasObjectSpace objectSpaceElement1)
        {
            objectSpaceElement1.PushObjectSpace(_gl);
        }

        if (sceneElement is IVolumeBound volumeBound)
        {
            _gl.LoadName(currentName);
            hitMap[currentName] = sceneElement;
            volumeBound.BoundingVolume.Render(_gl, RenderMode.HitTest);
            ++currentName;
        }

        foreach (var child in sceneElement.Children)
        {
            RenderElementForHitTest(child, hitMap, ref currentName);
        }

        if (sceneElement is IHasObjectSpace objectSpaceElement2)
        {
            objectSpaceElement2.PopObjectSpace(_gl);
        }

        for (var index = sceneElement.Effects.Count - 1; index >= 0; --index)
        {
            if (sceneElement.Effects[index].IsEnabled)
            {
                sceneElement.Effects[index].Pop(_gl, sceneElement);
            }
        }
    }

    public virtual void Resize(int width, int height)
    {
        if (width == -1 || height == -1 || _gl == null)
        {
            return;
        }

        _gl.Viewport(0, 0, width, height);
        Render = true;

        if (CurrentCamera == null)
        {
            return;
        }

        CurrentCamera.AspectRatio = (double)width / (double)height;
        CurrentCamera.Project(_gl);
    }

    public OpenGL CurrentOpenGLContext => _gl;

    [Description("The top-level object in the Scene Tree")]
    [Category("Scene")]
    public SharpGlSceneContainer SceneContainer { get; set; } = new();

    [Description("The scene assets.")]
    [Category("Scene")]
    public ObservableCollection<Asset> Assets { get; } = new();

    [Description("The current camera being used to view the scene.")]
    [Category("Scene")]
    public Camera CurrentCamera { get; set; }

    public bool RenderBoundingVolumes { get; set; }

    public void CreateInContext(OpenGL gl)
    {
        _gl = gl;

        foreach (var hasOpenGlContext in SceneContainer.Traverse<SceneElement>(se => se is IHasOpenGLContext).OfType<IHasOpenGLContext>())
        {
            hasOpenGlContext.CreateInContext(gl);
        }
    }

    public void DestroyInContext(OpenGL gl)
    {
        foreach (var hasOpenGlContext in SceneContainer.Traverse<SceneElement>(se => se is IHasOpenGLContext).OfType<IHasOpenGLContext>())
        {
            hasOpenGlContext.CreateInContext(gl);
        }
    }

    public void OnMouseWheel(int delta)
    {
        var coefficient = GetPositionCameraCoefficient(delta);
        CurrentCamera.Position = new Vertex(CurrentCamera.Position.X * coefficient, CurrentCamera.Position.Y * coefficient, CurrentCamera.Position.Z * coefficient);

        Render = true;
    }

    private float GetPositionCameraCoefficient(int delta)
    {
        var coefficient = 1 - (float)delta / 1000;
        if (coefficient < 0.001)
        {
            coefficient = 1;
        }

        return coefficient;
    }
}