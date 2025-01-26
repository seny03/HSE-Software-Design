namespace HW2;

public record PedalEngineParams : EngineParamsBase
{
    public int PedalSize { get; }

    public PedalEngineParams(int pedalSize)
    {
        PedalSize = pedalSize;
    }
}