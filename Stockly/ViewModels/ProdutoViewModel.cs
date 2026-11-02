using Stockly.Models;
using Stockly.Services;
using Stockly.Singleton;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace Stockly.ViewModels
{
    public class ProdutoViewModel : BaseNotifyViewModel
    {
        private DatabaseService _dbService;

        public string NomeUsuarioLogado
        {
            get { return UsuarioAtual.Instancia.UsuarioLogado?.Nome; }
        }

        private List<Produto> _listaProdutos;
        public List<Produto> ListaProdutos
        {
            get { return _listaProdutos; }
            set { _listaProdutos = value; OnPropertyChanged(); }
        }

        private List<Historico> _listaHistorico;
        public List<Historico> ListaHistorico
        {
            get { return _listaHistorico; }
            set { _listaHistorico = value; OnPropertyChanged(); }
        }

        private Produto _produtoSelecionado;
        public Produto ProdutoSelecionado
        {
            get { return _produtoSelecionado; }
            set { _produtoSelecionado = value; OnPropertyChanged(); }
        }

        public Command SalvarProdutoCommand { get; set; }
        public Command ExcluirProdutoCommand { get; set; }
        public Command VerHistoricoGeralCommand { get; set; }
        public Command SelecionarProdutoCommand { get; set; }
        public Command VerMeuHistoricoCommand { get; set; }
        public Command SairCommand { get; set; }

        public ProdutoViewModel()
        {
            _dbService = new DatabaseService();
            CarregarProdutos();

            SalvarProdutoCommand = new Command<Produto>(SalvarProduto);
            ExcluirProdutoCommand = new Command<Produto>(ExcluirProduto);
            VerHistoricoGeralCommand = new Command(CarregarHistoricoGeral);
            SelecionarProdutoCommand = new Command(RegistrarSelecaoCliente);
            VerMeuHistoricoCommand = new Command(CarregarHistoricoCliente);
            SairCommand = new Command(FazerLogout);
        }

        public void CarregarProdutos()
        {
            ListaProdutos = _dbService.ObterProdutos();
        }

        private void SalvarProduto(Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Nome))
            {
                Application.Current.MainPage.DisplayAlert("Aviso", "O nome do produto é obrigatório.", "OK");
                return;
            }

            _dbService.SalvarProduto(produto);
            CarregarProdutos();
            ProdutoSelecionado = new Produto(); // Limpa os campos após salvar
            Application.Current.MainPage.DisplayAlert("Sucesso", "Produto salvo com sucesso!", "OK");
        }

        private void ExcluirProduto(Produto produto)
        {
            if (produto != null && produto.Id != 0)
            {
                _dbService.ExcluirProduto(produto);
                CarregarProdutos();
                ProdutoSelecionado = new Produto();
                Application.Current.MainPage.DisplayAlert("Sucesso", "Produto excluído com sucesso!", "OK");
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Aviso", "Selecione um produto para excluir.", "OK");
            }
        }

        private void CarregarHistoricoGeral()
        {
            ListaHistorico = _dbService.ObterHistoricoTodos();
        }

        private async void RegistrarSelecaoCliente()
        {
            if (ProdutoSelecionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Selecione um produto na lista primeiro.", "OK");
                return;
            }

            bool confirma = await Application.Current.MainPage.DisplayAlert("Confirmar", $"Deseja reservar/selecionar o item {ProdutoSelecionado.Nome}?", "Sim", "Não");

            if (confirma)
            {
                Usuario logado = UsuarioAtual.Instancia.UsuarioLogado;

                Historico novoHistorico = new Historico
                {
                    UsuarioId = logado.Id,
                    NomeUsuario = logado.Nome,
                    ProdutoId = ProdutoSelecionado.Id,
                    NomeProduto = ProdutoSelecionado.Nome,
                    DataHora = DateTime.Now
                };

                _dbService.RegistrarHistorico(novoHistorico);
                await Application.Current.MainPage.DisplayAlert("Sucesso", "Seleção registrada com sucesso no banco de dados!", "OK");
            }
        }

        private void CarregarHistoricoCliente()
        {
            Usuario logado = UsuarioAtual.Instancia.UsuarioLogado;
            if (logado != null)
            {
                ListaHistorico = _dbService.ObterHistoricoPorUsuario(logado.Id);
            }
        }

        private void FazerLogout()
        {
            UsuarioAtual.Instancia.Logout();
            Application.Current.MainPage = new NavigationPage(new Views.LoginPage());
        }
    }
}