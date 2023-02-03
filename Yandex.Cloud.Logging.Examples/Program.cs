using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using System.Reflection.Emit;
using System.Text.Json;
using Yandex.Cloud.Logging.V1;
using Yandex.Cloud.SDK.IamJwtCredentialsProviderExtension;
using static Yandex.Cloud.Logging.V1.LogLevel.Types;

namespace Yandex.Cloud.Logging.Examples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = File.OpenText("authorized_key.json");
            var keyJson = JsonDocument.Parse(reader.ReadToEnd());
            var sdk = new Sdk(
                new IamJwtCredentialsProvider(
                    keyJson.RootElement.GetProperty("service_account_id").GetString(),
                    keyJson.RootElement.GetProperty("id").GetString(),
                    keyJson.RootElement.GetProperty("private_key").GetString()
                )
            );           
            
            var service = sdk.Services.Logging.LogIngestionService;
            var dt = DateTime.UtcNow.ToTimestamp().Seconds;

            var entries = new RepeatedField<IncomingLogEntry>()
            {
                new IncomingLogEntry
                {
                    JsonPayload = Struct.Parser.ParseJson("{\"life\": \"log\"}"),
                    Level = Level.Info,
                    Message = "Hello test!",
                    Timestamp = new Timestamp()
                    {
                        Seconds = dt
                    }
                }
            };
            var request = new WriteRequest()
            {
                Destination = new Destination
                {
                    LogGroupId = "e23nlm1po5bkocikgb0c",
                    FolderId = "b1g5g3933j1gg1tpj73r"
                }
            };
            request.Entries.Add(entries);

            var responce = service.Write(request);
        }
    }
}