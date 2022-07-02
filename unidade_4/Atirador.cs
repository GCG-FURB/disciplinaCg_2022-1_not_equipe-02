using CG_Biblioteca;
using OpenTK;
using OpenTK.Input;

namespace CG_N4
{
    public class Atirador
    {
        private const float Sensibilidade = 100.0f;
        
        public CameraPerspective Camera { get; set; }

        private EstadoAtirador _estado;
        private Vector3 _aceleracao;
        private Esfera _bocha;

        public void OnUpdateFrame(FrameEventArgs e)
        {
            if (_estado == EstadoAtirador.INICIO)
            {
                Posicionar();
            }

            if (_estado == EstadoAtirador.POSICIONAR)
            {
                KeyboardState state = Keyboard.GetState();
                if (state.IsKeyDown(Key.A))
                {
                    _bocha.Translacao(0, 0,  -(Sensibilidade * e.Time));
                }

                if (state.IsKeyDown(Key.D))
                {
                    _bocha.Translacao(0, 0,  Sensibilidade * e.Time);
                }

                if (state.IsKeyDown(Key.W))
                {
                    _bocha.Translacao(Sensibilidade * e.Time, 0,  0);
                }

                if (state.IsKeyDown(Key.S))
                {
                    _bocha.Translacao(-(Sensibilidade * e.Time), 0,  0);
                }

                GarantirPosicaoCorreta();

                if (state.IsKeyDown(Key.Enter))
                {
                    ProximoEstado();
                }
            }

            if (_estado == EstadoAtirador.ANGULO)
            {
                
            }
        }

        private void GarantirPosicaoCorreta()
        {
            BBox bbox = _bocha.BBox;
            if (bbox.obterMenorZ < 0)
            {
                _bocha.Translacao(0, 0, -bbox.obterMenorZ);
            }

            if (bbox.obterMaiorZ > Utilitario.MetrosEmPixels(2.5d))
            {
                _bocha.Translacao(0, 0, Utilitario.MetrosEmPixels(2.5d) - bbox.obterMaiorZ);
            }

            if (bbox.obterMenorX < 0)
            {
                _bocha.Translacao(-bbox.obterMenorX, 0, 0);
            }

            if (bbox.obterMaiorX > Utilitario.MetrosEmPixels(1d))
            {
                _bocha.Translacao(Utilitario.MetrosEmPixels(1d) - bbox.obterMaiorX, 0, 0);
            }
            
            // if (_transalacao.Z > Utilitario.MetrosEmPixels(2.5d))
            // {
            //     _transalacao.Z = (float)Utilitario.MetrosEmPixels(2.5d);
            // }
            //
            // if (_transalacao.X < 0)
            // {
            //     _transalacao.X = 0;
            // }
            //
            // if (_transalacao.X > Utilitario.MetrosEmPixels(1.0d))
            // {
            //     _transalacao.X = (float)Utilitario.MetrosEmPixels(1.0d);
            // }
        }

        private void ProximoEstado()
        {
            switch (_estado)
            {
                case EstadoAtirador.POSICIONAR:
                    Angulo();
                    break;
                case EstadoAtirador.ANGULO:
                    // ProcessarTiro();
                    break;
                case EstadoAtirador.ATIRAR:
                    Posicionar();
                    break;
            }
        }

        private void Angulo()
        {
            _estado = EstadoAtirador.ANGULO;
        }

        private void Posicionar()
        {
            _estado = EstadoAtirador.POSICIONAR;
            _bocha = BolaFactory.BuildBocha(Jogo.Instance.GetTimeAtual());
            _bocha.Translacao(0, 0, (float)Utilitario.MetrosEmPixels(1.25d));

            Mundo.GetInstance().ObjetosAdicionar(_bocha);

            Camera.Eye = new Vector3(
                0,
                (float)Utilitario.MetrosEmPixels(2.5d),
                (float)Utilitario.MetrosEmPixels(1.25d)
            );
            Camera.At = new Vector3(0.0175f, -0.9998477f, 0.0f);
        }
    }

    enum EstadoAtirador
    {
        INICIO,
        POSICIONAR,
        ANGULO,
        ATIRAR,
    }
}