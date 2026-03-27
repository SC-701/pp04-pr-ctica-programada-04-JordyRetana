using Abstracciones.DA;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DA.Repositorios
{
    public class RepositorioDapper : IRepositorioDapper
    {
        private readonly IConfiguration _configutarion;
        private readonly SqlConnection _connection;

        public RepositorioDapper(IConfiguration configuration)
        {
            _configutarion = configuration;
            _connection = new SqlConnection(_configutarion.GetConnectionString("BD"));
        }

        public SqlConnection ObtenerRepositorioDapper()
        {
            return _connection;
        }
    }
}
