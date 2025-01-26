namespace HW2
{
    public class HandEngine : IEngine
    {
        EngineType type = EngineType.Hand;

        public bool isCompatible(Customer customer)
        {
            return customer.ArmsPower > 5;
        }

        public override string ToString()
        {
            return $"Тип двигателя: {type}";
        }
    }
}
