using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using aplimat_final_exam.Models;
using aplimat_final_exam.Utilities;

namespace aplimat_final_exam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Vector3 mousePos = new Vector3();
        private List<CubeMesh> cubes = new List<CubeMesh>();
        private Vector3 gravity = new Vector3(0, -.2f, 0);
        private Vector3 mGravity = new Vector3();
        private float yBottom = -45;
        private Liquid ocean = new Liquid(0, -20, 50, 50, 0.8f);
        private CubeMesh mouseHitBox = new CubeMesh()
        {
            Position = new Vector3(0, 0, 0)
        };
        private int count = 0;
        private int frames = 0;

        #region Initialization
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_LIGHT0);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        #endregion

        #region Mouse Func
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            mousePos.x = (float)position.X - (float)Width / 2.0f;
            mousePos.y = -((float)position.Y - (float)Height / 2.0f);

            /*foreach(var c in cubes)
            {
                mousePos.Normalize();
                mousePos /= 10;
                c.ApplyForce(mousePos);
            }*/
        }
        #endregion

        #region KeyPress
        private void ManageKeyPress()
        {

        }
        #endregion

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            this.Title = "APLIMAT Final Exam";
            OpenGL gl = args.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Move Left And Into The Screen
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -100.0f);

            // Draw
            ocean.Draw(gl);
            gl.Color(1.0f, 1.0f, 1.0f);

            mousePos.Normalize();
            mousePos *= 10;
            mGravity = mousePos;
            mouseHitBox.Scale = new Vector3(1.5f,1.5f,1.5f);

            frames++;
            if (frames % 10 == 0 && count < 20)
            {
                CubeMesh cube = new CubeMesh();
                float x = (float)Randomizer.Generate(-20, 20);
                float y = (float)Randomizer.Generate(30, 35);
                float z = 0;
                cube.Position = new Vector3(x, y, z);
                float cubeScale = (float)Randomizer.Generate(0, 3);
                cube.Scale *= cubeScale;
                cube.Mass = (float)Randomizer.Generate(2, 6);
                cubes.Add(cube);
                count++;
            }
            
            foreach (var c in cubes)
            {
                c.ApplyGravity();
                if (ocean.Contains(c))
                {
                    var dragForce = ocean.CalculateDragForce(c);
                    c.ApplyForce(dragForce);
                }
                if (c.Position.y <= yBottom)
                {
                    c.Velocity.y *= -1;
                    c.Velocity /= 2;
                }
                if (c.HasCollidedWith(mouseHitBox))
                {
                    Console.WriteLine("HIT");
                    //mouseHitBox.Position.x--;
                    //c.Velocity *= -1;
                    c.Scale *= 0;
                }
                gl.Color(1.0f, 1.0f, 1.0f);
                c.Draw(gl);
            }

            if(Keyboard.IsKeyDown(Key.W))
            {
                mouseHitBox.Position.y++;
            }
            if (Keyboard.IsKeyDown(Key.A))
            {
                mouseHitBox.Position.x--;
            }
            if (Keyboard.IsKeyDown(Key.S))
            {
                mouseHitBox.Position.y--;
            }
            if (Keyboard.IsKeyDown(Key.D))
            {
                mouseHitBox.Position.x++;
            }

            float r = (float)Randomizer.Gaussian(0, 1);
            float g = (float)Randomizer.Generate(0, 1);
            float b = (float)Randomizer.Generate(0, 1);
            gl.Color(r,g,b);
            mouseHitBox.Draw(gl);

        }
    }
}
