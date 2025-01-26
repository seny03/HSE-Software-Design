namespace HW2
{
    public class Car
    {
        private static readonly Random _random = new();

        public int Number { get; set; }

        public IEngine Engine { get; set; }

        public Car(IEngine carEngine, int carNumber)
        {
            Engine = carEngine;
            Number = carNumber;
        }

        public bool isCompatible(Customer customer)
        {
            return Engine.isCompatible(customer);
        }

        public override string ToString()
        {
            return $"Номер: {Number}, {Engine}";
        }
    }
}
