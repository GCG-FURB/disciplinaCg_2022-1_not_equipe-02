using System;
using System.Collections.Generic;
using CG_Biblioteca;

namespace CG_N4
{
    public class Jogo
    {
        public static readonly Jogo Instance = new Jogo();

        public readonly List<Time> Times = new List<Time>();
        private int _timeAtual;

        private Jogo()
        {
            Times.Add(new Time(new Cor(255, 217, 61)));
            Times.Add(new Time(new Cor(43, 43, 255)));
        }

        public void Iniciar()
        {
            ValidarJogo();
            _timeAtual = new Random().Next(0, 1);
        }

        public void Resetar()
        {
            _timeAtual = 0;
            foreach (Time time in Times)
            {
                time.Resetar();
            }
        }

        public void AdicionarJogador(int time, string jogador)
        {
            Times[time].AdicionarJogador(jogador);
        }

        public Time GetTimeAtual()
        {
            // Time time = Times[_timeAtual];
            //
            // _timeAtual++;
            // if (_timeAtual == Times.Count)
            // {
            //     _timeAtual = 0;
            // }
            //
            // return time;
            return Times[_timeAtual];
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
    }

    public class Time
    {
        private readonly List<string> _jogadores = new List<string>();
        private int _jogadorAtual;

        public readonly Cor CorBola;
        public IReadOnlyList<string> Jogadores => _jogadores.AsReadOnly();

        public int pontos;

        public Time(Cor corBola)
        {
            CorBola = corBola;
        }

        public void Resetar()
        {
            pontos = 0;
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

    }
}