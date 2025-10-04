using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace _3DEngine
{
    public partial class Main : Form
    {
        public long MinCoordinate = long.MinValue;
        public long MaxCoordinate = long.MaxValue;

        public Scene scene = null;
        private int Rotation = 0;


        public Main()
        {
            InitializeComponent();

            scene = new Scene(RenderPicture, 5, 1000);

            IsUpdating = true;

            CameraTargetX.Minimum = CameraTargetY.Minimum = CameraTargetZ.Minimum = MinCoordinate;
            CameraPositionX.Minimum = CameraPositionY.Minimum = CameraPositionZ.Minimum = MinCoordinate;

            CameraTargetX.Maximum = CameraTargetY.Maximum = CameraTargetZ.Maximum = MaxCoordinate;
            CameraPositionX.Maximum = CameraPositionY.Maximum = CameraPositionZ.Maximum = MaxCoordinate;


            IsUpdating = false;
            scene.AddLight(new Vector3D(0, 0, 0));
            scene.AddLight(new Vector3D(40, 40, 40));
            scene.AddLight(new Vector3D(-100, 200, 100));
            scene.AddLight(new Vector3D(-100, 200, -100));
            RenderPicture.MouseWheel += new MouseEventHandler(Canvas_MouseWheel);
        }

        private void ResetCamera_Click(object sender, EventArgs e)
        {
            scene.ResetCamera();
            UpdateCameraValues();
            scene.PaintObjects();
        }

        bool IsUpdating = false;

        private void UpdateCameraValues()
        {
            IsUpdating = true;

            CameraTargetX.Value = (decimal)scene.Camera.Target.X;
            CameraTargetY.Value = (decimal)scene.Camera.Target.Y;
            CameraTargetZ.Value = (decimal)scene.Camera.Target.Z;

            CameraPositionX.Value = (decimal)scene.Camera.Position.X;
            CameraPositionY.Value = (decimal)scene.Camera.Position.Y;
            CameraPositionZ.Value = (decimal)scene.Camera.Position.Z;

            IsUpdating = false;
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {

        }

        private bool IsCanvasAction = false;
        private bool LeftMouse = false;
        private bool RightMouse = false;
        private Point start;

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            start = e.Location;
            IsCanvasAction = true;
            if (e.Button == MouseButtons.Left && !RightMouse)
            {
                LeftMouse = true;
            }
            else if (e.Button == MouseButtons.Right && !LeftMouse)
            {
                RightMouse = true;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsCanvasAction)
            {
                int dx = e.Location.X - start.X;
                int dy = e.Location.Y - start.Y;

                if (LeftMouse)
                {
                    if (Rotation == 0)
                    {
                        scene.Camera.RotatePositionLeftRight(-dx);
                        scene.Camera.RotatePositionUpDown(-dy);
                    }
                    else
                    {
                        scene.Camera.RotateTargetLeftRight(-dx);
                        scene.Camera.RotateTargetUpDown(dy * 25);
                    }
                }
                else if (RightMouse)
                {
                    scene.Camera.MoveCameraLeftRight(dx);
                    scene.Camera.MoveCameraUpDown(-dy);
                }

                start = e.Location;

                UpdateCameraValues();
                scene.PaintObjects();
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            IsCanvasAction = false;
            if (e.Button == MouseButtons.Left)
            {
                LeftMouse = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightMouse = false;
            }
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                Rotation = Rotation == 0 ? 1 : 0;
            }
        }

        private void Canvas_MouseWheel(object sender, MouseEventArgs e)
        {
            if (scene.Camera.IsCentralProjection)
            {
                scene.Camera.MoveCameraAlongVector(e.Delta / 12);
                UpdateCameraValues();
                scene.PaintObjects();
            }
        }

        private void CoordinateBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox box = (TextBox)sender;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != 8)
            {
                e.Handled = true;
            }
            else
            {
                if (e.KeyChar == '-')
                {
                    if (box.TextLength > 0 && (box.SelectionStart != 0 || box.SelectionLength == 0 && box.Text[0] == '-'))
                    {
                        e.Handled = true;
                    }
                }
                else if (char.IsDigit(e.KeyChar))
                {
                    if (box.TextLength > 0 && box.SelectionStart > 0 && box.Text[0] == '0')
                    {
                        int start = box.SelectionStart;
                        box.Text = box.Text.Substring(1);
                        box.SelectionStart = start - 1;
                    }
                    else if (box.TextLength > 1 && box.SelectionStart > 1 && box.Text[0] == '-' && box.Text[1] == '0')
                    {
                        int start = box.SelectionStart;
                        box.Text = "-" + box.Text.Substring(2);
                        box.SelectionStart = start - 1;
                    }
                }
            }
        }

        private void CoordinateBox_Leave(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            if (!int.TryParse(box.Text, out int value))
            {
                box.Text = "0";
            }
        }



        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                MainTimer.Enabled = false;
            }
            else
            {
                MainTimer.Enabled = true;
                if (scene != null)
                {
                    scene.Camera.ResizeFrame(RenderPicture.ClientSize.Width, RenderPicture.ClientSize.Height);
                    scene.PaintObjects();
                }
            }
        }
        private void CameraTargetX_ValueChanged(object sender, EventArgs e)
        {
            if (!IsUpdating)
            {
                scene.Camera.Target.Set(
                (double)CameraTargetX.Value,
                (double)CameraTargetY.Value,
                (double)CameraTargetZ.Value
                );
                scene.PaintObjects();
            }
        }

        private void CameraTargetY_ValueChanged(object sender, EventArgs e)
        {
            if (!IsUpdating)
            {
                scene.Camera.Target.Set(
                (double)CameraTargetX.Value,
                (double)CameraTargetY.Value,
                (double)CameraTargetZ.Value
                );
                scene.PaintObjects();
            }
        }

        private void CameraTargetZ_ValueChanged(object sender, EventArgs e)
        {
            if (!IsUpdating)
            {
                scene.Camera.Target.Set(
                (double)CameraTargetX.Value,
                (double)CameraTargetY.Value,
                (double)CameraTargetZ.Value
                );
                scene.PaintObjects();
            }
        }

        private void CameraPositionX_ValueChanged(object sender, EventArgs e)
        {
            if (!IsUpdating)
            {
                scene.Camera.Position.Set(
                (double)CameraPositionX.Value,
                (double)CameraPositionY.Value,
                (double)CameraPositionZ.Value
                );
                scene.PaintObjects();
            }
        }

        private void CameraPositionY_ValueChanged(object sender, EventArgs e)
        {
            if (!IsUpdating)
            {
                scene.Camera.Position.Set(
                (double)CameraPositionX.Value,
                (double)CameraPositionY.Value,
                (double)CameraPositionZ.Value
                );
                scene.PaintObjects();
            }
        }

        private void CameraPositionZ_ValueChanged(object sender, EventArgs e)
        {
            if (!IsUpdating)
            {
                scene.Camera.Position.Set(
                (double)CameraPositionX.Value,
                (double)CameraPositionY.Value,
                (double)CameraPositionZ.Value
                );
                scene.PaintObjects();
            }
        }



        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.L)
            {
                double a = 10;
                double r = 24.7;
                double b = 2.7;

                double x = 1;
                double y = 1;
                double z = 1;

                double time = 1000;
                double dt = 0.01;

                SceneObject sceneObject = new SceneObject("Lorenz")
                {
                    BasePoint = new Point3D(0, 0, 0)
                };

                for (int i = 0; i < time; i++)
                {
                    double nx = x + (-a * x + a * y) * dt;
                    double ny = y + (-x * z + r * x - y) * dt;
                    double nz = z + (x * y - b * z) * dt;

                    x = nx;
                    y = ny;
                    z = nz;

                    int red = (int)((i / (double)time) * 255);
                    int green = 0;
                    int blue = (int)(((time - i) / (double)time) * 255);
                    Sphere box = new Sphere(new Point3D(x * 10, y * 10, z * 10), 5, 4, Color.FromArgb(red, green, blue));
                    ScenePrimitive scenePrimitive = new ScenePrimitive(box, $"Point-{i}");
                    sceneObject.AddScenePrimitive(scenePrimitive);
                }

                scene.AddObject(sceneObject);
            }
            
        }

       

      
        private void Main_Load(object sender, EventArgs e)
        {
            scene.PaintObjects();
        }

    }
}