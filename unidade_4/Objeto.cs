/**
  Autor: Dalton Solano dos Reis
**/

using System;
using System.Collections.Generic;
using System.IO;
using CG_Biblioteca;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CG_N4
{
    public abstract class Objeto
    {
        public char Rotulo { get; }
        public Cor ObjetoCor { get; set; } = new Cor();
        public Textura Textura;
        public PrimitiveType PrimitivaTipo { get; set; } = PrimitiveType.LineLoop;
        public float PrimitivaTamanho { get; set; } = 1;

        public readonly BBox BBox = new BBox();
        public readonly ForcaFisica ForcaFisica;
        public Colisor Colisor { get; protected set; }

        public Object Pai { get; }
        private List<Objeto> Filhos = new List<Objeto>();

        private Transformacao4D MatrizTransformacao = Transformacao4DFactory.Get();

        public Objeto(char rotulo, Objeto paiRef)
        {
            Rotulo = rotulo;
            Pai = paiRef;
            ForcaFisica = new ForcaFisica(this);
        }

        public void Desenhar()
        {
            GL.PushMatrix();
            GL.MultMatrix(MatrizTransformacao.ObterDados());
            GL.Color3(ObjetoCor.CorR, ObjetoCor.CorG, ObjetoCor.CorB);
            GL.LineWidth(PrimitivaTamanho);
            GL.PointSize(PrimitivaTamanho);

            Textura?.Aplicar();
            DesenharGeometria();
            Textura?.Remover();

            foreach (var filho in Filhos)
            {
                filho.Desenhar();
            }

            GL.PopMatrix();
        }

        protected abstract void DesenharGeometria();

        public void FilhoAdicionar(Objeto filho)
        {
            Filhos.Add(filho);
        }

        public void FilhoRemover(Objeto filho)
        {
            Filhos.Remove(filho);
        }

        public IReadOnlyList<Objeto> GetFilhos()
        {
            return Filhos.AsReadOnly();
        }

        public void ImprimirMatrizTransformacao() => Console.WriteLine(MatrizTransformacao);

        public void AtribuirMatrizIdentidade()
        {
            MatrizTransformacao.AtribuirIdentidade();
            BBox.Atribuir(0.0, 0.0, 0.0);
            BBox.ProcessarCentro();
        }

        public void AtribuirTransformacao(Transformacao4D transformacao)
        {
            Transformacao4DFactory.Offer(MatrizTransformacao);
            MatrizTransformacao = transformacao;
        }

        public void Translacao(double tx, double ty, double tz)
        {
            Transformacao4D tmp = Transformacao4DFactory.Get();

            tmp.AtribuirTranslacao(tx, ty, tz);
            MatrizTransformacao.MultiplicarMatriz(tmp);

            Transformacao4DFactory.Offer(tmp);

            // aplica a translaçao na BBox
            BBox.Translacao(tx, ty, tz);
        }

        public void Rotacao(EixoRotacao eixoRotacao, double angulo)
        {
            Transformacao4D tmp = Transformacao4DFactory.Get();

            AplicarRotacao(tmp, eixoRotacao, angulo);
            MatrizTransformacao.MultiplicarMatriz(tmp);

            Transformacao4DFactory.Offer(tmp);
        }

        public void Escala(double sX, double sY, double sZ)
        {
            Transformacao4D tmp = Transformacao4DFactory.Get();

            tmp.AtribuirEscala(sX, sY, sZ);
            MatrizTransformacao.MultiplicarMatriz(tmp);

            Transformacao4DFactory.Offer(tmp);
        }

        public void RotacaoOrigem(EixoRotacao eixo, double angulo)
        {
            Transformacao4D tmp = Transformacao4DFactory.Get();
            Transformacao4D acumuladora = Transformacao4DFactory.Get();

            Vector3 pontoPivo = BBox.obterCentro.asVector3() - Vector3.Zero;
            acumuladora.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal

            AplicarRotacao(tmp, eixo, angulo);
            acumuladora.MultiplicarMatriz(tmp);

            tmp.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
            acumuladora.MultiplicarMatriz(tmp);

            MatrizTransformacao.MultiplicarMatriz(acumuladora);

            Transformacao4DFactory.Offer(tmp);
            Transformacao4DFactory.Offer(acumuladora);
        }

        public void EscalaOrigem(double sX, double sY, double sZ)
        {
            Transformacao4D tmp = Transformacao4DFactory.Get();
            Transformacao4D acumuladora = Transformacao4DFactory.Get();

            Ponto4D pontoPivo = BBox.obterCentro;
            acumuladora.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal

            tmp.AtribuirEscala(sX, sY, sZ);
            acumuladora.MultiplicarMatriz(tmp);

            tmp.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
            acumuladora.MultiplicarMatriz(tmp);

            MatrizTransformacao.MultiplicarMatriz(acumuladora);

            Transformacao4DFactory.Offer(tmp);
            Transformacao4DFactory.Offer(acumuladora);
        }

        private void AplicarRotacao(Transformacao4D matriz, EixoRotacao eixoRotacao, double angulo)
        {
            switch (eixoRotacao)
            {
                case EixoRotacao.X:
                    matriz.AtribuirRotacaoX(Transformacao4D.DEG_TO_RAD * angulo);
                    break;

                case EixoRotacao.Y:
                    matriz.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
                    break;

                case EixoRotacao.Z:
                    matriz.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
                    break;
            }
        }

        public void UpdateFrame(FrameEventArgs e)
        {
            OnUpdateFrame(e);
            foreach (Objeto filho in Filhos)
            {
                filho.UpdateFrame(e);
            }
        }

        protected virtual void OnUpdateFrame(FrameEventArgs e)
        {
            Console.WriteLine(ForcaFisica);

            // soma a aceleração na velocidade
            ForcaFisica.Velocidade += ForcaFisica.Aceleracao;
            ForcaFisica.Aceleracao = Vector3.Zero;

            // calcula o deslocamento (cm) dentro do tempo do frame
            Vector3 deslocamento = ForcaFisica.Velocidade * (float)e.Time;

            // adiciona o deslocamento no objeto
            Translacao(deslocamento.X, deslocamento.Y, deslocamento.Z);
        }

        public virtual void OnColisao(EventoColisao e)
        {
        }
    }
}