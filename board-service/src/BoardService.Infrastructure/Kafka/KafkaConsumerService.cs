using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BoardService.Infrastructure.Kafka;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IOptions<KafkaSettings> _kafkaOptions;
    private readonly IServiceProvider _serviceProvider;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IOptions<KafkaSettings> kafkaOptions,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _kafkaOptions = kafkaOptions;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.Value.BootstrapServers,
            GroupId = _kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        var topics = new[] { "sim.events", "handling.events", "board.events", "flights.events" };
        consumer.Subscribe(topics);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                if (consumeResult?.Message != null)
                {
                    await ProcessMessage(consumeResult.Topic, consumeResult.Message.Value, stoppingToken);
                    consumer.Commit(consumeResult);
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming Kafka message");
            }
        }
    }

    private async Task ProcessMessage(string topic, string json, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BoardDbContext>();

        try
        {
            var envelope = JsonSerializer.Deserialize<KafkaEnvelope>(json);
            if (envelope == null) return;

            _logger.LogInformation("Received: {Type} from {Topic}", envelope.Type, topic);

            // Здесь будет логика: handling.task.completed, flight.taxi.start и т.д.
            // Пока — заглушка
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process Kafka event");
        }
    }
}

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = null!;
    public string GroupId { get; set; } = null!;
}

public class KafkaEnvelope
{
    public string EventId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Ts { get; set; } = null!;
    public string Producer { get; set; } = null!;
    public Entity Entity { get; set; } = null!;
    public JsonElement Payload { get; set; }
}

public class Entity
{
    public string Kind { get; set; } = null!;
    public string Id { get; set; } = null!;
}
