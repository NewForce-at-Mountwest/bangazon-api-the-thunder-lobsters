<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc.Testing;
using BangazonAPI;
using System.Net.Http;
using Xunit;

=======
﻿using BangazonAPI;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;
>>>>>>> master
namespace TestBangazonAPI
{
    class APIClientProvider : IClassFixture<WebApplicationFactory<Startup>>
    {
<<<<<<< HEAD
        public HttpClient Client
        {
            get; private set;
        }
        private readonly WebApplicationFactory<Startup> _factory = new WebApplicationFactory<Startup>();

=======
        public HttpClient Client { get; private set; }
        private readonly WebApplicationFactory<Startup> _factory = new WebApplicationFactory<Startup>();
>>>>>>> master
        public APIClientProvider()
        {
            Client = _factory.CreateClient();
        }
<<<<<<< HEAD

=======
>>>>>>> master
        public void Dispose()
        {
            _factory?.Dispose();
            Client?.Dispose();
        }
    }
}
