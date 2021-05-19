using System;
using System.Collections.Generic;
using System.Linq;

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
        }
    }
}
