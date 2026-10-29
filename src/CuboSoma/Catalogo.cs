namespace Conde.CuboSoma;

/// <summary>
/// As sete peças do Soma, <b>derivadas</b> e não digitadas.
///
/// A definição é esta e só esta: <i>todos os policubos de 3 ou 4 cubinhos que
/// não são blocos maciços</i>. Os policubos saem de um crescimento — um
/// cubinho, depois dois, depois três — com a forma canônica jogando fora o que
/// repete. Dos blocos, saem o palito de três, o palito de quatro e o quadrado
/// de quatro; sobram sete peças.
///
/// E sete peças de 3 e 4 cubinhos somam <b>27</b>, que é exatamente o cubo de
/// lado 3. Essa soma é o motivo de o quebra-cabeça existir, e ela não é
/// imposta aqui: ela sai da conta.
/// </summary>
public static class Catalogo
{
    /// <summary>Todos os policubos de um tamanho, um por forma.</summary>
    public static List<Peca> Policubos(int tamanho)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(tamanho, 1);

        var atual = new HashSet<Peca> { new([(0, 0, 0)]) };

        for (var n = 2; n <= tamanho; n++)
        {
            var seguinte = new HashSet<Peca>();
            foreach (var peca in atual)
            {
                foreach (var cubo in peca.Cubos)
                {
                    foreach (var vizinho in Vizinhos(cubo))
                    {
                        if (peca.Cubos.Contains(vizinho))
                        {
                            continue;
                        }

                        var maior = new Peca(peca.Cubos.Append(vizinho));
                        seguinte.Add(maior.Canonica());
                    }
                }
            }

            atual = seguinte;
        }

        return [.. atual.OrderBy(p => p.ToString(), StringComparer.Ordinal)];
    }

    private static IEnumerable<(int X, int Y, int Z)> Vizinhos((int X, int Y, int Z) c)
    {
        yield return (c.X + 1, c.Y, c.Z);
        yield return (c.X - 1, c.Y, c.Z);
        yield return (c.X, c.Y + 1, c.Z);
        yield return (c.X, c.Y - 1, c.Z);
        yield return (c.X, c.Y, c.Z + 1);
        yield return (c.X, c.Y, c.Z - 1);
    }

    /// <summary>As sete peças do Soma, pela definição.</summary>
    public static List<Peca> Soma()
    {
        var pecas = new List<Peca>();
        foreach (var tamanho in new[] { 3, 4 })
        {
            pecas.AddRange(Policubos(tamanho).Where(p => !p.EhBloco()));
        }

        return pecas;
    }

    /// <summary>Os policubos que ficam de fora por serem blocos maciços.</summary>
    public static List<Peca> Blocos()
    {
        var blocos = new List<Peca>();
        foreach (var tamanho in new[] { 3, 4 })
        {
            blocos.AddRange(Policubos(tamanho).Where(p => p.EhBloco()));
        }

        return blocos;
    }
}
