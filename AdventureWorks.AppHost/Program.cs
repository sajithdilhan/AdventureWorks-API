var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AdventureWorks>("adventureworks");

builder.Build().Run();
