using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using SharpGL.SceneGraph.Primitives;
using System.Linq;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;
using SharpGL.Serialization.Wavefront;

namespace SharpGlTest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void SharpGlSceneControl_Loaded(object? sender, EventArgs e)
    {
        var obj = new ObjFileFormat();
        var initialPath = AppDomain.CurrentDomain.BaseDirectory;
        var path = Path.Combine(initialPath, "data", "ducky.obj");

        var polygonList = new List<Polygon>();

        for (int i = -5; i <= 5; i++)
        {
            var objScene = obj.LoadData(path);

            //// Add the materials to the scene
            //foreach (var asset in objScene.Assets)
            //{
            //    //_glSceneControl.Scene.Assets.Add(asset);
            //}

            // Get the polygons
            var polygons = objScene.SceneContainer.Traverse<Polygon>().ToList();

            var material = new Material
            {
                Diffuse = Color.Blue,
                Ambient = Color.Blue,
            };

            foreach (var polygon in polygons)
            {
                for (var j = 0; j < polygon.Vertices.Count; j++)
                {
                    var vert = polygon.Vertices[j];
                    polygon.Vertices[j] = new Vertex(vert.X + i * 200, vert.Y, vert.Z);
                }

                //polygon.Material = material;
                foreach (var polygonFace in polygon.Faces)
                {
                    polygonFace.Material = material;
                }
            }

            polygonList.AddRange(polygons);
        }

        

        _glSceneControl.Polygons = polygonList;
    }
}
