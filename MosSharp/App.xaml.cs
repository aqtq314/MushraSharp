using CommonKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MosSharp
{
    public partial class App : Application
    {
        System.Collections.ArrayList AllLocaleDictionaries => (System.Collections.ArrayList)Resources[nameof(AllLocaleDictionaries)];
        ContainerResourceDictionary CurrentLocaleContainer => FindContainerDictionary(nameof(CurrentLocaleContainer));

        public static new App Current => (App)Application.Current;

        public App()
        {
            Startup += OnAppStartup;
        }

        private void OnAppStartup(object sender, StartupEventArgs e)
        {
            var uiRFC3066 = CultureInfo.CurrentUICulture.IetfLanguageTag;
            var uiLocaleDictionary = AllLocaleDictionaries
                .OfType<ResourceDictionary>()
                .FirstOrDefault(localeDictionary => (string)localeDictionary["S.Lang.RFC3066"] == uiRFC3066);

            if (uiLocaleDictionary != null)
                SetLocaleDictionary(uiLocaleDictionary);
        }

        ContainerResourceDictionary FindContainerDictionary(string key) =>
            Resources.MergedDictionaries.OfType<ContainerResourceDictionary>().First(container => container.Key == key);

        public void SetLocaleDictionary(ResourceDictionary resourceDictionary)
        {
            CurrentLocaleContainer.MergedDictionaries.Clear();
            CurrentLocaleContainer.MergedDictionaries.Add(resourceDictionary);
        }

        public string GetLocalizedString(string key)
        {
            var resources = Resources;
            if (resources.Contains(key))
                return resources[key] as string ?? key;
            else
                return key;
        }
    }
}
