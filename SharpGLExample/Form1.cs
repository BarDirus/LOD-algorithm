using System;
using System.Windows.Forms;
using SharpGL;

namespace SharpGLExample
{
    public partial class Form1 : Form
    {
        private float rotation = 0.0f; // Cube rotation angle

        public Form1()
        {
            InitializeComponent(); // Initialize form components

            // Subscribe to the OpenGL draw event, which is triggered each time the screen needs to be refreshed.
            // The cube is rendered in this metod 
            openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;

            // Subscribe to the OpenGL initialization event, triggered once when the OpenGL context is loaded.
            // This is where the initial scene settings are configured.
            openGLControl.OpenGLInitialized += OpenGLControl_OpenGLInitialized;
        }

        /// <summary>
        /// OpenGL initialization event handler.
        /// Here, the initial OpenGL settings are configured.
        /// </summary>
        private void OpenGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            // Get the OpenGL instance for accessing OpenGL library methods
            SharpGL.OpenGL gl = (sender as OpenGLControl).OpenGL;

            // Set the backgtound color with a slight darkening
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
            //Get the OpenGL instance for accessing OpenGL library methods
            SharpGL.OpenGL gl = (sender as OpenGLControl).OpenGL;

            // Clear the screen and the depth buffer
            // Clears color buffer and fills it with the color set in gl.ClearColor
            // Also clears the depth buffer to property render objects according to their depth
            gl.Clear(SharpGL.OpenGL.GL_COLOR_BUFFER_BIT | SharpGL.OpenGL.GL_DEPTH_BUFFER_BIT);

            // Set the projection matrix
            gl.MatrixMode(SharpGL.OpenGL.GL_PROJECTION); // Матрица, управляющая проекцией сцены на экране

            // Reset the current projection matrix, returning it to the identity state
            gl.LoadIdentity();

            // Set up a perspective projection
            // 1) Set the field of view to 45 degrees (vertical camera field of view)
            // 2) Define the aspect ratio of the screen (Width and Height) to avoid distortion
            // 3) Set the near (0.1) and far (100.0) clipping planes
            gl.Perspective(45.0, (double)Width / (double)Height, 0.1, 100.0);

            // Set the view matrix
            gl.MatrixMode(SharpGL.OpenGL.GL_MODELVIEW); // Matrix for setting the "view" parameters of the scene
            gl.LoadIdentity(); // Reset the current matrix

            // Set the camera position, the point it looks at, and the "up" direction
            gl.LookAt(0, 0, 5,  // Camera position (where it is located)
                      0, 0, 0,  // Point the camera looks at
                      0, 1, 0); // "Up" direction (Y axis)

            // Apply rotation to the cube
            gl.Rotate(rotation, 1.0f, 1.0f, 1.0f); // Rotate around the vector (1, 1, 1)
            rotation += 5.0f; // Increase the rotation angle for continuous spinning


            // Set the color and draw the cube
            DrawCube(gl); // Draw the cube. The rotation angle increases by 180 degrees each time

            gl.Flush(); // clear the OpenGL command queue and display the current scene
        }

        /// <summary>
        /// Method for drawing the cube
        /// The cube has dimensions 2x2x2, and it's center is at the origin of the coordinates
        /// </summary>
        private void DrawCube(SharpGL.OpenGL gl)
        {
            // Start drawing the cube's faces (each face is a square)
            gl.Begin(SharpGL.OpenGL.GL_QUADS);

            // Draw the front face (red)
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);

            // Draw the back face (green)
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);

            // Draw the top face (blue)
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);

            // Draw the bottom face (yellow)
            gl.Color(1.0f, 1.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);

            // Draw the right face (purple)
            gl.Color(1.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);

            // Draw the left face (cyan)
            gl.Color(0.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);

            gl.End(); // End the cube drawing
        }

        // Event handlers for form loading (пустые методы, можно удалить)
        private void openGLControl1_Load(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void openGLControl1_Load_1(object sender, EventArgs e) { }
    }
}
