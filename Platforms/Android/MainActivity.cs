using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.Core.App;
using SUAP_Movil.Platforms.Android;



namespace SUAP_Movil.Platforms.Android
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {

        private bool isVpnConnected = false;
        private const string profileName = "MiVPN";

        public static MainActivity Instance { get; private set; }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);

            if (intent?.Action == "ACTION_DISCONNECT_VPN")
            {
                DisconnectVPN();                

                FinishAffinity(); // Cierra la app completamente
            }
           
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState );
            Instance = this;
            if (Build.VERSION.SdkInt > BuildVersionCodes.Tiramisu)
            {
                ActivityCompat.RequestPermissions(this, [Manifest.Permission.PostNotifications], 0);
            }
            //if (!isVpnConnected) { ConnectVPN();  }
            
        }


        //protected override void OnStop()
        //{
        //    FinishAffinity();
        //    if (isVpnConnected)
        //    {
        //        DisconnectVPN();
        //    }
        //    base.OnStop();
           
        //}

        //protected override void OnDestroy()
        //{
        //    if (isVpnConnected)
        //    {
        //        DisconnectVPN();
        //    }
        //    base.OnDestroy();
        //}

       
        protected override void OnStart()
        {
           base.OnStart();

            //    // Solo intentar conectar la VPN si no está conectada
            if (!isVpnConnected)
            {

                ConnectVPN();
            }
        }

        private void ConnectVPN()
        {
            try
            {
                StartVpnControlService(VpnConnectForegroundService.Actions.START);
                // Crear el Intent para conectar a OpenVPN
                Intent shortcutIntent = new Intent(Intent.ActionMain);
                shortcutIntent.SetClassName("de.blinkt.openvpn", "de.blinkt.openvpn.api.ConnectVPN");
                shortcutIntent.PutExtra("de.blinkt.openvpn.api.profileName", profileName);

                // Iniciar la actividad
                StartActivity(shortcutIntent);
                isVpnConnected = true;
                
            }
            catch (ActivityNotFoundException e)
            {
                // Mostrar un Toast si no se encuentra la actividad
                Toast.MakeText(this, "No se pudo encontrar la actividad de OpenVPN: " + e.Message, ToastLength.Long).Show();
            }
        }
          

        // Método para desconectar la VPN cuando el usuario salga de la aplicación
        public void DisconnectVPN()
        {
            try
            {
                StartVpnControlService(VpnConnectForegroundService.Actions.STOP);
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

        private void StartVpnControlService(VpnConnectForegroundService.Actions action)
        {
            var intent = new Intent();
            intent.SetClassName("com.spc", "com.spc.VpnConnectForegroundService");
            intent.SetAction(action.ToString());

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                this.StartForegroundService(intent);
            else
                this.StartService(intent);
        }

        

    }
}













































