using Confluent.Kafka;
using System;
using System.Threading;

namespace TestConsume
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                GroupId = "TestConsume",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };


            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Assign(new TopicPartitionOffset("EventSourcing", 0, new Offset(0)));

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = consumer.Consume(cts.Token);
                            Console.WriteLine(cr.Message.Value);
                            Console.WriteLine($"Partition offset: {cr.Offset.Value}");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            }
        }
    }
}
