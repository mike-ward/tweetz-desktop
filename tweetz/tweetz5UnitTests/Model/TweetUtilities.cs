using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tweetz5.Model;

namespace tweetz5UnitTests.Model
{
    [TestClass]
    public class TweetUtilities
    {
        [TestMethod]
        public void ExtendedTweetShouldHaveCorrectMarkup()
        {
            var statuses = Status.ParseJson(Tweet);
            var tweet = statuses[0].CreateTweet(TweetClassification.Home);
            tweet.MarkupNodes.Length.Should().Be(11);
        }

        private const string Tweet = @"
        [{
          ""created_at"": ""Thu Nov 16 00:26:41 +0000 2017"",
          ""id"": 930955244724404224,
          ""id_str"": ""930955244724404224"",
          ""text"": ""RT @gerardsans: Yay! Play with the full power of *GraphQL real-time* using new Apollo Client 2.0 together w\/ latest Angular (v5) an\u2026 "",
          ""source"": ""\u003ca href=\""http:\/\/twitter.com\"" rel=\""nofollow\""\u003eTwitter Web Client\u003c\/a\u003e"",
          ""truncated"": false,
          ""in_reply_to_status_id"": null,
          ""in_reply_to_status_id_str"": null,
          ""in_reply_to_user_id"": null,
          ""in_reply_to_user_id_str"": null,
          ""in_reply_to_screen_name"": null,
          ""user"": {
            ""id"": 202230373,
            ""id_str"": ""202230373"",
            ""name"": ""Angular"",
            ""screen_name"": ""angular"",
            ""location"": null,
            ""url"": ""https:\/\/angular.io"",
            ""description"": ""One framework.\nMobile & desktop."",
            ""translator_type"": ""none"",
            ""protected"": false,
            ""verified"": false,
            ""followers_count"": 251952,
            ""friends_count"": 141,
            ""listed_count"": 4075,
            ""favourites_count"": 1892,
            ""statuses_count"": 2856,
            ""created_at"": ""Wed Oct 13 15:49:37 +0000 2010"",
            ""utc_offset"": -28800,
            ""time_zone"": ""Pacific Time (US & Canada)"",
            ""geo_enabled"": false,
            ""lang"": ""en"",
            ""contributors_enabled"": false,
            ""is_translator"": false,
            ""profile_background_color"": ""C0DEED"",
            ""profile_background_image_url"": ""http:\/\/abs.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""profile_background_image_url_https"": ""https:\/\/abs.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""profile_background_tile"": false,
            ""profile_link_color"": ""1DA1F2"",
            ""profile_sidebar_border_color"": ""C0DEED"",
            ""profile_sidebar_fill_color"": ""DDEEF6"",
            ""profile_text_color"": ""333333"",
            ""profile_use_background_image"": true,
            ""profile_image_url"": ""http:\/\/pbs.twimg.com\/profile_images\/727278046646915072\/cb8U-gL6_normal.jpg"",
            ""profile_image_url_https"": ""https:\/\/pbs.twimg.com\/profile_images\/727278046646915072\/cb8U-gL6_normal.jpg"",
            ""profile_banner_url"": ""https:\/\/pbs.twimg.com\/profile_banners\/202230373\/1462066193"",
            ""default_profile"": true,
            ""default_profile_image"": false,
            ""following"": null,
            ""follow_request_sent"": null,
            ""notifications"": null
          },
          ""geo"": null,
          ""coordinates"": null,
          ""place"": null,
          ""contributors"": null,
          ""retweeted_status"": {
            ""created_at"": ""Wed Nov 15 10:20:04 +0000 2017"",
            ""id"": 930742188321005568,
            ""id_str"": ""930742188321005568"",
            ""text"": ""Yay! Play with the full power of *GraphQL real-time* using new Apollo Client 2.0 together w\/ latest Angular (v5) an\u2026 https:\/\/t.co\/2pILdXPkvu"",
            ""display_text_range"": [
              0,
              140
            ],
            ""source"": ""\u003ca href=\""http:\/\/bufferapp.com\"" rel=\""nofollow\""\u003eBuffer\u003c\/a\u003e"",
            ""truncated"": true,
            ""in_reply_to_status_id"": null,
            ""in_reply_to_status_id_str"": null,
            ""in_reply_to_user_id"": null,
            ""in_reply_to_user_id_str"": null,
            ""in_reply_to_screen_name"": null,
            ""user"": {
              ""id"": 9284062,
              ""id_str"": ""9284062"",
              ""name"": ""\u1438GerardSans \/\u1433\u2615\ud83c\uddec\ud83c\udde7"",
              ""screen_name"": ""gerardsans"",
              ""location"": ""London \u2602"",
              ""url"": ""https:\/\/medium.com\/@gerard.sans"",
              ""description"": ""Google Developer Expert | Coding is fun | Just be awesome | Blogger Speaker Trainer Community Leader | @angular_zone @graphql_london @ngcruise"",
              ""translator_type"": ""none"",
              ""protected"": false,
              ""verified"": false,
              ""followers_count"": 8683,
              ""friends_count"": 2157,
              ""listed_count"": 356,
              ""favourites_count"": 9079,
              ""statuses_count"": 8148,
              ""created_at"": ""Sat Oct 06 20:04:48 +0000 2007"",
              ""utc_offset"": -10800,
              ""time_zone"": ""Greenland"",
              ""geo_enabled"": true,
              ""lang"": ""en"",
              ""contributors_enabled"": false,
              ""is_translator"": false,
              ""profile_background_color"": ""000000"",
              ""profile_background_image_url"": ""http:\/\/abs.twimg.com\/images\/themes\/theme1\/bg.png"",
              ""profile_background_image_url_https"": ""https:\/\/abs.twimg.com\/images\/themes\/theme1\/bg.png"",
              ""profile_background_tile"": false,
              ""profile_link_color"": ""FF0057"",
              ""profile_sidebar_border_color"": ""000000"",
              ""profile_sidebar_fill_color"": ""000000"",
              ""profile_text_color"": ""000000"",
              ""profile_use_background_image"": false,
              ""profile_image_url"": ""http:\/\/pbs.twimg.com\/profile_images\/796861611030024192\/pVl1eq7f_normal.jpg"",
              ""profile_image_url_https"": ""https:\/\/pbs.twimg.com\/profile_images\/796861611030024192\/pVl1eq7f_normal.jpg"",
              ""profile_banner_url"": ""https:\/\/pbs.twimg.com\/profile_banners\/9284062\/1495758442"",
              ""default_profile"": false,
              ""default_profile_image"": false,
              ""following"": null,
              ""follow_request_sent"": null,
              ""notifications"": null
            },
            ""geo"": null,
            ""coordinates"": null,
            ""place"": null,
            ""contributors"": null,
            ""is_quote_status"": false,
            ""extended_tweet"": {
              ""full_text"": ""Yay! Play with the full power of *GraphQL real-time* using new Apollo Client 2.0 together w\/ latest Angular (v5) and Angular Material (rc0) https:\/\/t.co\/r5yV8LiM5S #graphql #javascript #angular \u2728\ud83d\ude80 https:\/\/t.co\/JUqdYo14wa"",
              ""display_text_range"": [
                0,
                196
              ],
              ""entities"": {
                ""hashtags"": [
                  {
                    ""text"": ""graphql"",
                    ""indices"": [
                      164,
                      172
                    ]
                  },
                  {
                    ""text"": ""javascript"",
                    ""indices"": [
                      173,
                      184
                    ]
                  },
                  {
                    ""text"": ""angular"",
                    ""indices"": [
                      185,
                      193
                    ]
                  }
                ],
                ""urls"": [
                  {
                    ""url"": ""https:\/\/t.co\/r5yV8LiM5S"",
                    ""expanded_url"": ""https:\/\/github.com\/gsans\/todo-apollo-v2"",
                    ""display_url"": ""github.com\/gsans\/todo-apo\u2026"",
                    ""indices"": [
                      140,
                      163
                    ]
                  }
                ],
                ""user_mentions"": [


                ],
                ""symbols"": [


                ],
                ""media"": [
                  {
                    ""id"": 930742183002628096,
                    ""id_str"": ""930742183002628096"",
                    ""indices"": [
                      197,
                      220
                    ],
                    ""media_url"": ""http:\/\/pbs.twimg.com\/tweet_video_thumb\/DOqpFYtX4AAXsuT.jpg"",
                    ""media_url_https"": ""https:\/\/pbs.twimg.com\/tweet_video_thumb\/DOqpFYtX4AAXsuT.jpg"",
                    ""url"": ""https:\/\/t.co\/JUqdYo14wa"",
                    ""display_url"": ""pic.twitter.com\/JUqdYo14wa"",
                    ""expanded_url"": ""https:\/\/twitter.com\/gerardsans\/status\/930742188321005568\/photo\/1"",
                    ""type"": ""animated_gif"",
                    ""sizes"": {
                      ""large"": {
                        ""w"": 600,
                        ""h"": 350,
                        ""resize"": ""fit""
                      },
                      ""small"": {
                        ""w"": 600,
                        ""h"": 350,
                        ""resize"": ""fit""
                      },
                      ""thumb"": {
                        ""w"": 150,
                        ""h"": 150,
                        ""resize"": ""crop""
                      },
                      ""medium"": {
                        ""w"": 600,
                        ""h"": 350,
                        ""resize"": ""fit""
                      }
                    },
                    ""video_info"": {
                      ""aspect_ratio"": [
                        12,
                        7
                      ],
                      ""variants"": [
                        {
                          ""bitrate"": 0,
                          ""content_type"": ""video\/mp4"",
                          ""url"": ""https:\/\/video.twimg.com\/tweet_video\/DOqpFYtX4AAXsuT.mp4""
                        }
                      ]
                    }
                  }
                ]
              },
              ""extended_entities"": {
                ""media"": [
                  {
                    ""id"": 930742183002628096,
                    ""id_str"": ""930742183002628096"",
                    ""indices"": [
                      197,
                      220
                    ],
                    ""media_url"": ""http:\/\/pbs.twimg.com\/tweet_video_thumb\/DOqpFYtX4AAXsuT.jpg"",
                    ""media_url_https"": ""https:\/\/pbs.twimg.com\/tweet_video_thumb\/DOqpFYtX4AAXsuT.jpg"",
                    ""url"": ""https:\/\/t.co\/JUqdYo14wa"",
                    ""display_url"": ""pic.twitter.com\/JUqdYo14wa"",
                    ""expanded_url"": ""https:\/\/twitter.com\/gerardsans\/status\/930742188321005568\/photo\/1"",
                    ""type"": ""animated_gif"",
                    ""sizes"": {
                      ""large"": {
                        ""w"": 600,
                        ""h"": 350,
                        ""resize"": ""fit""
                      },
                      ""small"": {
                        ""w"": 600,
                        ""h"": 350,
                        ""resize"": ""fit""
                      },
                      ""thumb"": {
                        ""w"": 150,
                        ""h"": 150,
                        ""resize"": ""crop""
                      },
                      ""medium"": {
                        ""w"": 600,
                        ""h"": 350,
                        ""resize"": ""fit""
                      }
                    },
                    ""video_info"": {
                      ""aspect_ratio"": [
                        12,
                        7
                      ],
                      ""variants"": [
                        {
                          ""bitrate"": 0,
                          ""content_type"": ""video\/mp4"",
                          ""url"": ""https:\/\/video.twimg.com\/tweet_video\/DOqpFYtX4AAXsuT.mp4""
                        }
                      ]
                    }
                  }
                ]
              }
            },
            ""quote_count"": 0,
            ""reply_count"": 1,
            ""retweet_count"": 9,
            ""favorite_count"": 40,
            ""entities"": {
              ""hashtags"": [


              ],
              ""urls"": [
                {
                  ""url"": ""https:\/\/t.co\/2pILdXPkvu"",
                  ""expanded_url"": ""https:\/\/twitter.com\/i\/web\/status\/930742188321005568"",
                  ""display_url"": ""twitter.com\/i\/web\/status\/9\u2026"",
                  ""indices"": [
                    117,
                    140
                  ]
                }
              ],
              ""user_mentions"": [


              ],
              ""symbols"": [


              ]
            },
            ""favorited"": false,
            ""retweeted"": false,
            ""possibly_sensitive"": false,
            ""filter_level"": ""low"",
            ""lang"": ""en""
          },
          ""is_quote_status"": false,
          ""quote_count"": 0,
          ""reply_count"": 0,
          ""retweet_count"": 0,
          ""favorite_count"": 0,
          ""entities"": {
            ""hashtags"": [


            ],
            ""urls"": [


            ],
            ""user_mentions"": [
              {
                ""screen_name"": ""gerardsans"",
                ""name"": ""\u1438GerardSans \/\u1433\u2615\ud83c\uddec\ud83c\udde7"",
                ""id"": 9284062,
                ""id_str"": ""9284062"",
                ""indices"": [
                  3,
                  14
                ]
              }
            ],
            ""symbols"": [


            ]
          },
          ""favorited"": false,
          ""retweeted"": false,
          ""filter_level"": ""low"",
          ""lang"": ""en"",
          ""timestamp_ms"": ""1510792001311""
        }]";
    }
}
