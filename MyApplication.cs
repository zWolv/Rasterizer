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
        // member variables
        public Surface screen;                  // background surface for printing etc.
        Mesh? mesh, floor;                      // a mesh to draw using OpenGL                          // teapot rotation angle
        readonly Stopwatch timer = new();       // timer for measuring frame duration
        RenderTarget? target;                   // intermediate render target
        ScreenQuad? quad;                       // screen filling quad for post processing
        readonly bool useRenderTarget = true;
        SceneGraph sceneGraph;
        Camera camera;
        public static Vector3[] lightData;

        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
            lightData = new Vector3[2];
        }

        // initialize
        public void Init()
        {
            sceneGraph = new SceneGraph();
            // load teapot
            mesh = new Mesh("../../../assets/teapot.obj");
            floor = new Mesh("../../../assets/floor.obj");
            
            
            Matrix4 Tpot = Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);
            Matrix4 Tfloor = Matrix4.CreateScale(4.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);

            floor.modelMatrix = Tfloor;
            mesh.modelMatrix = Tpot;

            sceneGraph.AddWorldObject(mesh);
            sceneGraph.AddWorldObject(floor);
            Vector3 pos = new Vector3(0, 4, 5);
            Vector3 intensity = new Vector3(255, 255, 255);
            sceneGraph.AddWorldObject(new Light(pos, intensity));
            lightData[0] = pos;
            lightData[1] = intensity;

            // initialize stopwatch
            timer.Reset();
            timer.Start();
            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
                quad = new ScreenQuad();
            camera = new Camera(new Vector3(0, 3, -14.5f), new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 1, 0));
            camera.UpdateFrontDirection();
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            //screen.Print("hello world", 2, 2, 0xffff00);

            KeyboardInput(OpenTKApp.keyboard);
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
                    sceneGraph.Render(Tcamera);
                }

                // render quad
                target.Unbind();
                if (sceneGraph.PostProcess != null)
                    quad.Render(sceneGraph.PostProcess, target.GetTextureID());
            }
            else
            {
                // render scene directly to the screen
                if (sceneGraph.Shader != null && sceneGraph.Wood != null)
                {
                    sceneGraph.Render(Tcamera);
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
                camera.pitch++;
                if (camera.pitch > 89.0f)
                {
                    camera.pitch = 89.0f;
                }
                camera.UpdateFrontDirection();
                camera.UpdateUpDirection();
            }
            if (keyboard[Keys.Down])
            {
                camera.pitch--;
                if (camera.pitch < -89.0f)
                {
                    camera.pitch = -89.0f;
                }
                camera.UpdateFrontDirection();
                camera.UpdateUpDirection();
            }
            if (keyboard[Keys.Left])
            {
                camera.yaw++;
                camera.UpdateFrontDirection();
                camera.UpdateRightDirection();
                camera.UpdateUpDirection();
            }
            if (keyboard[Keys.Right])
            {
                camera.yaw--;
                camera.UpdateFrontDirection();
                camera.UpdateRightDirection();
                camera.UpdateUpDirection();
            }
        }
    }
}