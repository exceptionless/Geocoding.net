using System.Text.Json;
using Geocoding.Microsoft.Json;
using Xunit;

namespace Geocoding.Tests;

public class MicrosoftJsonCompatibilityTest
{
    [Fact]
    public void Response_WithLocationResource_DeserializesToLocation()
    {
        // Arrange
        const string json = """
        {
          "resourceSets": [
            {
              "resources": [
                {
                  "name": "White House",
                  "entityType": "Address",
                  "confidence": "High",
                  "point": { "type": "Point", "coordinates": [38.8976777, -77.036517] },
                  "address": {
                    "formattedAddress": "1600 Pennsylvania Ave NW, Washington, DC 20500",
                    "addressLine": "1600 Pennsylvania Ave NW",
                    "adminDistrict": "DC",
                    "adminDistrict2": "District of Columbia",
                    "countryRegion": "United States",
                    "locality": "Washington",
                    "postalCode": "20500"
                  }
                }
              ]
            }
          ]
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<Response>(json, Extensions.JsonOptions);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response!.ResourceSets);
        Assert.Single(response.ResourceSets[0].Resources);
        Assert.IsType<Geocoding.Microsoft.Json.Location>(response.ResourceSets[0].Resources[0]);
        Assert.Equal("White House", response.ResourceSets[0].Resources[0].Name);
        Assert.NotNull(response.ResourceSets[0].Resources[0].Point);
        Assert.Equal(38.8976777, response.ResourceSets[0].Resources[0].Point!.Coordinates[0]);
        Assert.Single(response.ResourceSets[0].Locations);
        Assert.Equal("White House", response.ResourceSets[0].Locations[0].Name);
    }

    [Fact]
    public void Response_WithRouteResource_DeserializesToRoute()
    {
        // Arrange
        const string json = """
        {
          "resourceSets": [
            {
              "resources": [
                {
                  "name": "Route",
                  "distanceUnit": "Kilometer",
                  "durationUnit": "Second",
                  "travelDistance": 1.2,
                  "travelDuration": 120,
                  "routeLegs": [],
                  "routePath": { "line": { "point": [] } }
                }
              ]
            }
          ]
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<Response>(json, Extensions.JsonOptions);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response!.ResourceSets);
        Assert.Single(response.ResourceSets[0].Resources);
        Assert.IsType<Geocoding.Microsoft.Json.Route>(response.ResourceSets[0].Resources[0]);
        Assert.Empty(response.ResourceSets[0].Locations);
    }
}