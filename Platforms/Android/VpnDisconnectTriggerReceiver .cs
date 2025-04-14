using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAP_Movil.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = false)] // Exported a false para que solo tu app lo reciba
    [IntentFilter(new[] { "com.spc.ACTION_TRIGGER_DISCONNECT_VPN" })] // Acción para activar la desconexión
    internal class VpnDisconnectTriggerReceiver : BroadcastReceiver
    {
        private const string profileName = "MiVPN";
        private const string OpenVpnPackage = "de.blinkt.openvpn";
        private const string DisconnectAction = "de.blinkt.openvpn.DISCONNECT";
        private bool Isconnected = false;

        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context != null && intent?.Action == "com.spc.ACTION_TRIGGER_DISCONNECT_VPN")
            {
                Thread.Sleep(10000); 
                if (Isconnected)
                {
                    DisconnectVPN(context);                    
                }               
            }
            else if (context != null && intent?.Action == "com.spc.ACTION_TRIGGER_CONNECT_VPN")
            {
                if (!Isconnected)
                {
                    ConnectVPN(context);
                }
            }
        }

        private void ConnectVPN(Context context)
        {
            try
            {
                // Crear el Intent para conectar a OpenVPN
                Intent connectIntent = new Intent(Intent.ActionMain);
                connectIntent.SetClassName("de.blinkt.openvpn", "de.blinkt.openvpn.api.ConnectVPN");
                connectIntent.PutExtra("de.blinkt.openvpn.api.profileName", profileName);

                context.SendBroadcast(connectIntent);
                Isconnected = true;
            }
            catch (ActivityNotFoundException e)
            {
                // Mostrar un Toast si no se encuentra la actividad
                Toast.MakeText(context, "No se pudo encontrar la actividad de OpenVPN: " + e.Message, ToastLength.Long).Show();
            }
        }

        private void DisconnectVPN(Context context)
        {
            try
            {
                // Verificar si la aplicación OpenVPN está instalada
                PackageManager pm = context.PackageManager;
                pm.GetPackageInfo(OpenVpnPackage, PackageInfoFlags.Activities);

                Intent disconnectIntent = new Intent(DisconnectAction);
                disconnectIntent.SetPackage(OpenVpnPackage);
                context.SendBroadcast(disconnectIntent);

                Isconnected = false;

                // Opcional: Mostrar un Toast para indicar que se intentó la desconexión
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(context, "Enviando solicitud de desconexión a OpenVPN...", ToastLength.Short).Show();
                });
            }
            catch (PackageManager.NameNotFoundException)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(context, "La aplicación OpenVPN no está instalada.", ToastLength.Long).Show();
                });
            }
            catch (ActivityNotFoundException e)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(context, $"No se encontró la actividad de OpenVPN: {e.Message}", ToastLength.Long).Show();
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(context, $"Error al intentar desconectar la VPN: {ex.Message}", ToastLength.Long).Show();
                });
            }
        }
    }
}
