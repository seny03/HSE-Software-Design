namespace HW2
{
    public class HandCarFactory : ICarFactory<EmptyEngineParams>
    {
        public Car CreateCar(EmptyEngineParams carParams, int number)
        {
            var engine = new HandEngine();
            return new Car(engine, number);
        }
    }
}
