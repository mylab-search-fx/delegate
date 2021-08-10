using System;
using System.Threading.Tasks;
using Xunit;

namespace FunctionTests
{
    public partial class QueryProcessingBehavior 
    {
        [Fact]
        public async Task ShouldFindText()
        {
            //Arrange
            var indexName = CreateIndexName();

            await using var disposer = await _esFxt.Manager.CreateIndexAsync(indexName);

            var indexer = _esFxt.CreateIndexer<TestEntity>().ForIndex(indexName);

            await indexer.IndexManyAsync(new[]
            {
                new TestEntity{ Id = 1, Value = "foo"}, 
                new TestEntity{ Id = 2, Value = "bar"}, 
                new TestEntity{ Id = 3, Value = "baz"}, 
            });
            await Task.Delay(2000);

            var api = StartApi(indexName);

            //Act
            var found = await api.SearchAsync("bar");

            //Assert
            Assert.NotNull(found);
            Assert.Single(found);
            Assert.Equal(2, found[0].Content.Id);
        }

        [Theory]
        [InlineData("124", 124)]
        [InlineData("<124", 123)]
        [InlineData(">124", 125)]
        public async Task ShouldFindByNumeric(string query, int expected)
        {
            //Arrange
            var indexName = CreateIndexName();

            await using var disposer = await _esFxt.Manager.CreateIndexAsync(indexName);

            var indexer = _esFxt.CreateIndexer<TestEntity>().ForIndex(indexName);

            await indexer.IndexManyAsync(new[]
            {
                new TestEntity{ Id = 123, Value = "foo"},
                new TestEntity{ Id = 124, Value = "bar"},
                new TestEntity{ Id = 125, Value = "baz"},
            });
            await Task.Delay(2000);

            var api = StartApi(indexName);

            //Act
            var found = await api.SearchAsync(query);

            //Assert
            Assert.NotNull(found);
            Assert.Single(found);
            Assert.Equal(expected, found[0].Content.Id);
        }

        [Theory]
        [InlineData("124-126", 124, 125,126)]
        [InlineData("123-125", 123, 124,125)]
        public async Task ShouldFindByNumericRange(string query, int expected1, int expected2, int expected3)
        {
            //Arrange
            var indexName = CreateIndexName();

            await using var disposer = await _esFxt.Manager.CreateIndexAsync(indexName);

            var indexer = _esFxt.CreateIndexer<TestEntity>().ForIndex(indexName);

            await indexer.IndexManyAsync(new[]
            {
                new TestEntity{ Id = 123, Value = "foo"},
                new TestEntity{ Id = 124, Value = "bar"},
                new TestEntity{ Id = 125, Value = "baz"},
                new TestEntity{ Id = 126, Value = "qoz"},
            });
            await Task.Delay(2000);

            var api = StartApi(indexName);

            //Act
            var found = await api.SearchAsync(query);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(3, found.Length);
            Assert.Contains(found, e => e.Content.Id == expected1);
            Assert.Contains(found, e => e.Content.Id == expected2);
            Assert.Contains(found, e => e.Content.Id == expected3);
        }

        [Fact]
        public async Task ShouldFindByDate()
        {
            //Arrange
            var indexName = CreateIndexName();

            await using var disposer = await _esFxt.Manager.CreateIndexAsync(indexName);

            var indexer = _esFxt.CreateIndexer<TestEntity>().ForIndex(indexName);

            await indexer.IndexManyAsync(new[]
            {
                new TestEntity{ Id = 1, Date = new DateTime(2001, 03, 01)},
                new TestEntity{ Id = 2, Date = new DateTime(2001, 03, 02)},
                new TestEntity{ Id = 3, Date = new DateTime(2001, 03, 03)},
                new TestEntity{ Id = 4, Date = new DateTime(2001, 03, 03, 10, 10, 01)},
                new TestEntity{ Id = 5, Date = new DateTime(2001, 03, 04)},
            });
            await Task.Delay(2000);

            var api = StartApi(indexName);

            //Act
            var found = await api.SearchAsync("03.03.2001");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(2, found.Length);
            Assert.Contains(found, f => f.Content.Id == 3);
            Assert.Contains(found, f => f.Content.Id == 4);
        }

        [Fact]
        public async Task ShouldFindByDateRange()
        {
            //Arrange
            var indexName = CreateIndexName();

            await using var disposer = await _esFxt.Manager.CreateIndexAsync(indexName);

            var indexer = _esFxt.CreateIndexer<TestEntity>().ForIndex(indexName);

            await indexer.IndexManyAsync(new[]
            {
                new TestEntity{ Id = 1, Date = new DateTime(2001, 03, 01)},
                new TestEntity{ Id = 2, Date = new DateTime(2001, 03, 02)},
                new TestEntity{ Id = 3, Date = new DateTime(2001, 03, 03)},
                new TestEntity{ Id = 4, Date = new DateTime(2001, 03, 03, 10, 10, 01)},
                new TestEntity{ Id = 5, Date = new DateTime(2001, 03, 04)},
                new TestEntity{ Id = 6, Date = new DateTime(2001, 03, 05)},
            });
            await Task.Delay(2000);

            var api = StartApi(indexName);

            //Act
            var found = await api.SearchAsync("02.03.2001-04.03.2001");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(3, found.Length);
            Assert.Contains(found, e => e.Content.Id == 2);
            Assert.Contains(found, e => e.Content.Id == 3);
            Assert.Contains(found, e => e.Content.Id == 4);
        }

        [Fact]
        public async Task ShouldFindWithRank()
        {
            //Arrange
            var indexName = CreateIndexName();

            await using var disposer = await _esFxt.Manager.CreateIndexAsync(indexName);

            var indexer = _esFxt.CreateIndexer<TestEntity>().ForIndex(indexName);

            await indexer.IndexManyAsync(new[]
            {
                new TestEntity{ Id = 1, Value = "foo_1", Date = new DateTime(2001, 03, 01)},
                new TestEntity{ Id = 2, Value = "foo_2", Date = new DateTime(2001, 03, 02)},
                new TestEntity{ Id = 3, Value = "foo_3", Date = new DateTime(2001, 03, 03)},
                new TestEntity{ Id = 4, Value = "foo_4", Date = new DateTime(2001, 03, 03, 10, 10, 01)},
                new TestEntity{ Id = 5, Value = "foo_5", Date = new DateTime(2001, 03, 04)},
                new TestEntity{ Id = 6, Value = "foo_6", Date = new DateTime(2001, 03, 05)},
            });
            await Task.Delay(2000);

            var api = StartApi(indexName);

            //Act
            var found1 = await api.SearchAsync("04.03.2001 foo_2");
            var found2 = await api.SearchAsync("foo_2 04.03.2001");

            //Assert
            Assert.NotNull(found1);
            Assert.Equal(2, found1.Length);
            Assert.Equal(5, found1[0].Content.Id);
            Assert.Equal(2, found1[1].Content.Id);

            Assert.NotNull(found2);
            Assert.Equal(2, found2.Length);
            Assert.Equal(2, found2[0].Content.Id);
            Assert.Equal(5, found2[1].Content.Id);
        }

        [Fact]
        public async Task ShouldFindWithFullRequest()
        {
            //Arrange
            var indexName = CreateIndexName();

            await using var disposer = await _esFxt.Manager.CreateIndexAsync(indexName);

            var indexer = _esFxt.CreateIndexer<TestEntity>().ForIndex(indexName);

            await indexer.IndexManyAsync(new[]
            {
                new TestEntity{ Id = 1123, Value = "foo_1", Date = new DateTime(2001, 03, 01)},
                new TestEntity{ Id = 2123, Value = "foo_2", Date = new DateTime(2001, 03, 02)},
                new TestEntity{ Id = 3123, Value = "foo_3", Date = new DateTime(2001, 03, 03)},
                new TestEntity{ Id = 4123, Value = "bar_4", Date = new DateTime(2001, 03, 03, 10, 10, 01)},
                new TestEntity{ Id = 5123, Value = "bar_5", Date = new DateTime(2001, 03, 04)},
                new TestEntity{ Id = 6123, Value = "bar_6", Date = new DateTime(2001, 03, 05)},
            });
            await Task.Delay(2000);

            var api = StartApi(indexName);

            //Act
            var found = await api.SearchAsync("<04.03.2001 foo", "from1123to6123", "revert");

            //Assert
            Assert.NotNull(found);
            Assert.Equal(3, found.Length);
            Assert.Equal(4123, found[0].Content.Id);
            Assert.Equal(3123, found[1].Content.Id);
            Assert.Equal(2123, found[2].Content.Id);
        }

    }
}
