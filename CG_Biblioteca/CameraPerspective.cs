using System;
using OpenTK;
using OpenTK.Input;

namespace CG_Biblioteca
{
  public class CameraPerspective
  {
    private float fovy, aspect, near, far;
    private Vector3 eye, at, up;

    private float yaw;
    private float pitch;

    public CameraPerspective(float fovy = (float)Math.PI / 4, float aspect = 1.0f, float near = 1.0f, float far = 50.0f)
    {
      this.fovy = fovy;
      this.aspect = aspect;
      this.near = near;
      this.far = far;

      eye = Vector3.Zero; eye.Z = 15;   // ( 0, 0,15)
      at = Vector3.Zero;                // ( 0, 0, 0)
      up = Vector3.UnitY;               // ( 0, 1, 0)
    }

    public float Fovy { get => fovy; set => fovy = value; }
    public float Aspect { get => aspect; set => aspect = value; }
    public float Near { get => near; set => near = value; }
    public float Far { get => far; set => far = value; }
    public Vector3 Eye { get => eye; set => eye = value; }
    public Vector3 At { get => at; set => at = value; }
    public Vector3 Up { get => up; }

    public void LookAround(float deltaX, float deltaY)
    {
      yaw += deltaX;
      pitch += deltaY;
      if (pitch > 89.0f)
      {
        pitch = 89.0f;
      }
      else if (pitch < -89.0f)
      {
        pitch = -89.0f;
      }

      var x = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
      var y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
      var z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
      At = Vector3.Normalize(new Vector3(x, y, z));
    }
    
    public override string ToString()
    {
      string retorno;
      retorno = "__ CameraPerspective: " + "\n";
      retorno += "eye [" + eye.X + "," + eye.Y + "," + eye.Z + "]" + "\n";
      retorno += "at [" + at.X + "," + at.Y + "," + at.Z + "]" + "\n";
      retorno += "up [" + up.X + "," + up.Y + "," + up.Z + "]" + "\n";
      retorno += "near: " + near + "\n";
      retorno += "far: " + far + "\n";
      retorno += "fovy: " + fovy + "\n";
      retorno += "aspect: " + aspect + "\n";
      return (retorno);
    }

  }
}