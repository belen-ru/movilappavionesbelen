using movilappaviones.Models;
using movilappaviones.Services;
using movilappaviones.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace movilappaviones.ViewModels
{
    public class VuelosViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<Vuelo> ListaVuelos { get; set; } = new ObservableCollection<Vuelo>();
        public DateTime ValorFecha { get; set; }
        public TimeSpan ValorHora { get; set; }
        public string Errores { get; set; }
        public Vuelo Vuelo { get; set; }
        private VuelosService VuelosService { get; set; }

        public ICommand IniciarCommand { get; set; }
        public ICommand AgregarVueloCommand { get; set; }
        public ICommand VerAgregarVueloCommand { get; set; }
        public ICommand VerEditarVueloCommand { get; set; }
        public ICommand EditarVueloCommand { get; set; }
        public ICommand ActualizarCommand { get; set; }
        public ICommand CaneclarCommand { get; set; }

        public ICommand CaneclarVueloCommand { get; set; }
        public VuelosViewModel()
        {
            VuelosService=new VuelosService();
            IniciarCommand = new Command(Iniciar);
            VuelosService.Error += VuelosService_Error;
            VerAgregarVueloCommand = new Command(VerAgregarVuelo);
            VerEditarVueloCommand = new Command<Vuelo>(VerEditarVuelo);
            AgregarVueloCommand= new Command(AgregarVuelo);
            EditarVueloCommand = new Command(EditarVuelo);
            ActualizarCommand = new Command(actualizarlista);
            CaneclarCommand = new Command(Cancel);
            CaneclarVueloCommand = new Command<Vuelo>(CancelarVuelo);

        }

        private async void CancelarVuelo(Vuelo vuelo)
        {
            vuelo.Remarks = "CANCELADO";
            await VuelosService.Update(vuelo);
            CargarVuelos();
        }
        public bool validar(Vuelo vuelo)
        {
            Errores = null;
            if (vuelo == null)
            {
                Errores="Debe especificar la aerolinea a agregar.";
            }
            if (string.IsNullOrWhiteSpace(vuelo.Destination))
            {
                Errores = "Debe escribir un destino .";
            }
            if (string.IsNullOrWhiteSpace(vuelo.Flight))
            {
                Errores = "Debe escribir el vuelo.";
            }
            if (vuelo.Gate <= 0)
            {
                Errores = "Escriba el numero de puerta";
            }
            if (vuelo.Gate > 100)
            {
                Errores = "Escriba un numero del 1 al 100";
            }
            if(string.IsNullOrEmpty(Errores))
            {
                return true;
            }
            else
            {
                return false;

            }
           
        }
            private void Cancel()
        {
            CargarVuelos();
            Application.Current.MainPage.Navigation.PopAsync();
        }

        private void actualizarlista()
        {
            CargarVuelos();
        }

        private void Iniciar()
        {
            CargarVuelos();
            VuelosVista = new VuelosView();
            VuelosVista.BindingContext = this;
            Application.Current.MainPage.Navigation.PushAsync(VuelosVista);
        }

        VuelosView VuelosVista;
        AgregarView AgregarVista;
        EditarView EditarVista;
        private async void EditarVuelo()
        {
            if (validar(Vuelo))
            {
                Vuelo.Time = ValorFecha + ValorHora;
            await VuelosService.Update(Vuelo);
            CargarVuelos();
            Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                Actualizar(nameof(Errores));
            }

        }

        private async void AgregarVuelo()
        {
            if(validar(Vuelo))
            {

            Vuelo.Time = ValorFecha + ValorHora;
            await VuelosService.Insert(Vuelo);
            CargarVuelos();
            Application.Current.MainPage.Navigation.PopAsync();

            }
            else
            {
                Actualizar(nameof(Errores));
            }
        }

        private void VerEditarVuelo(Vuelo vuelo)
        {
           Vuelo= vuelo;
            ValorFecha = vuelo.Time.Date;
            ValorHora = vuelo.Time.TimeOfDay;
            EditarVista = new EditarView();
            EditarVista.BindingContext = this;
            Application.Current.MainPage.Navigation.PushAsync(EditarVista);
        }

        private void VerAgregarVuelo()
        {
            ValorFecha = DateTime.Now.Date;
            ValorHora = DateTime.Now.TimeOfDay;
            Vuelo = new Vuelo();
          
            
            AgregarVista = new AgregarView();
            AgregarVista.BindingContext = this;
            Application.Current.MainPage.Navigation.PushAsync(AgregarVista);
        }

        private void VuelosService_Error(string Error)
        {
            Errores = Error;
            Actualizar(nameof(Errores));
        }

       
        async void CargarVuelos()
        {

            ListaVuelos.Clear();
            var datos = await VuelosService.GetVuelos();
            datos.ForEach(x => ListaVuelos.Add(x));
            

            Actualizar(nameof(ListaVuelos));
           

        }
      

        public void Actualizar(string nombre)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
