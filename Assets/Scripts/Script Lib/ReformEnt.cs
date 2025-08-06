using NUnit.Framework.Constraints;
using System;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace ReformEnt
{
    public class Utilities
    {
        public static class UI
        {
            //Creates a Text object in the world at any localPosition.
            public static TextMesh CreateWorldText(string gameObjectName, string textToDisplay, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 12, Color color = default(Color), TextAnchor textAnchor = default(TextAnchor), TextAlignment textAlignment = default(TextAlignment), int sortingOrder = 0, int renderingLayer = 0)
            {
                if (color == default(Color))
                    color = Color.white;
                return CreateWorldText(parent, gameObjectName, textToDisplay, localPosition, fontSize, color, textAnchor, textAlignment, sortingOrder, renderingLayer);
            }
            public static TextMesh CreateWorldText(Transform parent, string gameObjectName, string textToDisplay, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder, int renderingLayer)
            {
                GameObject newText = new GameObject(gameObjectName, typeof(TextMesh));
                Transform newTextTransform = newText.transform;
                newTextTransform.SetParent(parent, false);
                newTextTransform.localPosition = localPosition;
                TextMesh newTextMesh = newText.GetComponent<TextMesh>();
                newTextMesh.anchor = textAnchor;
                newTextMesh.alignment = textAlignment;
                newTextMesh.text = textToDisplay;
                newTextMesh.fontSize = fontSize;
                newTextMesh.color = color;
                newTextMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
                newTextMesh.GetComponent<MeshRenderer>().renderingLayerMask = (uint)(1 << renderingLayer);
                return newTextMesh;
            }
        }
        public static class MeshUtils
        {
            public enum MeshOrientation
            {
                Horizontal,
                Vertical
            }

            private static readonly Vector3 Vector3zero = Vector3.zero;
            private static readonly Vector3 Vector3one = Vector3.one;
            private static readonly Vector3 Vector3yDown = new Vector3(0, -1);
            private static readonly int MAX_ANGLE = 360;
            private static readonly int OBTUSE_ANGLE = 270;
            private static readonly int STRAIGHT_ANGLE = 180;
            private static readonly int RIGHT_ANGLE = 90;
            private static readonly int MIN_ANGLE = 0;
            private static readonly int POINTS_IN_QUAD = 4;
            private static readonly int POINTS_FOR_TRIANGLE_IN_QUAD = 6;


            private static Quaternion[] cachedQuaternionEulerArr;
            private static void CacheQuaternionEuler()
            {
                if (cachedQuaternionEulerArr != null) return;
                cachedQuaternionEulerArr = new Quaternion[MAX_ANGLE];
                for (int i = MIN_ANGLE; i < MAX_ANGLE; i++)
                {
                    cachedQuaternionEulerArr[i] = Quaternion.Euler(0, 0, i);
                }
            }
            private static void CacheQuaternionEuler(MeshOrientation meshOrientation = MeshOrientation.Horizontal)
            {
                if (cachedQuaternionEulerArr != null) return;
                cachedQuaternionEulerArr = new Quaternion[MAX_ANGLE];
                switch (meshOrientation)
                {
                    case MeshOrientation.Horizontal:
                        for (int i = MIN_ANGLE; i < MAX_ANGLE; i++)
                        {
                            cachedQuaternionEulerArr[i] = Quaternion.Euler(i, 0, 0);
                        }
                        break;
                    case MeshOrientation.Vertical:
                        for (int i = MIN_ANGLE; i < MAX_ANGLE; i++)
                        {
                            cachedQuaternionEulerArr[i] = Quaternion.Euler(0, 0, i);
                        }
                        break;
                    default:
                        break;
                }
                
            }
            private static Quaternion GetQuaternionEuler(float rotFloat)
            {
                int rot = Mathf.RoundToInt(rotFloat);
                rot = rot % MAX_ANGLE;
                if (rot < MIN_ANGLE) rot += MAX_ANGLE;
                //if (rot >= 360) rot -= 360;
                if (cachedQuaternionEulerArr == null) CacheQuaternionEuler();
                return cachedQuaternionEulerArr[rot];
            }
            private static Quaternion GetQuaternionEuler(float rotFloat, MeshOrientation meshOrientation = MeshOrientation.Horizontal)
            {
                int rot = Mathf.RoundToInt(rotFloat);
                rot = rot % MAX_ANGLE;
                if (rot < MIN_ANGLE) rot += MAX_ANGLE;
                //if (rot >= 360) rot -= 360;
                if (cachedQuaternionEulerArr == null) CacheQuaternionEuler(meshOrientation);
                return cachedQuaternionEulerArr[rot];
            }
            public static Mesh CreateEmptyMesh()
            {
                Mesh mesh = new Mesh();
                mesh.vertices = new Vector3[0];
                mesh.uv = new Vector2[0];
                mesh.triangles = new int[0];
                return mesh;
            }
            public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles)
            {
                vertices = new Vector3[POINTS_IN_QUAD * quadCount];
                uvs = new Vector2[POINTS_IN_QUAD * quadCount];
                triangles = new int[POINTS_FOR_TRIANGLE_IN_QUAD * quadCount];
            }
            public static Mesh CreateMesh(Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
            {
                return AddToMesh(null, pos, rot, baseSize, uv00, uv11);
            }
            public static Mesh CreateMesh(Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11, MeshOrientation meshOrientation = MeshOrientation.Horizontal)
            {
                return AddToMesh(null, pos, rot, baseSize, uv00, uv11, meshOrientation);
            }
            public static Mesh AddToMesh(Mesh mesh, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
            {
                if (mesh == null)
                {
                    mesh = CreateEmptyMesh();
                }
                Vector3[] vertices = new Vector3[POINTS_IN_QUAD + mesh.vertices.Length];
                Vector2[] uvs = new Vector2[POINTS_IN_QUAD + mesh.uv.Length];
                int[] triangles = new int[POINTS_FOR_TRIANGLE_IN_QUAD + mesh.triangles.Length];

                mesh.vertices.CopyTo(vertices, 0);
                mesh.uv.CopyTo(uvs, 0);
                mesh.triangles.CopyTo(triangles, 0);

                int index = vertices.Length / POINTS_IN_QUAD - 1;
                //Relocate vertices
                int vIndex = index * POINTS_IN_QUAD;
                int vIndex0 = vIndex;
                int vIndex1 = vIndex + 1;
                int vIndex2 = vIndex + 2;
                int vIndex3 = vIndex + 3;

                baseSize *= .5f;

                bool skewed = baseSize.x != baseSize.y;
                if (skewed)
                {
                    vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
                    vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
                    vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
                    vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
                }
                else
                {
                    vertices[vIndex0] = pos + GetQuaternionEuler(rot - OBTUSE_ANGLE) * baseSize;
                    vertices[vIndex1] = pos + GetQuaternionEuler(rot - STRAIGHT_ANGLE) * baseSize;
                    vertices[vIndex2] = pos + GetQuaternionEuler(rot - RIGHT_ANGLE) * baseSize;
                    vertices[vIndex3] = pos + GetQuaternionEuler(rot - MIN_ANGLE) * baseSize;
                }

                //Relocate UVs
                uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
                uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
                uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
                uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

                //Create triangles
                int tIndex = index * POINTS_FOR_TRIANGLE_IN_QUAD;

                triangles[tIndex + 0] = vIndex0;
                triangles[tIndex + 1] = vIndex3;
                triangles[tIndex + 2] = vIndex1;

                triangles[tIndex + 3] = vIndex1;
                triangles[tIndex + 4] = vIndex3;
                triangles[tIndex + 5] = vIndex2;

                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.uv = uvs;

                //mesh.bounds = bounds;

                return mesh;
            }
            public static Mesh AddToMesh(Mesh mesh, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11, MeshOrientation meshOrientation = MeshOrientation.Horizontal)
            {
                if (mesh == null)
                {
                    mesh = CreateEmptyMesh();
                }
                Vector3[] vertices = new Vector3[POINTS_IN_QUAD + mesh.vertices.Length];
                Vector2[] uvs = new Vector2[POINTS_IN_QUAD + mesh.uv.Length];
                int[] triangles = new int[POINTS_FOR_TRIANGLE_IN_QUAD + mesh.triangles.Length];

                mesh.vertices.CopyTo(vertices, 0);
                mesh.uv.CopyTo(uvs, 0);
                mesh.triangles.CopyTo(triangles, 0);

                SetVerticesBasedOnOrientation(meshOrientation, vertices, uvs, triangles, baseSize, uv00, uv11, pos, rot);

                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.uv = uvs;

                //mesh.bounds = bounds;
                mesh.RecalculateNormals();
                return mesh;
            }
            public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
            {
                //Relocate vertices
                int vIndex = index * POINTS_IN_QUAD;
                int vIndex0 = vIndex;
                int vIndex1 = vIndex + 1;
                int vIndex2 = vIndex + 2;
                int vIndex3 = vIndex + 3;

                baseSize *= .5f;

                bool skewed = baseSize.x != baseSize.y;
                if (skewed)
                {
                    vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
                    vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
                    vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
                    vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
                }
                else
                {
                    vertices[vIndex0] = pos + GetQuaternionEuler(rot - OBTUSE_ANGLE) * baseSize;
                    vertices[vIndex1] = pos + GetQuaternionEuler(rot - STRAIGHT_ANGLE) * baseSize;
                    vertices[vIndex2] = pos + GetQuaternionEuler(rot - RIGHT_ANGLE) * baseSize;
                    vertices[vIndex3] = pos + GetQuaternionEuler(rot - MIN_ANGLE) * baseSize;
                }

                //Relocate UVs
                uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
                uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
                uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
                uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

                //Create triangles
                int tIndex = index * POINTS_FOR_TRIANGLE_IN_QUAD;

                triangles[tIndex + 0] = vIndex0;
                triangles[tIndex + 1] = vIndex3;
                triangles[tIndex + 2] = vIndex1;

                triangles[tIndex + 3] = vIndex1;
                triangles[tIndex + 4] = vIndex3;
                triangles[tIndex + 5] = vIndex2;
            }
            public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11, MeshOrientation meshOrientation = MeshOrientation.Horizontal)
            {
                SetVerticesBasedOnOrientation(meshOrientation, index, vertices, uvs, triangles, baseSize, uv00, uv11, pos, rot);
            }
            private static void SetVerticesBasedOnOrientation(MeshOrientation meshOrientation, Vector3[] vertices, Vector2[] uvs, int[] triangles, Vector3 baseSize, Vector2 uv00, Vector2 uv11, Vector3 pos, float rot)
            {
                int index = vertices.Length / POINTS_IN_QUAD - 1;
                //Relocate vertices
                int vIndex = index * POINTS_IN_QUAD;
                int vIndex0 = vIndex;
                int vIndex1 = vIndex + 1;
                int vIndex2 = vIndex + 2;
                int vIndex3 = vIndex + 3;

                baseSize *= .5f;
                bool skewed;
                switch (meshOrientation)
                {

                    case MeshOrientation.Horizontal:
                        skewed = baseSize.x != baseSize.y;
                        if (skewed)
                        {
                            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, 0, baseSize.z);
                            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, 0, -baseSize.z);
                            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, 0, -baseSize.z);
                            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
                        }
                        else
                        {
                            vertices[vIndex0] = pos + GetQuaternionEuler(rot - OBTUSE_ANGLE) * baseSize;
                            vertices[vIndex1] = pos + GetQuaternionEuler(rot - STRAIGHT_ANGLE) * baseSize;
                            vertices[vIndex2] = pos + GetQuaternionEuler(rot - RIGHT_ANGLE) * baseSize;
                            vertices[vIndex3] = pos + GetQuaternionEuler(rot - MIN_ANGLE) * baseSize;
                        }
                        break;
                    case MeshOrientation.Vertical:
                        skewed = baseSize.x != baseSize.y;
                        if (skewed)
                        {
                            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
                            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
                            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
                            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
                        }
                        else
                        {
                            vertices[vIndex0] = pos + GetQuaternionEuler(rot - OBTUSE_ANGLE) * baseSize;
                            vertices[vIndex1] = pos + GetQuaternionEuler(rot - STRAIGHT_ANGLE) * baseSize;
                            vertices[vIndex2] = pos + GetQuaternionEuler(rot - RIGHT_ANGLE) * baseSize;
                            vertices[vIndex3] = pos + GetQuaternionEuler(rot - MIN_ANGLE) * baseSize;
                        }
                        break;
                    default:
                        break;
                }

                //Relocate UVs
                uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
                uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
                uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
                uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

                //Create triangles
                int tIndex = index * POINTS_FOR_TRIANGLE_IN_QUAD;

                triangles[tIndex + 0] = vIndex0;
                triangles[tIndex + 1] = vIndex3;
                triangles[tIndex + 2] = vIndex1;

                triangles[tIndex + 3] = vIndex1;
                triangles[tIndex + 4] = vIndex3;
                triangles[tIndex + 5] = vIndex2;
            }
            private static void SetVerticesBasedOnOrientation(MeshOrientation meshOrientation, int index, Vector3[] vertices, Vector2[] uvs, int[] triangles, Vector3 baseSize, Vector2 uv00, Vector2 uv11, Vector3 pos, float rot)
            {
                //Relocate vertices
                int vIndex = index * POINTS_IN_QUAD;
                int vIndex0 = vIndex;
                int vIndex1 = vIndex + 1;
                int vIndex2 = vIndex + 2;
                int vIndex3 = vIndex + 3;

                baseSize *= .5f;
                bool skewed;
                switch (meshOrientation)
                {

                    case MeshOrientation.Horizontal:
                        skewed = baseSize.x != baseSize.y;
                        if (skewed)
                        {
                            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, 0, baseSize.z);
                            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, 0, -baseSize.z);
                            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, 0, -baseSize.z);
                            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
                        }
                        else
                        {
                            vertices[vIndex0] = pos + GetQuaternionEuler(rot - OBTUSE_ANGLE) * baseSize;
                            vertices[vIndex1] = pos + GetQuaternionEuler(rot - STRAIGHT_ANGLE) * baseSize;
                            vertices[vIndex2] = pos + GetQuaternionEuler(rot - RIGHT_ANGLE) * baseSize;
                            vertices[vIndex3] = pos + GetQuaternionEuler(rot - MIN_ANGLE) * baseSize;
                        }
                        break;
                    case MeshOrientation.Vertical:
                        skewed = baseSize.x != baseSize.y;
                        if (skewed)
                        {
                            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
                            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
                            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
                            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
                        }
                        else
                        {
                            vertices[vIndex0] = pos + GetQuaternionEuler(rot - OBTUSE_ANGLE) * baseSize;
                            vertices[vIndex1] = pos + GetQuaternionEuler(rot - STRAIGHT_ANGLE) * baseSize;
                            vertices[vIndex2] = pos + GetQuaternionEuler(rot - RIGHT_ANGLE) * baseSize;
                            vertices[vIndex3] = pos + GetQuaternionEuler(rot - MIN_ANGLE) * baseSize;
                        }
                        break;
                    default:
                        break;
                }

                //Relocate UVs
                uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
                uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
                uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
                uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

                //Create triangles
                int tIndex = index * POINTS_FOR_TRIANGLE_IN_QUAD;

                triangles[tIndex + 0] = vIndex0;
                triangles[tIndex + 1] = vIndex3;
                triangles[tIndex + 2] = vIndex1;

                triangles[tIndex + 3] = vIndex1;
                triangles[tIndex + 4] = vIndex3;
                triangles[tIndex + 5] = vIndex2;
            }
            
        }
        public static class PixelMath
        {
            public static Vector3 RoundToPixel(Vector3 nextPos, int PPU)
            {
                float unitsPerPixel = 1.0f / PPU;
                Vector3 vectorRounded = new Vector3(
                    Mathf.Round(nextPos.x * unitsPerPixel),
                    Mathf.Round(nextPos.y * unitsPerPixel),
                    nextPos.z
                );

                return vectorRounded / PPU;
            }
        }
    }


}
