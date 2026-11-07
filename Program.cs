using Assignment_3.Context;
using Assignment_3.Enums;
using Assignment_3.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment_3
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            using AppDbContext dbContext = new AppDbContext();

            await dbContext.Database.MigrateAsync();

            #region Seeding Data (Students & Enrollments)

            if (!await dbContext.Students.AnyAsync())
            {
                var students = new List<Student>
                {
                    new Student { FullName = "Hassan", Email = "Hassan@test.com", DateOfBirth = new DateTime(2000, 1, 1) },
                    new Student { FullName = "Nour", Email = "Nour@test.com", DateOfBirth = new DateTime(1999, 5, 5) },
                    new Student { FullName = "Ali", Email = "Ali@test.com", DateOfBirth = new DateTime(2001, 8, 8) },
                    new Student { FullName = "Mona", Email = "Mona@test.com", DateOfBirth = new DateTime(2002, 2, 2) },
                    new Student { FullName = "Ziad", Email = "Ziad@test.com", DateOfBirth = new DateTime(1998, 12, 12) }
                };

                await dbContext.Students.AddRangeAsync(students);
                await dbContext.SaveChangesAsync();

                var enrollments = new List<Enrollment>
                {
                    new Enrollment { StudentId = students[0].Id, CourseId = 1, EnrollmentDate = DateTime.Now, ProgressPercentage = 50, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = students[1].Id, CourseId = 1, EnrollmentDate = DateTime.Now, ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 95 },
                    new Enrollment { StudentId = students[2].Id, CourseId = 2, EnrollmentDate = DateTime.Now, ProgressPercentage = 10, Status = EnrollmentStatus.Dropped },
                    new Enrollment { StudentId = students[3].Id, CourseId = 4, EnrollmentDate = DateTime.Now, ProgressPercentage = 80, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = students[4].Id, CourseId = 6, EnrollmentDate = DateTime.Now, ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 88 },
                    new Enrollment { StudentId = students[0].Id, CourseId = 4, EnrollmentDate = DateTime.Now, ProgressPercentage = 20, Status = EnrollmentStatus.Dropped },
                    new Enrollment { StudentId = students[1].Id, CourseId = 6, EnrollmentDate = DateTime.Now, ProgressPercentage = 0, Status = EnrollmentStatus.Active },
                    new Enrollment { StudentId = students[2].Id, CourseId = 1, EnrollmentDate = DateTime.Now, ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 75 }
                };

                await dbContext.Enrollments.AddRangeAsync(enrollments);
                await dbContext.SaveChangesAsync();

                Console.WriteLine("Seed Data Inserted Successfully.\n");
            }

            #endregion Seeding Data (Students & Enrollments)

            #region Create

            // 1. Add an instructor
            var Instructor = new Instructor { FullName = "Abdelrahman", Email = "Abdelrahman@test.com", JoinDate = DateTime.Now };
            Console.WriteLine(dbContext.Entry(Instructor).State); // Detached

            await dbContext.Instructors.AddAsync(Instructor);
            Console.WriteLine(dbContext.Entry(Instructor).State); // Added

            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Instructor added with Id: {Instructor.Id}");

            // 2. Add a course
            var currentInstructor = await dbContext.Instructors.FirstOrDefaultAsync();
            var currentCategory = await dbContext.Categories.FirstOrDefaultAsync();

            if (currentInstructor == null || currentCategory == null)
            {
                Console.WriteLine("Cannot add course: Instructor or Category not found in DB.");
            }
            else
            {
                var newCourse = new Course
                {
                    Title = "New DB Course",
                    Price = 400,
                    Level = CourseLevel.Beginner,
                    IsPublished = false,
                    InstructorId = currentInstructor.Id,
                    CategoryId = currentCategory.Id
                };
                await dbContext.Courses.AddAsync(newCourse);
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Course added with Id: {newCourse.Id}");
            }

            // 3. Add students using AddRange
            var studentsList = new List<Student>
                             {
                              new Student { FullName = "Omar", Email = "Omar@test.com", DateOfBirth = DateTime.Now.AddYears(-20) },
                              new Student { FullName = "Ali", Email = "Ali@test.com", DateOfBirth = DateTime.Now.AddYears(-21) },
                              new Student { FullName = "Sufyan", Email = "Sufyan@test.com", DateOfBirth = DateTime.Now.AddYears(-22) }
                             };
            await dbContext.Students.AddRangeAsync(studentsList);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Added students with Ids: {string.Join(", ", studentsList.Select(s => s.Id))}");

            // 4. Enroll a student
            var studentToEnroll = await dbContext.Students.FirstOrDefaultAsync();
            var courseToEnroll = await dbContext.Courses.FirstOrDefaultAsync(c => c.IsPublished);

            if (courseToEnroll == null || studentToEnroll == null)
            {
                Console.WriteLine("Enrollment failed: Student or Course does not exist.");
            }
            else if (!courseToEnroll.IsPublished)
            {
                Console.WriteLine("Enrollment failed: Course is not published.");
            }
            else if (await dbContext.Enrollments.AnyAsync(e => e.StudentId == studentToEnroll.Id && e.CourseId == courseToEnroll.Id))
            {
                Console.WriteLine("Enrollment failed: Student is already enrolled in this course.");
            }
            else
            {
                await dbContext.Enrollments.AddAsync(new Enrollment
                {
                    StudentId = studentToEnroll.Id,
                    CourseId = courseToEnroll.Id,
                    EnrollmentDate = DateTime.Now,
                    ProgressPercentage = 0,
                    Status = EnrollmentStatus.Active
                });
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Student {studentToEnroll.Id} successfully enrolled in Course {courseToEnroll.Id}.");
            }

            #endregion Create

            #region Read

            // 1. Find a course
            var Course = await dbContext.Courses.FirstOrDefaultAsync();

            if (Course != null)
            {
                var byId = await dbContext.Courses.FindAsync(Course.Id);
                Console.WriteLine(byId != null ? $"Found by Id: {byId.Title}" : "Course by Id not found.");

                // AsNoTracking is useful for read-only operations
                var byTitle = await dbContext.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Title == Course.Title);
                Console.WriteLine(byTitle != null ? $"Found by Title: {byTitle.Title}" : "Course by Title not found.");
            }
            else
            {
                Console.WriteLine("No courses found in the database to search for.");
            }

            // 2. Filter courses
            decimal minPrice = 400;
            decimal maxPrice = 900;
            var filteredCourses = await dbContext.Courses
                .AsNoTracking()
                .Where(c => c.Price >= minPrice && c.Price <= maxPrice)
                .OrderByDescending(c => c.Price)
                .Select(c => new { c.Title, c.Level, c.Price })
                .ToListAsync();

            Console.WriteLine($"Courses between {minPrice} and {maxPrice}:");
            foreach (var c in filteredCourses) Console.WriteLine($"- {c.Title} ({c.Level}) - ${c.Price}");

            // 3. Sub-categories
            var parentCat = await dbContext.Categories.FirstOrDefaultAsync(c => dbContext.Categories.Any(sub => sub.ParentId == c.Id));

            if (parentCat != null)
            {
                var subCats = await dbContext.Categories.Where(c => c.ParentId == parentCat.Id).Select(c => c.Name).ToListAsync();
                Console.WriteLine($"Sub-categories for Parent '{parentCat.Name}' (Id: {parentCat.Id}): {string.Join(", ", subCats)}");
            }

            var topLevel = await dbContext.Categories.Where(c => c.ParentId == null).Select(c => c.Name).ToListAsync();
            Console.WriteLine($"Top-level categories: {string.Join(", ", topLevel)}");

            // 4. Course roster
            var activeCourseId = await dbContext.Enrollments.Select(e => e.CourseId).FirstOrDefaultAsync();

            if (activeCourseId != 0)
            {
                var studentIds = await dbContext.Enrollments.Where(e => e.CourseId == activeCourseId).Select(e => e.StudentId).ToListAsync();
                var enrolledStudents = await dbContext.Students.Where(s => studentIds.Contains(s.Id)).Select(s => s.FullName).ToListAsync();
                Console.WriteLine($"Course {activeCourseId} has {enrolledStudents.Count} students: {string.Join(", ", enrolledStudents)}");
            }
            else
            {
                Console.WriteLine("No enrollments found for any course.");
            }

            #endregion Read

            #region Update

            // Connected Scenario

            // 1. Change a price
            var courseToUpdate = await dbContext.Courses.FirstOrDefaultAsync();
            if (courseToUpdate != null)
            {
                Console.WriteLine(dbContext.Entry(courseToUpdate).State); // Unchanged
                courseToUpdate.Price = 550;
                Console.WriteLine(dbContext.Entry(courseToUpdate).State); // Modified
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Price updated to {courseToUpdate.Price} for course {courseToUpdate.Id}");
            }
            else
            {
                Console.WriteLine("No course found to update.");
            }

            // 2. Update progress
            var enrollmentToUpdate = await dbContext.Enrollments.FirstOrDefaultAsync();
            if (enrollmentToUpdate != null)
            {
                int newProgress = 100;
                if (newProgress >= 0 && newProgress <= 100)
                {
                    enrollmentToUpdate.ProgressPercentage = newProgress;
                    if (newProgress == 100)
                        enrollmentToUpdate.Status = EnrollmentStatus.Completed;

                    await dbContext.SaveChangesAsync();
                    Console.WriteLine($"Progress updated for Student {enrollmentToUpdate.StudentId} in Course {enrollmentToUpdate.CourseId}.");
                }
                else
                {
                    Console.WriteLine("Invalid progress value.");
                }
            }
            else
            {
                Console.WriteLine("No enrollment found to update progress.");
            }

            // 3. Publish a course
            var courseToPublish = await dbContext.Courses.FirstOrDefaultAsync(c => !c.IsPublished);
            if (courseToPublish != null)
            {
                courseToPublish.IsPublished = true;
                courseToPublish.PublishedDate = DateTime.Now;
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Course {courseToPublish.Id} ({courseToPublish.Title}) is now published.");
            }
            else
            {
                Console.WriteLine("All courses are already published or no courses exist.");
            }

            #endregion Update

            #region Delete

            // Connected Scenario

            // 1. Unenroll
            var enrollmentToDelete = await dbContext.Enrollments.FirstOrDefaultAsync();
            if (enrollmentToDelete != null)
            {
                dbContext.Enrollments.Remove(enrollmentToDelete);
                Console.WriteLine(dbContext.Entry(enrollmentToDelete).State); // Deleted
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Student {enrollmentToDelete.StudentId} unenrolled from Course {enrollmentToDelete.CourseId}.");
            }
            else
            {
                Console.WriteLine("No enrollment found to delete.");
            }

            // 2. Delete a category
            var catToDelete = await dbContext.Categories.FirstOrDefaultAsync(c => dbContext.Categories.Any(sub => sub.ParentId == c.Id));
            if (catToDelete != null)
            {
                try
                {
                    dbContext.Categories.Remove(catToDelete);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Delete failed! Reason: Parent has sub-categories. Exception: {ex.InnerException?.Message}");
                    dbContext.ChangeTracker.Clear();
                }

                bool hasSubCats = await dbContext.Categories.AnyAsync(c => c.ParentId == catToDelete.Id);
                if (!hasSubCats)
                {
                    var safeCatToDelete = await dbContext.Categories.FindAsync(catToDelete.Id);
                    if (safeCatToDelete != null)
                    {
                        dbContext.Categories.Remove(safeCatToDelete);
                        await dbContext.SaveChangesAsync();
                        Console.WriteLine("Category deleted safely.");
                    }
                }
                else
                {
                    Console.WriteLine("Safe Delete: Category has sub-categories and cannot be deleted.");
                }
            }
            else
            {
                Console.WriteLine("No category with sub-categories found.");
            }

            // 3. Delete a course
            var courseToDelete = await dbContext.Courses.FirstOrDefaultAsync(c => dbContext.Enrollments.Any(e => e.CourseId == c.Id));
            if (courseToDelete != null)
            {
                int enrollCountBefore = await dbContext.Enrollments.CountAsync(e => e.CourseId == courseToDelete.Id);
                dbContext.Courses.Remove(courseToDelete);
                await dbContext.SaveChangesAsync();
                int enrollCountAfter = await dbContext.Enrollments.CountAsync(e => e.CourseId == courseToDelete.Id);

                Console.WriteLine($"Deleted Course {courseToDelete.Id} ({courseToDelete.Title}).");
                Console.WriteLine($"Enrollments before: {enrollCountBefore}, after: {enrollCountAfter}");

                // Enrollments were removed automatically because EF Core configures Cascade Delete by default for required foreign keys
            }
            else
            {
                Console.WriteLine("No course with enrollments found to delete.");
            }

            #endregion Delete

            #region Bulk operations

            // 1. Bulk price raise
            var updatedRows = await dbContext.Courses
                .Where(c => c.Level == CourseLevel.Beginner && c.IsPublished)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.Price, c => c.Price * 1.10m));

            Console.WriteLine($"Bulk Price Raise affected {updatedRows} rows.");

            // 2. Bulk delete dropped
            var deletedRows = await dbContext.Enrollments
                .Where(e => e.Status == EnrollmentStatus.Dropped)
                .ExecuteDeleteAsync();

            Console.WriteLine($"Bulk Delete removed {deletedRows} dropped enrollments.");

            #endregion Bulk operations
        }
    }
}