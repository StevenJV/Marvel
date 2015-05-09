using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MarvelConsole1
{
  class Program
  {
    static void Main(string[] args) {

      var client = new RestClient("http://gateway.marvel.com:80/v1/public/");

      var request = new RestRequest("characters/1009220", Method.GET);

      var timestamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
      string pubkey = ConfigurationManager.AppSettings["pubkey"];
      string privkey = ConfigurationManager.AppSettings["privkey"];
      var hash = CalculateMd5Hash(timestamp + privkey + pubkey);

      request.AddParameter("ts", timestamp);
      request.AddParameter("apikey", pubkey);
      request.AddParameter("hash", hash);

      var response = client.Execute(request);
      if (response == null) { throw new Exception("Marvel responded with null or did not respond at all.");}
      var content = response.Content;
      var marvelResult = JObject.Parse(content);

      var marvelDataInfo = JsonConvert.DeserializeObject<MarvelDataInfo>(marvelResult.ToString());

      var resultItemsToken = marvelResult["data"]["results"][0]["comics"]["items"];
      IList<ComicsItem> comicsItems = resultItemsToken.Select(item => JsonConvert.DeserializeObject<ComicsItem>(item.ToString())).ToList();

      foreach (var item in comicsItems) Console.WriteLine(item.name);
      Console.WriteLine("------");
      Console.WriteLine(marvelDataInfo.attributionText);

      Console.ReadLine();

    }


    private static string CalculateMd5Hash(string input) {
      // ganked from http://bit.ly/1JvEhVZ
      // step 1, calculate MD5 hash from input
      var md5 = System.Security.Cryptography.MD5.Create();
      var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
      var hash = md5.ComputeHash(inputBytes);
      // step 2, convert byte array to hex string
      var sb = new StringBuilder();
      foreach (var t in hash) {
        sb.Append(t.ToString("x2"));
      }
      return sb.ToString();
    }

  }

  public class Url
  {
    public string type { get; set; }
    public string url { get; set; }
  }

  public class Thumbnail
  {
    public string path { get; set; }
    public string extension { get; set; }
  }

  public class ComicsItem
  {
    public string resourceUri { get; set; }
    public string name { get; set; }
  }

  public class Comics
  {
    public string available { get; set; }
    public string returned { get; set; }
    public string collectionURI { get; set; }
    public IList<ComicsItem> comicsitems { get; set; }
  }

  public class Storiesitem
  {
    public string resourceURI { get; set; }
    public string name { get; set; }
    public string type { get; set; }
  }

  public class Stories
  {
    public string available { get; set; }
    public string returned { get; set; }
    public string collectionURI { get; set; }
    public IList<Storiesitem> storiesitems { get; set; }
  }

  public class Eventsitem
  {
    public string resourceURI { get; set; }
    public string name { get; set; }
  }

  public class Events
  {
    public string available { get; set; }
    public string returned { get; set; }
    public string collectionURI { get; set; }
    public IList<Eventsitem> eventsitems { get; set; }
  }

  public class Seriesitem
  {
    public string resourceURI { get; set; }
    public string name { get; set; }
  }

  public class Series
  {
    public string available { get; set; }
    public string returned { get; set; }
    public string collectionURI { get; set; }
    public IList<Seriesitem> seriesitems { get; set; }
  }

  public class Result
  {
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string modified { get; set; }
    public string resourceURI { get; set; }
    public IList<Url> urls { get; set; }
    public Thumbnail thumbnail { get; set; }
    public Comics comics { get; set; }
    public Stories stories { get; set; }
    public Events events { get; set; }
    public Series series { get; set; }
  }

  public class Data
  {
    public string offset { get; set; }
    public string limit { get; set; }
    public string total { get; set; }
    public string count { get; set; }
    public IList<Result> results { get; set; }
  }

  public class MarvelDataInfo
  {
    public string code { get; set; }
    public string status { get; set; }
    public string copyright { get; set; }
    public string attributionText { get; set; }
    public string attributionHTML { get; set; }
    public Data data { get; set; }
    public string etag { get; set; }
  }


}
