using BoDi;
using Neo4j.Driver;
using TechTalk.SpecFlow;

namespace TestContainersDemo
{
    [Binding]
    public class Neo4jSupport
    {
        private IDriver driver;
        private readonly IObjectContainer objectContainer;

        public Neo4jSupport(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void InitializeDriver()
        {
            driver = GraphDatabase.Driver(
                "bolt://localhost",
                AuthTokens.Basic(
                    "neo4j",
                    "admin"
                ));

            objectContainer.RegisterInstanceAs(driver);
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            await using var session = driver.AsyncSession();

            await session.WriteTransactionAsync(async tr =>
            {
                var cursor = await tr.RunAsync(@$"MATCH (n) DETACH DELETE n");
            });
        }
    }
}
