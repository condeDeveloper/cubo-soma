namespace Conde.CuboSoma;

/// <summary>
/// A montagem: encaixar as sete peças numa caixa, varrendo todas as maneiras.
///
/// A caixa de lado 3 tem 27 posições, que cabem num inteiro de 32 bits. Cada
/// colocação possível de cada peça vira uma máscara, calculada uma vez, e a
/// busca só faz ou-exclusivo e teste de bit.
///
/// A ordem da busca é a que não deixa escolha: sempre a <b>primeira posição
/// vazia</b> é preenchida, e só as colocações que cobrem exatamente aquela
/// posição são tentadas. Isso corta a árvore sem precisar de heurística
/// nenhuma e garante que nenhuma montagem é contada duas vezes.
/// </summary>
public sealed class Montagem
{
    private readonly int _largura;
    private readonly int _altura;
    private readonly int _profundidade;
    private readonly List<Peca> _pecas;
    private readonly List<int>[][] _porPosicao;

    public int Celulas => _largura * _altura * _profundidade;

    public long Nos { get; private set; }

    public long Colocacoes { get; }

    public Montagem(IReadOnlyList<Peca> pecas, int largura, int altura, int profundidade)
    {
        ArgumentNullException.ThrowIfNull(pecas);
        _pecas = [.. pecas];
        _largura = largura;
        _altura = altura;
        _profundidade = profundidade;

        _porPosicao = new List<int>[_pecas.Count][];
        long total = 0;

        for (var i = 0; i < _pecas.Count; i++)
        {
            _porPosicao[i] = new List<int>[Celulas];
            for (var c = 0; c < Celulas; c++)
            {
                _porPosicao[i][c] = [];
            }

            foreach (var mascara in Colocacoesde(_pecas[i]))
            {
                total++;
                var primeira = PrimeiroBit(mascara);
                _porPosicao[i][primeira].Add(mascara);
            }
        }

        Colocacoes = total;
    }

    private IEnumerable<int> Colocacoesde(Peca peca)
    {
        var vistas = new HashSet<int>();

        for (var giro = 0; giro < 24; giro++)
        {
            var girada = peca.Girada(giro);
            var largura = girada.Cubos.Max(c => c.X) + 1;
            var altura = girada.Cubos.Max(c => c.Y) + 1;
            var profundidade = girada.Cubos.Max(c => c.Z) + 1;

            for (var dx = 0; dx + largura <= _largura; dx++)
            {
                for (var dy = 0; dy + altura <= _altura; dy++)
                {
                    for (var dz = 0; dz + profundidade <= _profundidade; dz++)
                    {
                        var mascara = 0;
                        foreach (var (x, y, z) in girada.Cubos)
                        {
                            mascara |= 1 << Indice(x + dx, y + dy, z + dz);
                        }

                        if (vistas.Add(mascara))
                        {
                            yield return mascara;
                        }
                    }
                }
            }
        }
    }

    public int Indice(int x, int y, int z) => (((x * _altura) + y) * _profundidade) + z;

    private static int PrimeiroBit(int mascara)
    {
        var i = 0;
        while ((mascara & (1 << i)) == 0)
        {
            i++;
        }

        return i;
    }

    /// <summary>Quantas montagens existem, contando todas as orientações da caixa.</summary>
    public long Contar()
    {
        Nos = 0;
        return Passo(0, new bool[_pecas.Count]);
    }

    /// <summary>A primeira montagem, como a peça que ocupa cada posição.</summary>
    public int[]? Primeira()
    {
        var resposta = new int[Celulas];
        Array.Fill(resposta, -1);
        return Monta(0, new bool[_pecas.Count], resposta) ? resposta : null;
    }

    private long Passo(int ocupado, bool[] usada)
    {
        Nos++;
        if (ocupado == (1 << Celulas) - 1)
        {
            return 1;
        }

        var vazia = PrimeiroBit(~ocupado & ((1 << Celulas) - 1));
        long total = 0;

        for (var i = 0; i < _pecas.Count; i++)
        {
            if (usada[i])
            {
                continue;
            }

            usada[i] = true;
            foreach (var mascara in _porPosicao[i][vazia])
            {
                if ((mascara & ocupado) == 0)
                {
                    total += Passo(ocupado | mascara, usada);
                }
            }

            usada[i] = false;
        }

        return total;
    }

    private bool Monta(int ocupado, bool[] usada, int[] resposta)
    {
        if (ocupado == (1 << Celulas) - 1)
        {
            return true;
        }

        var vazia = PrimeiroBit(~ocupado & ((1 << Celulas) - 1));

        for (var i = 0; i < _pecas.Count; i++)
        {
            if (usada[i])
            {
                continue;
            }

            usada[i] = true;
            foreach (var mascara in _porPosicao[i][vazia])
            {
                if ((mascara & ocupado) != 0)
                {
                    continue;
                }

                for (var c = 0; c < Celulas; c++)
                {
                    if ((mascara & (1 << c)) != 0)
                    {
                        resposta[c] = i;
                    }
                }

                if (Monta(ocupado | mascara, usada, resposta))
                {
                    return true;
                }

                for (var c = 0; c < Celulas; c++)
                {
                    if ((mascara & (1 << c)) != 0)
                    {
                        resposta[c] = -1;
                    }
                }
            }

            usada[i] = false;
        }

        return false;
    }

    /// <summary>
    /// Em quantas montagens cada peça ocupa uma posição dada. É a pergunta que
    /// separa as peças umas das outras: elas não são intercambiáveis, e o
    /// centro do cubo não aceita qualquer uma.
    /// </summary>
    public long[] DonosDa(int celula)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(celula);
        var contagem = new long[_pecas.Count];
        var dono = new int[Celulas];
        Array.Fill(dono, -1);
        Conta(0, new bool[_pecas.Count], dono, celula, contagem);
        return contagem;
    }

    private void Conta(int ocupado, bool[] usada, int[] dono, int celula, long[] contagem)
    {
        if (ocupado == (1 << Celulas) - 1)
        {
            contagem[dono[celula]]++;
            return;
        }

        var vazia = PrimeiroBit(~ocupado & ((1 << Celulas) - 1));

        for (var i = 0; i < _pecas.Count; i++)
        {
            if (usada[i])
            {
                continue;
            }

            usada[i] = true;
            foreach (var mascara in _porPosicao[i][vazia])
            {
                if ((mascara & ocupado) != 0)
                {
                    continue;
                }

                if ((mascara & (1 << celula)) != 0)
                {
                    dono[celula] = i;
                }

                Conta(ocupado | mascara, usada, dono, celula, contagem);
            }

            usada[i] = false;
        }
    }

    /// <summary>
    /// Quantas colocações cada peça tem na caixa. A peça com menos colocações
    /// é a mais travada, e saber qual é diz mais sobre o quebra-cabeça do que
    /// o número total.
    /// </summary>
    public List<int> ColocacoesPorPeca()
    {
        var quantas = new List<int>();
        for (var i = 0; i < _pecas.Count; i++)
        {
            quantas.Add(_porPosicao[i].Sum(l => l.Count));
        }

        return quantas;
    }
}
