
// using Bogus;

// namespace Events.API.Data
// {
//   public class DataGenerator
//   {
//     public static List<User> GenerateUsers(int count)
//     {
//       var users = new List<User>();

//       var faker = new Faker<User>()
//           .RuleFor(u => u.GetId(), f => Guid.NewGuid())
//           .RuleFor(u => u.FirstName, f => f.Name.FirstName())
//           .RuleFor(u => u.LastName, f => f.Name.LastName())
//           .RuleFor(u => u.UserName, (f, u) => $"{u.FirstName.ToLower()}.{u.LastName.ToLower()}")
//           .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName));

//       for (int i = 0; i < count; i++)
//       {
//         users.Add(faker.Generate());
//       }

//       return users;
//     }
//   }
// }