namespace HW2;

public interface ICarFactory<TParams>
    where TParams : EngineParamsBase
{
    Car CreateCar(TParams carParams, int number);
}
