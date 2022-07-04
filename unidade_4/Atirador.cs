using System;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace CG_N4
{
    public class Atirador
    {
        private static readonly float SensibilidadeMovimento = 100.0f;
        private static readonly float SensibilidadeAngulo = 20.0f;
        private static readonly float SensibilidadeForca = 800.0f;

        private static readonly float ForcaMinima = 1.0f;
        private static readonly float ForcaMaxima = 2000.0f;

        private static readonly float AnguloMinimo = 10.0f;
        private static readonly float AnguloMaximo = 170.0f;

        public static Atirador Instance = new Atirador();
        public CameraPerspective Camera { get; set; }

        private EstadoAtirador _estado;
        private Vector3 _aceleracao;
        private Esfera _esfera;
        private Mira _mira = new Mira();
        private float _forca = ForcaMinima;
        private float _angulo = 90.0f;

        private double _frameUltimoEnter;
        private double _frameAtirou;
        private double _frameAtual;

        public float Angulo => _angulo;
        public float Forca => _mira.Forca;

        public void Iniciar(Esfera esfera)
        {
            _estado = EstadoAtirador.POSICIONAR;
            _forca = (ForcaMaxima - ForcaMinima) / 2;
            _angulo = 90.0f;

            _esfera = esfera;
            _esfera.Translacao(0, _esfera.Raio, (float) Utilitario.MetrosEmPixels(1.25d));

            Ponto4D centro = _esfera.BBox.obterCentro;
            _esfera.Translacao(
                Jogo.LinhaLancamentoXMinimo - centro.X + _esfera.Raio,
                -centro.Y + _esfera.Raio,
                (Jogo.LinhaLancamentoZMaximo - Jogo.LinhaLancamentoZMinimo) / 2 - centro.Z + _esfera.Raio
            );

            Mundo.GetInstance().ObjetosAdicionar(_esfera);

            PosicionarCamera();
        }

        public void OnUpdateFrame(FrameEventArgs e, KeyboardDevice keyboard)
        {
            _frameAtual = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if (_estado == EstadoAtirador.INICIO)
            {
                ProximoEstado();
            }
            else if (_estado == EstadoAtirador.POSICIONAR)
            {
                if (keyboard[Key.A])
                {
                    _esfera.Translacao(0, 0, -(SensibilidadeMovimento * e.Time));
                }

                if (keyboard[Key.D])
                {
                    _esfera.Translacao(0, 0, SensibilidadeMovimento * e.Time);
                }

                if (keyboard[Key.W])
                {
                    _esfera.Translacao(SensibilidadeMovimento * e.Time, 0, 0);
                }

                if (keyboard[Key.S])
                {
                    _esfera.Translacao(-(SensibilidadeMovimento * e.Time), 0, 0);
                }

                GarantirPosicaoCorreta();
            }
            else if (_estado == EstadoAtirador.ANGULO)
            {
                if (keyboard[Key.A])
                {
                    _angulo -= (float) (SensibilidadeAngulo * e.Time);
                }

                if (keyboard[Key.D])
                {
                    _angulo += (float) (SensibilidadeAngulo * e.Time);
                }

                if (keyboard[Key.W])
                {
                    _forca += (float) (SensibilidadeForca * e.Time);
                }

                if (keyboard[Key.S])
                {
                    _forca -= (float) (SensibilidadeForca * e.Time);
                }

                GarantirForcaAnguloCorretos();
                ProcessarMira();
            }
            else if (_estado == EstadoAtirador.ATIRANDO)
            {
                if (IsTodasEsferasParadas())
                {
                    ProximoEstado();
                }
                else
                {
                    PosicionarCamera();
                }
            }
            else if (_estado == EstadoAtirador.ATIROU)
            {
                if (_frameAtual - _frameAtirou > 1000)
                {
                    ProximoEstado();
                }
            }

            if (_estado == EstadoAtirador.POSICIONAR || _estado == EstadoAtirador.ANGULO ||
                _estado == EstadoAtirador.ATIROU)
            {
                if ((_estado == EstadoAtirador.POSICIONAR || _estado == EstadoAtirador.ANGULO) && keyboard[Key.Escape])
                {
                    VoltarEstado();
                }

                if (keyboard[Key.Enter] || keyboard[Key.KeypadEnter])
                {
                    if (_frameAtual - _frameUltimoEnter > 250)
                    {
                        ProximoEstado();
                        _frameUltimoEnter = _frameAtual;
                    }
                }
            }
        }

        private bool IsTodasEsferasParadas()
        {
            IReadOnlyList<Objeto> objetos = Mundo.GetInstance().GetObjetos();
            foreach (Objeto objeto in objetos)
            {
                if (objeto.GetType().IsEquivalentTo(typeof(Esfera)))
                {
                    if (objeto.ForcaFisica.Velocidade != Vector3.Zero)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void GarantirPosicaoCorreta()
        {
            BBox bbox = _esfera.BBox;
            if (bbox.obterMenorZ < Jogo.LinhaLancamentoZMinimo)
            {
                _esfera.Translacao(0, 0, Jogo.LinhaLancamentoZMinimo - bbox.obterMenorZ);
            }

            if (bbox.obterMaiorZ > Jogo.LinhaLancamentoZMaximo)
            {
                _esfera.Translacao(0, 0, Jogo.LinhaLancamentoZMaximo - bbox.obterMaiorZ);
            }

            if (bbox.obterMenorX < Jogo.LinhaLancamentoXMinimo)
            {
                _esfera.Translacao(Jogo.LinhaLancamentoXMinimo - bbox.obterMenorX, 0, 0);
            }

            if (bbox.obterMaiorX > Jogo.LinhaLancamentoXMaximo)
            {
                _esfera.Translacao(Jogo.LinhaLancamentoXMaximo - bbox.obterMaiorX, 0, 0);
            }
        }

        private void GarantirForcaAnguloCorretos()
        {
            if (_forca < ForcaMinima)
            {
                _forca = ForcaMinima;
            }

            if (_forca > ForcaMaxima)
            {
                _forca = ForcaMaxima;
            }

            if (_angulo < AnguloMinimo)
            {
                _angulo = AnguloMinimo;
            }

            if (_angulo > AnguloMaximo)
            {
                _angulo = AnguloMaximo;
            }
        }

        private void ProximoEstado()
        {
            switch (_estado)
            {
                case EstadoAtirador.POSICIONAR:
                    _estado = EstadoAtirador.ANGULO;
                    NovaMira();
                    PosicionarCamera();
                    break;

                case EstadoAtirador.ANGULO:
                    _estado = EstadoAtirador.ATIRANDO;
                    ProcessarTiro();
                    Mundo.GetInstance().ObjetosRemover(_mira);
                    break;

                case EstadoAtirador.ATIRANDO:
                    _estado = EstadoAtirador.ATIROU;
                    PosicionarCamera();
                    Jogo.Instance.OnJogadaFinalizada();
                    break;

                case EstadoAtirador.ATIROU:
                    _estado = EstadoAtirador.POSICIONAR;
                    _frameAtirou = _frameAtual;
                    PosicionarCamera();
                    break;
            }
        }

        private void VoltarEstado()
        {
            switch (_estado)
            {
                case EstadoAtirador.ANGULO:
                    _estado = EstadoAtirador.POSICIONAR;
                    Mundo.GetInstance().ObjetosRemover(_mira);
                    PosicionarCamera();
                    break;

                case EstadoAtirador.ATIRANDO:
                    _estado = EstadoAtirador.ANGULO;
                    PosicionarCamera();
                    break;
            }
        }

        private void NovaMira()
        {
            Mundo.GetInstance().ObjetosAdicionar(_mira);
            _angulo = 90.0f;
            _forca = (ForcaMaxima - ForcaMinima) / 2;
        }

        private void PosicionarCamera()
        {
            if (_estado == EstadoAtirador.POSICIONAR)
            {
                Camera.Eye = new Vector3(
                    0,
                    (float) Utilitario.MetrosEmPixels(2.5d),
                    (float) Utilitario.MetrosEmPixels(1.25d)
                );
                Camera.At = new Vector3(0.35f, -0.9998477f, 0.0f);
            }
            else if (_estado == EstadoAtirador.ANGULO || _estado == EstadoAtirador.ATIRANDO)
            {
                Ponto4D centro = _esfera.BBox.obterCentro;
                Camera.Eye = new Vector3(
                    (float) (centro.X - Utilitario.MetrosEmPixels(1)),
                    (float) (centro.Y + Utilitario.CentimetrosEmPixels(80)),
                    (float) centro.Z
                );
                Camera.At = new Vector3(0.9265f, -0.38f, 0.0f);
            }
            else if (_estado == EstadoAtirador.ATIROU)
            {
                Ponto4D centro = _esfera.BBox.obterCentro;
                Camera.Eye = new Vector3(
                    (float) centro.X,
                    (float) (centro.Y + Utilitario.MetrosEmPixels(5)),
                    (float) centro.Z
                );
                Camera.At = new Vector3(0.018f, -0.9998477f, 0.0f);
            }
        }

        private void ProcessarMira()
        {
            _mira.Forca = _forca / ForcaMaxima;
            _mira.Angulo = _angulo + 270;
            _mira.Ponto = _esfera.BBox.obterCentro.asVector3();
        }

        private void ProcessarTiro()
        {
            Vector3 p1 = _mira.Ponto;
            Vector3 p2 = new Vector3(
                p1.X + (float) (1 * Math.Cos(Math.PI * _mira.Angulo / 180.0)),
                p1.Y,
                p1.Z + (float) (1 * Math.Sin(Math.PI * _mira.Angulo / 180.0))
            );

            float d = Matematica.Distancia(p1, p2);
            Vector3 n = (p2 - p1) / d;

            _esfera.ForcaFisica.Aceleracao = new Vector3(
                _forca * n.X,
                _forca * n.Y,
                _forca * n.Z
            );
        }

        public bool IsPosicionando()
        {
            return _estado == EstadoAtirador.POSICIONAR || _estado == EstadoAtirador.ANGULO;
        }
    }

    enum EstadoAtirador
    {
        INICIO,
        POSICIONAR,
        ANGULO,
        ATIRANDO,
        ATIROU
    }

    class Mira : Objeto
    {
        public float Angulo;
        public float Forca;
        public Vector3 Ponto;

        public Mira() : base(Utilitario.charProximo(), null)
        {
            PrimitivaTamanho = 10.0f;
            PrimitivaTipo = PrimitiveType.Lines;
        }

        protected override void DesenharGeometria()
        {
            double raio = Utilitario.CentimetrosEmPixels(80);
            float angulo = Angulo;

            float d1 = (float) (Forca * raio);
            float d2 = (float) (raio - d1);

            Vector3 pontoMeio = new Vector3(
                Ponto.X + (float) (d1 * Math.Cos(Math.PI * angulo / 180.0)),
                Ponto.Y,
                Ponto.Z + (float) (d1 * Math.Sin(Math.PI * angulo / 180.0))
            );

            Vector3 pontoFinal = new Vector3(
                pontoMeio.X + (float) (d2 * Math.Cos(Math.PI * angulo / 180.0)),
                pontoMeio.Y,
                pontoMeio.Z + (float) (d2 * Math.Sin(Math.PI * angulo / 180.0))
            );

            GL.Color3((byte) 0, (byte) 255, (byte) 0);
            GL.Begin(PrimitivaTipo);
            GL.Vertex3(Ponto.X, Ponto.Y, Ponto.Z);
            GL.Vertex3(pontoMeio.X, pontoMeio.Y, pontoMeio.Z);
            GL.End();

            GL.Color3((byte) 255, (byte) 0, (byte) 0);
            GL.Begin(PrimitivaTipo);
            GL.Vertex3(pontoMeio.X, pontoMeio.Y, pontoMeio.Z);
            GL.Vertex3(pontoFinal.X, pontoFinal.Y, pontoFinal.Z);
            GL.End();
        }
    }
}