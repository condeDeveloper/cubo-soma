namespace Conde.CuboSoma;

/// <summary>
/// Um policubo: um conjunto de cubinhos grudados face a face, guardado como
/// lista de posições e comparado pela forma, não pelo lugar.
///
/// Duas peças são a mesma quando uma pode ser girada até virar a outra. As 24
/// rotações do cubo estão escritas aqui, e a forma canônica é a menor delas em
/// ordem — é isso que impede o catálogo de contar a mesma peça duas vezes.
/// </summary>
public sealed class Peca : IEquatable<Peca>
{
    private readonly (int X, int Y, int Z)[] _cubos;

    public IReadOnlyList<(int X, int Y, int Z)> Cubos => _cubos;

    public int Tamanho => _cubos.Length;

    public Peca(IEnumerable<(int X, int Y, int Z)> cubos)
    {
        ArgumentNullException.ThrowIfNull(cubos);
        _cubos = Normalizar(cubos);
    }

    private static (int X, int Y, int Z)[] Normalizar(IEnumerable<(int X, int Y, int Z)> cubos)
    {
        var lista = cubos.Distinct().ToArray();
        if (lista.Length == 0)
        {
            return lista;
        }

        var minX = lista.Min(c => c.X);
        var minY = lista.Min(c => c.Y);
        var minZ = lista.Min(c => c.Z);

        for (var i = 0; i < lista.Length; i++)
        {
            lista[i] = (lista[i].X - minX, lista[i].Y - minY, lista[i].Z - minZ);
        }

        Array.Sort(lista, Comparar);
        return lista;
    }

    private static int Comparar((int X, int Y, int Z) a, (int X, int Y, int Z) b)
    {
        if (a.X != b.X)
        {
            return a.X.CompareTo(b.X);
        }

        if (a.Y != b.Y)
        {
            return a.Y.CompareTo(b.Y);
        }

        return a.Z.CompareTo(b.Z);
    }

    /// <summary>As 24 rotações do cubo, aplicadas a um ponto.</summary>
    public static (int X, int Y, int Z) Girar(int giro, (int X, int Y, int Z) p)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(giro);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(giro, 24);

        var (x, y, z) = p;

        // Primeiro escolhe-se para onde vai o eixo X (seis possibilidades) e
        // depois gira-se em volta dele (quatro). Seis vezes quatro sao as 24
        // rotacoes do cubo, sem espelho nenhum.
        (x, y, z) = (giro / 4) switch
        {
            0 => (x, y, z),
            1 => (-x, -y, z),
            2 => (y, -x, z),
            3 => (-y, x, z),
            4 => (z, y, -x),
            _ => (-z, y, x),
        };

        for (var i = 0; i < giro % 4; i++)
        {
            (y, z) = (-z, y);
        }

        return (x, y, z);
    }

    public Peca Girada(int giro) => new(_cubos.Select(c => Girar(giro, c)));

    /// <summary>A menor das 24 rotações: a identidade da forma.</summary>
    public Peca Canonica()
    {
        var melhor = this;
        for (var giro = 1; giro < 24; giro++)
        {
            var candidata = Girada(giro);
            if (candidata.CompareTo(melhor) < 0)
            {
                melhor = candidata;
            }
        }

        return melhor;
    }

    /// <summary>Quantas das 24 rotações deixam a peça igual a ela mesma.</summary>
    public int Simetrias()
    {
        var quantas = 0;
        for (var giro = 0; giro < 24; giro++)
        {
            if (Girada(giro).Equals(this))
            {
                quantas++;
            }
        }

        return quantas;
    }

    /// <summary>
    /// A peça é um bloco maciço? Quer dizer: a caixa que a envolve está cheia.
    ///
    /// É essa a pergunta que define o jogo: as peças do Soma são exatamente os
    /// policubos de 3 e 4 cubinhos que NÃO são blocos.
    /// </summary>
    public bool EhBloco()
    {
        var largura = _cubos.Max(c => c.X) + 1;
        var altura = _cubos.Max(c => c.Y) + 1;
        var profundidade = _cubos.Max(c => c.Z) + 1;
        return largura * altura * profundidade == _cubos.Length;
    }

    public int CompareTo(Peca outra)
    {
        ArgumentNullException.ThrowIfNull(outra);
        if (_cubos.Length != outra._cubos.Length)
        {
            return _cubos.Length.CompareTo(outra._cubos.Length);
        }

        for (var i = 0; i < _cubos.Length; i++)
        {
            var c = Comparar(_cubos[i], outra._cubos[i]);
            if (c != 0)
            {
                return c;
            }
        }

        return 0;
    }

    public bool Equals(Peca? outra) => outra is not null && CompareTo(outra) == 0;

    public override bool Equals(object? obj) => Equals(obj as Peca);

    public override int GetHashCode()
    {
        var h = 17;
        foreach (var (x, y, z) in _cubos)
        {
            h = (h * 31) + (x * 100) + (y * 10) + z;
        }

        return h;
    }

    public override string ToString() => string.Join(" ", _cubos.Select(c => $"{c.X}{c.Y}{c.Z}"));
}
