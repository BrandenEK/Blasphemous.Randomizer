using System.Collections.Generic;

namespace Blasphemous.Randomizer.Filling;

public abstract class FillResult
{
    public bool Success { get; set; } = false;
}

public class SingleResult : FillResult
{
    public Dictionary<string, string> Mapping { get; set; }
}

public class DoubleResult : FillResult
{
    public Dictionary<string, string> Mapping1 { get; set; }

    public Dictionary<string, string> Mapping2 { get; set; }
}
