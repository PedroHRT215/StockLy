using SQLite;
using System.Collections.Generic;
using System.IO;
using System;
using Stockly.Models;

namespace Stockly.Services
{
    public class DatabaseService
    {
        private SQLiteConnection _db;

        public DatabaseService()
        {
            // Cria e conecta ao banco de dados local do dispositivo
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stockly.db3");
            _db = new SQLiteConnection(dbPath);

            _db.CreateTable<Usuario>();
            _db.CreateTable<Produto>();
            _db.CreateTable<Historico>();
        }

        // --- MÉTODOS PARA USUÁRIOS ---
        public void SalvarUsuario(Usuario usuario)
        {
            _db.Insert(usuario);
        }

        public Usuario ValidarLogin(string email, string senha)
        {
            return _db.Table<Usuario>().FirstOrDefault(usuario => usuario.Email == email && usuario.Senha == senha);
        }

        public Usuario GetUsuarioPorId(int id)
        {
            return _db.Table<Usuario>().FirstOrDefault(u => u.Id == id);
        }

        // --- MÉTODOS PARA PRODUTOS ---
        public List<Produto> ObterProdutos()
        {
            return _db.Table<Produto>().ToList();
        }

        public void SalvarProduto(Produto produto)
        {
            if (produto.Id != 0)
                _db.Update(produto);
            else
                _db.Insert(produto);
        }

        public void ExcluirProduto(Produto produto)
        {
            _db.Delete(produto);
        }

        // --- MÉTODOS PARA HISTÓRICO ---
        public void RegistrarHistorico(Historico historico)
        {
            _db.Insert(historico);
        }

        public List<Historico> ObterHistoricoTodos()
        {
            return _db.Table<Historico>().OrderByDescending(h => h.DataHora).ToList();
        }

        public List<Historico> ObterHistoricoPorUsuario(int usuarioId)
        {
            return _db.Table<Historico>()
                      .Where(historico => historico.UsuarioId == usuarioId)
                      .OrderByDescending(historico => historico.DataHora)
                      .ToList();
        }

        public List<Historico> GetHistoricoPorUsuarioId(int usuarioId)
        {
            return ObterHistoricoPorUsuario(usuarioId);
        }
    }
}