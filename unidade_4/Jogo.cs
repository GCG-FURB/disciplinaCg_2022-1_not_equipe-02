using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_Biblioteca;

namespace CG_N4
{
    public class Jogo
    {
        
        public static readonly float LinhaLancamentoXMinimo = 0.0f;
        public static readonly float LinhaLancamentoXMaximo = (float)Utilitario.MetrosEmPixels(1.0d);

        public static readonly float LinhaLancamentoZMinimo = 0.0f;
        public static readonly float LinhaLancamentoZMaximo = (float)Utilitario.MetrosEmPixels(2.5d);

        public static readonly Jogo Instance = new Jogo();

        public readonly List<Time> Times = new List<Time>();
        public Esfera BolaAtual { get; protected set; }

        private int _timeAtual;
        private TipoBola _tipoBola = TipoBola.BOLIN;

        private Jogo()
        {
            Times.Add(new Time(new Cor(255, 217, 61)));
            Times.Add(new Time(new Cor(43, 43, 255)));
        }

        public void Iniciar()
        {
            ValidarJogo();
            _timeAtual = new Random().Next(0, 1);

            Esfera bolin = BolaFactory.BuildBolin();
            BolaAtual = bolin;
            Atirador.Instance.Iniciar(bolin);
        }

        public void Resetar()
        {
            _timeAtual = 0;
            _tipoBola = TipoBola.BOLIN;
            foreach (Time time in Times)
            {
                time.Resetar();
            }
            ResetarBolas();
        }

        public void AdicionarJogador(int time, string jogador)
        {
            Times[time].AdicionarJogador(jogador);
        }

        public Time GetTimeAtual()
        {
            return Times[_timeAtual];
        }

        public int GetIdxTimeAtual()
        {
            return _timeAtual;
        }

        public bool IsPodeIniciar() // TODO -> Usar para renderizar o botão
        {
            try
            {
                ValidarJogo();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ValidarJogo()
        {
            IReadOnlyList<string> jogadoresTimeA = Times[0].Jogadores;
            IReadOnlyList<string> jogadoresTimeB = Times[1].Jogadores;

            if (jogadoresTimeA.Count != jogadoresTimeB.Count)
            {
                throw new JogoException("Times desbalanceados, não possuem a mesma quantidade de jogadores.");
            }

            if (jogadoresTimeA.Count != 1
                && jogadoresTimeA.Count != 2
                && jogadoresTimeA.Count != 4)
            {
                throw new JogoException("É permitido somente times com 1, 2 ou 4 jogadores.");
            }
        }

        public void OnJogadaFinalizada()
        {
            IReadOnlyList<Objeto> objetos = Mundo.GetInstance().GetObjetos();

            Esfera bolin = GetBolin(objetos);
            if (bolin == null)
            {
                throw new JogoException("Estado do jogo inválido! Bolin não encontrado.");
            }

            Dictionary<float, Esfera> distanciaPorBocha = GetDistanciaPorBocha(objetos, bolin);
            if (distanciaPorBocha.Count == 0) // somente o bolin foi jogado
            {
                if (IsLancamentoQueimado(bolin))
                {
                    Atirador.Instance.Iniciar(bolin);
                    return;
                }

                NovaBocha();
                return;
            }
            
            float menorDistancia = distanciaPorBocha.Keys.Min();
            Esfera bochaMaisProxima = distanciaPorBocha[menorDistancia];

            int timeMaisProximo = GetTime(bochaMaisProxima);
            int outroTime = GetTime(bochaMaisProxima) == 0 ? 1 : 0;

            _timeAtual = Times[outroTime].Bolas > 0 ? outroTime : timeMaisProximo;

            if (ExisteBochaDisponivel())
            {
                NovaBocha();
            }
            else
            {
                int pontosRodada = 0;
                List<Esfera> esferasOrdenadasPorDistancia = GetEsferasOrdenadasPorDistancia(distanciaPorBocha);
                foreach (Esfera esfera in esferasOrdenadasPorDistancia)
                {
                    if (GetTime(esfera) == timeMaisProximo)
                    {
                        pontosRodada += 2;
                    }
                    else
                    {
                        break;
                    }
                }

                Times[timeMaisProximo].Pontos += pontosRodada;

                if (Times[timeMaisProximo].Pontos >= 24)
                {
                    string mensagem = "Time " + Times[timeMaisProximo].JogadoresToString() +
                                      " foi o vencedor\ncom " + Times[timeMaisProximo].Pontos +
                                      " pontos!";
                    Console.WriteLine(mensagem);
                    Mundo.GetInstance().TextoCentral = mensagem;
                    Mundo.GetInstance().PodeProcessarObjetos = false; // trava o jogo para parar tudo!
                    Resetar();
                    Iniciar();
                }
                else
                {
                    ProximaRodada();
                }
            }
        }

        private void ProximaRodada()
        {
            ResetarBolas();
            _tipoBola = TipoBola.BOLIN;
            Esfera bolin = BolaFactory.BuildBolin();
            BolaAtual = bolin;
            Atirador.Instance.Iniciar(bolin);
        }

        private void ResetarBolas()
        {
            foreach (Time time in Times)
            {
                time.Bolas = 4;
            }

            List<Objeto> objetosRemover = new List<Objeto>();
            foreach (Objeto objeto in Mundo.GetInstance().GetObjetos())
            {
                if (objeto.GetType().IsEquivalentTo(typeof(Esfera)))
                {
                    objetosRemover.Add(objeto);
                }
            }
            
            objetosRemover.ForEach(o => Mundo.GetInstance().ObjetosRemover(o));
        }

        private void NovaBocha()
        {
            Time timeAtual = GetTimeAtual();
            timeAtual.Bolas--;

            Esfera bocha = BolaFactory.BuildBocha(timeAtual);
            BolaAtual = bocha;
            Atirador.Instance.Iniciar(bocha);
        }

        private bool IsLancamentoQueimado(Esfera bolin)
        {
            return bolin.BBox.obterMenorX < LinhaLancamentoXMaximo;
        }

        private List<Esfera> GetEsferasOrdenadasPorDistancia(Dictionary<float,Esfera> distanciaPorBocha)
        {
            List<float> distancias = new List<float>(distanciaPorBocha.Keys);
            distancias.Sort();

            List<Esfera> esferas = new List<Esfera>(distanciaPorBocha.Count);
            foreach (float distancia in distancias)
            {
                esferas.Add(distanciaPorBocha[distancia]);
            }
            return esferas;
        }

        private bool ExisteBochaDisponivel()
        {
            foreach (Time time in Times)
            {
                if (time.Bolas > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private int GetTime(Esfera esfera)
        {
            return esfera.ObjetoCor.Equals(Times[0].CorBola) ? 0 : 1;
        }

        private static Dictionary<float, Esfera> GetDistanciaPorBocha(IReadOnlyList<Objeto> objetos, Esfera bolin)
        {
            Dictionary<float, Esfera> distanciaPorBocha = new Dictionary<float, Esfera>();
            foreach (Objeto objeto in objetos)
            {
                if (objeto != bolin && objeto.GetType().IsEquivalentTo(typeof(Esfera)))
                {
                    Esfera esfera = (Esfera)objeto;
                    float distancia = (float)Matematica.Distancia(esfera.BBox.obterCentro, bolin.BBox.obterCentro);
                    while (distanciaPorBocha.ContainsKey(distancia))
                    {
                        distancia += 0.00001f;
                    }

                    distanciaPorBocha.Add(distancia, esfera);
                }
            }

            return distanciaPorBocha;
        }

        public Esfera GetBolin(IReadOnlyList<Objeto> objetos)
        {
            foreach (Objeto objeto in objetos)
            {
                if (objeto.GetType().IsEquivalentTo(typeof(Esfera)))
                {
                    Esfera esfera = (Esfera)objeto;
                    if (esfera.Raio == BolaFactory.RaioBolin)
                    {
                        return esfera;
                    }
                }
            }

            return null;
        }
    }

    public class Time
    {
        private readonly List<string> _jogadores = new List<string>();
        private int _jogadorAtual;

        public readonly Cor CorBola;
        public IReadOnlyList<string> Jogadores => _jogadores.AsReadOnly();

        public int Bolas = 4;
        public int Pontos = 0;

        public Time(Cor corBola)
        {
            CorBola = corBola;
        }

        public void Resetar()
        {
            Pontos = 0;
            Bolas = 4;
            _jogadorAtual = 0;
            _jogadores.Clear();
        }

        public void AdicionarJogador(string jogador)
        {
            _jogadores.Add(jogador);
        }

        public string GetProximoJogador()
        {
            string jogador = _jogadores[_jogadorAtual];

            _jogadorAtual++;
            if (_jogadorAtual == _jogadores.Count)
            {
                _jogadorAtual = 0;
            }

            return jogador;
        }

        public string JogadoresToString()
        {
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < _jogadores.Count; i++)
            {
                sb.Append(_jogadores[i]);
                if (i + 1 != _jogadores.Count)
                {
                    sb.Append(", ");
                }
            }
            return sb.ToString();
        }
    }
}