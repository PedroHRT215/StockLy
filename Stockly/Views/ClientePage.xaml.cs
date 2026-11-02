using Microsoft.Maui.Controls;
using Stockly.ViewModels;
using Stockly.Models;

namespace Stockly.Views
{
    public partial class ClientePage : ContentPage
    {
        public ClientePage()
        {
            InitializeComponent();
            var viewModel = new ProdutoViewModel();
            viewModel.ProdutoSelecionado = new Produto();
            BindingContext = viewModel;
        }
    }
}