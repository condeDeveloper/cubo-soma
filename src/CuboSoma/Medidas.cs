namespace Conde.CuboSoma;

/// <summary>
/// As tabelas do README. Cada número sai de um método daqui e está preso num
/// teste; só entra aqui o que eu vi o programa imprimir.
/// </summary>
public static class Medidas
{
    public readonly record struct LinhaPolicubo(int Tamanho, int Formas, int Blocos, int NoSoma);

    public readonly record struct LinhaPeca(int Indice, int Cubinhos, int Simetrias, int Colocacoes, string Forma);

    public readonly record struct LinhaCaixa(string Nome, int Largura, int Altura, int Profundidade, long Montagens, long Nos, long Colocacoes);

    /// <summary>Quantos policubos existem de cada tamanho, e quantos são blocos.</summary>
    public static List<LinhaPolicubo> Policubos(IEnumerable<int> tamanhos)
    {
        var linhas = new List<LinhaPolicubo>();
        foreach (var t in tamanhos)
        {
            var todos = Catalogo.Policubos(t);
            var blocos = todos.Count(p => p.EhBloco());
            linhas.Add(new LinhaPolicubo(t, todos.Count, blocos, t is 3 or 4 ? todos.Count - blocos : 0));
        }

        return linhas;
    }

    /// <summary>As sete peças, com as simetrias e as colocações no cubo.</summary>
    public static List<LinhaPeca> Pecas()
    {
        var pecas = Catalogo.Soma();
        var montagem = new Montagem(pecas, 3, 3, 3);
        var colocacoes = montagem.ColocacoesPorPeca();

        var linhas = new List<LinhaPeca>();
        for (var i = 0; i < pecas.Count; i++)
        {
            linhas.Add(new LinhaPeca(i, pecas[i].Tamanho, pecas[i].Simetrias(), colocacoes[i], pecas[i].ToString()));
        }

        return linhas;
    }

    /// <summary>O total de cubinhos das sete peças.</summary>
    public static int Cubinhos() => Catalogo.Soma().Sum(p => p.Tamanho);

    /// <summary>
    /// As caixas de 27 células que as sete peças conseguem preencher, e as que
    /// não conseguem.
    /// </summary>
    public static List<LinhaCaixa> Caixas()
    {
        var pecas = Catalogo.Soma();
        var linhas = new List<LinhaCaixa>();

        foreach (var (nome, l, a, p) in new (string, int, int, int)[]
        {
            ("cubo 3x3x3", 3, 3, 3),
            ("torre 1x3x9", 1, 3, 9),
            ("parede 1x9x3", 1, 9, 3),
            ("tijolo 3x9x1", 3, 9, 1),
            ("caixa 1x27x1", 1, 27, 1),
        })
        {
            var montagem = new Montagem(pecas, l, a, p);
            var total = montagem.Contar();
            linhas.Add(new LinhaCaixa(nome, l, a, p, total, montagem.Nos, montagem.Colocacoes));
        }

        return linhas;
    }

    /// <summary>
    /// Em quantas das 11.520 montagens cada peca ocupa o centro do cubo.
    /// </summary>
    public static List<(int Peca, int Cubinhos, long NoCentro, double Fracao)> Centro()
    {
        var pecas = Catalogo.Soma();
        var montagem = new Montagem(pecas, 3, 3, 3);
        var donos = montagem.DonosDa(montagem.Indice(1, 1, 1));
        var total = donos.Sum();

        var linhas = new List<(int, int, long, double)>();
        for (var i = 0; i < pecas.Count; i++)
        {
            linhas.Add((i, pecas[i].Tamanho, donos[i], Math.Round(100.0 * donos[i] / total, 1)));
        }

        return linhas;
    }

    /// <summary>
    /// As montagens do cubo divididas pelas 48 simetrias do cubo (24 rotações e
    /// os espelhos). A divisão ser exata é o que prova que nenhuma montagem é
    /// simétrica em si mesma.
    /// </summary>
    public static (long Total, int Simetrias, long Essenciais, bool DivisaoExata) Essenciais()
    {
        var montagem = new Montagem(Catalogo.Soma(), 3, 3, 3);
        var total = montagem.Contar();
        return (total, 48, total / 48, total % 48 == 0);
    }
}
