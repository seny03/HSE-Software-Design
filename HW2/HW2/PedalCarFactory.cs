
namespace HW2
{
    public class PedalCarFactory : ICarFactory<PedalEngineParams>
    {
        public Car CreateCar(PedalEngineParams carParams, int number)
        {
            var engine = new PedalEngine(carParams.PedalSize);
            return new Car(engine, number);
        }
    }
}
