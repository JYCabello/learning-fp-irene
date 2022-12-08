using DeFuncto;
using DeFuncto.Extensions;

public record Fruit(int Weight);

public enum Errors
{
    Unauthorized,
    NotFound
}

[Flags]
public enum Roles
{
    None = 0,
    AccessFruits = 1
}

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var output =
            await GetFruitWeight(10, 8)
                .Match(
                    w => $"Weight is: {w}",
                    e =>
                        e switch
                        {
                            Errors.Unauthorized => "Unauthorized",
                            Errors.NotFound => "Not found"
                        }
                );
        
        Console.WriteLine(output);
        return 0;
    }

    public static async Task<string> GetFileContents(string fileName)
    {
        var lines = await File.ReadAllLinesAsync(fileName);
        return string.Concat(lines);
    }

    public static AsyncResult<int, Errors> GetFruitWeight(int userID, int fruitID) =>
        from roles in Ok<Roles, Errors>(GetRoles(userID)).Async()
        from _ in roles.VerifyRole(Roles.AccessFruits).Async()
        from fruit in FindFruit(fruitID).Result(Errors.NotFound)
        select fruit.Weight;

    public static Result<Unit, Errors> VerifyRole(this Roles self, Roles role) =>
        self.HasFlag(role) ? Ok(unit) : Error(Errors.Unauthorized);

    public static Roles GetRoles(int id) =>
        id == 10 ? Roles.AccessFruits : Roles.None;

    public static AsyncOption<Fruit> FindFruit(int id) =>
        id == 8 ? Some(new Fruit(13)) : None;
}

public class NotFoundException : Exception { }
public class UnauthorizedException : Exception { }
