﻿using Flurl.Http.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftwareApproach.TestingExtensions;
using Flurl;
using System.Net.Http;

namespace Taut.Test
{
    [TestClass]
    public class BaseApiServiceTests
    {
        private const string STUB_RESPONSE = "{\"ok\":true}";

        private HttpTest _httpTest;

        [TestInitialize]
        public void Setup()
        {
            _httpTest = new HttpTest();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpTest.Dispose();
        }

        #region BuildRequestUrl

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GivenABaseApiService_WhenMethodIsNull_ThenBuildRequestUrlThrowsException()
        {
            // Arrange
            var apiService = new StubApiService();

            // Act
            apiService.BuildRequestUrl(null);
        }

        [TestMethod]
        public void GivenABaseApiService_WhenMethodHasValue_ThenItIsAppendedToUrl()
        {
            // Arrange
            var apiService = new StubApiService();

            // Act
            var url = apiService.BuildRequestUrl("test");

            // Assert
            url.Path.ShouldEqual("https://slack.com/api/test");
        }

        [TestMethod]
        public void GivenABaseApiService_WhenQueryParamsIsNull_ThenBuildRequestUrlAppendsNoParams()
        {
            // Arrange
            var apiService = new StubApiService();

            // Act
            var url = apiService.BuildRequestUrl("test", null);

            // Assert
            url.QueryParams.ShouldBeEmpty();
        }

        #endregion

        #region GetResponseAsync

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GivenABaseApiService_WhenUrlIsNull_ThenGetResponseAsyncThrowsException()
        {
            // Arrange
            var apiService = new StubApiService();

            // Act
            await apiService.GetResponseAsync<StubResponse>(null);
        }

        [TestMethod]
        public async Task GivenABaseApiService_WhenUrlHasValue_ThenGetResponseAsyncDeserializesResponse()
        {
            // Arrange
            var apiService = new StubApiService();
            _httpTest.RespondWith(STUB_RESPONSE);

            // Act
            var response = await apiService.GetResponseAsync<StubResponse>(new Url(BaseApiService.API_URL));

            // Assert
            response.Ok.ShouldBeTrue();
        }

        [TestMethod]
        public async Task GivenABaseApiService_WhenUrlHasValue_ThenGetResponseAsyncMakesGetRequest()
        {
            // Arrange
            var apiService = new StubApiService();
            _httpTest.RespondWith(STUB_RESPONSE);

            // Act
            var response = await apiService.GetResponseAsync<StubResponse>(new Url(BaseApiService.API_URL));

            // Assert
            _httpTest.ShouldHaveCalled(BaseApiService.API_URL)
                .WithVerb(HttpMethod.Get)
                .Times(1);
        }

        #endregion
    }

    internal class StubApiService : BaseApiService {}

    internal class StubResponse
    {
        public bool Ok { get; set; }
    }
}