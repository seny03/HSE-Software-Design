using AutoFixture;
using AutoFixture.Xunit2;
using Bogus;
using S4.HseCarShop.Models;
using System.Reflection;

namespace S4.HseCarShop.Tests;

public class CustomerCustomization : ICustomization
{
    private readonly Faker _faker;

    private readonly uint _legStrength;
    private readonly uint _handStrength;

    public CustomerCustomization(uint legStrength, uint handStrength)
    {
        _legStrength = legStrength;
        _handStrength = handStrength;

        _faker = new Faker();
    }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<Customer>(composer => composer
            .FromFactory(() => new Customer(_faker.Name.FullName(), _legStrength, _handStrength))
            .Without(c => c.Car));
    }
}

public class CustomizeCustomerAttribute : CustomizeAttribute
{
    private readonly uint _legStrength;
    private readonly uint _handStrength;

    public CustomizeCustomerAttribute(uint legStrength, uint handStrength)
    {
        _legStrength = legStrength;
        _handStrength = handStrength;
    }

    public override ICustomization? GetCustomization(ParameterInfo parameter)
    {
        if (parameter.ParameterType != typeof(Customer))
        {
            return null;
        }

        return new CustomerCustomization(_legStrength, _handStrength);
    }
}