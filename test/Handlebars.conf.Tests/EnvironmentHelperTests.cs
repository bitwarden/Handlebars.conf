using System.Collections;

namespace Handlebars.conf.Tests;

public class EnvironmentHelperTests
{
    [Fact]
    public void GetLowercaseEnvironmentVariables_ReturnsNonNullHashtable()
    {
        // Act
        var result = EnvironmentHelper.GetLowercaseEnvironmentVariables();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Hashtable>(result);
    }

    [Fact]
    public void GetLowercaseEnvironmentVariables_ReturnsLowercaseKeys()
    {
        // Act
        var result = EnvironmentHelper.GetLowercaseEnvironmentVariables();

        // Assert
        foreach (DictionaryEntry entry in result)
        {
            var key = entry.Key.ToString();
            Assert.NotNull(key);
            Assert.Equal(key.ToLowerInvariant(), key);
        }
    }

    [Fact]
    public void GetLowercaseEnvironmentVariables_ContainsEnvironmentVariables()
    {
        // Arrange
        var testVarName = "TEST_ENV_VAR_" + Guid.NewGuid().ToString("N");
        var testVarValue = "test_value";
        Environment.SetEnvironmentVariable(testVarName, testVarValue);

        try
        {
            // Act
            var result = EnvironmentHelper.GetLowercaseEnvironmentVariables();

            // Assert
            var lowercaseKey = testVarName.ToLowerInvariant();
            Assert.True(result.Contains(lowercaseKey));
            Assert.Equal(testVarValue, result[lowercaseKey]);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable(testVarName, null);
        }
    }
}
