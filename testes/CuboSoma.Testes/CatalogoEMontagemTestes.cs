using Conde.CuboSoma;
using Xunit;

namespace Conde.CuboSoma.Testes;

public class CatalogoEMontagemTestes
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 8)]
    [InlineData(5, 29)]
    public void AQuantidadeDePolicubosDeCadaTamanho(int tamanho, int esperado)
    {
        Assert.Equal(esperado, Catalogo.Policubos(tamanho).Count);
    }

    [Fact]
    public void NenhumPolicuboApareceDuasVezes()
    {
        foreach (var tamanho in new[] { 3, 4, 5 })
        {
            var todos = Catalogo.Policubos(tamanho);
            Assert.Equal(todos.Count, todos.Distinct().Count());
            Assert.All(todos, p => Assert.Equal(tamanho, p.Tamanho));
        }
    }

    [Fact]
    public void TodoPolicuboEstaNaFormaCanonica()
    {
        foreach (var tamanho in new[] { 3, 4 })
        {
            Assert.All(Catalogo.Policubos(tamanho), p => Assert.Equal(p, p.Canonica()));
        }
    }

    [Fact]
    public void TodoPolicuboEGrudado()
    {
        foreach (var peca in Catalogo.Policubos(5))
        {
            var vistos = new HashSet<(int, int, int)> { peca.Cubos[0] };
            var fila = new Queue<(int X, int Y, int Z)>();
            fila.Enqueue(peca.Cubos[0]);

            while (fila.Count > 0)
            {
                var c = fila.Dequeue();
                foreach (var v in new[]
                {
                    (c.X + 1, c.Y, c.Z), (c.X - 1, c.Y, c.Z),
                    (c.X, c.Y + 1, c.Z), (c.X, c.Y - 1, c.Z),
                    (c.X, c.Y, c.Z + 1), (c.X, c.Y, c.Z - 1),
                })
                {
                    if (peca.Cubos.Contains(v) && vistos.Add(v))
                    {
                        fila.Enqueue(v);
                    }
                }
            }

            Assert.Equal(peca.Tamanho, vistos.Count);
        }
    }

    [Fact]
    public void SaoSetePecasESomamVinteESeteCubinhos()
    {
        var pecas = Catalogo.Soma();
        Assert.Equal(7, pecas.Count);
        Assert.Equal(27, pecas.Sum(p => p.Tamanho));
        Assert.Equal(1, pecas.Count(p => p.Tamanho == 3));
        Assert.Equal(6, pecas.Count(p => p.Tamanho == 4));
    }

    [Fact]
    public void NenhumaPecaDoSomaEBlocoMacico()
    {
        Assert.All(Catalogo.Soma(), p => Assert.False(p.EhBloco()));
    }

    [Fact]
    public void OsTresBlocosQueFicamDeForaSaoOsPalitosEOQuadrado()
    {
        var blocos = Catalogo.Blocos();
        Assert.Equal(3, blocos.Count);
        Assert.Equal(1, blocos.Count(b => b.Tamanho == 3));
        Assert.Equal(2, blocos.Count(b => b.Tamanho == 4));
        Assert.All(blocos, b => Assert.True(b.EhBloco()));
    }

    [Fact]
    public void OCuboTemOnzeMilQuinhentasEVinteMontagens()
    {
        Assert.Equal(11520, new Montagem(Catalogo.Soma(), 3, 3, 3).Contar());
    }

    [Fact]
    public void AMontagemAchadaUsaCadaPecaUmaVez()
    {
        var montagem = new Montagem(Catalogo.Soma(), 3, 3, 3);
        var resposta = montagem.Primeira();
        Assert.NotNull(resposta);
        Assert.All(resposta!, dono => Assert.InRange(dono, 0, 6));

        var pecas = Catalogo.Soma();
        for (var i = 0; i < 7; i++)
        {
            Assert.Equal(pecas[i].Tamanho, resposta!.Count(d => d == i));
        }
    }

    [Fact]
    public void AsCaixasPlanasNaoTemMontagemNenhuma()
    {
        foreach (var (l, a, p) in new[] { (1, 3, 9), (1, 9, 3), (3, 9, 1) })
        {
            Assert.Equal(0, new Montagem(Catalogo.Soma(), l, a, p).Contar());
        }
    }

    [Fact]
    public void TresDasSetePecasNaoCabemNumaCaixaDeEspessuraUm()
    {
        var naoPlanas = Catalogo.Soma().Count(p =>
        {
            for (var giro = 0; giro < 24; giro++)
            {
                var g = p.Girada(giro);
                if (g.Cubos.Max(c => c.Z) == 0)
                {
                    return false;
                }
            }

            return true;
        });

        // Sao tres, e e por isso que nenhuma caixa plana de 27 celulas fecha.
        Assert.Equal(3, naoPlanas);
    }

    [Fact]
    public void ArgumentosInvalidosNaoSaoAceitos()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Catalogo.Policubos(0));
        Assert.Throws<ArgumentNullException>(() => new Montagem(null!, 3, 3, 3));
    }
}
