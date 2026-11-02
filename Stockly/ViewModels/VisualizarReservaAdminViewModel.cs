using Stockly.Models;
using Stockly.Services;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Stockly.ViewModels
{
    public class VisualizarReservaAdminViewModel : BaseNotifyViewModel
    {
        private DatabaseService _dbService;

        private Usuario _usuarioSelecionado;
        public Usuario UsuarioSelecionado
        {
            get => _usuarioSelecionado;
            set { _usuarioSelecionado = value; OnPropertyChanged(); }
        }

        // CORREÇÃO: Substituído Reserva por Historico
        private ObservableCollection<Historico> _listaReservas;
        public ObservableCollection<Historico> ListaReservas
        {
            get => _listaReservas;
            set { _listaReservas = value; OnPropertyChanged(); }
        }

        public Command VoltarCommand { get; }

        public VisualizarReservaAdminViewModel(Usuario usuario)
        {
            _dbService = new DatabaseService();
            UsuarioSelecionado = usuario;

            VoltarCommand = new Command(() => Application.Current.MainPage.Navigation.PopAsync());

            CarregarReservas();
        }

        private void CarregarReservas()
        {
            if (UsuarioSelecionado != null)
            {
                // CORREÇÃO: Substituído GetReservasPorUsuarioId por GetHistoricoPorUsuarioId
                var historico = _dbService.GetHistoricoPorUsuarioId(UsuarioSelecionado.Id);

                if (historico != null && historico.Any())
                {
                    // CORREÇÃO: Substituído Reserva por Historico
                    ListaReservas = new ObservableCollection<Historico>(historico);
                }
                else
                {
                    // CORREÇÃO: Substituído Reserva por Historico
                    ListaReservas = new ObservableCollection<Historico>();
                }
            }
        }
    }
}