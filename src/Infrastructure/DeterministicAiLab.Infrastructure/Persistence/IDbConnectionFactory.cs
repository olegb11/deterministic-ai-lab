using System.Data;

namespace DeterministicAiLab.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}