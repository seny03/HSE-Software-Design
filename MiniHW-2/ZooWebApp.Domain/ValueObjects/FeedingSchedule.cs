using System.Collections.Immutable;
using System.Collections.Generic;
using ZooWebApp.Domain.Enums;

namespace ZooWebApp.Domain.ValueObjects;

public record FeedingSchedule
{
    public TimeSpan Time { get; init; }
    public ImmutableList<FoodType> FoodItems { get; init; }
    public string Notes { get; init; }

    public FeedingSchedule(TimeSpan Time, List<FoodType> FoodItems, string Notes)
    {
        this.Time = Time;
        this.FoodItems = FoodItems.ToImmutableList();
        this.Notes = Notes;
    }

    // Override Equals to properly compare ImmutableList contents
    public virtual bool Equals(FeedingSchedule? other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Time.Equals(other.Time) && 
               Notes == other.Notes &&
               ((FoodItems == null && other.FoodItems == null) ||
                (FoodItems != null && other.FoodItems != null && 
                 FoodItems.SequenceEqual(other.FoodItems)));
    }

    // Always override GetHashCode when overriding Equals
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Time.GetHashCode();
            hash = hash * 23 + (Notes?.GetHashCode() ?? 0);
            
            if (FoodItems != null)
            {
                foreach (var item in FoodItems)
                {
                    hash = hash * 23 + item.GetHashCode();
                }
            }
            
            return hash;
        }
    }
}
