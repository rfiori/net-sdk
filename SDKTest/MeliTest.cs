using System;
using NUnit.Framework;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using RestSharp;
using System.Collections.Generic;


namespace MercadoLibre.SDK.Test
{
    [TestFixture]
    public class MeliTest
    {
        readonly long _ClienteID = 12345;
        readonly string _ClientSecret = "zxczxczxc";
        readonly string _AccessToken = "";

        // Set your ML Country (Meli.AuthUrls.MLA, Meli.AuthUrls.MLC, Meli.AuthUrls.MLB, Meli.AuthUrls.MLM, ...)
        readonly string _MLCountry = Meli.AuthUrls.MLB;


        [Test]
        public void GetAuthUrl()
        {
            var m = new Meli(_ClienteID, _ClientSecret);
            var ml_auth = m.GetAuthUrl(_MLCountry, "");
            Assert.AreEqual(_MLCountry + "/authorization?response_type=code&client_id=" + _ClienteID + "&redirect_uri=", ml_auth);
        }

        [Test]
        public void AuthorizationSuccess()
        {
            Meli.ApiUrl = "http://localhost:3000";

            Meli m = new Meli(_ClienteID, _ClientSecret);
            m.Authorize("valid code with refresh token", "http://someurl.com");

            Assert.AreEqual(_ClientSecret, m.AccessToken);
            Assert.AreEqual("valid refresh token", m.RefreshToken);
        }

        [Test]
        public void AuthorizationFailure()
        {
            Meli.ApiUrl = "http://localhost:3000";

            Meli m = new Meli(_ClienteID, _ClientSecret);

            Assert.Throws<AuthorizationException>(() => m.Authorize("invalid code", "http://someurl.com"));
        }

        [Test]
        public void Get()
        {
            Meli.ApiUrl = "http://localhost:3000";

            Meli m = new Meli(_ClienteID, _ClientSecret, _ClientSecret);

            var response = m.Get("/sites");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.That(response.Content, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void GetWithRefreshToken()
        {
            Meli.ApiUrl = "http://localhost:3000";

            Meli m = new Meli(_ClienteID, _ClientSecret, "expired token", "valid refresh token");

            var p = new Parameter();
            p.Name = "access_token";
            p.Value = m.AccessToken;

            var ps = new List<Parameter>();
            ps.Add(p);
            var response = m.Get("/users/me", ps);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.That(response.Content, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void HandleErrors()
        {
            Meli.ApiUrl = "http://localhost:3000";
            Meli m = new Meli(_ClienteID, _ClientSecret, "invalid token");

            var p = new Parameter();
            p.Name = "access_token";
            p.Value = m.AccessToken;
            var ps = new List<Parameter>();
            ps.Add(p);
            var response = m.Get("/users/me", ps);
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Test]
        public void Post()
        {
            Meli.ApiUrl = "http://localhost:3000";
            Meli m = new Meli(_ClienteID, _ClientSecret, _ClientSecret);

            var p = new Parameter();
            p.Name = "access_token";
            p.Value = m.AccessToken;

            var ps = new List<Parameter>();
            ps.Add(p);
            var r = m.Post("/items", ps, new { foo = "bar" });

            Assert.AreEqual(HttpStatusCode.Created, r.StatusCode);
        }

        [Test]
        public void PostWithRefreshToken()
        {
            Meli.ApiUrl = "http://localhost:3000";
            Meli m = new Meli(_ClienteID, _ClientSecret, "expired token", "valid refresh token");

            var p = new Parameter();
            p.Name = "access_token";
            p.Value = m.AccessToken;

            var ps = new List<Parameter>();
            ps.Add(p);
            var r = m.Post("/items", ps, new { foo = "bar" });

            Assert.AreEqual(HttpStatusCode.Created, r.StatusCode);
        }

        [Test]
        public void Put()
        {
            Meli.ApiUrl = "http://localhost:3000";
            Meli m = new Meli(_ClienteID, _ClientSecret, _ClientSecret);

            var p = new Parameter();
            p.Name = "access_token";
            p.Value = m.AccessToken;

            var ps = new List<Parameter>();
            ps.Add(p);
            var r = m.Put("/items/123", ps, new { foo = "bar" });

            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
        }

        [Test]
        public void PutWithRefreshToken()
        {
            Meli.ApiUrl = "http://localhost:3000";
            Meli m = new Meli(_ClienteID, _ClientSecret, "expired token", "valid refresh token");

            var p = new Parameter();
            p.Name = "access_token";
            p.Value = m.AccessToken;

            var ps = new List<Parameter>();
            ps.Add(p);
            var r = m.Put("/items/123", ps, new { foo = "bar" });

            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
        }

        [Test]
        public void Delete()
        {
            Meli.ApiUrl = "http://localhost:3000";
            Meli m = new Meli(_ClienteID, _ClientSecret, _ClientSecret);

            var p = new Parameter();
            p.Name = "access_token";
            p.Value = m.AccessToken;

            var ps = new List<Parameter>();
            ps.Add(p);
            var r = m.Delete("/items/123", ps);

            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
        }

        [Test]
        public void DeleteWithRefreshToken()
        {
            Meli.ApiUrl = "http://localhost:3000";
            Meli m = new Meli(_ClienteID, _ClientSecret, "expired token", "valid refresh token");

            var p = new Parameter();
            p.Name = "access_token";
            p.Value = m.AccessToken;

            var ps = new List<Parameter>();
            ps.Add(p);
            var r = m.Delete("/items/123", ps);

            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
        }

        [Test]
        public void TestUserAgent()
        {
            Meli.ApiUrl = "http://localhost:3000";
            Meli m = new Meli(_ClienteID, _ClientSecret, "expired token", "valid refresh token");

            var response = m.Get("/echo/user_agent");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}