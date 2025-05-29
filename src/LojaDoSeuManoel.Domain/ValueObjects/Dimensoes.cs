namespace LojaDoSeuManoel.Domain.ValueObjects;

public class Dimensoes
{
    public int AlturaCm { get; }
    public int LarguraCm { get; }
    public int ComprimentoCm { get; }
    public int VolumeCm3 => AlturaCm * LarguraCm * ComprimentoCm;

    public Dimensoes(int alturaCm, int larguraCm, int comprimentoCm)
    {
        if (alturaCm <= 0 || larguraCm <= 0 || comprimentoCm <= 0)
        {
            throw new ArgumentException("Dimensões devem ser positivas.");
        }
        AlturaCm = alturaCm;
        LarguraCm = larguraCm;
        ComprimentoCm = comprimentoCm;
    }

    public bool CabeEm(Dimensoes dimensoesExternas)
    {
        return AlturaCm <= dimensoesExternas.AlturaCm &&
               LarguraCm <= dimensoesExternas.LarguraCm &&
               ComprimentoCm <= dimensoesExternas.ComprimentoCm;
    }

    public IEnumerable<Dimensoes> ObterRotacoes()
    {
        yield return this; // Original
        yield return new Dimensoes(AlturaCm, ComprimentoCm, LarguraCm);
        yield return new Dimensoes(LarguraCm, AlturaCm, ComprimentoCm);
        yield return new Dimensoes(LarguraCm, ComprimentoCm, AlturaCm);
        yield return new Dimensoes(ComprimentoCm, AlturaCm, LarguraCm);
        yield return new Dimensoes(ComprimentoCm, LarguraCm, AlturaCm);
    }

    public override string ToString()
    {
        return $"A:{AlturaCm} L:{LarguraCm} C:{ComprimentoCm}";
    }
}