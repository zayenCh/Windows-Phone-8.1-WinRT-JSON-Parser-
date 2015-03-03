using JSONparser.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace JSONparser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //The Windows.Web.Http.HttpClient class provides the main class for 
        // sending HTTP requests and receiving HTTP responses from a resource identified by a URI.
        private HttpClient httpClient;
        private HttpResponseMessage response;

        // This is the feed address that will be parsed and displayed
        private String feedAddress = "http://www.telize.com/geoip";

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            httpClient = new HttpClient();

            // Add a user-agent header
            var headers = httpClient.DefaultRequestHeaders;

            // HttpProductInfoHeaderValueCollection is a collection of 
            // HttpProductInfoHeaderValue items used for the user-agent header

            headers.UserAgent.ParseAdd("ie");
            headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            response = new HttpResponseMessage();

            // if 'feedAddress' value changed the new URI must be tested --------------------------------
            // if the new 'feedAddress' doesn't work, 'feedStatus' informs the user about the incorrect input.

            feedStatus.Text = "Test if URI is valid";

            Uri resourceUri;
            if (!Uri.TryCreate(feedAddress.Trim(), UriKind.Absolute, out resourceUri))
            {
                feedStatus.Text = "Invalid URI, please re-enter a valid URI";
                return;
            }
            if (resourceUri.Scheme != "http" && resourceUri.Scheme != "https")
            {
                feedStatus.Text = "Only 'http' and 'https' schemes supported. Please re-enter URI";
                return;
            }
            // ---------- end of test---------------------------------------------------------------------

            string responseText;
            feedStatus.Text = "Waiting for response ...";

            try
            {
                response = await httpClient.GetAsync(resourceUri);

                response.EnsureSuccessStatusCode();

                responseText = await response.Content.ReadAsStringAsync();
               // statusPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                // Need to convert int HResult to hex string
                feedStatus.Text = "Error = " + ex.HResult.ToString("X") +
                    "  Message: " + ex.Message;
                responseText = "";
            }
            feedStatus.Text =  response.ReasonPhrase;

            // now 'responseText' contains the feed as a verified text.
            // next 'responseText' is converted as the rssItems class model definition to be displayed as a list

       

            try
            {
                DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(jsonItems));
                jsonItems cl = obj.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responseText))) as jsonItems;

                // every TextBlock is bound to the appropriate data: the final result of the JSON parsing
                txtcountry.Text = cl.country.ToString();
                txtteimezone.Text = cl.timezone;
                txtip.Text = cl.ip;
                txtcountrycode.Text = cl.country_code3;
            }
            catch (Exception)
            {
               
            }

          
            

        }
    }
}
