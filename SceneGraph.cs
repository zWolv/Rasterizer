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

        public void AddWorldObject(WorldObject mesh)
        {
            world.AddChild(mesh);
        }

        public void AddWorldObjectToObject(WorldObject mesh, WorldObject parent)
        {
            var parentNode = FindMeshNode(parent);
            parentNode.AddChild(mesh);
        }

        Node FindMeshNode(WorldObject mesh, Node next = null!)
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
            // render the mesh if the node contains a mesh. Lights don't need to be rendered.
            if (node.Data is Light)
            {
                return;
            }

            Mesh? mesh = node.Data as Mesh;
            Matrix4 view = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            Matrix4 transform = //mesh.relativeModelMatrix * 
                mesh.modelMatrix * camera * view;

            mesh?.Render(shader, transform, wood);

            if (node.Children.Count == 0)
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
        private WorldObject data;

        public WorldObject Data { get { return data; } }

        private List<Node> children = new List<Node>();

        public List<Node> Children { get { return children; } }

        public Node(WorldObject data, Node parent = null!)
        {
            this.parent = parent;
            this.data = data;
        }

        public void AddChild(WorldObject mesh)
        {
            var node = new Node(mesh, this);
            children.Add(node);
            if (node.Data is Mesh q)
            {
                q.relativeModelMatrix = node.CalculateRelativeModelMatrix();
            }
            
        }

        public Matrix4 CalculateRelativeModelMatrix()
        {
            Matrix4 result = Matrix4.Identity;
            List<Matrix4> parentMatrices = new List<Matrix4>();
            Node above = parent;
            while(above != null && above.Data != null)
            {
                Mesh? mesh = above.Data as Mesh;
                parentMatrices.Add(mesh.modelMatrix);
                above = above.parent;
            }
            for(int i = parentMatrices.Count - 1; i >= 0;i--)
            {
                if(result == Matrix4.Identity)
                {
                    result = parentMatrices[i];
                }
                else
                {
                    result *= parentMatrices[i];
                }
            }

            Mesh? q = Data as Mesh;

            return result * q.modelMatrix;
        }
    }
}
