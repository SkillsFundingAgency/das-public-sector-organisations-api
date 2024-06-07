using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Domain.Exceptions;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.Ons.OnsExcelReaderServiceUsingClosedXml;

public class WhenReadingAnExcelFile 
{
    [Test]
    public async Task Then_reads_valid_file_successfully()
    {
        var path = TestContext.CurrentContext.TestDirectory;

        var sut = new Data.Ons.OnsExcelReaderServiceUsingClosedXml(
            Mock.Of<ILogger<Data.Ons.OnsExcelReaderServiceUsingClosedXml>>());

        var rows = sut.GetOnsDataFromSpreadsheet(path + @"\Data\pscgmay2024Test.xlsx");

        rows.Count.Should().Be(7);
        var first = rows.First();
        first.Name.Should().StartWith("124 Facilities Ltd");
        first.Sector.Should().StartWith("Not in the Public");
        first.EsaCode.Should().StartWith("Disbanded or Deleted");

        var last = rows.Last();
        last.Name.Should().StartWith("Aberdeen City");
        last.Sector.Should().StartWith("Local Gov");
        last.EsaCode.Should().StartWith("S.13");
    }

    [Test]
    public async Task Then_reads_invalid_file_and_throws_error()
    {
        var path = TestContext.CurrentContext.TestDirectory;

        var sut = new Data.Ons.OnsExcelReaderServiceUsingClosedXml(
            Mock.Of<ILogger<Data.Ons.OnsExcelReaderServiceUsingClosedXml>>());

        try
        {
            sut.GetOnsDataFromSpreadsheet(path + @"\Data\pscgjan2024Test.xlsx");
            Assert.Fail("Should reach here");

        }
        catch (ReadingOnsExcelFileException ex)
        {
            ex.InnerException.Message.Should().Be("Expected column title 'Name' not present");
        }
    }
}