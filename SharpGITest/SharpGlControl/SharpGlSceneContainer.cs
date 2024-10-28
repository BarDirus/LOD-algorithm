using System.Xml.Serialization;
using SharpGL.SceneGraph.Core;

namespace SharpGlControl;

public class SharpGlSceneContainer : SceneElement
{
    public SharpGlSceneContainer() => Name = "SharpGl Scene Container";

    [XmlIgnore]
    public SharpGlScene ParentScene { get; set; }
}