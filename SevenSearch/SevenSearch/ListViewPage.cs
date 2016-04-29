using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.IO;
using PCLStorage;
using System.Threading.Tasks;

namespace SevenSearch
{
    public class ListViewPage : ContentPage
    {
        private Button search_button_;
        private Entry city_contect_;
        private Entry area_contect_;
        private List<SevenStore> store_data_list_;
        private readonly WebApiServices web_api_;

        public ListViewPage(string v)
        {
            Title = v;

            store_data_list_ = new List<SevenStore>();
            web_api_ = new WebApiServices();

            search_button_ = new Button() { Text = "Search"};
            city_contect_ = new Entry { Placeholder = "請輸入城市名稱: ex: 台北市" };
            area_contect_ = new Entry { Placeholder = "請輸入區域名稱: ex: 中山區" };

            if (string.IsNullOrWhiteSpace(city_contect_.Text))
            {
                city_contect_.Text = "台北市";
            }
            if (string.IsNullOrWhiteSpace(area_contect_.Text))
            {
                area_contect_.Text = "中山區";
            }

            search_button_.Clicked += async (sender, e) =>
            {
                store_data_list_ = await GetSevenStoreData(city_contect_.Text, area_contect_.Text);
                if (store_data_list_.Count > 0)
                {
                    var sd = store_data_list_.Select(s => new StoreData
                    {
                        Name = s.POIName,
                        Addr = s.Address,
                        Tel = s.Telno
                    }).ToList();
                    BindListView(sd);
                }
                await DisplayAlert("搜尋結果", store_data_list_.Count + "筆", "Close");
            };

            var defaultData = new List<StoreData>
            {
                new StoreData {Name = "大台", Addr = "台北市大安區羅斯福路3段283巷14弄16號1樓", Tel = "(02)23636229"},
                new StoreData {Name = "大安", Addr = "台北市大安區大安路1段77號B1樓(東區地下街)", Tel = "(02)87728951"},
            };

            BindListView(defaultData);
        }

        private async Task<List<SevenStore>> GetSevenStoreData(string city, string area)
        {
            var result = new List<SevenStore>();

            var resultData = await web_api_.GetDataAsync(city_contect_.Text, area_contect_.Text);

            if (string.IsNullOrWhiteSpace(resultData) == false)
            {
                var xdoc = XDocument.Parse(resultData, LoadOptions.None);
                var mapsdk = from geoposition in xdoc.Descendants("GeoPosition")
                             select new SevenStore
                             {
                                 POIName = geoposition.Element("POIName").Value,
                                 Address = geoposition.Element("Address").Value,
                                 Telno = geoposition.Element("Telno").Value
                             };
                string json = JsonConvert.SerializeObject(mapsdk);
                result = JsonConvert.DeserializeObject<List<SevenStore>>(json);
            }

            return result;
        }

        private void BindListView(List<StoreData> sourceData)
        {
            var listView = new ListView
            {
                IsPullToRefreshEnabled = true,
                RowHeight = 80,
                ItemsSource = sourceData,
                ItemTemplate = new DataTemplate(typeof(ListViewCell))
            };

            listView.IsPullToRefreshEnabled = false;
            listView.ItemTapped += (sender, e) =>
            {
                var baseUrl = "https://www.google.com.tw/maps/place/";
                var storeData = e.Item as StoreData;

                if (storeData != null)
                {
                    Device.OpenUri(new Uri($"{baseUrl}{storeData.Addr}"));
                }

                ((ListView)sender).SelectedItem = null;
            };

            Padding = new Thickness(0, 20, 0, 0);
            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    city_contect_,
                    area_contect_,
                    search_button_,
                    new Label
                    {
                        HorizontalTextAlignment= TextAlignment.Center,
                        Text = Title,
                        FontSize = 30
                    },
                    listView
                }
            };
        }
    }
}