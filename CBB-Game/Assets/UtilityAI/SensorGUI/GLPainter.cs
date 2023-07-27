using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtificialIntelligence.Utility
{
    public class GLPainter
    {
        public GLPainter() { }

        public void DrawCilinder(Vector3 position,float radius, float height, Vector3 normal,Color color)
        {
            var displacement = (normal * height / 2f);
            DrawCircle(position + displacement, radius, normal, color);
            DrawCircle(position - displacement, radius, normal, color);
            var vertexs = MathUtils.GetCircleVertices(position, radius, 32, normal);
            foreach (var vertex in vertexs)
            {
                DrawLine(vertex + displacement, vertex - displacement, color);
            }
        }

        public void DrawCircle(Vector3 position, float radius, Vector3 normal, Color color)
        {
            if (!GLInstance.IsInited)
                return;

            var mat = GLInstance.GetMaterial();
            mat.SetPass(0);

            GL.PushMatrix();

            GL.Begin(GL.LINES);
            GL.Color(color);
            var vertex = MathUtils.GetCircleVertices(position, radius, 32, normal);
            for (int i = 0; i < vertex.Length - 1; i++)
            {
                GL.Vertex(vertex[i]);
                GL.Vertex(vertex[i + 1]);
            }
            GL.Vertex(vertex[vertex.Length -1]);
            GL.Vertex(vertex[0]);
            GL.End();

            GL.PopMatrix();
        }

        public void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            if (!GLInstance.IsInited)
                return;

            var mat = GLInstance.GetMaterial();
            mat.SetPass(0);

            GL.PushMatrix();

            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(from);
            GL.Vertex(to);
            GL.End();

            GL.PopMatrix();
        }
    }

    public class GLInstance : MonoBehaviour
    {
        private static Material material;
        private static Shader shader;
        private static bool init;

        public static bool IsInited => init;

        [RuntimeInitializeOnLoadMethod]
        private static void CreateInstance()
        {
            var go = new GameObject("GLInstance");
            go.AddComponent(typeof(GLInstance));
            DontDestroyOnLoad(go);
        }

        private void Awake()
        {
            shader = Shader.Find("Hidden/Internal-Colored");
            material = new Material(shader);
            material.hideFlags = HideFlags.HideAndDontSave;
            init = true;
        }

        public static Material GetMaterial()
        {
            return material;
        }
    }
}