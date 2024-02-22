using WebApplication1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<CalendarItem>();

var app = builder.Build();

app.MapGraphQL();

app.Run();