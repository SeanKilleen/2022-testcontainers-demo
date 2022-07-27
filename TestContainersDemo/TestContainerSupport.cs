using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using TechTalk.SpecFlow;

namespace TestContainersDemo;

[Binding]
public static class TestContainerSupport
{
    private static bool _containerHasBeenInitialized = false;

    [BeforeTestRun]
    public static void SetupTestContainer()
    {
        if (!_containerHasBeenInitialized)
        {
            _containerHasBeenInitialized = true;
            var randomName = Path.GetRandomFileName().Replace(".", string.Empty);

            var builder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("neo4j:4.4.9-enterprise")
                .WithName($"excella-tests-{randomName}")
                .WithEnvironment("NEO4J_AUTH", "neo4j/admin")
                .WithEnvironment("NEO4J_ACCEPT_LICENSE_AGREEMENT", "yes")
                .WithPortBinding("7687")
                .WithPortBinding("7474")
                .WithCleanUp(true)
                .WithAutoRemove(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(7474));

            var container = builder.Build();
            container.StartAsync().Wait();
        }
    }
}