using System;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph.Assets; // Used for textures
using Assimp; // For importing 3D models

namespace SharpGLExample
{
    public partial class Form1 : Form
    {
        private float rotationX = 0.0f; // Rotation angle around the X axis
        private float rotationY = 0.0f; // Rotation angle around the Y axis
        private bool isDragging = false; // Flag to check if the mouse is being dragged
        private int lastMouseX; // Last mouse position on the X axis
        private int lastMouseY; // Last mouse position on the Y axis
        private Assimp.Scene model;
        private Assimp.Material material;

        public Form1()
        {
            InitializeComponent(); // Initialize form components

            // Subscribe to the OpenGL draw event, which is triggered each time the screen needs to be refreshed.
            openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;

            // Subscribe to the OpenGL initialization event, triggered once when the OpenGL context is loaded.
            openGLControl.OpenGLInitialized += OpenGLControl_OpenGLInitialized;

            // Subscribe to mouse events
            openGLControl.MouseDown += OpenGLControl_MouseDown;
            openGLControl.MouseMove += OpenGLControl_MouseMove;
            openGLControl.MouseUp += OpenGLControl_MouseUp;
        }

        /// <summary>
        /// OpenGL initialization event handler.
        /// Here, the initial OpenGL settings are configured.
        /// </summary>
        private void OpenGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            // Get the OpenGL instance for accessing OpenGL library methods
            SharpGL.OpenGL gl = (sender as OpenGLControl).OpenGL;

            // Set the background color
            gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

            // Enable depth testing for correct rendering of 3D objects
            gl.Enable(SharpGL.OpenGL.GL_DEPTH_TEST); // Depth check for each pixel
        }

        /// <summary>
        /// OpenGL drawing event handler.
        /// This is where all scene elements (e.g., the cube) are rendered.
        /// </summary>
        private void OpenGLControl_OpenGLDraw(object sender, RenderEventArgs args)
        {
            // Get the OpenGL instance for accessing OpenGL library methods
            SharpGL.OpenGL gl = (sender as OpenGLControl).OpenGL;

            // Clear the screen and the depth buffer
            gl.Clear(SharpGL.OpenGL.GL_COLOR_BUFFER_BIT | SharpGL.OpenGL.GL_DEPTH_BUFFER_BIT);

            // Set the projection matrix
            gl.MatrixMode(SharpGL.OpenGL.GL_PROJECTION);

            // Reset the current projection matrix
            gl.LoadIdentity();

            // Set up a perspective projection
            gl.Perspective(45.0, (double)Width / (double)Height, 0.1, 100.0);

            // Set the view matrix
            gl.MatrixMode(SharpGL.OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // Set the camera position, the point it looks at, and the "up" direction
            gl.LookAt(0, 0, 5, 0, 0, 0, 0, 1, 0);

            // Apply user-defined rotation
            gl.Rotate(rotationX, 1.0f, 0.0f, 0.0f); // Rotation around X
            gl.Rotate(rotationY, 0.0f, 1.0f, 0.0f); // Rotation around Y

            if (model != null)
            {
                DrawModel(gl, model);
            }

            // Draw the scene
            gl.Flush();
        }

        private void LoadModelButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "3D Models|*.obj"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                AssimpContext importer = new AssimpContext();
                model = importer.ImportFile(openFileDialog.FileName, PostProcessSteps.Triangulate | PostProcessSteps.CalculateTangentSpace);
            }
        }

        /// <summary>
        /// Method for drawing the 3D model.
        /// </summary>
        private void DrawModel(SharpGL.OpenGL gl, Assimp.Scene model)
        {
            foreach (var mesh in model.Meshes)
            {
                // Iterate over model faces
                foreach (var face in mesh.Faces)
                {
                    // Get the material index for this face
                    var materialIndex = mesh.MaterialIndex;

                    // Check if the material index is valid
                    if (materialIndex >= 0 && materialIndex < model.Materials.Count)
                    {
                        var mat = model.Materials[materialIndex];
                        var diffuseColor = mat.ColorDiffuse;

                        // Set the material color for this face
                        gl.Color(diffuseColor.R, diffuseColor.G, diffuseColor.B);
                    }

                    // Draw triangles for this face
                    gl.Begin(SharpGL.OpenGL.GL_TRIANGLES);
                    foreach (var index in face.Indices)
                    {
                        var vertex = mesh.Vertices[index];
                        gl.Vertex(vertex.X, vertex.Y, vertex.Z);
                    }
                    gl.End();
                }
            }
        }

        // Mouse down event
        private void OpenGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMouseX = e.X;
                lastMouseY = e.Y;
            }
        }

        // Mouse move event
        private void OpenGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Update rotation angles based on mouse movement
                rotationY += (e.X - lastMouseX) * 0.5f; // Change angle around Y
                rotationX += (e.Y - lastMouseY) * 0.5f; // Change angle around X

                lastMouseX = e.X;
                lastMouseY = e.Y;

                // Redraw OpenGLControl
                openGLControl.Invalidate();
            }
        }

        // Mouse up event
        private void OpenGLControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        // Event handlers for form loading (empty methods, can be removed)
        private void openGLControl1_Load(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void openGLControl1_Load_1(object sender, EventArgs e) { }
    }
}
