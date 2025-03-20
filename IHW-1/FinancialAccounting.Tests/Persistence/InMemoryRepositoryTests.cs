using System;
using System.Linq;
using Xunit;
using FinancialAccounting.Persistence;
using FinancialAccounting.Domain;

namespace FinancialAccounting.Tests.Persistence
{
    public class InMemoryRepositoryTests
    {
        private class TestEntity
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public TestEntity(Guid id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        [Fact]
        public void Add_NewEntity_StoresEntityInRepository()
        {
            
            var repository = new InMemoryRepository<TestEntity>(e => e.Id);
            var id = Guid.NewGuid();
            var entity = new TestEntity(id, "Test Entity");

            
            repository.Add(entity);
            var retrievedEntity = repository.Get(id);

            
            Assert.NotNull(retrievedEntity);
            Assert.Equal(entity.Id, retrievedEntity.Id);
            Assert.Equal(entity.Name, retrievedEntity.Name);
        }

        [Fact]
        public void Add_EntityWithExistingId_OverwritesExistingEntity()
        {
            
            var repository = new InMemoryRepository<TestEntity>(e => e.Id);
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id, "Original Entity");
            var entity2 = new TestEntity(id, "Updated Entity");

            
            repository.Add(entity1);
            repository.Add(entity2);
            var retrievedEntity = repository.Get(id);

            
            Assert.NotNull(retrievedEntity);
            Assert.Equal(entity2.Name, retrievedEntity.Name);
        }

        [Fact]
        public void Update_ExistingEntity_UpdatesEntityInRepository()
        {
            
            var repository = new InMemoryRepository<TestEntity>(e => e.Id);
            var id = Guid.NewGuid();
            var entity = new TestEntity(id, "Original Name");
            repository.Add(entity);

            
            entity.Name = "Updated Name";
            repository.Update(entity);
            var retrievedEntity = repository.Get(id);

            
            Assert.NotNull(retrievedEntity);
            Assert.Equal("Updated Name", retrievedEntity.Name);
        }

        [Fact]
        public void Delete_ExistingEntity_RemovesEntityFromRepository()
        {
            
            var repository = new InMemoryRepository<TestEntity>(e => e.Id);
            var id = Guid.NewGuid();
            var entity = new TestEntity(id, "Test Entity");
            repository.Add(entity);

            
            repository.Delete(id);
            var retrievedEntity = repository.Get(id);

            
            Assert.Null(retrievedEntity);
        }

        [Fact]
        public void Delete_NonExistingEntity_DoesNotThrowException()
        {
            
            var repository = new InMemoryRepository<TestEntity>(e => e.Id);
            var id = Guid.NewGuid();

            
            var exception = Record.Exception(() => repository.Delete(id));
            Assert.Null(exception);
        }

        [Fact]
        public void Get_NonExistingEntity_ReturnsNull()
        {
            
            var repository = new InMemoryRepository<TestEntity>(e => e.Id);
            var id = Guid.NewGuid();

            
            var retrievedEntity = repository.Get(id);

            
            Assert.Null(retrievedEntity);
        }

        [Fact]
        public void GetAll_WithMultipleEntities_ReturnsAllEntities()
        {
            
            var repository = new InMemoryRepository<TestEntity>(e => e.Id);
            var entity1 = new TestEntity(Guid.NewGuid(), "Entity 1");
            var entity2 = new TestEntity(Guid.NewGuid(), "Entity 2");
            var entity3 = new TestEntity(Guid.NewGuid(), "Entity 3");
            repository.Add(entity1);
            repository.Add(entity2);
            repository.Add(entity3);

            
            var allEntities = repository.GetAll().ToList();

            
            Assert.Equal(3, allEntities.Count);
            Assert.Contains(allEntities, e => e.Id == entity1.Id);
            Assert.Contains(allEntities, e => e.Id == entity2.Id);
            Assert.Contains(allEntities, e => e.Id == entity3.Id);
        }

        [Fact]
        public void GetAll_WithNoEntities_ReturnsEmptyCollection()
        {
            
            var repository = new InMemoryRepository<TestEntity>(e => e.Id);

            
            var allEntities = repository.GetAll().ToList();

            
            Assert.Empty(allEntities);
        }
    }
}
