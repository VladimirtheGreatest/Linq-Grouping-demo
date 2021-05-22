using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GroupLinqExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            StudentClass sc = new StudentClass();
            sc.QueryHighScores(1, 90);
            sc.GroupByRange();
            sc.GroupByBoolean();
            sc.ExampleAllAny();
            sc.ReflectionExample();
            sc.SetOperationsExamples();
            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        public class StudentClass
        {
            #region data
            protected enum GradeLevel { FirstYear = 1, SecondYear, ThirdYear, FourthYear };
            protected class Student
            {
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public int ID { get; set; }
                public GradeLevel Year;
                public List<int> ExamScores;
            }

            protected static List<Student> students = new List<Student>
    {
        new Student {FirstName = "Terry", LastName = "Adams", ID = 120,
            Year = GradeLevel.SecondYear,
            ExamScores = new List<int>{ 99, 82, 81, 79}},
        new Student {FirstName = "Fadi", LastName = "Fakhouri", ID = 116,
            Year = GradeLevel.ThirdYear,
            ExamScores = new List<int>{ 99, 86, 90, 94}},
        new Student {FirstName = "Hanying", LastName = "Feng", ID = 117,
            Year = GradeLevel.FirstYear,
            ExamScores = new List<int>{ 93, 92, 80, 87}},
        new Student {FirstName = "Cesar", LastName = "Garcia", ID = 114,
            Year = GradeLevel.FourthYear,
            ExamScores = new List<int>{ 97, 89, 85, 82}},
        new Student {FirstName = "Debra", LastName = "Garcia", ID = 115,
            Year = GradeLevel.ThirdYear,
            ExamScores = new List<int>{ 35, 72, 91, 70}},
        new Student {FirstName = "Hugo", LastName = "Garcia", ID = 118,
            Year = GradeLevel.SecondYear,
            ExamScores = new List<int>{ 92, 90, 83, 78}},
        new Student {FirstName = "Sven", LastName = "Mortensen", ID = 113,
            Year = GradeLevel.FirstYear,
            ExamScores = new List<int>{ 88, 94, 65, 91}},
        new Student {FirstName = "Claire", LastName = "O'Donnell", ID = 112,
            Year = GradeLevel.FourthYear,
            ExamScores = new List<int>{ 75, 84, 91, 39}},
        new Student {FirstName = "Svetlana", LastName = "Omelchenko", ID = 111,
            Year = GradeLevel.SecondYear,
            ExamScores = new List<int>{ 97, 92, 81, 60}},
        new Student {FirstName = "Lance", LastName = "Tucker", ID = 119,
            Year = GradeLevel.ThirdYear,
            ExamScores = new List<int>{ 68, 79, 88, 92}},
        new Student {FirstName = "Michael", LastName = "Tucker", ID = 122,
            Year = GradeLevel.FirstYear,
            ExamScores = new List<int>{ 94, 92, 91, 91}},
        new Student {FirstName = "Eugene", LastName = "Zabokritski", ID = 121,
            Year = GradeLevel.FourthYear,
            ExamScores = new List<int>{ 96, 85, 91, 60}}
    };
            #endregion

            //Helper method, used in GroupByRange.
            protected static int GetPercentile(Student s)
            {
                double avg = s.ExamScores.Average();
                return avg > 0 ? (int)avg / 10 : 0;
            }

            public void QueryHighScores(int exam, int score)
            {
                var highScores = from student in students
                                 where student.ExamScores[exam] > score
                                 select new { Name = student.FirstName, Score = student.ExamScores[exam] };

                foreach (var item in highScores)
                {
                    Console.WriteLine($"{item.Name,-15}{item.Score}");
                }
            }

            ///group by range example
            public void GroupByRange()
            {
                Console.WriteLine("\r\nGroup by numeric range and project into a new anonymous type:");

                var queryNumericRange =
                    from student in students
                    let percentile = GetPercentile(student)
                    group new { student.FirstName, student.LastName } by percentile into percentGroup
                    orderby percentGroup.Key
                    select percentGroup;

                // Nested foreach required to iterate over groups and group items.
                foreach (var studentGroup in queryNumericRange)
                {
                    Console.WriteLine($"Key: {studentGroup.Key * 10}");
                    foreach (var item in studentGroup)
                    {
                        Console.WriteLine($"\t{item.LastName}, {item.FirstName}");
                    }
                }
            }
            public void GroupByBoolean()
            {
                Console.WriteLine("\r\nGroup by a Boolean into two groups with string keys");
                Console.WriteLine("\"True\" and \"False\" and project into a new anonymous type:");
                var queryGroupByAverages = from student in students
                                           group new { student.FirstName, student.LastName }
                                                by student.ExamScores.Average() > 75 into studentGroup
                                           select studentGroup;
                var lambaExample = students.GroupBy(x => x.ExamScores.Average() > 75);

                foreach (var sg in lambaExample)
                {
                    Console.WriteLine($"Key: {sg.Key}");
                    foreach (var student in sg)
                        Console.WriteLine($"\t{student.FirstName} {student.LastName}");
                }

                foreach (var studentGroup in queryGroupByAverages)
                {
                    Console.WriteLine($"Key: {studentGroup.Key}");
                    foreach (var student in studentGroup)
                        Console.WriteLine($"\t{student.FirstName} {student.LastName}");
                }
            }
           internal class Market
            {
                public string Name { get; set; }
                public string[] Items { get; set; }
            }
            public void ExampleAllAny()
            {
                List<Market> markets = new List<Market>
                  {
                  new Market { Name = "Emily's", Items = new string[] { "kiwi", "cheery", "banana" } },
                  new Market { Name = "Kim's", Items = new string[] { "melon", "mango", "olive" } },
                  new Market { Name = "Adam's", Items = new string[] { "kiwi", "apple", "orange" } },
                  };
                // Determine which market have all fruit names length equal to 5
                IEnumerable<string> names = from market in markets
                                            where market.Items.All(item => item.Length == 5)
                                            select market.Name;

                // Determine which market have any(if there is at least one) fruit names length equal to 5
                IEnumerable<string> anyNames = from market in markets
                                            where market.Items.Any(item => item.Length == 5)
                                            select market.Name;

                // Determine which market have any fruit names start with 'o'
                IEnumerable<string> namesWhereFruitStartWithO = from market in markets
                                            where market.Items.Any(item => item.StartsWith("o"))
                                            select market.Name;
                // Determine which market offers kiwi
                IEnumerable<string> kiwiMarkets = from market in markets
                                            where market.Items.Contains("kiwi")
                                            select market.Name;

            }
            public void ReflectionExample()
            {
                Assembly assembly = Assembly.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken= b77a5c561934e089");
                var pubTypesQuery = from type in assembly.GetTypes()
                                    where type.IsPublic
                                    from method in type.GetMethods()
                                    where method.ReturnType.IsArray == true
                                        || (method.ReturnType.GetInterface(
                                            typeof(System.Collections.Generic.IEnumerable<>).FullName) != null
                                        && method.ReturnType.FullName != "System.String")
                                    group method.ToString() by type.ToString();

                foreach (var groupOfMethods in pubTypesQuery)
                {
                    Console.WriteLine("Type: {0}", groupOfMethods.Key);
                    foreach (var method in groupOfMethods)
                    {
                        Console.WriteLine("  {0}", method);
                    }
                }

            }
            //Set operations in LINQ refer to query operations that produce a result set that is based on the presence or absence of equivalent elements within the same or separate collections (or sets).
            public void SetOperationsExamples()
            {
                string[] planetsDuplicates = { "Mercury", "Venus", "Venus", "Earth", "Mars", "Earth" };
                string[] planets1 = { "Mercury", "Venus", "Earth", "Jupiter" };
                string[] planets2 = { "Mercury", "Earth", "Mars", "Jupiter" };

                var removedDuplicates = planetsDuplicates.Distinct();
                //or
                IEnumerable<string> removedDuplicatesQueryExp = from planet in planetsDuplicates.Distinct()
                                            select planet;

                //The returned sequence contains only the elements from the first input sequence that are not in the second input sequence.
                IEnumerable<string> planetsFrom1ThatAreNOTIn2 = planets1.Except(planets2);
                //same thing??!
                var test = planets1.Where(x => !planets2.Contains(x));

                //intersecting The returned sequence contains the elements that are common to both(same) of the input sequences.

                IEnumerable<string> queryIntersect = from planet in planets1.Intersect(planets2)
                                            select planet;

                //same thing??!
                var testIntersect = planets1.Where(x => planets2.Contains(x));


                //The returned sequence contains the unique elements from both input sequences.
                IEnumerable<string> query = from planet in planets1.Union(planets2)
                                            select planet;


            }
        }
    }
}
