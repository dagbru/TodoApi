using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoApi.Models;
using TodoApi.Repositories;
using TodoApi.Tests.Infrastructure;

namespace TodoApi.Tests.Steps;

[Binding]
public class TodoSteps
{
    private readonly ScenarioContext _scenarioContext;
    private Mock<IDbContextFactory<TodoDbContext>> _dbContextFactory;
    private TodoDbContext _dbContext;
    private CustomWebApplicationFactory<Program> _apiFactory;

    public TodoSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TodoDbContext(dbOptions);
        _dbContext.Database.EnsureCreated();

        _dbContextFactory = new Mock<IDbContextFactory<TodoDbContext>>();
        _dbContextFactory.Setup(x => x.CreateDbContext()).Returns(_dbContext);
        _apiFactory =
            new CustomWebApplicationFactory<Program>(new TodoRepository(_dbContextFactory.Object),
                _dbContextFactory.Object);
    }

    [Given(@"the following todo items in database")]
    public void GivenTheFollowingTodoItemsInDatabase(Table table)
    {
        var todoItem = table.CreateSet<TodoItem>();
        _dbContext.AddRange(todoItem);
        _dbContext.SaveChanges();
    }

    [When(@"I run a GET request with endpoint (.*)")]
    public async Task WhenICallEndpoint(string endpoint)
    {
        var client = _apiFactory.CreateClient();
        var response = await client.GetAsync($"todo{endpoint}");
        _scenarioContext.Add("response", response);
    }

    [Then(@"the returned status should be code (.*)")]
    public void ThenTheReturnedStatusShouldBeCode(int statusCode)
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("response");
        var actualStatusCode = (int)response.StatusCode;
        actualStatusCode.Should().Be(statusCode);
    }

    [Then(@"the result should be")]
    public async Task ThenTheResultShouldBe(string expectedText)
    {
        var expected = JsonSerializer.Deserialize<IEnumerable<TodoItem>>(expectedText,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var content = await _scenarioContext.Get<HttpResponseMessage>("response").Content.ReadAsStringAsync();

        var actual =
            JsonSerializer.Deserialize<IEnumerable<TodoItem>>(content,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

        expected.Should().BeEquivalentTo(actual);
    }

    [When(@"I run a POST request with endpoint (.*)")]
    public async Task WhenIRunApostRequestWithEndpointAdd(string endpoint, string payload)
    {
        var client = _apiFactory.CreateClient();
        var response = await client.PostAsync($"todo{endpoint}",
            new StringContent(payload, new MediaTypeHeaderValue("application/json")));

        _scenarioContext.Add("response", response);
    }


    [Then(@"the following todo item should exist in the database")]
    public void ThenTheFollowingTodoItemShouldExistInTheDatabase(Table table)
    {
        var expected = table.CreateInstance<TodoItem>();

        var actual = _dbContext.TodoItems.FirstOrDefault(x =>
            x.Title == expected.Title && x.Text == expected.Text && x.Completed == expected.Completed);
        actual.Should().NotBeNull();
    }

    [When(@"I run PUT request with endpoint (.*)")]
    public async Task WhenIRunPutRequestWithEndpointUpdate(string endpoint, string payload)
    {
        var client = _apiFactory.CreateClient();
        var response = await client.PutAsync($"todo{endpoint}",
            new StringContent(payload, new MediaTypeHeaderValue("application/json")));

        _scenarioContext.Add("response", response);
    }
}