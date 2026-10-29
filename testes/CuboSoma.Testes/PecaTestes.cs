using Conde.CuboSoma;
using Xunit;

namespace Conde.CuboSoma.Testes;

public class PecaTestes
{
    [Fact]
    public void AsVinteEQuatroRotacoesSaoVinteEQuatroTransformacoesDiferentes()
    {
        // Se duas delas fossem a mesma, o catalogo contaria pecas a menos e
        // ninguem reclamaria. Este teste e a unica coisa que garante o 24.
        var pontos = new[] { (1, 0, 0), (0, 1, 0), (0, 0, 1), (1, 2, 3) };
        var vistas = new HashSet<string>();

        for (var giro = 0; giro < 24; giro++)
        {
            var imagem = string.Join("|", pontos.Select(p => Peca.Girar(giro, p)));
            Assert.True(vistas.Add(imagem), $"o giro {giro} repete outro");
        }

        Assert.Equal(24, vistas.Count);
    }

    [Fact]
    public void TodaRotacaoPreservaDistanciaDaOrigem()
    {
        for (var giro = 0; giro < 24; giro++)
        {
            var (x, y, z) = Peca.Girar(giro, (1, 2, 3));
            Assert.Equal(14, (x * x) + (y * y) + (z * z));
        }
    }

    [Fact]
    public void ORotacaoZeroEAIdentidade()
    {
        Assert.Equal((1, 2, 3), Peca.Girar(0, (1, 2, 3)));
    }

    [Fact]
    public void APecaEGuardadaEncostadaNaOrigem()
    {
        var peca = new Peca([(5, 5, 5), (6, 5, 5)]);
        Assert.Contains((0, 0, 0), peca.Cubos);
        Assert.Equal(2, peca.Tamanho);
    }

    [Fact]
    public void CubinhoRepetidoNaoContaDuasVezes()
    {
        Assert.Equal(1, new Peca([(0, 0, 0), (0, 0, 0)]).Tamanho);
    }

    [Fact]
    public void ACanonicaEAMesmaParaTodasAsRotacoesDaMesmaPeca()
    {
        var peca = new Peca([(0, 0, 0), (1, 0, 0), (0, 1, 0)]);
        var canonica = peca.Canonica();
        for (var giro = 0; giro < 24; giro++)
        {
            Assert.Equal(canonica, peca.Girada(giro).Canonica());
        }
    }

    [Theory]
    [InlineData(1, 24)]
    // O dominozinho: as 4 rotacoes em volta do proprio eixo mais as 4 que o
    // viram de ponta-cabeca, que o devolvem igual porque a peca e normalizada.
    [InlineData(2, 8)]
    public void AsSimetriasDosCasosObvios(int tamanho, int esperado)
    {
        var peca = tamanho == 1
            ? new Peca([(0, 0, 0)])
            : new Peca([(0, 0, 0), (1, 0, 0)]);
        Assert.Equal(esperado, peca.Simetrias());
    }

    [Fact]
    public void ASimetriaDivideAsVinteEQuatro()
    {
        foreach (var peca in Catalogo.Soma())
        {
            Assert.Equal(0, 24 % peca.Simetrias());
        }
    }

    [Theory]
    [InlineData(1, 1, 1, true)]
    [InlineData(3, 1, 1, true)]
    [InlineData(2, 2, 1, true)]
    [InlineData(2, 2, 2, true)]
    public void BlocosMacicosSaoReconhecidos(int l, int a, int p, bool esperado)
    {
        var cubos = new List<(int, int, int)>();
        for (var x = 0; x < l; x++)
        {
            for (var y = 0; y < a; y++)
            {
                for (var z = 0; z < p; z++)
                {
                    cubos.Add((x, y, z));
                }
            }
        }

        Assert.Equal(esperado, new Peca(cubos).EhBloco());
    }

    [Fact]
    public void UmaPecaTortaNaoEBloco()
    {
        Assert.False(new Peca([(0, 0, 0), (1, 0, 0), (0, 1, 0)]).EhBloco());
    }

    [Fact]
    public void PecasIguaisSaoIguais()
    {
        var a = new Peca([(0, 0, 0), (1, 0, 0)]);
        var b = new Peca([(1, 0, 0), (0, 0, 0)]);
        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.False(a.Equals(null));
        Assert.False(a.Equals("texto"));
    }

    [Fact]
    public void ArgumentosInvalidosNaoSaoAceitos()
    {
        Assert.Throws<ArgumentNullException>(() => new Peca(null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => Peca.Girar(-1, (0, 0, 0)));
        Assert.Throws<ArgumentOutOfRangeException>(() => Peca.Girar(24, (0, 0, 0)));
    }
}
