using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Android.Content;
using SUAP_Movil.Platforms.Android;
using Microsoft.Maui;
using System.Net.NetworkInformation;
using System.Net.Sockets;



#if ANDROID
using Android.Webkit;
#endif

namespace SUAP_Movil
{
    public partial class MainPage : ContentPage
    {
        private string currentUrl = string.Empty;


        public MainPage()
        {
            InitializeComponent();

        }



        protected override bool OnBackButtonPressed()
        {
            
            // Si estamos en login.aspx, desconectamos y salimos
            if (currentUrl.Contains("login.aspx", StringComparison.OrdinalIgnoreCase))
            {
                MainActivity.Instance?.DisconnectVPN();
                return base.OnBackButtonPressed(); // Cierra la app
            }

            // Si hay historial de navegación, volver atrás
            if (webView.CanGoBack)
            {
                webView.GoBack();
                return true;
            }

            // Caso por defecto
            return base.OnBackButtonPressed();
        }


        protected override async void OnAppearing()
        {
            webView.Navigated += (s, e) =>
            {
                currentUrl = e.Url;
            };

            base.OnAppearing();

            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            await WaitForVpnAndLoadWebView();

           
        }

        private async Task WaitForVpnAndLoadWebView()
        {
            loadingIndicator.IsVisible = true;
            webView.IsVisible = false;

            bool vpnReady = false;
            int retries = 10;

            for (int i = 0; i < retries; i++)
            {
                if (await IsVpnHostReachable("192.44.30.11", 80))
                {
                    vpnReady = true;
                    break;
                }
                else
                {
                    vpnReady = false;
                  
                }

                await Task.Delay(1000);
            }

            if (vpnReady)
            {
               
                // Luego navegar a la URL real
                webView.Source = "http://192.44.30.11/login.aspx";
                webView.IsVisible = true;
                loadingIndicator.IsVisible = false;

            }
            else
            {

                await DisplayAlert("Error", "No se pudo alcanzar el servidor por VPN.", "OK");
            }
        }


        private async Task<bool> IsVpnHostReachable(string ipAddress, int port)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(ipAddress, port);
                return client.Connected;
            }
            catch
            {
                return false;
            }
        }
    }

}
