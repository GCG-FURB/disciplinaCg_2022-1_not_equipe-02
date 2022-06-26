using System;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace CG_N4
{
    public class Esfera : Objeto
    {
        public float Raio;
        private uint Stacks;
        private uint Setores;

        private List<float> Vertices = new List<float>();
        private List<float> Normals = new List<float>();
        private List<float> TexCoords = new List<float>();

        private float[] InterleavedVertices = Array.Empty<float>();
        private uint[] Indices = Array.Empty<uint>();

        public Esfera(float raio, uint stackCount = 32, uint sectorCount = 32) : base(Utilitario.charProximo(), null)
        {
            Set(raio, stackCount, sectorCount);
            Colisor = new ColisorEsfera(this);
        }

        private void Set(float raio, uint setores, uint stacks)
        {
            Raio = raio;
            Setores = setores;
            Stacks = stacks;

            BuildVertices();

            BBox.Atribuir(new Ponto4D(-raio, -raio, -raio));
            BBox.Atualizar(new Ponto4D(raio, raio, raio));
            BBox.ProcessarCentro();
        }

        private void BuildVertices()
        {
            // clear memory of prev arrays
            ClearArrays();

            List<uint> indices = new List<uint>((int)(Stacks * Setores * 3 * 2));

            float x, y, z, xy; // vertex position
            float nx, ny, nz, lengthInv = 1.0f / Raio; // normal
            float s, t; // texCoord

            float sectorStep = (float)(2.0 * Math.PI) / Setores;
            float stackStep = (float)Math.PI / Stacks;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= Stacks; ++i)
            {
                stackAngle = (float)Math.PI / 2 - i * stackStep; // starting from pi/2 to -pi/2
                xy = (float)(Raio * Math.Cos(stackAngle)); // r * cos(u)
                z = (float)(Raio * Math.Sin(stackAngle)); // r * sin(u)

                // add (sectorCount+1) vertices per stack
                // the first and last vertices have same position and normal, but different tex coords
                for (int j = 0; j <= Setores; ++j)
                {
                    sectorAngle = j * sectorStep; // starting from 0 to 2pi

                    // vertex position
                    x = (float)(xy * Math.Cos(sectorAngle)); // r * cos(u) * cos(v)
                    y = (float)(xy * Math.Sin(sectorAngle)); // r * cos(u) * sin(v)
                    AddVertex(x, y, z);

                    // normalized vertex normal
                    nx = x * lengthInv;
                    ny = y * lengthInv;
                    nz = z * lengthInv;
                    AddNormal(nx, ny, nz);

                    // vertex tex coord between [0, 1]
                    s = (float)j / Setores;
                    t = (float)i / Stacks;
                    AddTexCoord(s, t);
                }
            }

            // indices
            //  k1--k1+1
            //  |  / |
            //  | /  |
            //  k2--k2+1
            uint k1, k2;
            for (uint i = 0; i < Stacks; ++i)
            {
                k1 = i * (Setores + 1); // beginning of current stack
                k2 = k1 + Setores + 1; // beginning of next stack

                for (uint j = 0; j < Setores; ++j, ++k1, ++k2)
                {
                    // 2 triangles per sector excluding 1st and last stacks
                    if (i != 0)
                    {
                        AddIndices(indices, k1, k2, k1 + 1); // k1---k2---k1+1
                    }

                    if (i != (Stacks - 1))
                    {
                        AddIndices(indices, k1 + 1, k2, k2 + 1); // k1+1---k2---k2+1
                    }
                }
            }

            Indices = indices.ToArray();

            // generate interleaved vertex array as well
            BuildInterleavedVertices();
        }

        private void BuildInterleavedVertices()
        {
            int x = 0;
            InterleavedVertices = new float[Vertices.Count * 8];
            for (int i = 0, j = 0; i < Vertices.Count; i += 3, j += 2)
            {
                InterleavedVertices[x++] = Vertices[i];
                InterleavedVertices[x++] = Vertices[i + 1];
                InterleavedVertices[x++] = Vertices[i + 2];

                InterleavedVertices[x++] = Normals[i];
                InterleavedVertices[x++] = Normals[i + 1];
                InterleavedVertices[x++] = Normals[i + 2];

                InterleavedVertices[x++] = TexCoords[j];
                InterleavedVertices[x++] = TexCoords[j + 1];
            }
        }

        private void AddIndices(List<uint> indices, uint i1, uint i2, uint i3)
        {
            indices.Add(i1);
            indices.Add(i2);
            indices.Add(i3);
        }

        private void AddVertex(float x, float y, float z)
        {
            Vertices.Add(x);
            Vertices.Add(y);
            Vertices.Add(z);
        }

        private void AddNormal(float x, float y, float z)
        {
            Normals.Add(x);
            Normals.Add(y);
            Normals.Add(z);
        }

        private void AddTexCoord(float s, float t)
        {
            TexCoords.Add(s);
            TexCoords.Add(t);
        }

        private void ClearArrays()
        {
            Vertices.Clear();
            Normals.Clear();
            TexCoords.Clear();
            Indices = Array.Empty<uint>();
        }

        protected override void DesenharGeometria()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.VertexPointer(3, VertexPointerType.Float, 32, ref InterleavedVertices[0]);
            GL.NormalPointer(NormalPointerType.Float, 32, ref InterleavedVertices[3]);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 32, ref InterleavedVertices[6]);

            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, ref Indices[0]);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
        }
    }
}