using Abstracciones.Interfaces.DA;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DA.Repositorios
{
    public class RepositorioDapper : IRepositorioDapper
    {
        private readonly IConfiguration _configuration;

        public RepositorioDapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection ConexionBD()
        {
            return new SqlConnection(_configuration.GetConnectionString("BD"));
        }
    }
}