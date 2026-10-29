using Conde.CuboSoma;

if (args.Length == 0)
{
    Console.WriteLine("uso: medidor <policubos|pecas|cubo|caixas|centro>");
    return 1;
}

switch (args[0])
{
    case "policubos":
        Policubos();
        break;
    case "pecas":
        Pecas();
        break;
    case "cubo":
        Cubo();
        break;
    case "caixas":
        Caixas();
        break;
    case "centro":
        Centro();
        break;
    default:
        Console.WriteLine($"comando desconhecido: {args[0]}");
        return 1;
}

return 0;

static void Policubos()
{
    Console.WriteLine("Quantos policubos existem de cada tamanho");
    Console.WriteLine("cubinhos  formas  blocos macicos  vao para o Soma");
    foreach (var l in Medidas.Policubos([1, 2, 3, 4, 5]))
    {
        Console.WriteLine($"{l.Tamanho,-10}{l.Formas,-8}{l.Blocos,-16}{l.NoSoma}");
    }

    Console.WriteLine();
    Console.WriteLine($"as sete pecas somam {Medidas.Cubinhos()} cubinhos");
}

static void Pecas()
{
    Console.WriteLine("As sete pecas do Soma");
    Console.WriteLine("n  cubinhos  simetrias  colocacoes no cubo  forma");
    foreach (var l in Medidas.Pecas())
    {
        Console.WriteLine($"{l.Indice,-3}{l.Cubinhos,-10}{l.Simetrias,-11}{l.Colocacoes,-20}{l.Forma}");
    }
}

static void Cubo()
{
    var (total, simetrias, essenciais, exata) = Medidas.Essenciais();
    Console.WriteLine($"montagens do cubo 3x3x3: {total}");
    Console.WriteLine($"simetrias do cubo:       {simetrias}");
    Console.WriteLine($"essencialmente diferentes: {essenciais}");
    Console.WriteLine($"a divisao e exata:       {(exata ? "sim" : "NAO")}");
}

static void Caixas()
{
    Console.WriteLine("As caixas de 27 celulas");
    Console.WriteLine("caixa           montagens  nos        colocacoes");
    foreach (var l in Medidas.Caixas())
    {
        Console.WriteLine($"{l.Nome,-16}{l.Montagens,-11}{l.Nos,-11}{l.Colocacoes}");
    }
}

static void Centro()
{
    Console.WriteLine("Qual peca ocupa o centro do cubo");
    Console.WriteLine("peca  cubinhos  montagens  %");
    foreach (var (peca, cubinhos, noCentro, fracao) in Medidas.Centro())
    {
        Console.WriteLine($"{peca,-6}{cubinhos,-10}{noCentro,-11}{fracao}");
    }
}
