# EventSourcing

I recommend to open solution in the latest version of Visual Studio 2019 with all updates.
.NET Core 3.1 should be installed on computer.
Solution uses MySQL DB for Read DB and Apache Kafka for Write DB. Solution is written using modern technologies including .NET Core 3.1 WebApi, Entity Framework Core (Code First), CQRS + Event Sourcing, Serilog, Swagger, AutoMapper.
Deployment can be done using Docker.
Nuget is used as .NET package manager.

## Local development environment setup
* Build solution
* Run docker-compose command with the following arguments: docker-compose up -d
* Open local Kafka Manager URL in browser http://localhost:9000/
* Create Cluster using the following zookeeper: "zookeeper:2181"
* Create Topic with name "EventSourcing" with two partitions and parameter retention.ms=-1 (this to keep messages forever instead of 1 week by default)
* Do the same for Topic with name "CommandSourcing"
* Set up multiple startup projects to "EventSourcing.API", "Worker.Balance" and "Worker.Transaction"
* Run solution

## Usage
Commands can be created through swagger of "EventSourcing.API" project.
First you need to create at least two accounts with some initial balance through swagger.
Created accounts you can see by URL: https://localhost:44391/accountbalances
Then you can move funds from source account to target account.
Transactions can be seen by URL: https://localhost:44395/moneytransactions
