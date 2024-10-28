using System.Drawing;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Effects;
using SharpGL.SceneGraph.Lighting;
using SharpGL.SceneGraph.Primitives;

namespace SharpGlControl;

public class SharpGlSceneHelper
{
    public static void InitialiseModelingScene(SharpGlScene scene)
    {
        var lookAtCamera1 = new LookAtCamera
        {
            Position = new Vertex(-20f, -20f, 20f),
            Target = new Vertex(0.0f, 0.0f, 0.0f),
            UpVector = new Vertex(0.0f, 0.0f, 1f),
            Far = double.MaxValue
        };

        var lookAtCamera2 = lookAtCamera1;
        scene.CurrentCamera = lookAtCamera2;

        var folder1 = new Folder
        {
            Name = "Design Primitives"
        };
        var child1 = folder1;


        //child1.AddChild((SceneElement)new Grid());
        //child1.AddChild(new SharpGlAxes());


        scene.SceneContainer.AddChild(child1);

        var ambient = Color.FromArgb(255, 20, 20, 20);
        var diffuse1 = Color.FromArgb(255, 100, 100, 100);
        var diffuse2 = Color.FromArgb(255, 5, 5, 5);
        var specular = Color.FromArgb(255, 0, 0, 0);

        var light1 = new Light
        {
            Name = "Light 1",
            On = true,
            Position = new Vertex(-150f, -150f, 15f),
            GLCode = 16384U,
            Ambient = ambient,
            Diffuse = diffuse1,
            Specular = specular
        };

        var child2 = light1;

        var light2 = new Light
        {
            Name = "Light 2",
            On = true,
            Position = new Vertex(150f, -150f, 15f),
            GLCode = 16385U,
            Ambient = ambient,
            Diffuse = diffuse2,
            Specular = specular
        };

        var child3 = light2;

        var light3 = new Light
        {
            Name = "Light 3",
            On = true,
            Position = new Vertex(-150.0f, 150f, 15f),
            GLCode = 16386U,
            Ambient = ambient,
            Diffuse = diffuse2,
            Specular = specular
        };

        var child4 = light3;

        var folder2 = new Folder
        {
            Name = "Lights"
        };

        var child5 = folder2;
        child5.AddChild(child2);
        child5.AddChild(child3);
        child5.AddChild(child4);
        scene.SceneContainer.AddChild(child5);

        var attributesEffect1 = new OpenGLAttributesEffect
        {
            Name = "Scene Attributes"
        };

        var attributesEffect2 = attributesEffect1;
        attributesEffect2.EnableAttributes.EnableDepthTest = true;
        attributesEffect2.EnableAttributes.EnableNormalize = true;
        attributesEffect2.EnableAttributes.EnableLighting = true;
        attributesEffect2.EnableAttributes.EnableTexture2D = true;
        attributesEffect2.EnableAttributes.EnableBlend = true;
        attributesEffect2.ColorBufferAttributes.BlendingSourceFactor = BlendingSourceFactor.SourceAlpha;
        attributesEffect2.ColorBufferAttributes.BlendingDestinationFactor = BlendingDestinationFactor.OneMinusSourceAlpha;
        attributesEffect2.LightingAttributes.TwoSided = true;
        scene.SceneContainer.AddEffect(attributesEffect2);
    }
}