using ZooERP.Models.Animals;
using ZooERP.Services;

namespace ZooERP.Tests.Services
{
    public class VeterinaryClinicTests
    {
        private readonly VeterinaryClinic _clinic;

        public VeterinaryClinicTests()
        {
            _clinic = new VeterinaryClinic();
        }

        [Fact]
        public void CheckHealth_HealthyAnimal_ShouldReturnTrue()
        {
            var healthyMonkey = new Monkey("Чарли", 5, true, 1, 7);
            bool result = _clinic.CheckHealth(healthyMonkey);

            Assert.True(result);
        }

        [Fact]
        public void CheckHealth_UnhealthyAnimal_ShouldReturnFalse()
        {
            var sickTiger = new Tiger("Шерхан", 10, false, 2);
            bool result = _clinic.CheckHealth(sickTiger);

            Assert.False(result);
        }
    }
}
