namespace BloomFilterDemo.BloomFilter;

public class SimpleBloomFilter(int size)
{
    private readonly bool[] _bits = new bool[size];
    private readonly int _size = size;

    public void Add(int value)
    {
        int hash1 = GetHash1(value);
        int hash2 = GetHash2(value);

        _bits[hash1] = true;
        _bits[hash2] = true;
    }

    public bool MightContain(int value)
    {
        int hash1 = GetHash1(value);
        int hash2 = GetHash2(value);

        return _bits[hash1] && _bits[hash2];
    }

    private int GetHash1(int value)
    {
        return Math.Abs(value) % _size;
    }

    private int GetHash2(int value)
    {
        return Math.Abs(value * 31) % _size;
    }
}