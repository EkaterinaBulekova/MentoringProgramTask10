using System;
using System.Collections.Generic;
using System.Linq;
using DALMongoDb.Abstract;
using DALMongoDb.Concrete;
using DALMongoDb.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;

namespace DALMongoDb.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        #region Private initial definition

        private const string ColumnString = "|Name                |Authtor   |Count|Genre               |Year |";
        private const string LineString = "|--------------------|----------|-----|--------------------|-----|";
        private const string MongoServerUrl = "mongodb://localhost:27017";
        private const string MongoDbName = "MyTestBase";
        private DataContext _context;
        private IRepository<Book, ObjectId> _repository; 
        private readonly List<Book> _books = new List<Book>
            {
                new Book
                {
                    Name = "Hobbit",
                    Author = "Tolkien",
                    Count = 5,
                    Genre = new []
                    {
                        "fantasy"
                    },
                    Year = 2014
                },
                new Book
                {
                    Name ="Lord of the rings",
                    Author = "Tolkien",
                    Count = 3,
                    Genre = new []
                    {
                        "fantasy"
                    },
                    Year = 2015
                },
                new Book
                {
                    Name ="Kolobok",
                    Count = 10,
                    Genre = new []
                    {
                        "kids"
                    },
                    Year = 2000
                },
                new Book
                {
                    Name ="Repka",
                    Count = 11,
                    Genre = new []
                    {
                        "kids"
                    },
                    Year = 2000
                },
                new Book
                {
                    Name ="Dyadya Stiopa",
                    Author = "Mihalkov",
                    Count = 1,
                    Genre = new []
                    {
                        "kids"
                    },
                    Year = 2001
                }	
            };       

        #endregion

        #region Initialiase and Cleanup Methods

        [TestInitialize]
        public void InitMethod()
        {
            _context = new DataContext(MongoServerUrl, MongoDbName);
            _repository = new Repository<Book>(_context);
        }

        [TestCleanup]
        public void CleanMethod()
        {
            _context.Dispose();
        }

        #endregion

        #region Test Methods for Task10
        
        //1.Добавьте следующие книги (название, автор, количество экземпляров, жанр, год издания):
        //•	Hobbit, Tolkien, 5, fantasy, 2014
        //•	Lord of the rings, Tolkien, 3, fantasy, 2015
        //•	Kolobok, 10, kids, 2000
        //•	Repka, 11, kids, 2000
        //•	Dyadya Stiopa, Mihalkov, 1, kids, 2001
        [TestMethod]
        public void CanInsertAll()
        {
            var oldCount = _repository.Count();
            _repository.InsertMany(_books);
            var newCount = _repository.Count();

            Assert.AreEqual(oldCount +_books.Count, newCount);
        }

        //2.Найдите книги с количеством экземпляров больше единицы.
        //a.Покажите в результате только название книги.
        [TestMethod]
        public void CanGetAllByCount()
        {
            var books = _repository.List(_ => _.Count > 1);
            foreach (var book in books)
            {
                Console.WriteLine(book.Name);
            }
        }

        //b.Отсортируйте книги по названию.
        [TestMethod]
        public void CanGetAllByCountOrderedName()
        {
            var books = _repository.List(_ => _.Count > 1, _ => _.Name);
            foreach (var book in books)
            {
                Console.WriteLine(book.Name);
            }
        }

        //c.Ограничьте количество возвращаемых книг тремя.
        [TestMethod]
        public void CanGetAllByCountWithTake()
        {
            var books = _repository.List(_ => _.Count > 1, take: 3);
            foreach (var book in books)
            {
                Console.WriteLine(book.Name);
            }
        }

        [TestMethod]
        public void CanGetAllByCountOrderedNameWithTake()
        {
            var books = _repository.List(_ => _.Count > 1, _ => _.Name, 3);
            foreach (var book in books)
            {
                Console.WriteLine(book.Name);
            }
        }

        //d.Подсчитайте количество таких книг.
        [TestMethod]
        public void CanCountAllByCount()
        {
            var count = _repository.Count(_ => _.Count > 1);
            Console.WriteLine(count);
        }

        //3. Найдите книгу с макимальным/минимальным количеством (count).
        [TestMethod]
        public void CanGetBookWithMaxCount()
        {
            var max = _repository.Max(_ => _.Count);
            var book = _repository.Single(_ => _.Count == max);
            Console.WriteLine(ColumnString);
            Console.WriteLine(LineString);
            Console.WriteLine(book);
        }

        [TestMethod]
        public void CanGetBookWithMinCount()
        {
            var min = _repository.Min(_ => _.Count);
            var book = _repository.Single(_ => _.Count == min);
            Console.WriteLine(ColumnString);
            Console.WriteLine(LineString);
            Console.WriteLine(book);
        }

        //4.Найдите список авторов (каждый автор должен быть в списке один раз).
        [TestMethod]
        public void CanListPropertyDist()
        {
            var authors = _repository.ListPropertyDist(_ => _.Author);
            Console.WriteLine(string.Join(",", authors));
        }

        //5.Выберите книги без авторов.
        [TestMethod]
        public void CanGetListBookWithoutAuthors()
        {
            var books = _repository.List(_ => _.Author == null);
            Console.WriteLine(ColumnString);
            Console.WriteLine(LineString);
            foreach (var book in books)
            {
                Console.WriteLine(book);
            }
        }

        //6. Увеличьте количество экземпляров каждой книги на единицу.
        [TestMethod]
        public void CanUpdateAllCountValue()
        {
            var books = _repository.List();
            foreach (var book in books)
            {
                book.Count++;
                _repository.Update(book);
            }

            Assert.IsTrue(_repository.List()[0].Count == books[0].Count);
        }

        //7. Добавьте дополнительный жанр “favority” всем книгам с жанром “fantasy” 
        //  (последующие запуски запроса не должны дублировать жанр “favority”).
        [TestMethod]
        public void CanUpdateGenre()
        {
            var books = _repository.List(_=>_.Genre.Contains("fantasy")&& !_.Genre.Contains("favority"));
            foreach (var book in books)
            {
                book.Genre = book.Genre.Concat(new []{"favority"}).ToArray();
                _repository.Update(book);
            }

            var newBooks = _repository.List(_ => _.Genre.Contains("fantasy"));

            Assert.IsTrue(newBooks.All(_ => _.Genre.Contains("favority")));
        }
        
        //8. Удалите книги с количеством экземпляров меньше трех.
        [TestMethod]
        public void CanDeleteByCondition()
        {
            var allCount = _repository.Count();
            var conditionCount = _repository.Count(_ => _.Count < 3);
            _repository.DeleteMany(_ => _.Count < 3);
            var resultCount = _repository.Count();

            Assert.AreEqual(allCount - conditionCount, resultCount);
        }

        //9. Удалите все книги.
        [TestMethod]
        public void CanDeleteAll()
        {
            _repository.DeleteMany();
            var count = _repository.Count();

            Assert.AreEqual(0, count);
        }
        
        #endregion

        #region Additional Test Methodes

        [TestMethod]
        public void CanInsertOne()
        {
            var oldCount = _repository.Count();
            _repository.Insert(_books[0]);
            var newCount = _repository.Count();

            Assert.AreEqual(oldCount + 1, newCount);
        }

        [TestMethod]
        public void CanGetAll()
        {
            var books = _repository.List();
            Console.WriteLine(ColumnString);
            Console.WriteLine(LineString);
            foreach (var book in books)
            {
                Console.WriteLine(book);
            }
        }

        [TestMethod]
        public void CanGetMaxCount()
        {
            var count = _repository.Max(_ => _.Count);
            Console.WriteLine(count);
        }

        [TestMethod]
        public void CanGetMinCount()
        {
            var count = _repository.Min(_ => _.Count);
            Console.WriteLine(count);
        }

        [TestMethod]
        public void CanUpdateOne()
        {
            var book = _repository.List()[0];
            book.Count = 23;
            _repository.Update(book);
            var newbook = _repository.List()[0];

            Assert.AreEqual(23, newbook.Count);
        }
        
        [TestMethod]
        public void CanUpdateAllCount()
        {
            _repository.UpdateMany(_ => _.Author == null, _ => _.Count, 2);

            Assert.IsTrue(_repository.List(_ => _.Author == null).All(_ => _.Count == 2));
        }

        [TestMethod]
        public void CanDeleteOne()
        {
            var oldCount = _repository.Count();
            var book = _repository.List()[0];
            _repository.Delete(book.Id);
            var newCount = _repository.Count();

            Assert.AreEqual(oldCount - 1, newCount);
        }

        #endregion
    }
}
