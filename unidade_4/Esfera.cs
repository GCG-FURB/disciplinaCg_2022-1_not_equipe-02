using System;
using System.Collections.Generic;
using gcgcg;
using OpenTK.Graphics.OpenGL;

namespace CG_N3
{
    public class Esfera : Objeto
    {
        private float Radius;
        private uint StackCount;
        private uint SectorCount;

        private List<float> Vertices = new List<float>();
        private List<float> Normals = new List<float>();
        private List<float> TexCoords = new List<float>();
        private List<float> InterleavedVertices = new List<float>();
        private List<uint> Indices = new List<uint>();
        private List<uint> LineIndices = new List<uint>();

        public Esfera(float raio, uint stackCount = 72, uint sectorCount = 32) : base(Utilitario.charProximo(), null)
        {
            Set(raio, stackCount, sectorCount);
        }

        private void Set(float radius, uint sectors, uint stacks)
        {
            Radius = radius;
            SectorCount = sectors;
            StackCount = stacks;

            BuildVertices();
        }

        private void BuildVertices()
        {
            // clear memory of prev arrays
            ClearArrays();

            float x, y, z, xy; // vertex position
            float nx, ny, nz, lengthInv = 1.0f / Radius; // normal
            float s, t; // texCoord

            float sectorStep = (float)(2.0 * Math.PI) / SectorCount;
            float stackStep = (float)Math.PI / StackCount;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= StackCount; ++i)
            {
                stackAngle = (float)Math.PI / 2 - i * stackStep; // starting from pi/2 to -pi/2
                xy = (float)(Radius * Math.Cos(stackAngle)); // r * cos(u)
                z = (float)(Radius * Math.Sin(stackAngle)); // r * sin(u)

                // add (sectorCount+1) vertices per stack
                // the first and last vertices have same position and normal, but different tex coords
                for (int j = 0; j <= SectorCount; ++j)
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
                    s = (float)j / SectorCount;
                    t = (float)i / StackCount;
                    AddTexCoord(s, t);
                }
            }

            // indices
            //  k1--k1+1
            //  |  / |
            //  | /  |
            //  k2--k2+1
            uint k1, k2;
            for (uint i = 0; i < StackCount; ++i)
            {
                k1 = i * (SectorCount + 1); // beginning of current stack
                k2 = k1 + SectorCount + 1; // beginning of next stack

                for (uint j = 0; j < SectorCount; ++j, ++k1, ++k2)
                {
                    // 2 triangles per sector excluding 1st and last stacks
                    if (i != 0)
                    {
                        AddIndices(k1, k2, k1 + 1); // k1---k2---k1+1
                    }

                    if (i != (StackCount - 1))
                    {
                        AddIndices(k1 + 1, k2, k2 + 1); // k1+1---k2---k2+1
                    }

                    // vertical lines for all stacks
                    LineIndices.Add(k1);
                    LineIndices.Add(k2);
                    if (i != 0) // horizontal lines except 1st stack
                    {
                        LineIndices.Add(k1);
                        LineIndices.Add(k1 + 1);
                    }
                }
            }

            // generate interleaved vertex array as well
            BuildInterleavedVertices();
        }

        private void BuildInterleavedVertices()
        {
            InterleavedVertices.Clear();

            for(int i = 0, j = 0; i < Vertices.Count; i += 3, j += 2)
            {
                InterleavedVertices.Add(Vertices[i]);
                InterleavedVertices.Add(Vertices[i+1]);
                InterleavedVertices.Add(Vertices[i+2]);

                InterleavedVertices.Add(Normals[i]);
                InterleavedVertices.Add(Normals[i+1]);
                InterleavedVertices.Add(Normals[i+2]);

                InterleavedVertices.Add(TexCoords[j]);
                InterleavedVertices.Add(TexCoords[j+1]);
            }
        }

        private void AddIndices(uint i1, uint i2, uint i3)
        {
            Indices.Add(i1);
            Indices.Add(i2);
            Indices.Add(i3);
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

            Indices.Clear();
            LineIndices.Clear();
        }

        protected override void DesenharGeometria()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            var floats = InterleavedVertices.ToArray();
            GL.VertexPointer(3, VertexPointerType.Float, 32, ref floats[0]);
            GL.NormalPointer(NormalPointerType.Float, 32, ref floats[3]);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 32, ref floats[6]);

            GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, ref Indices.ToArray()[0]);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
        }
    }
}