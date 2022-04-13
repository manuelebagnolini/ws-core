﻿using xCore.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace xCore
{
    public class Injector : BaseTest
    {
        public Injector(Program factory, ITestOutputHelper output) : base(factory, output)
        {
        }

        [Theory]
        [InlineData("/?culture=fr-FR", "fr-FR")]
        [InlineData("/api/diagnostic?culture=en-US", "en-US")]
        public async Task Get_HeaderCurrentCulture(string url, string expectedCulture)
        {
            // Arrange
            var factory = GetFactory(WebApplicationFactoryType.Local);
            var client = factory.CreateClient();
            // Act
            var response = await client.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();
            response.Headers.TryGetValues("X-culture", out IEnumerable<string> values);
            var culture = values?.FirstOrDefault();
            _output.Write(url, response.StatusCode.ToString(), response.Headers.ToString(), response.Content.Headers.ToString(), content, culture);

            // Assert
            Assert.Equal(expectedCulture, culture);
        }

        [Theory]
        [InlineData("/branch?text=hello-world", "hello-world")]
        public async Task Get_ContentTextByQsOnMap(string url, string expectedContent)
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            _output.Write(url, response.StatusCode.ToString(), response.Headers.ToString(), response.Content.Headers.ToString(), content);

            // Assert
            Assert.Equal(content, expectedContent);
        }
    }

}
