using Pg.DataverseSync.Domain.Dto;
using Pg.DataverseSync.Domain.Services;
using Pg.DataverseSync.Domain.Tests.Core;
using System;
using System.Collections.Generic;
using Xunit;

namespace Pg.DataverseSync.Domain.Tests.Services
{
    public class ParseToJsonServiceTests : ServiceTestBase
    {
        private readonly ParseToJsonService _service;

        public ParseToJsonServiceTests()
        {
            _service = new ParseToJsonService(null, this.tracingService);
        }

        [Fact]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.Parse(null));
        }

        [Fact]
        public void Parse_EmptyList_ReturnsEmptyJsonArray()
        {
            var list = new List<Table>();
            var json = _service.Parse(list);
            Assert.Equal("[]", json);
        }

        [Fact]
        public void Parse_FilledList_ReturnsExpectedJson()
        {
            var expected = "[{\"Name\":\"Account\",\"SchemaName\":\"account\"},{\"Name\":\"Contact\",\"SchemaName\":\"contact\"}]"; 
            var list = new List<Table>
            {
                new Table { Name = "Account", SchemaName = "account"},
                new Table { Name = "Contact", SchemaName = "contact"},
            };

            var actual = _service.Parse(list);
           
            Assert.Equal(expected, actual);
        }
    }
}

