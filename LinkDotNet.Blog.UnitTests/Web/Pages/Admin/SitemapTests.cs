﻿using System.Collections.Generic;
using System.Linq;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using LinkDotNet.Blog.Web.Pages.Admin;
using LinkDotNet.Blog.Web.Shared.Services.Sitemap;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Pages.Admin;

public class SitemapTests : TestContext
{
    [Fact]
    public void ShouldSaveSitemap()
    {
        this.AddTestAuthorization().SetAuthorized("steven");
        var sitemapMock = new Mock<ISitemapService>();
        Services.AddScoped(_ => sitemapMock.Object);
        var sitemap = new SitemapUrlSet();
        sitemapMock.Setup(s => s.CreateSitemapAsync())
            .ReturnsAsync(sitemap);
        var cut = RenderComponent<Sitemap>();

        cut.Find("button").Click();

        sitemapMock.Verify(s => s.SaveSitemapToFileAsync(sitemap));
    }

    [Fact]
    public void ShouldDisplaySitemap()
    {
        this.AddTestAuthorization().SetAuthorized("steven");
        var sitemapMock = new Mock<ISitemapService>();
        Services.AddScoped(_ => sitemapMock.Object);
        var sitemap = new SitemapUrlSet
        {
            Urls = new List<SitemapUrl>
                {
                    new() { Location = "loc", LastModified = "Now" },
                },
        };
        sitemapMock.Setup(s => s.CreateSitemapAsync())
            .ReturnsAsync(sitemap);
        var cut = RenderComponent<Sitemap>();

        cut.Find("button").Click();

        cut.WaitForState(() => cut.FindAll("tr").Count > 1);
        var row = cut.FindAll("tr").Last();
        row.Children.First().InnerHtml.Should().Be("loc");
        row.Children.Last().InnerHtml.Should().Be("Now");
    }
}