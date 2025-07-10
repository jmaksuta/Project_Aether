using NUnit.Framework;
using UnityEngine;
using static Assets.Scripts.ApiSettings;

public class TestApiSettings
{
    [SetUp]
    public void Setup()
    {
        // This method is called before each test.
        // You can use it to initialize any common resources needed for the tests.
    }

    [TearDown]
    public void TearDown()
    {

    }

    [Test]
    public void TestApiDomain_NoneToStringIsEmpty()
    {
        string expected = string.Empty;
        string actual = ApiDomains.None.ToString();

        Assert.AreEqual(expected, actual, "ApiDomains.None does not equal string.Empty.");
    }
}
