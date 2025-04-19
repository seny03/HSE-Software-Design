using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Presentation.Models;

public class CreateEnclosureDto
{
    public string Name { get; set; } = string.Empty;
    public EnclosureType Type { get; set; }
    public double Size { get; set; }
    public int MaxCapacity { get; set; }
    public Species SpeciesType { get; set; }
}

public class UpdateEnclosureDto
{
    public string Name { get; set; } = string.Empty;
    public EnclosureType Type { get; set; }
    public double Size { get; set; }
    public int CurrentOccupancy { get; set; }
    public int MaxCapacity { get; set; }
    public Species SpeciesType { get; set; }
}
