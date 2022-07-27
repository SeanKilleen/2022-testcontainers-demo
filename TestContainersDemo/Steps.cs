using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using FluentAssertions;
using Neo4j.Driver;
using TechTalk.SpecFlow;

namespace TestContainersDemo
{
    [Binding]
    public class Steps
    {
        private readonly IDriver _driver;
        private int _employeeCount;

        public Steps(IDriver driver)
        {
            _driver = driver;
        }

        [Given(@"I have added (.*) as an employee who works at (.*)")]
        public async Task GivenIHaveAddedPersonAsAnEmployeeWhoWorksAt(string empName, string companyName)
        {
            await using var session = _driver.AsyncSession();

            await session.WriteTransactionAsync(async tr =>
            {
                var cursor = await tr.RunAsync(@"
                    MERGE(e:Employee {name:$empName})
                    MERGE(c:Company {name:$companyName})
                    CREATE(e)-[:WORKS_AT]->(c)
                    ",
                    new { empName, companyName});

                await cursor.ConsumeAsync();
            });
        }

        [When(@"I get the employees who work at (.*)")]
        public async Task WhenIGetTheEmployeesWhoWorkAt(string companyName)
        {
            await using var session = _driver.AsyncSession();

            await session.ReadTransactionAsync(async tr =>
            {
                var cursor = await tr.RunAsync(@"
                    MATCH (e:Employee)-[:WORKS_AT]->(c:Company {name: $companyName})
                    return e",
                    new { companyName });

                var result = await cursor.ToListAsync(r => 1);
                _employeeCount = result.Sum();
            });
        }

        [Then(@"the employee list should contain (.*) people")]
        public void ThenTheEmployeeListShouldContainPeople(int expectedCount)
        {
            _employeeCount.Should().Be(expectedCount);
        }


    }
}
