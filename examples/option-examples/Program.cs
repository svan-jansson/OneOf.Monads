/*
This example shows how to use the Option monad in a stream scenario.
By building a pipeline with a set of conditions and then evaluating the results with Match.
*/

using OneOf.Monads;

StreamOfIntegers(randomInteger =>
{
    var result = randomInteger.ToOption()
                    .Filter(i => i > 50)
                    .Map(i => i * 99)
                    .Bind<int>(i => i % 2 == 0 ? new Some<int>(i) : new None())
                    .Match(
                        none => "did not pass checks",
                        some => $"{randomInteger} is above 50 and becomes the even number {some.Value} when multiplied by 99");

    Console.WriteLine(result);
});

// Produces random integers between 1 and 100
static void StreamOfIntegers(Action<int> callback)
{
    const int SleepMsBetweenCallbacks = 500;
    var rng = new Random(Seed: 1);
    while (true)
    {
        callback(rng.Next(100) + 1);
        Thread.Sleep(SleepMsBetweenCallbacks);
    }
}
