using System.Diagnostics;
using System.Numerics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Rasterizer;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Template
{
    class MyApplication
    {
        // MEMBER VARIABLES
        public Surface screen;                  // background surface for printing etc.
        Mesh? mesh, floor;                      // a mesh to draw using OpenGL                          // teapot rotation angle
        readonly Stopwatch timer = new();       // timer for measuring frame duration
        RenderTarget? target;                   // intermediate render target
        ScreenQuad? quad;                       // screen filling quad for post processing
        readonly bool useRenderTarget = true;
        SceneGraph sceneGraph;
        Camera camera;
        public static Light[] lightData;
        public static Vector3 ambientLight;
        private int light = 20;
        private int selectedLight;

        // CONSTRUCTOR
        public MyApplication(Surface screen)
        {
            float temp = (float)this.light / 255;
            ambientLight = new Vector3(temp, temp, temp);
            sceneGraph = new SceneGraph();
            this.screen = screen;
            lightData = new Light[4];
            camera = new Camera(new Vector3(0, 3, -14.5f), new Vector3(2, 1, 3), new Vector3(1, 0, 0), new Vector3(0, 1, 0));
        }

        // CLASS METHODS

        // initialize
        public void Init()
        {
            
            // load teapot and floor
            mesh = new Mesh("../../../assets/teapot.obj");
            floor = new Mesh("../../../assets/floor.obj");
            
            
            Matrix4 Tpot = Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);
            Matrix4 Tfloor = Matrix4.CreateScale(4.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);

            floor.modelMatrix = Tfloor;
            mesh.modelMatrix = Tpot;
            // add the teapot and floor to the world
            sceneGraph.AddWorldObject(mesh);
            sceneGraph.AddWorldObject(floor);

            // add the lights to the world
            sceneGraph.AddWorldObject(new Light(new Vector3(0, 4, 5), new Vector3(255, 255, 255)));
            sceneGraph.AddWorldObject(new Light(new Vector3(0, 7, 9), new Vector3(0, 255, 0)));
            sceneGraph.AddWorldObject(new Light(new Vector3(0, 5, 6), new Vector3(0, 0, 255)));
            sceneGraph.AddWorldObject(new Light(new Vector3(0, 4, 2), new Vector3(255, 0, 0)));

            // initialize stopwatch
            timer.Reset();
            timer.Start();

            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
                quad = new ScreenQuad();
            camera.UpdateFrontDirection();
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("Camera = " + camera.position, 0, 20, 255);
            KeyboardInput(OpenTKApp.keyboard);
            screen = OpenTKApp.screen;
            if (useRenderTarget) 
                target = new RenderTarget(screen.width, screen.height);
        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            Matrix4 Tcamera = Matrix4.LookAt(camera.position, camera.position + camera.frontDirection, camera.upDirection);

            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            if (useRenderTarget && target != null && quad != null)
            {
                // enable render target
                target.Bind();

                // render scene to render target
                if (sceneGraph.Shader != null && sceneGraph.Wood != null)
                {
                    sceneGraph.Render(Tcamera, camera);
                }

                // render quad
                target.Unbind();
                if (sceneGraph.PostProcess != null)
                    quad.Render(sceneGraph.PostProcess, target.GetTextureID(), sceneGraph.LUT.id);
            }
            else
            {
                // render scene directly to the screen
                if (sceneGraph.Shader != null && sceneGraph.Wood != null)
                {
                    sceneGraph.Render(Tcamera, camera);
                }
            }
        }

        public void KeyboardInput(KeyboardState keyboard)
        {
            if (keyboard[Keys.E])
                camera.position += camera.upDirection;
            if (keyboard[Keys.Q])
                camera.position -= camera.upDirection;
            if (keyboard[Keys.W])
                camera.position += camera.frontDirection;
            if (keyboard[Keys.S])
                camera.position -= camera.frontDirection;
            if (keyboard[Keys.A])
                camera.position += camera.rightDirection;
            if (keyboard[Keys.D])
                camera.position -= camera.rightDirection;

            if (keyboard[Keys.Up])
            {
                camera.pitch += 2;
                if (camera.pitch > 89.0f)
                {
                    camera.pitch = 89.0f;
                }
                camera.UpdateFrontDirection();
                camera.UpdateUpDirection();
            }
            if (keyboard[Keys.Down])
            {
                camera.pitch -= 2;
                if (camera.pitch < -89.0f)
                {
                    camera.pitch = -89.0f;
                }
                camera.UpdateFrontDirection();
                camera.UpdateUpDirection();
            }
            if (keyboard[Keys.Left])
            {
                camera.yaw += 2;
                camera.UpdateFrontDirection();
                camera.UpdateRightDirection();
                camera.UpdateUpDirection();
            }
            if (keyboard[Keys.Right])
            {
                camera.yaw -= 2;
                camera.UpdateFrontDirection();
                camera.UpdateRightDirection();
                camera.UpdateUpDirection();
            }

            if (keyboard[Keys.F1])
                sceneGraph.LUT = new Texture("../../../assets/luts/lut0.png");
            if (keyboard[Keys.F2])
                sceneGraph.LUT = new Texture("../../../assets/luts/lut1.png");
            if (keyboard[Keys.F3])
                sceneGraph.LUT = new Texture("../../../assets/luts/lut2.png");
            if (keyboard[Keys.F4])
                sceneGraph.LUT = new Texture("../../../assets/luts/lut3.png");
            if (keyboard[Keys.F5])
                sceneGraph.LUT = new Texture("../../../assets/luts/lut4.png");
            if (keyboard[Keys.F6])
                sceneGraph.LUT = new Texture("../../../assets/luts/lut5.png");
            if (keyboard[Keys.F7])
                sceneGraph.LUT = new Texture("../../../assets/luts/lut6.png");
            if (keyboard[Keys.F8])
                sceneGraph.LUT = new Texture("../../../assets/luts/lut7.png");
            if (keyboard[Keys.F9])
                sceneGraph.LUT = new Texture("../../../assets/luts/lut8.png");
            if (keyboard[Keys.F10])
                sceneGraph.LUT = new Texture("../../../assets/luts/lut9.png");

            if (keyboard[Keys.P])
            {
                selectedLight++;
                if (selectedLight > 3)
                {
                    selectedLight = 0;
                }
            }

            if (keyboard[Keys.L])
            {
                lightData[selectedLight].Position.X += 0.5f;
            }
            else if (keyboard[Keys.K])
            {
                lightData[selectedLight].Position.Z += 0.5f;
            }
            else if (keyboard[Keys.J])
            {
                lightData[selectedLight].Position.X -= 0.5f;
            }
            else if (keyboard[Keys.I])
            {
                lightData[selectedLight].Position.Z -= 0.5f;
            }
            else if (keyboard[Keys.O])
            {
                lightData[selectedLight].Position.Y += 0.5f;
            }
            else if (keyboard[Keys.U])
            {
                lightData[selectedLight].Position.Y -= 0.5f;
            }

            if (keyboard[Keys.LeftShift])
            {
                if (keyboard[Keys.R])
                    lightData[selectedLight].Intensity.X--;
                if (keyboard[Keys.G])
                    lightData[selectedLight].Intensity.Y--;
                if (keyboard[Keys.B])
                    lightData[selectedLight].Intensity.Z--;
            }
            else
            {
                if (keyboard[Keys.R])
                    lightData[selectedLight].Intensity.X++;
                if (keyboard[Keys.G])
                    lightData[selectedLight].Intensity.Y++;
                if (keyboard[Keys.B])
                    lightData[selectedLight].Intensity.Z++;
            }
        }
    }
}