using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using tweetz5.Model;

namespace tweetz5UnitTests.Model
{
    [TestClass]
    public class TwitterTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            WebRequestWrapper.OverrideImplementation = null;
        }

        [TestMethod]
        public async Task HomeTimelineTest()
        {
            const string json = @"[{""created_at"":""Mon Nov 26 00:18:26 +0000 2012"",""id"":272856804650782720,""id_str"":""272856804650782720"",""text"":""Mere sense is never to be talked to a mob"",""source"":""\u003ca href=\""http:\/\/itunes.apple.com\/us\/app\/twitter\/id409789998?mt=12\"" rel=\""nofollow\""\u003eTwitter for Mac\u003c\/a\u003e"",""truncated"":false,""in_reply_to_status_id"":null,""in_reply_to_status_id_str"":null,""in_reply_to_user_id"":null,""in_reply_to_user_id_str"":null,""in_reply_to_screen_name"":null,""user"":{""id"":140108433,""id_str"":""140108433"",""name"":""Angus Croll"",""screen_name"":""angustweets"",""location"":""San Francisco"",""description"":""super-fantastique"",""url"":""http:\/\/t.co\/U83LF6gR"",""entities"":{""url"":{""urls"":[{""url"":""http:\/\/t.co\/U83LF6gR"",""expanded_url"":""http:\/\/javascriptweblog.wordpress.com"",""display_url"":""javascriptweblog.wordpress.com"",""indices"":[0,20]}]},""description"":{""urls"":[]}},""protected"":false,""followers_count"":4514,""friends_count"":48,""listed_count"":390,""created_at"":""Tue May 04 16:11:55 +0000 2010"",""favourites_count"":1073,""utc_offset"":-28800,""time_zone"":""Pacific Time (US & Canada)"",""geo_enabled"":true,""verified"":false,""statuses_count"":2061,""lang"":""en"",""contributors_enabled"":false,""is_translator"":false,""profile_background_color"":""FFFFFF"",""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/635294458\/ir7ek09fhxqjp00h6hbe.jpeg"",""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/635294458\/ir7ek09fhxqjp00h6hbe.jpeg"",""profile_background_tile"":false,""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2777728124\/5e092d781248974754b97358565204c2_normal.png"",""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2777728124\/5e092d781248974754b97358565204c2_normal.png"",""profile_banner_url"":""https:\/\/si0.twimg.com\/profile_banners\/140108433\/1347913889"",""profile_link_color"":""2D0DE0"",""profile_sidebar_border_color"":""FFFFFF"",""profile_sidebar_fill_color"":""FFF7CC"",""profile_text_color"":""0C3E53"",""profile_use_background_image"":true,""default_profile"":false,""default_profile_image"":false,""following"":true,""follow_request_sent"":null,""notifications"":null},""geo"":null,""coordinates"":null,""place"":null,""contributors"":null,""retweet_count"":0,""entities"":{""hashtags"":[],""urls"":[],""user_mentions"":[]},""favorited"":false,""retweeted"":false}]";
            var mockWebRequest = new Mock<IWebRequest>();
            var mockWebResponse = new Mock<IWebResponse>();
            WebRequestWrapper.OverrideImplementation = (address) => mockWebRequest.Object;
            mockWebRequest.Setup(request => request.Headers.Add("Authorization", It.IsAny<string>()));
            mockWebRequest.Setup(request => request.GetResponseAsync()).Returns(Task.FromResult(mockWebResponse.Object));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            mockWebResponse.Setup(response => response.GetResponseStream()).Returns(stream);

            var timeline = await Twitter.HomeTimeline(1);
            timeline.Should().HaveCount(1);

            stream.Dispose();
            mockWebRequest.VerifyAll();
            mockWebResponse.VerifyAll();
        }

        [TestMethod]
        public async Task MentionsTimeLine()
        {
            const string json = @"[{""created_at"":""Mon Nov 26 00:18:26 +0000 2012"",""id"":272856804650782720,""id_str"":""272856804650782720"",""text"":""Mere sense is never to be talked to a mob"",""source"":""\u003ca href=\""http:\/\/itunes.apple.com\/us\/app\/twitter\/id409789998?mt=12\"" rel=\""nofollow\""\u003eTwitter for Mac\u003c\/a\u003e"",""truncated"":false,""in_reply_to_status_id"":null,""in_reply_to_status_id_str"":null,""in_reply_to_user_id"":null,""in_reply_to_user_id_str"":null,""in_reply_to_screen_name"":null,""user"":{""id"":140108433,""id_str"":""140108433"",""name"":""Angus Croll"",""screen_name"":""angustweets"",""location"":""San Francisco"",""description"":""super-fantastique"",""url"":""http:\/\/t.co\/U83LF6gR"",""entities"":{""url"":{""urls"":[{""url"":""http:\/\/t.co\/U83LF6gR"",""expanded_url"":""http:\/\/javascriptweblog.wordpress.com"",""display_url"":""javascriptweblog.wordpress.com"",""indices"":[0,20]}]},""description"":{""urls"":[]}},""protected"":false,""followers_count"":4514,""friends_count"":48,""listed_count"":390,""created_at"":""Tue May 04 16:11:55 +0000 2010"",""favourites_count"":1073,""utc_offset"":-28800,""time_zone"":""Pacific Time (US & Canada)"",""geo_enabled"":true,""verified"":false,""statuses_count"":2061,""lang"":""en"",""contributors_enabled"":false,""is_translator"":false,""profile_background_color"":""FFFFFF"",""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/635294458\/ir7ek09fhxqjp00h6hbe.jpeg"",""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/635294458\/ir7ek09fhxqjp00h6hbe.jpeg"",""profile_background_tile"":false,""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/2777728124\/5e092d781248974754b97358565204c2_normal.png"",""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/2777728124\/5e092d781248974754b97358565204c2_normal.png"",""profile_banner_url"":""https:\/\/si0.twimg.com\/profile_banners\/140108433\/1347913889"",""profile_link_color"":""2D0DE0"",""profile_sidebar_border_color"":""FFFFFF"",""profile_sidebar_fill_color"":""FFF7CC"",""profile_text_color"":""0C3E53"",""profile_use_background_image"":true,""default_profile"":false,""default_profile_image"":false,""following"":true,""follow_request_sent"":null,""notifications"":null},""geo"":null,""coordinates"":null,""place"":null,""contributors"":null,""retweet_count"":0,""entities"":{""hashtags"":[],""urls"":[],""user_mentions"":[]},""favorited"":false,""retweeted"":false}]";
            var mockWebRequest = new Mock<IWebRequest>();
            var mockWebResponse = new Mock<IWebResponse>();
            WebRequestWrapper.OverrideImplementation = (address) => mockWebRequest.Object;
            mockWebRequest.Setup(request => request.Headers.Add("Authorization", It.IsAny<string>()));
            mockWebRequest.Setup(request => request.GetResponseAsync()).Returns(Task.FromResult(mockWebResponse.Object));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            mockWebResponse.Setup(response => response.GetResponseStream()).Returns(stream);

            var timeline = await Twitter.MentionsTimeline(1);
            timeline.Should().HaveCount(1);

            stream.Dispose();
            mockWebRequest.VerifyAll();
            mockWebResponse.VerifyAll();            
        }
    }
}
