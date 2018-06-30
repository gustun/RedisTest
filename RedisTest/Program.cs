using Model;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // ilk önce Sunucu ile aramızda bir kanal açalım 
            using (RedisClient client = new RedisClient("127.0.0.1", 6379))
            {
                // T ile belirtilen tip ile çalışabileceğimiz bir Redis arayüzünü tedarik etmemizi sağlar 
                IRedisTypedClient<Person> personStore = client.As<Person>();

                #region Örnek bir veri kümesinin eklenmesi

                // Temiz bir başlangıç için istenirse var olan Person kümesi silinebilir de 
                if (personStore.GetAll().Count > 0)
                    personStore.DeleteAll();

                // Bir kaç örnek departman ve personel verisi oluşturuyoruz 
                Department itDepartment = new Department
                {
                    DepartmentId = 1000,
                    Name = "IT",
                    Description = "Information Technologies"
                };
                Department financeDepartment = new Department
                {
                    DepartmentId = 1000,
                    Name = "Finance",
                    Description = "Finance Unit"
                };

                List<Person> persons = new List<Person>
                {
                    new Person
                    {
                        PersonId=personStore.GetNextSequence()
                        , Name="Burak"
                        , Level=100
                        , Department=itDepartment
                    },
                    new Person
                    {
                        PersonId=personStore.GetNextSequence()
                        , Name="Bill"
                        , Level=200
                        , Department=itDepartment
                    },
                    new Person
                    {
                        PersonId=personStore.GetNextSequence()
                        , Name="Adriana"
                        , Level=250
                        , Department=itDepartment
                    },
                    new Person
                    {
                        PersonId=personStore.GetNextSequence()
                        , Name="Sakira"
                        , Level=300
                        , Department=financeDepartment
                    },
                    new Person
                    {
                        PersonId=personStore.GetNextSequence()
                        , Name="Bob"
                        , Level=550
                        , Department=financeDepartment
                    }
                };

                // Elemanları StoreAll metodu yardımıyla Redis' e alıyoruz. 
                personStore.StoreAll(persons);

                #endregion Örnek bir veri kümesinin eklenmesi

                #region Verileri elde etmek, sorgulamak

                Console.WriteLine("Tüm Personel");
                // Kaydettiğimiz elemanların tamamını GetAll metodu yardımıyla çekebiliriz. 
                foreach (var person in personStore.GetAll())
                {
                    Console.WriteLine(person.ToString());
                }

                // Dilersek içeride tutulan Key/Value çiftlerinden Key değerlerine ulaşabiliriz 
                List<string> personKeys = personStore.GetAllKeys();

                Console.WriteLine("\nKey Bilgileri");
                foreach (var personKey in personKeys)
                {
                    Console.WriteLine(personKey);
                }

                // İstersek bir LINQ sorgusunu GetAll metodu üstünden dönen liste üzerinden çalıştırabiliriz 
                IOrderedEnumerable<Person> itPersons = personStore
                    .GetAll()
                   .Where<Person>(p => p.Department.Name == "IT")
                    .OrderByDescending(p => p.Level);

                Console.WriteLine("\nSadece IT personeli");
                foreach (var itPerson in itPersons)
                {
                    Console.WriteLine(itPerson.ToString());
                }

                // Random bir Key değeri alabilir ve bunun karşılığı olan value' yu çekebiliriz 
                string randomKey = personStore.GetRandomKey();
                Console.WriteLine("\nBulunan Key {0}", randomKey);
                // seq:Person ve ids:Person key değerleri için hata oluşacağından try...catch' den kaçıp başka bir kontrol yapmaya çalışıyoruz. 
                if (randomKey != personStore.SequenceKey
                    && randomKey != "ids:Person")
                {
                    var personByKey = personStore[randomKey];
                    Console.WriteLine("{0}", personByKey.ToString());
                }

                personStore.SaveAsync(); // Kalıcı olarak veriyi persist edebiliriz. Asenkron olarak yapılabilen bir işlemdir.

                #endregion Verileri elde etmek, sorgulamak

                Console.WriteLine("\nÇıkmak için bir tuşa basınız");
                Console.ReadLine();

            }
        }
    }
}
