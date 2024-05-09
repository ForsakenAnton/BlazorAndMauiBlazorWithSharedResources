using Microsoft.Extensions.Logging;

namespace MauiBlazorHybridApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Create HttpClient
            builder.Services.AddScoped(sp =>
            {
                Uri baseAddress;

                // Кожен екземпляр Android Emulator ізольований
                // від мережевих інтерфейсів комп'ютера розробки
                // за допомогою віртуального маршрутизатора.
                // Таким чином, емульований пристрій не може бачити
                // комп'ютер розробки або інші екземпляри емулятора в мережі.
                // Віртуальний маршрутизатор кожного емулятора керує
                // спеціалізованим мережевим простором, що має
                // попередньо виділені адреси, а адреса 10.0.2.2
                // є псевдонімом для інтерфейсу замикання вузла на себе
                // (127.0.0.1 на комп'ютері розробки).
                // https://learn.microsoft.com/ru-ru/dotnet/maui/data-cloud/local-web-services?view=net-maui-8.0#android
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    baseAddress = new Uri("https://10.0.2.2:7217");
                }
                else if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    baseAddress = new Uri("https://localhost:7217");
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }

#if DEBUG
                // (!) Довіряємо сертифікату, без цього запит до API з Android НЕ запрацює
                // https://learn.microsoft.com/ru-ru/dotnet/maui/data-cloud/local-web-services?view=net-maui-8.0#bypass-the-certificate-security-check
                HttpsClientHandlerService handler = new HttpsClientHandlerService();

                return new HttpClient(handler.GetPlatformMessageHandler()) // (!)
                {
                    BaseAddress = baseAddress 
                };
#else
                return new HttpClient("Qwerty");
#endif
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
