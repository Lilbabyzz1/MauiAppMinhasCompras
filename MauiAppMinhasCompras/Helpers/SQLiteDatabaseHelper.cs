using MauiAppMinhasCompras.Models;
using SQLite;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;
        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();
        }

        public Task<int> Insert(Produto p) 
        {
            return _conn.InsertAsync(p);
        }

        public Task<List<Produto>> Update(Produto p) 
        {
            string sql = "UPDATE Produto SET Descricao=?, Quantidade=?, Preco=?, Categoria=? WHERE Id=?";
            
            return _conn.QueryAsync<Produto>(
                sql, p.Descricao, p.Quantidade, p.Preco, p.Categoria, p.Id
                
                );  
        }

        public Task<int> Delete(int id) 
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }

        public Task<List<Produto>> GetAll() 
        {
            return _conn.Table<Produto>().ToListAsync();
        }

        public Task<List<Produto>> Search(string q) 
        {
            string sql = "SELECT * FROM Produto WHERE Descricao LIKE ?";
            string parametro = "%" + q + "%";

            return _conn.QueryAsync<Produto>(sql, parametro);
        }

        public async Task ResetId()
        {
            await _conn.ExecuteAsync("DELETE FROM sqlite_sequence WHERE name='Produto'");
        }

        public Task<List<Produto>> FilterCategoria(string categoria)
        {
            string sql = "SELECT * FROM Produto WHERE Categoria LIKE ?";
            string param = "%" + categoria + "%";

            return _conn.QueryAsync<Produto>(sql, param);
        }

    }
}
