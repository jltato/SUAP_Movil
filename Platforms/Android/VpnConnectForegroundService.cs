using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.Core.App;



namespace SUAP_Movil.Platforms.Android
{
    [Service(Name = "com.spc.VpnConnectForegroundService", ForegroundServiceType = ForegroundService.TypeConnectedDevice)] // O el tipo adecuado
    internal class VpnConnectForegroundService : Service
    {

        public const int SERVICE_NOTIFICATION_ID = 1004; // ID para este servicio
        private const string NOTIFICATION_CHANNEL_ID = "vpn_service_channel";
        private const string NOTIFICATION_CHANNEL_NAME = "VPN Service Channel";
        private NotificationManager? notificationManager;



        public override IBinder? OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            switch (intent.Action)
            {
                case "START": start(); break;
                case "STOP":
                    StopForeground(true);
                    StopSelf(); 
                    break;
                
            }
                

            return StartCommandResult.Sticky; // Queremos que el servicio se reinicie si es detenido
        }

        public override void OnCreate()
        {
            base.OnCreate();
            notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME, NotificationImportance.High);
                notificationManager?.CreateNotificationChannel(channel);
            }
        }

        public enum Actions
        {
            START, STOP
        }

        private void start()
        {

            var notificationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID)
                                    .SetSmallIcon(Resource.Drawable.notification_bg_normal)
                                    .SetContentTitle("SUAP")
                                    .SetContentText("El Sistema se encuentra activo")
                                    .SetOngoing(true)
                                    .SetPriority((int)NotificationPriority.High);

            // Acción para desconectar
            var disconnectIntent = new Intent(this, typeof(MainActivity));
            disconnectIntent.SetAction("ACTION_DISCONNECT_VPN");
            disconnectIntent.SetFlags(ActivityFlags.NewTask);

            var disconnectPendingIntent = PendingIntent.GetActivity(this, 0, disconnectIntent, PendingIntentFlags.Immutable);

            // Crear acción con NotificationCompat.Action
            var disconnectAction = new NotificationCompat.Action.Builder(
                Resource.Drawable.notification_bg_normal,
                "Cerrar",
                disconnectPendingIntent
            ).Build();

            notificationBuilder.AddAction(disconnectAction);

            // Mostrar la notificación
            StartForeground(SERVICE_NOTIFICATION_ID, notificationBuilder.Build());
            
        }

       //public static void ForceDisconnect(Context context)
       // {
       //     try
       //     {
       //         Intent disconnectIntent = new Intent(Intent.ActionMain);
       //         disconnectIntent.SetClassName("de.blinkt.openvpn", "de.blinkt.openvpn.api.DisconnectVPN");
       //         disconnectIntent.PutExtra("de.blinkt.openvpn.api.profileName", "MiVPN");
       //         disconnectIntent.SetFlags(ActivityFlags.NewTask);

       //         context.StartActivity(disconnectIntent);
       //         NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
       //         notificationManager.Cancel(1); // Cancela la notificación foreground
       //     }
       //     catch (System.Exception ex)
       //     {
       //         Toast.MakeText(context, "Error al desconectar VPN: " + ex.Message, ToastLength.Long).Show();
       //     }
            
       // }

    }
}

//private void ConnectVPNInternal()
//{
//    try
//    {
//        Intent shortcutIntent = new Intent(Intent.ActionMain);
//        shortcutIntent.SetClassName("de.blinkt.openvpn", "de.blinkt.openvpn.api.ConnectVPN");
//        shortcutIntent.PutExtra("de.blinkt.openvpn.api.profileName", profileName);
//        shortcutIntent.SetFlags(ActivityFlags.NewTask);

//        // Iniciar la actividad
//        StartActivity(shortcutIntent);
//        IsConnected = true;
//        // Cancelar cualquier desconexión pendiente al conectar
//        disconnectHandler?.RemoveCallbacks(disconnectRunnable);
//        disconnectRunnable = null;
//    }
//    catch (ActivityNotFoundException e)
//    {
//        Toast.MakeText(this, "No se pudo encontrar la actividad de OpenVPN: " + e.Message, ToastLength.Long).Show();
//    }
//}

//private void DisconnectVPNInternal()
//{
//    try
//    {
//        Intent disconnectIntent = new Intent(Intent.ActionMain);
//        disconnectIntent.SetClassName("de.blinkt.openvpn", "de.blinkt.openvpn.api.DisconnectVPN");
//        disconnectIntent.PutExtra("de.blinkt.openvpn.api.profileName", profileName);
//        disconnectIntent.SetFlags(ActivityFlags.NewTask);

//        StartActivity(disconnectIntent);

//        IsConnected = false;
//        StopSelf(); // Detener el servicio después de desconectar
//    }
//    catch (PackageManager.NameNotFoundException)
//    {
//        Toast.MakeText(this, "La aplicación OpenVPN no está instalada.", ToastLength.Long).Show();
//    }
//    catch (ActivityNotFoundException e)
//    {
//        Toast.MakeText(this, $"No se encontró la actividad de OpenVPN: {e.Message}", ToastLength.Long).Show();
//    }
//    catch (System.Exception ex)
//    {
//        Toast.MakeText(this, $"Error al intentar desconectar la VPN: {ex.Message}", ToastLength.Long).Show();
//    }
//}


//private VpnControlBroadcastReceiver? receiver; // Para escuchar la desconexión



//public override void OnCreate()
//{
//    base.OnCreate();
//    RegisterReceiver(); // Registrar para escuchar la desconexión
//    ShowNotification("Iniciando servicio VPN...", false);
//    // Intentar conectar al iniciar el servicio
//    ConnectVPNInternal();
//}

//public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
//{
//    return StartCommandResult.Sticky; // Queremos que el servicio se reinicie si es detenido
//}

//        private void RegisterReceiver()
//        {
//            receiver = new VpnControlBroadcastReceiver(this);
//            var filter = new IntentFilter();
//            filter.AddAction(ACTION_TRIGGER_DISCONNECT_VPN_SERVICE);
//            filter.AddAction(ACTION_TRIGGER_CONNECT_VPN_SERVICE);

//#if ANDROID
//            if (OperatingSystem.IsAndroidVersionAtLeast(33))
//            {
//                RegisterReceiver(receiver, filter, ReceiverFlags.NotExported);
//            }
//            else
//            {
//                RegisterReceiver(receiver, filter);
//            }
//#else
//                    RegisterReceiver(receiver, filter); // No aplicable en otras plataformas
//#endif
//        }

//private void ShowNotification(string message, bool isConnected)
//{
//    var channelId = "vpn_control_channel";
//    var channelName = "VPN Control Service";
//    var notificationManager = (NotificationManager)GetSystemService(NotificationService);

//    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
//    {
//        var channel = new NotificationChannel(channelId, channelName, NotificationImportance.High);
//        notificationManager?.CreateNotificationChannel(channel);
//    }

//    var notificationBuilder = new NotificationCompat.Builder(this, channelId)
//        .SetSmallIcon(Resource.Drawable.notification_template_icon_bg) // Reemplaza con tu icono
//        .SetContentTitle("Servicio VPN")
//        .SetContentText(message)
//        .SetPriority(NotificationCompat.PriorityHigh)
//        .SetOngoing(true); // Para que no se pueda descartar fácilmente

//    if (isConnected)
//    {
//        // Agregar acción para desconectar desde la notificación
//        var disconnectIntent = new Intent(Intent.ActionMain);
//        disconnectIntent.SetClassName("de.blinkt.openvpn", "de.blinkt.openvpn.api.DisconnectVPN");
//        disconnectIntent.PutExtra("de.blinkt.openvpn.api.profileName", profileName);
//        disconnectIntent.SetFlags(ActivityFlags.NewTask);
//        var disconnectPendingIntent = PendingIntent.GetBroadcast(this, 0, disconnectIntent, PendingIntentFlags.Immutable);
//        notificationBuilder.AddAction(Resource.Drawable.notification_template_icon_bg, "Desconectar", disconnectPendingIntent); // Reemplaza con tu icono
//    }
//    else
//    {
//        // No es necesario un botón de "Conectar" aquí, ya que se conecta al inicio
//    }

//    StartForeground(SERVICE_NOTIFICATION_ID, notificationBuilder.Build());
//}

//public override void OnDestroy()
//{
//    base.OnDestroy();
//    UnregisterReceiver(receiver);
//    disconnectHandler?.RemoveCallbacks(disconnectRunnable);
//    disconnectHandler?.Dispose();
//    disconnectHandler = null;
//    disconnectRunnable = null;
//}

//internal class VpnControlBroadcastReceiver : BroadcastReceiver
//{
//    private readonly VpnConnectForegroundService service;

//    public VpnControlBroadcastReceiver(VpnConnectForegroundService svc)
//    {
//        service = svc;
//    }

//    public override void OnReceive(Context? context, Intent? intent)
//    {
//        if (intent?.Action == ACTION_TRIGGER_DISCONNECT_VPN_SERVICE)
//        {
//            if (service.IsConnected)
//            {
//                // Programar la desconexión diferida
//                if (service.disconnectHandler != null)
//                {
//                    service.disconnectRunnable = new Runnable(() => { service.DisconnectVPNInternal(); });
//                    service.disconnectHandler.PostDelayed(service.disconnectRunnable, DISCONNECT_DELAY_MS);
//                    service.ShowNotification("Desconexión en 10 segundos...", true);
//                }
//            }
//        }
//        else if (intent?.Action == ACTION_TRIGGER_CONNECT_VPN_SERVICE)
//        {
//            // Si se recibe una solicitud de conexión, cancelar cualquier desconexión pendiente
//            service.disconnectHandler?.RemoveCallbacks(service.disconnectRunnable);
//            service.disconnectRunnable = null;
//            if (!service.IsConnected)
//            {
//                service.ConnectVPNInternal();
//            }
//        }
//    }
//}

