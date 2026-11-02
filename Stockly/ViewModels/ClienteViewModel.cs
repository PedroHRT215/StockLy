using Stockly.Models;
using Stockly.Services;
using Stockly.Singleton;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Stockly.ViewModels
{
    public class ClienteViewModel : BaseNotifyViewModel
    {
        private DatabaseService _dbService;
        private ObservableCollection<Produto> _listaProdutos;
        public ObservableCollection<Produto> ListaProdutos
        {
            get => _listaProdutos;
            set { _listaProdutos = value; OnPropertyChanged(); }
        }

        private Produto _produtoSelecionado;
        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set
            {
                _produtoSelecionado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TemProdutoSelecionado));
                QuantidadeSelecionadaTexto = "1";
            }
        }

        public bool TemProdutoSelecionado => ProdutoSelecionado != null;
        private string _quantidadeSelecionadaTexto = "1";
        public string QuantidadeSelecionadaTexto
        {
            get => _quantidadeSelecionadaTexto;
            set { _quantidadeSelecionadaTexto = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Historico> _listaHistorico;
        public ObservableCollection<Historico> ListaHistorico
        {
            get => _listaHistorico;
            set { _listaHistorico = value; OnPropertyChanged(); }
        }

        public string NomeUsuarioLogado => UsuarioAtual.Instancia.UsuarioLogado?.Nome ?? "Cliente";

        public Command SairCommand { get; }
        public Command SelecionarProdutoCommand { get; }
        public Command VerMeuHistoricoCommand { get; }

        public ClienteViewModel()
        {
            _dbService = new DatabaseService();

            SairCommand = new Command(() =>
            {
                Application.Current.MainPage = new NavigationPage(new Views.LoginPage());
            });

            SelecionarProdutoCommand = new Command(ConfirmarReserva);
            VerMeuHistoricoCommand = new Command(CarregarHistorico);

            CarregarVitrine();
            CarregarHistorico();
        }


        private void CarregarVitrine()
        {
            var produtosDoBanco = _dbService.ObterProdutos();
            var apenasComEstoque = produtosDoBanco.Where(p => p.Quantidade > 0).ToList();

            ListaProdutos = new ObservableCollection<Produto>(apenasComEstoque);
        }

        private void CarregarHistorico()
        {
            if (UsuarioAtual.Instancia.UsuarioLogado != null)
            {
                var historicoCompleto = _dbService.ObterHistoricoTodos();
                var meuHistorico = historicoCompleto.Where(h => h.UsuarioId == UsuarioAtual.Instancia.UsuarioLogado.Id).ToList();
                ListaHistorico = new ObservableCollection<Historico>(meuHistorico);
            }
        }

        private void AtualizarQtd()
        {
            if ()
            {

            }
        }

        private async void ConfirmarReserva()
        {

            if (ProdutoSelecionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Selecione um produto no catálogo primeiro.", "OK");
                return;
            }

            if (!int.TryParse(QuantidadeSelecionadaTexto, out int qtdDesejada) || qtdDesejada <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "Insira uma quantidade válida maior que zero.", "OK");
                return;
            }

            if (qtdDesejada > ProdutoSelecionado.Quantidade)
            {
                await Application.Current.MainPage.DisplayAlert("Estoque Insuficiente",
                    $"Não é possível reservar. O estoque disponível para '{ProdutoSelecionado.Nome}' é de apenas {ProdutoSelecionado.Quantidade} unidades.", "OK");
                return;
            }


            bool confirma = await Application.Current.MainPage.DisplayAlert("Confirmar",
                $"Deseja reservar {qtdDesejada}x do item {ProdutoSelecionado.Nome}?", "Sim", "Não");

            if (!confirma)
                return;

            var novaReserva = new Historico
            {
                UsuarioId = UsuarioAtual.Instancia.UsuarioLogado.Id,
                ProdutoId = ProdutoSelecionado.Id,
                NomeUsuario = UsuarioAtual.Instancia.UsuarioLogado.Nome,
                NomeProduto = ProdutoSelecionado.Nome,
                DataHora = System.DateTime.Now
            };
            _dbService.RegistrarHistorico(novaReserva);

            ProdutoSelecionado.Quantidade -= qtdDesejada;
            _dbService.SalvarProduto(ProdutoSelecionado);

            await Application.Current.MainPage.DisplayAlert("Sucesso", "Seleção registada com sucesso na base de dados!", "OK");

            ProdutoSelecionado = null;
            QuantidadeSelecionadaTexto = "1";
            CarregarVitrine();
            CarregarHistorico();
        }
    }
}