using System.Net.Http.Headers;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoApi.Models;
using TodoApi.Repositories;
using TodoApi.Tests.Infrastructure;

namespace TodoApi.Tests.Steps;

[Binding]
public class TodoApiSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly TodoDbContext _dbContext;
    private CustomWebApplicationFactory<Program> _apiFactory;

    public TodoApiSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        var dbOptions = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _dbContext = new TodoDbContext(dbOptions);
        _dbContext.Database.EnsureCreated();
        
        var dbContextFactory = new Mock<IDbContextFactory<TodoDbContext>>();
        dbContextFactory.Setup(x => x.CreateDbContext()).Returns(_dbContext);

        _apiFactory =
            new CustomWebApplicationFactory<Program>(new TodoRepository(dbContextFactory.Object),
                dbContextFactory.Object);
    }
    
    [Given(@"the following todo items are in the database")]
    public void GivenTheFollowingTodoItemsAreInTheDatabase(Table table)
    {
        var todoItems = table.CreateSet<TodoItem>();
        _dbContext.AddRange(todoItems);
        _dbContext.SaveChanges();
    }

    [When(@"I call GET endpoint (.*)")]
    public async Task WhenICallGetEndpoint(string endpoint)
    {
        var client = _apiFactory.CreateClient();
        var response = await client.GetAsync(endpoint);
        _scenarioContext.Add("response", response);
    }

    [Then(@"the returned response status code is (.*)")]
    public void ThenTheReturnedResponseStatusCodeIs(int statusCode)
    {
        var actualResponse = _scenarioContext.Get<HttpResponseMessage>("response");
        var actualResponseCode = (int)actualResponse.StatusCode;

        actualResponseCode.Should().Be(statusCode);
    }

    [Then(@"the returned response is")]
    public async Task ThenTheReturnedResponseIs(string expectedResultJson)
    {
        var expected = JsonSerializer.Deserialize<IEnumerable<TodoItem>>(expectedResultJson,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        
        var actualResponse = _scenarioContext.Get<HttpResponseMessage>("response");
        var actualResponseContent = await actualResponse.Content.ReadAsStringAsync();
        var actualResponseTodoItems = JsonSerializer.Deserialize<IEnumerable<TodoItem>>(actualResponseContent,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        expected.Should().BeEquivalentTo(actualResponseTodoItems);
    }

    [When(@"I call POST endpoint (.*) with payload")]
    public async Task WhenICallPostEndpointTodoAddWithPayload(string endpoint, string payload)
    {
        var client = _apiFactory.CreateClient();
        var response = await client.PostAsync(endpoint, new StringContent(payload, new MediaTypeHeaderValue("application/json")));
        _scenarioContext.Add("response", response);
    }

    [Then(@"the following todo item exists in the database")]
    public void ThenTheFollowingTodoItemExistsInTheDatabase(Table table)
    {
        var expectedItem = table.CreateInstance<TodoItem>();

        var itemInDatabase = _dbContext.TodoItems.FirstOrDefault(x =>
            x.Title == expectedItem.Title && x.Text == expectedItem.Text && x.Completed == expectedItem.Completed);
        
        itemInDatabase.Should().NotBeNull();
    }

    [When(@"I call PUT endpoint (.*) with payload")]
    public async Task WhenICallPutEndpointTodoUpdateWithPayload(string endpoint, string payload)
    {
        var client = _apiFactory.CreateClient();
        var response = await client.PutAsync(endpoint, new StringContent(payload, new MediaTypeHeaderValue("application/json")));
        _scenarioContext.Add("response", response);
    }
}