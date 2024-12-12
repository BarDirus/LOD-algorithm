namespace SharpGLExample
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openGLControl = new SharpGL.OpenGLControl();
            this.LoadModelButton = new System.Windows.Forms.Button();
            this.VoxelizeTrigger = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.openGLControl)).BeginInit();
            this.SuspendLayout();
            // 
            // openGLControl
            // 
            this.openGLControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openGLControl.DrawFPS = false;
            this.openGLControl.Location = new System.Drawing.Point(0, 0);
            this.openGLControl.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.openGLControl.Name = "openGLControl";
            this.openGLControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.openGLControl.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.openGLControl.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.openGLControl.Size = new System.Drawing.Size(800, 450);
            this.openGLControl.TabIndex = 0;
            this.openGLControl.Load += new System.EventHandler(this.openGLControl1_Load_1);
            this.openGLControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OpenGLControl_MouseDown);
            this.openGLControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OpenGLControl_MouseMove);
            this.openGLControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OpenGLControl_MouseUp);
            // 
            // LoadModelButton
            // 
            this.LoadModelButton.Location = new System.Drawing.Point(700, 422);
            this.LoadModelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LoadModelButton.Name = "LoadModelButton";
            this.LoadModelButton.Size = new System.Drawing.Size(100, 28);
            this.LoadModelButton.TabIndex = 1;
            this.LoadModelButton.Tag = "";
            this.LoadModelButton.Text = "Load Model";
            this.LoadModelButton.UseVisualStyleBackColor = true;
            this.LoadModelButton.Click += new System.EventHandler(this.LoadModelButton_Click);
            // 
            // VoxelizeTrigger
            // 
            this.VoxelizeTrigger.Location = new System.Drawing.Point(700, 384);
            this.VoxelizeTrigger.Name = "VoxelizeTrigger";
            this.VoxelizeTrigger.Size = new System.Drawing.Size(100, 31);
            this.VoxelizeTrigger.TabIndex = 2;
            this.VoxelizeTrigger.Text = "Voxelize";
            this.VoxelizeTrigger.UseVisualStyleBackColor = true;
            this.VoxelizeTrigger.Click += new System.EventHandler(this.VoxelizeTrigger_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.VoxelizeTrigger);
            this.Controls.Add(this.LoadModelButton);
            this.Controls.Add(this.openGLControl);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.openGLControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SharpGL.OpenGLControl openGLControl;
        private System.Windows.Forms.Button LoadModelButton;
        private System.Windows.Forms.Button VoxelizeTrigger;
    }
}

