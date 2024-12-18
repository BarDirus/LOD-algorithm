using System;
using System.Windows.Forms;
using SharpGL;
using Assimp; // For importing 3D models

namespace SharpGLExample
{
    public partial class Form1 : Form
    {
        private float rotationX = 0.0f; // Rotation angle around the X axis
        private float rotationY = 0.0f; // Rotation angle around the Y axis
        private float zoom = 5.0f;
        private bool isDragging = false; // Flag to check if the mouse is being dragged
        private int lastMouseX; // Last mouse position on the X axis
        private int lastMouseY; // Last mouse position on the Y axis
        private Assimp.Scene model;
        private Assimp.Material material;
        private Assimp.Vector3D modelCenter = new Assimp.Vector3D(0, 0, 0);
        static SharpGL.OpenGL gl;

        bool voxelize; //signal to start voxelazation
        bool isVoxelization;
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
            openGLControl.MouseWheel += OpenGLControl_MouseWheel;

            isVoxelization = false;
        }

        /// <summary>
        /// OpenGL initialization event handler.
        /// Here, the initial OpenGL settings are configured.
        /// </summary>
        private void OpenGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            // Get the OpenGL instance for accessing OpenGL library methods
            gl = (sender as OpenGLControl).OpenGL;
            voxelize = false;
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
            gl = (sender as OpenGLControl).OpenGL;

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
            gl.LookAt(modelCenter.X, modelCenter.Y, modelCenter.Z + zoom, modelCenter.X, modelCenter.Y, modelCenter.Z, 0, 1, 0);

            // Apply user-defined rotation
            gl.Rotate(rotationX, 1.0f, 0.0f, 0.0f); // Rotation around X
            gl.Rotate(rotationY, 0.0f, 1.0f, 0.0f); // Rotation around Y

            if (model != null)
            {
                if (isVoxelization) // ИЗМЕНЕНИЕ: если включено освещение
                {
                    DrawModelForVoxelization(gl, model);
                }
                else // Если освещение выключено, используем обычную отрисовку
                {
                    DrawModelForLoad(gl, model);

                    if (voxelize)
                    {
                        Voxelization.VoxelizeModel(gl, model, 0.1f);
                    }
                }
            }

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
                model = importer.ImportFile(openFileDialog.FileName,
    PostProcessSteps.Triangulate
    );
                voxelize = false;

                isVoxelization = true;
                // Calculate the model center
                if (model != null && model.Meshes.Count > 0)
                {
                    var min = new Assimp.Vector3D(float.MaxValue, float.MaxValue, float.MaxValue);
                    var max = new Assimp.Vector3D(float.MinValue, float.MinValue, float.MinValue);

                    foreach (var mesh in model.Meshes)
                    {
                        foreach (var vertex in mesh.Vertices)
                        {
                            min.X = Math.Min(min.X, vertex.X);
                            min.Y = Math.Min(min.Y, vertex.Y);
                            min.Z = Math.Min(min.Z, vertex.Z);

                            max.X = Math.Max(max.X, vertex.X);
                            max.Y = Math.Max(max.Y, vertex.Y);
                            max.Z = Math.Max(max.Z, vertex.Z);
                        }
                    }

                    modelCenter = new Assimp.Vector3D(
                        (min.X + max.X) / 2,
                        (min.Y + max.Y) / 2,
                        (min.Z + max.Z) / 2
                    );
                }

            }
        }
        private void VoxelizeTrigger_Click(object sender, EventArgs e)
        {
            if (model != null)
            {
                voxelize = true;

                isVoxelization = false;

                openGLControl.Invalidate(); //???
            }
        }

        /// <summary>
        /// Method for drawing the 3D model.
        /// </summary>
        private void DrawModelForLoad(SharpGL.OpenGL gl, Assimp.Scene model)
        {
            gl.Enable(SharpGL.OpenGL.GL_DEPTH_TEST);
            gl.Enable(SharpGL.OpenGL.GL_LIGHTING);
            gl.Enable(SharpGL.OpenGL.GL_LIGHT0); // Включаем первый источник света
            gl.Enable(SharpGL.OpenGL.GL_LIGHT1);
            gl.Enable(SharpGL.OpenGL.GL_NORMALIZE);

            // Устанавливаем свойства источника света
            float[] lightPosition0 = { 0.0f, 0.0f, 90.0f, 1.0f }; // Позиция света
            float[] ambientLight0 = { 0.2f, 0.2f, 0.2f, 1.0f };   // Фоновое освещение
            float[] diffuseLight0 = { 0.8f, 0.8f, 0.8f, 1.0f };   // Диффузное освещение
            float[] specularLight0 = { 1.0f, 1.0f, 1.0f, 1.0f };  // Зеркальное освещение

            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_POSITION, lightPosition0);
            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_AMBIENT, ambientLight0);
            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_DIFFUSE, diffuseLight0);
            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_SPECULAR, specularLight0);

            // Устанавливаем материал модели
            float[] materialAmbient = { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] materialDiffuse = { 0.8f, 0.8f, 0.8f, 1.0f };
            float[] materialSpecular = { 1.0f, 1.0f, 1.0f, 1.0f };
            float shininess = 50.0f;

            gl.Material(SharpGL.OpenGL.GL_FRONT_AND_BACK, SharpGL.OpenGL.GL_AMBIENT, materialAmbient);
            gl.Material(SharpGL.OpenGL.GL_FRONT_AND_BACK, SharpGL.OpenGL.GL_DIFFUSE, materialDiffuse);
            gl.Material(SharpGL.OpenGL.GL_FRONT_AND_BACK, SharpGL.OpenGL.GL_SPECULAR, materialSpecular);
            gl.Material(SharpGL.OpenGL.GL_FRONT_AND_BACK, SharpGL.OpenGL.GL_SHININESS, shininess);

            foreach (var mesh in model.Meshes)
            {
                gl.Begin(SharpGL.OpenGL.GL_TRIANGLES);

                foreach (var face in mesh.Faces)
                {
                    foreach (var index in face.Indices)
                    {
                        var vertex = mesh.Vertices[index];

                        // Используем нормали, если они есть
                        if (mesh.HasNormals)
                        {
                            var normal = mesh.Normals[index];
                            gl.Normal(normal.X, normal.Y, normal.Z);
                        }

                        gl.Vertex(vertex.X, vertex.Y, vertex.Z);
                    }
                }

                gl.End();
            }
            gl.Disable(SharpGL.OpenGL.GL_LIGHTING);
            gl.Disable(SharpGL.OpenGL.GL_LIGHT0);
            gl.Disable(SharpGL.OpenGL.GL_NORMALIZE);
        }
        private void DrawModelForVoxelization(SharpGL.OpenGL gl, Assimp.Scene model)
        {
            // Включаем освещение и глубину
            gl.Enable(SharpGL.OpenGL.GL_DEPTH_TEST);
            gl.Enable(SharpGL.OpenGL.GL_LIGHTING);
            gl.Enable(SharpGL.OpenGL.GL_LIGHT0); // Включаем первый источник света
            gl.Enable(SharpGL.OpenGL.GL_LIGHT1);
            gl.Enable(SharpGL.OpenGL.GL_NORMALIZE);

            // Устанавливаем свойства источника света
            float[] lightPosition0 = { 0.0f, 0.0f, 10.0f, 1.0f }; // Позиция света
            float[] ambientLight0 = { 0.2f, 0.2f, 0.2f, 1.0f };   // Фоновое освещение
            float[] diffuseLight0 = { 0.8f, 0.8f, 0.8f, 1.0f };   // Диффузное освещение
            float[] specularLight0 = { 1.0f, 1.0f, 1.0f, 1.0f };  // Зеркальное освещение

            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_POSITION, lightPosition0);
            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_AMBIENT, ambientLight0);
            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_DIFFUSE, diffuseLight0);
            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_SPECULAR, specularLight0);

            // Устанавливаем свойства источника света
            float[] lightPosition1 = { 0.0f, 0.0f, 50.0f, 1.0f }; // Позиция света
            float[] ambientLight1 = { 0.2f, 0.2f, 0.2f, 1.0f };   // Фоновое освещение
            float[] diffuseLight1 = { 0.8f, 0.8f, 0.8f, 1.0f };   // Диффузное освещение
            float[] specularLight1 = { 1.0f, 1.0f, 1.0f, 1.0f };  // Зеркальное освещение

            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_POSITION, lightPosition1);
            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_AMBIENT, ambientLight1);
            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_DIFFUSE, diffuseLight1);
            gl.Light(SharpGL.OpenGL.GL_LIGHT0, SharpGL.OpenGL.GL_SPECULAR, specularLight1);

            // Устанавливаем материал модели
            float[] materialAmbient = { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] materialDiffuse = { 0.8f, 0.8f, 0.8f, 1.0f };
            float[] materialSpecular = { 1.0f, 1.0f, 1.0f, 1.0f };
            float shininess = 50.0f;

            gl.Material(SharpGL.OpenGL.GL_FRONT_AND_BACK, SharpGL.OpenGL.GL_AMBIENT, materialAmbient);
            gl.Material(SharpGL.OpenGL.GL_FRONT_AND_BACK, SharpGL.OpenGL.GL_DIFFUSE, materialDiffuse);
            gl.Material(SharpGL.OpenGL.GL_FRONT_AND_BACK, SharpGL.OpenGL.GL_SPECULAR, materialSpecular);
            gl.Material(SharpGL.OpenGL.GL_FRONT_AND_BACK, SharpGL.OpenGL.GL_SHININESS, shininess);

            foreach (var mesh in model.Meshes)
            {
                gl.Begin(SharpGL.OpenGL.GL_TRIANGLES);

                foreach (var face in mesh.Faces)
                {
                    foreach (var index in face.Indices)
                    {
                        var vertex = mesh.Vertices[index];

                        // Используем нормали, если они есть
                        if (mesh.HasNormals)
                        {
                            var normal = mesh.Normals[index];
                            gl.Normal(normal.X, normal.Y, normal.Z);
                        }

                        gl.Vertex(vertex.X, vertex.Y, vertex.Z);
                    }
                }

                gl.End();
            }
            gl.Disable(SharpGL.OpenGL.GL_LIGHTING);
            gl.Disable(SharpGL.OpenGL.GL_LIGHT0);
            gl.Disable(SharpGL.OpenGL.GL_LIGHT1);
            gl.Disable(SharpGL.OpenGL.GL_NORMALIZE);
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
        private void OpenGLControl_MouseWheel(object sender, MouseEventArgs e)
        {
            // Adjust zoom factor based on the direction of the wheel rotation
            zoom -= e.Delta * 0.007f;

            // Clamp zoom factor to avoid extreme values
            zoom = Math.Max(1.0f, Math.Min(zoom, 20.0f));

            // Redraw OpenGLControl
            openGLControl.Invalidate();
        }
        // Event handlers for form loading (empty methods, can be removed)
        private void openGLControl1_Load(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void openGLControl1_Load_1(object sender, EventArgs e) { }
    }
}
