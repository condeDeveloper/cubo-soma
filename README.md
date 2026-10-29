# O cubo Soma

Sete peças feitas de cubinhos grudados, que montam um cubo de lado 3. Piet Hein inventou
o quebra-cabeça em 1933, e a história conta que foi durante uma palestra de física
quântica.

As peças não estão digitadas neste repositório. Elas são **derivadas** de uma frase:

> todos os policubos de 3 ou 4 cubinhos que não são blocos maciços.

São exatamente sete, e somam exatamente 27 cubinhos — que é o cubo de lado 3. A soma não
é imposta em lugar nenhum: ela sai da conta.

```
dotnet test testes/CuboSoma.Testes/CuboSoma.Testes.csproj -c Release
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- policubos
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- pecas
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- cubo
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- caixas
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- centro
```

## A derivação

```
cubinhos  formas  blocos macicos  vao para o Soma
1         1       1               0
2         1       1               0
3         2       1               1
4         8       2               6
5         29      1               0
```

Os policubos saem de um crescimento: um cubinho, depois dois, depois três, com a forma
canônica — a menor das 24 rotações — jogando fora o que repete. As contagens 1, 1, 2, 8 e
29 são as conhecidas, e o 29 do tamanho 5 está aí só para mostrar que o gerador não para
de funcionar fora do que o jogo usa.

Os blocos maciços que ficam de fora são o palito de três, o palito de quatro e o quadrado
de quatro. Sobram **uma peça de três cubinhos e seis de quatro**: `3 + 24 = 27`.

## Os achados

### 1. As 11.520 montagens dividem por 48 sem deixar resto

```
montagens do cubo 3x3x3: 11520
simetrias do cubo:       48
essencialmente diferentes: 240
a divisao e exata:       sim
```

O cubo tem 48 simetrias: 24 rotações e os espelhos de cada uma. Se alguma montagem fosse
simétrica em si mesma — se existisse um giro que a devolvesse igual —, ela seria contada
menos de 48 vezes e a divisão deixaria resto.

A divisão é exata. **Nenhuma das 240 montagens tem simetria própria**, e isso é uma
afirmação sobre todas elas que sai de uma única conta.

### 2. Duas das sete peças nunca ocupam o centro do cubo

```
peca  cubinhos  montagens  %
0     3         912        7.9
1     4         0          0
2     4         0          0
3     4         1776       15.4
4     4         3624       31.5
5     4         3624       31.5
6     4         1584       13.8
```

Em nenhuma das 11.520 montagens as peças 1 e 2 — as duas tetracubos planas mais
compridas — passam pelo cubinho do meio. **Zero**, não "pouco".

E as peças 4 e 5, que são uma o espelho da outra, aparecem no centro exatamente o mesmo
número de vezes: 3.624 cada. A simetria do conjunto de peças aparece na contagem sem que
ninguém a tenha programado.

### 3. O cubo é a única caixa de 27 células que fecha

```
caixa           montagens  nos        colocacoes
cubo 3x3x3      11520      861285     688
torre 1x3x9     0          1696       240
parede 1x9x3    0          143        240
tijolo 3x9x1    0          1696       240
caixa 1x27x1    0          1          0
```

Vinte e sete células cabem em caixas de `1×3×9`, `1×9×3`, `3×9×1` e `3×3×3`. Só a última
é montável, e o motivo é curto: **três das sete peças não cabem numa caixa de espessura
1**, em nenhuma das 24 rotações. O quebra-cabeça é tridimensional por obrigação, não por
escolha.

### 4. A peça mais travada é a de três braços

```
n  cubinhos  simetrias  colocacoes no cubo
0  3         2          144
1  4         1          144
3  4         3          64
4  4         2          96
```

A peça 3 é a que sai de um canto em três direções. Ela tem **3 simetrias** — o maior
número do conjunto — e por isso o menor número de colocações distintas: 64, menos da
metade das 144 da peça mais livre.

Simetria e liberdade andam ao contrário: quanto mais a peça se parece consigo mesma depois
de girada, menos lugares diferentes ela tem para ficar.

## O que as medidas me corrigiram

- **Eu escrevi que quatro peças não são planas. São três.** As planas são a de três
  cubinhos e três das de quatro; as não planas são a de três braços e o par espelhado. Eu
  contei errado olhando a lista, e o teste pegou. O número importa porque é ele que
  explica por que nenhuma caixa achatada fecha.

- **Eu chutei que o dominó tem 4 simetrias.** Tem 8: as quatro rotações em volta do
  próprio eixo e mais quatro que o viram de ponta-cabeça — e que o devolvem igual, porque a
  peça é comparada pela forma e não pelo lugar. Mais um número escrito de cabeça dentro de
  um teste, no mesmo lote em que isso já tinha acontecido três vezes.

- **As 24 rotações eu ia deixar sem teste.** Se duas delas fossem a mesma transformação, o
  catálogo acharia menos peças e ninguém reclamaria: o programa rodaria, a tabela sairia, e
  o número estaria errado em silêncio. O teste que confere que as 24 são 24 diferentes é o
  mais importante do repositório, e ele não testa nada do que o repositório faz.

- **Eu ia dizer que a divisão por 48 "confere".** Ela faz mais do que conferir: o resto
  zero é uma afirmação sobre todas as 240 montagens ao mesmo tempo. Se eu tivesse me
  contentado com "240, como se sabe", teria perdido o que a conta estava dizendo.

## Estrutura

```
src/CuboSoma/
  Peca.cs       o policubo, as 24 rotacoes e a forma canonica
  Catalogo.cs   os policubos por crescimento, e as sete pecas pela definicao
  Montagem.cs   a busca por mascara de bits, sempre pela primeira celula vazia
  Medidas.cs    as tabelas deste README
testes/CuboSoma.Testes/
ferramentas/Medidor/
```

64 testes.
