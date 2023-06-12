using OpenTK.Mathematics;
using Template;

namespace Rasterizer
{
    public class SceneGraph
    {
        private Node world;
        Shader shader;
        Texture wood;
        Shader postProcess;

        public Shader Shader { get { return shader; } }
        public Texture Wood { get { return wood; } }
        public Shader PostProcess { get { return postProcess; } }

        public SceneGraph()
        {
            world = new Node(null!);
            shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            wood = new Texture("../../../assets/wood.jpg");
            postProcess = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl"); ;
        }

        public void AddMeshToWorld(Mesh mesh)
        {
            world.AddChild(mesh);
        }

        public void AddMeshToMesh(Mesh mesh, Mesh parent)
        {
            var parentNode = FindMeshNode(parent);
            parentNode.AddChild(mesh);
        }

        Node FindMeshNode(Mesh mesh, Node next = null!)
        {
            if (world.Children.Count != 0 && next == null)
            {
                foreach(var child in world.Children)
                {
                    if(child.Data == mesh)
                    {
                        return child;
                    }
                    else
                    {
                        return FindMeshNode(mesh, child);
                    }
                }
            }
            if(next != null)
            {
                foreach (var child in next.Children)
                {
                    if (child.Data == mesh)
                    {
                        return child;
                    }
                    else
                    {
                        return FindMeshNode(mesh, child);
                    }
                }
            }
            return null!;
        }

        public void Render(Matrix4 camera)
        {
            if(world.Children.Count != 0) 
            {
                world.Children.ForEach(child => { ProcessNode(camera, child); });
            }
            else
            {
                throw new Exception("There are no meshes to be rendered");
            }
        }

        public void ProcessNode(Matrix4 camera, Node node)
        {
            // render the mesh
            Mesh mesh = node.Data;
            if(node.Parent != null)
            {
                //translocate the the mesh to be relative to the parent mesh
                //mesh.modelMatrix *= ;
            }
            Matrix4 view = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            Matrix4 transform = mesh.modelMatrix * camera * view;

            mesh?.Render(shader, transform, wood);

            if(node.Children.Count == 0)
            {
                return;
            }
            node.Children.ForEach(child => { ProcessNode(camera, child); });
        }
    }





    public class Node
    {
        private Node parent;
        public Node Parent { get { return parent; } }
        private Mesh data;

        public Mesh Data { get { return data; } }

        private List<Node> children = new List<Node>();

        public List<Node> Children { get { return children; } }

        public Node(Mesh data, Node parent = null!)
        {
            this.parent = parent;
            this.data = data;
        }

        public void AddChild(Mesh mesh)
        {
            var node = new Node(mesh, this);
            children.Add(node);
            node.Data.relativeModelMatrix = node.CalculateRelativeModelMatrix();
        }

        public Matrix4 CalculateRelativeModelMatrix()
        {
            Matrix4 result = Data.modelMatrix;
            Node above = parent;
            while(above != null && above.Data != null)
            {
                result *= parent.Data.modelMatrix;
                above = above.parent;
            }
            return result;
        }


    }
}
