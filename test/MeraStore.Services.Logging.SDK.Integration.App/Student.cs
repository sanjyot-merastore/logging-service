using Bogus;

namespace MeraStore.Services.Logging.SDK.Integration.App;

public class Student
{
  private static readonly Faker Faker = new Faker();
  public string Name { get; set; }
  public int Age { get; set; }
  public string City { get; set; }

  public static Student GetFakeStudent()
  {
    
    return new Student()
    {
      Age = Faker.Random.Int(18, 25),
      Name = Faker.Name.FullName(),
      City = Faker.Address.City()
    };
  }
}