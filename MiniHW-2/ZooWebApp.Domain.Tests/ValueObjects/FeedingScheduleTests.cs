using System;
using System.Collections.Generic;
using Xunit;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Domain.Tests.ValueObjects;

public class FeedingScheduleTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var time = new TimeSpan(10, 0, 0);
        var foodItems = new List<FoodType> { FoodType.Meat, FoodType.Fish };
        var notes = "Morning feeding";

        // Act
        var schedule = new FeedingSchedule(
            Time: time,
            FoodItems: foodItems,
            Notes: notes
        );

        // Assert
        Assert.Equal(time, schedule.Time);
        Assert.Equal(foodItems, schedule.FoodItems);
        Assert.Equal(notes, schedule.Notes);
    }

    [Fact]
    public void FeedingSchedule_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var time = new TimeSpan(10, 0, 0);
        var foodItems1 = new List<FoodType> { FoodType.Meat, FoodType.Fish };
        var foodItems2 = new List<FoodType> { FoodType.Meat, FoodType.Fish };
        var notes = "Morning feeding";

        // Act
        var schedule1 = new FeedingSchedule(
            Time: time,
            FoodItems: foodItems1,
            Notes: notes
        );
        
        var schedule2 = new FeedingSchedule(
            Time: time,
            FoodItems: foodItems2,
            Notes: notes
        );

        // Assert
        Assert.Equal(schedule1, schedule2);
    }

    [Fact]
    public void FeedingSchedule_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var time1 = new TimeSpan(10, 0, 0);
        var time2 = new TimeSpan(15, 0, 0);
        var foodItems = new List<FoodType> { FoodType.Meat, FoodType.Fish };
        var notes = "Feeding";

        // Act
        var schedule1 = new FeedingSchedule(
            Time: time1,
            FoodItems: foodItems,
            Notes: notes
        );
        
        var schedule2 = new FeedingSchedule(
            Time: time2,
            FoodItems: foodItems,
            Notes: notes
        );

        // Assert
        Assert.NotEqual(schedule1, schedule2);
    }

    [Fact]
    public void FeedingSchedule_WithDifferentFoodItems_ShouldNotBeEqual()
    {
        // Arrange
        var time = new TimeSpan(10, 0, 0);
        var foodItems1 = new List<FoodType> { FoodType.Meat, FoodType.Fish };
        var foodItems2 = new List<FoodType> { FoodType.Vegetables, FoodType.Fruits };
        var notes = "Feeding";

        // Act
        var schedule1 = new FeedingSchedule(
            Time: time,
            FoodItems: foodItems1,
            Notes: notes
        );
        
        var schedule2 = new FeedingSchedule(
            Time: time,
            FoodItems: foodItems2,
            Notes: notes
        );

        // Assert
        Assert.NotEqual(schedule1, schedule2);
    }
}
