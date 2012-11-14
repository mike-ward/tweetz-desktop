// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tweetz5.Model;

namespace tweetz5UnitTests.Model
{
    [TestClass]
    public class OAuthTests
    {
        [TestMethod]
        public void UrlEncodeEncodeTests()
        {
            Assert.AreEqual(OAuth.UrlEncode("%"), "%25");
            const string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            OAuth.UrlEncode(unreservedChars).Should().Be(unreservedChars);
        }

        [TestMethod]
        public void NounceReturnsDifferntValuesOnEachCall()
        {
            OAuth.Nonce().Should().NotBe(OAuth.Nonce());
        }

        [TestMethod]
        public void TimeStampReturnsValid64IntString()
        {
            var timeStamp = OAuth.TimeStamp();
            var int64 = Int64.Parse(timeStamp);
            int64.Should().BeGreaterThan(1352852131);
        }

        [TestMethod]
        public void AuthorizationHeaderIsCorrectFormat()
        {
            var oAuth = new OAuth
            {
                ConsumerKey = "4431",
                ConsumerSecret = "xa2Q",
                AccessToken = "s8I",
                AccessTokenSecret = "zEgShI"
            };
            const string authorizationHeader =
                "OAuth " +
                    "oauth_consumer_key=\"4431\"," +
                    "oauth_nonce=\"123\"," +
                    "oauth_timestamp=\"345\"," +
                    "oauth_token=\"s8I\"," +
                    "oauth_signature=\"signature\"," +
                    "oauth_signature_method=\"HMAC-SHA1\"," +
                    "oauth_version=\"1.0\"";
            oAuth.AuthorizationHeader("123", "345", "signature").Should().Be(authorizationHeader);
        }

        [TestMethod]
        public void SignatureMethodTest()
        {
            var oAuth = new OAuth
            {
                ConsumerKey = "4431",
                ConsumerSecret = "xa2Q",
                AccessToken = "s8I",
                AccessTokenSecret = "zEgShI"
            };
            var signature = oAuth.Signature("GET", "http://twitter.com", "123", "456", new[] {new[] {"entities", "true"}});
            signature.Should().Be("DFXMW0qmQACqiAnGTOJFKJlkn3c=");
        }
    }
}