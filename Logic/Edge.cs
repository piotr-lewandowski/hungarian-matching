namespace Logic;

public record struct Edge(int Start, int End, int Weight)
{
    public string SimpleToString()
    {
        return $"{Start+1} {End+1} {Weight}";
    }
}
