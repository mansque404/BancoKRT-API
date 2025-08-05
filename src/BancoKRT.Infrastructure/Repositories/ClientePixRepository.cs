using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BancoKRT.Application.Interfaces.Repositories;
using BancoKRT.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace BancoKRT.Infrastructure.Repositories
{
    public class ClientePixRepository(IAmazonDynamoDB dynamoDb, IConfiguration configuration) : IClientePixRepository
    {
        private readonly IAmazonDynamoDB _dynamoDb = dynamoDb;
        private readonly string _tableName = configuration["AWS:DynamoDb:TableName"];

        public async Task<IEnumerable<ClientePix>> GetByDocumentoAsync(string documento)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = "documento = :v_documento",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":v_documento", new AttributeValue { S = documento } }
                }
            };

            var response = await _dynamoDb.QueryAsync(request);

            return response.Items.Select(item => MapToClientePix(item));
        }


        public async Task CreateAsync(ClientePix cliente)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "documento", new AttributeValue(cliente.Documento) },
                    { "conta_id", new AttributeValue(cliente.ContaId) },
                    { "limite_pix", new AttributeValue { N = cliente.LimitePix.ToString() } },
                    { "numero_agencia", new AttributeValue(cliente.NumeroAgencia) },
                    { "numero_conta", new AttributeValue(cliente.NumeroConta) }
                },
                ConditionExpression = "attribute_not_exists(documento) AND attribute_not_exists(conta_id)"
            };

            try
            {
                await _dynamoDb.PutItemAsync(request);
            }
            catch (ConditionalCheckFailedException)
            {
                throw new InvalidOperationException($"Já existe um registro para o documento " +
                    $"'{cliente.Documento}' e conta '{cliente.ContaId}'.");
            }
        }
        public async Task<ClientePix> GetByKeyAsync(string documento, string contaId)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "documento", new AttributeValue { S = documento } },
                    { "conta_id", new AttributeValue { S = contaId } }
                }
            };

            var response = await _dynamoDb.GetItemAsync(request);

            if (response.Item == null || response.Item.Count == 0)
            {
                return null;
            }

            return MapToClientePix(response.Item);
        }

        public async Task<bool> UpdateLimitePixAsync(string documento, string contaId, decimal novoLimite)
        {
            var request = new UpdateItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "documento", new AttributeValue(documento) },
                    { "conta_id", new AttributeValue(contaId) }
                },
                UpdateExpression = "SET limite_pix = :novoLimite",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":novoLimite", new AttributeValue { N = novoLimite.ToString() } }
                },
                ConditionExpression = "attribute_exists(documento) AND attribute_exists(conta_id)",
                ReturnValues = "NONE"
            };

            try
            {
                var response = await _dynamoDb.UpdateItemAsync(request);
                return true;
            }
            catch (ConditionalCheckFailedException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string documento, string contaId)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "documento", new AttributeValue(documento) },
                    { "conta_id", new AttributeValue(contaId) }
                },
                ConditionExpression = "attribute_exists(documento) AND attribute_exists(conta_id)"
            };

            try
            {
                var response = await _dynamoDb.DeleteItemAsync(request);
                return true;
            }
            catch (ConditionalCheckFailedException)
            {
                return false;
            }
        }

        public async Task<(bool Aprovada, decimal? NovoLimite)> ProcessarTransacaoAsync(string documento, string contaId, decimal valorTransacao)
        {
            var request = new UpdateItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "documento", new AttributeValue(documento) },
                    { "conta_id", new AttributeValue(contaId) }
                },
                UpdateExpression = "SET limite_pix = limite_pix - :valor",

                ConditionExpression = "attribute_exists(documento) AND limite_pix >= :valor",

                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":valor", new AttributeValue { N = valorTransacao.ToString() } }
                },
                ReturnValues = "UPDATED_NEW"
            };

            try
            {
                var response = await _dynamoDb.UpdateItemAsync(request);
                var novoLimite = decimal.Parse(response.Attributes["limite_pix"].N);
                return (Aprovada: true, NovoLimite: novoLimite);
            }
            catch (ConditionalCheckFailedException)
            {
                return (Aprovada: false, NovoLimite: null);
            }
        }

        private static ClientePix MapToClientePix(Dictionary<string, 
            AttributeValue> item)
        {
            return new ClientePix
            {
                Documento = item["documento"].S,
                ContaId = item["conta_id"].S,
                LimitePix = decimal.Parse(item["limite_pix"].N),
                NumeroAgencia = item["numero_agencia"].S,
                NumeroConta = item["numero_conta"].S
            };
        }

    }
}