# Kafka base project with Producer and Consumers (Json and Avro)

This project is a example of how produce and consume Kafka messages using Avro and Json formats.


## Using Docker to run kafka

- This step is needed because we need Kafka and the Schema Registry running
- Execute the "compose up" using the docker compose file existed in the base folder
    - if using VsCode > Rclick + Compose Up
    - if using cmd/bash
```bash
docker-compose -f docker-compose.yaml up
```
<br>

## Executing the API to Post messages

- inside the "Producer" folder, start the aplication
```bash
dotnet run --urls "http://localhost:5001"
```
- Now you can see in the terminal "`Now listening on: http://localhost:5001`"
- You can access [http://localhost:5001/swagger](http://localhost:5001/swagger) to see the swagger portal
- Using the Swagger page you can post messages in Json and Avro format
- Each format is using a different topic

<br>

## Looking the topics, schemas and messages using Kafdrop

- You can access [http://localhost:19000/](http://localhost:19000/) to see Kafdrop initial page
- In this page you can see all the topics already created
- Inside the topics "MessageTopicAvro" and "MessageTopicJson" you can click to see the messages posted
- Inside the topic "_schemas" you can see the schemas already created

> [!TIP]
> This application is creating topics and including registry items when messages are added

## Consuming messages

- Inside each consumer folder (ConsumerAvro, ConsumerJson), execute a new terminal and run the application
```bash
dotnet run
```
- Now you can see the terminal listing the messages posted in each topic (Avro/Json)
<br>

- Console example - Json
```
Message: {"Id":"b00b0000-00bb-0bbb-bbb0-00000b0bb000","Text":"test test test","CreatedDate":"2024-01-29T00:00:00"}
```

- Console example - Avro
```
Message: {"Schema":{"Tag":1,"Name":"AvroMessage","Fullname":"NamespageMessage.AvroMessage"},"Id":"b00b0000-00bb-0bbb-bbb0-00000b0bb000","Text":"test test test","CreatedDate":"01/29/2024 00:00:00"}
```