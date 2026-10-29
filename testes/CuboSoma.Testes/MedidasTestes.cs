using Conde.CuboSoma;
using Xunit;

namespace Conde.CuboSoma.Testes;

/// <summary>
/// Os números do README presos aqui. Só entra o que eu vi o medidor imprimir.
/// </summary>
public class MedidasTestes
{
    [Theory]
    [InlineData(1, 1, 1, 0)]
    [InlineData(2, 1, 1, 0)]
    [InlineData(3, 2, 1, 1)]
    [InlineData(4, 8, 2, 6)]
    [InlineData(5, 29, 1, 0)]
    public void ATabelaDosPolicubos(int tamanho, int formas, int blocos, int noSoma)
    {
        var l = Medidas.Policubos([tamanho])[0];
        Assert.Equal(formas, l.Formas);
        Assert.Equal(blocos, l.Blocos);
        Assert.Equal(noSoma, l.NoSoma);
    }

    [Fact]
    public void OsVinteESeteCubinhosSaemDaConta()
    {
        Assert.Equal(27, Medidas.Cubinhos());
        Assert.Equal(3 * 3 * 3, Medidas.Cubinhos());
    }

    [Theory]
    [InlineData(0, 3, 2, 144)]
    [InlineData(1, 4, 1, 144)]
    [InlineData(2, 4, 2, 72)]
    [InlineData(3, 4, 3, 64)]
    [InlineData(4, 4, 2, 96)]
    [InlineData(5, 4, 2, 96)]
    [InlineData(6, 4, 2, 72)]
    public void ATabelaDasPecas(int indice, int cubinhos, int simetrias, int colocacoes)
    {
        var l = Medidas.Pecas()[indice];
        Assert.Equal(cubinhos, l.Cubinhos);
        Assert.Equal(simetrias, l.Simetrias);
        Assert.Equal(colocacoes, l.Colocacoes);
    }

    [Fact]
    public void AsColocacoesSomamSeiscentasEOitentaEOito()
    {
        Assert.Equal(688, Medidas.Pecas().Sum(l => l.Colocacoes));
    }

    [Fact]
    public void APecaMaisTravadaEADeTresBracos()
    {
        var pecas = Medidas.Pecas();
        var menos = pecas.MinBy(l => l.Colocacoes);
        Assert.Equal(3, menos.Indice);
        Assert.Equal(64, menos.Colocacoes);
        Assert.Equal(3, menos.Simetrias);
    }

    [Fact]
    public void ADivisaoPelasQuarentaEOitoSimetriasEExata()
    {
        var (total, simetrias, essenciais, exata) = Medidas.Essenciais();
        Assert.Equal(11520, total);
        Assert.Equal(48, simetrias);
        Assert.Equal(240, essenciais);
        Assert.True(exata);
    }

    [Theory]
    [InlineData("cubo 3x3x3", 11520, 861285, 688)]
    [InlineData("torre 1x3x9", 0, 1696, 240)]
    [InlineData("parede 1x9x3", 0, 143, 240)]
    [InlineData("tijolo 3x9x1", 0, 1696, 240)]
    [InlineData("caixa 1x27x1", 0, 1, 0)]
    public void ATabelaDasCaixas(string nome, long montagens, long nos, long colocacoes)
    {
        var l = Medidas.Caixas().Single(x => x.Nome == nome);
        Assert.Equal(montagens, l.Montagens);
        Assert.Equal(nos, l.Nos);
        Assert.Equal(colocacoes, l.Colocacoes);
    }

    [Fact]
    public void SoOCuboEMontavel()
    {
        var montaveis = Medidas.Caixas().Where(c => c.Montagens > 0).ToList();
        Assert.Single(montaveis);
        Assert.Equal("cubo 3x3x3", montaveis[0].Nome);
    }

    [Theory]
    [InlineData(0, 912, 7.9)]
    [InlineData(1, 0, 0.0)]
    [InlineData(2, 0, 0.0)]
    [InlineData(3, 1776, 15.4)]
    [InlineData(4, 3624, 31.5)]
    [InlineData(5, 3624, 31.5)]
    [InlineData(6, 1584, 13.8)]
    public void ATabelaDoCentro(int peca, long noCentro, double fracao)
    {
        var l = Medidas.Centro().Single(x => x.Peca == peca);
        Assert.Equal(noCentro, l.NoCentro);
        Assert.Equal(fracao, l.Fracao);
    }

    [Fact]
    public void DuasPecasNuncaOcupamOCentro()
    {
        var nunca = Medidas.Centro().Where(l => l.NoCentro == 0).Select(l => l.Peca).ToList();
        Assert.Equal([1, 2], nunca);
    }

    [Fact]
    public void AsDuasPecasEspelhadasEmpatamNoCentro()
    {
        var centro = Medidas.Centro();
        Assert.Equal(centro.Single(l => l.Peca == 4).NoCentro, centro.Single(l => l.Peca == 5).NoCentro);
    }

    [Fact]
    public void AContagemDoCentroSomaAsOnzeMilQuinhentasEVinte()
    {
        Assert.Equal(11520, Medidas.Centro().Sum(l => l.NoCentro));
    }
}
