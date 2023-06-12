using System.Diagnostics;
using INFOGR2023TemplateP2;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        Mesh? mesh, floor;                      // a mesh to draw using OpenGL
        float a = 0;                            // teapot rotation angle
        readonly Stopwatch timer = new();       // timer for measuring frame duration
        Shader? shader;                         // shader to use for rendering
        Shader? postproc;                       // shader to use for post processing
        Texture? wood;                          // texture to use for rendering
        RenderTarget? target;                   // intermediate render target
        ScreenQuad? quad;                       // screen filling quad for post processing
        readonly bool useRenderTarget = true;
        Camera camera;

        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }
        // initialize
        public void Init()
        {
            // load teapot
            mesh = new Mesh("../../../assets/teapot.obj");
            floor = new Mesh("../../../assets/floor.obj");
            // initialize stopwatch
            timer.Reset();
            timer.Start();
            // create shaders
            shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            postproc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            // load a texture
            wood = new Texture("../../../assets/wood.jpg");
            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();
            camera = new Camera(new Vector3(0, 14.5f, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 1, 0));
            camera.UpdateFrontDirection();
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffff00);

            KeyboardInput(OpenTKApp.keyboard);
        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            // prepare matrix for vertex shader
            float angle90degrees = MathF.PI / 2;
            Matrix4 Tpot = Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
            Matrix4 Tfloor = Matrix4.CreateScale(4.0f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
            Matrix4 Tcamera = Matrix4.LookAt(camera.position, camera.position + camera.frontDirection, camera.upDirection);
            Matrix4 Tview = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);

            // update rotation
            //a += 0.001f * frameDuration;
            //if (a > 2 * MathF.PI) a -= 2 * MathF.PI;

            if (useRenderTarget && target != null && quad != null)
            {
                // enable render target
                target.Bind();

                // render scene to render target
                if (shader != null && wood != null)
                {
                    mesh?.Render(shader, Tpot * Tcamera * Tview, wood);
                    floor?.Render(shader, Tfloor * Tcamera * Tview, wood);
                }

                // render quad
                target.Unbind();
                if (postproc != null)
                    quad.Render(postproc, target.GetTextureID());
            }
            else
            {
                // render scene directly to the screen
                if (shader != null && wood != null)
                {
                    mesh?.Render(shader, Tpot * Tcamera * Tview, wood);
                    floor?.Render(shader, Tfloor * Tcamera * Tview, wood);
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