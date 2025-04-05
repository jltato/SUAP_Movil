using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;

#if ANDROID
using Android.Webkit;
#endif

namespace SUAP_Movil
{
    public partial class MainPage : ContentPage
    {
       

        public MainPage()
        {
            InitializeComponent();


            #if ANDROID
                        webView.HandlerChanged += (s, e) =>
                        {
                            var handler = webView.Handler as WebViewHandler;
                            var nativeWebView = handler?.PlatformView as Android.Webkit.WebView;
                            nativeWebView?.ClearCache(true);
                            nativeWebView?.ClearHistory();
                        };
            #endif
        }


        protected override bool OnBackButtonPressed()
        {
            if (webView.CanGoBack)
            {
                webView.GoBack();
                return true; // Cancelar la acción por defecto (salir de la app)
            }

            return base.OnBackButtonPressed(); // No hay historial, continuar con comportamiento por defecto
        }







}

}
