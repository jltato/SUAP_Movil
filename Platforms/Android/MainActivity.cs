using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Java.Lang;


namespace SUAP_Movil.Platforms.Android
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        // Definir el nombre del perfil
        string profileName = "MiVPN"; // Reemplaza con el nombre de tu perfil
        private bool isVpnConnected = false; // Controla el estado de la conexión VPN
        public static MainActivity Instance { get; private set; }
        //public static event Action OnVpnConnected; // Evento que notifica a MainPage


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;
            // Connectar la VPN cuando la aplicacion inicia
            if (!isVpnConnected)
            {
                ConnectVPN();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Solo intentar conectar la VPN si no está conectada
            if (!isVpnConnected)
            {
                ConnectVPN();
            }
        }

        private void ConnectVPN()
        {
            try
            {
                // Crear el Intent para conectar a OpenVPN
                Intent shortcutIntent = new Intent(Intent.ActionMain);
                shortcutIntent.SetClassName("de.blinkt.openvpn", "de.blinkt.openvpn.api.ConnectVPN");
                shortcutIntent.PutExtra("de.blinkt.openvpn.api.profileName", profileName);

                // Iniciar la actividad
                StartActivity(shortcutIntent);
                isVpnConnected = true;
                //OnVpnConnected?.Invoke();
            }
            catch (ActivityNotFoundException e)
            {
                // Mostrar un Toast si no se encuentra la actividad
                Toast.MakeText(this, "No se pudo encontrar la actividad de OpenVPN: " + e.Message, ToastLength.Long).Show();
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            if (level == TrimMemory.UiHidden)
            {
                RunOnUiThread(() =>
                {
                    if (isVpnConnected)
                    {
                        DisconnectVPN(); // Llama mientras aún está visible
                    }

                    JavaSystem.Exit(0); // Termina la app
                });
            }
        }

        //protected override void OnStop()
        //{
        //    if (isVpnConnected)
        //    {
        //        DisconnectVPN();
        //    }
        //    JavaSystem.Exit(0); // Mata la app al irse a segundo plano

        //    base.OnStop();
           
        //}

        //protected override void OnDestroy()
        //{
        //    if (isVpnConnected)
        //    {
        //        DisconnectVPN();
        //    }
        //    JavaSystem.Exit(0);
        //    base.OnDestroy();   
        //}




        // Método para desconectar la VPN cuando el usuario salga de la aplicación
        public void DisconnectVPN()
        {
            try
            {
                Intent disconnectIntent = new Intent(Intent.ActionMain);
                disconnectIntent.SetClassName("de.blinkt.openvpn", "de.blinkt.openvpn.api.DisconnectVPN");
                disconnectIntent.PutExtra("de.blinkt.openvpn.api.profileName", profileName);

                isVpnConnected = false;
                StartActivity(disconnectIntent);
               
              
            }
            catch (ActivityNotFoundException e)
            {
                Toast.MakeText(this, "No se pudo encontrar la actividad de OpenVPN: " + e.Message, ToastLength.Long).Show();
            }
        }


       

    }
}
