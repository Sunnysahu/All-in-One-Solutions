## First, the Idea

We'll keep it intentionally simple:

```
Bit array
   ↓
2 hash functions
   ↓
Add()
   ↓
MightContain()
```

We'll use `bool[]` instead of a real `BitArray` initially because it's easier to understand.

## SimpleBloomFilter.cs

```
namespace BloomFilterDemo.BloomFilter;

public class SimpleBloomFilter
{
    private readonly bool[] _bits;
    private readonly int _size;

    public SimpleBloomFilter(int size)
    {
        _size = size;
        _bits = new bool[size];
    }

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
```

## Understand the Important Parts

We create:

```
private readonly bool[] _bits;
```

If we create:

```
new SimpleBloomFilter(10)
```

we get:

```
Index:  0 1 2 3 4 5 6 7 8 9
Bits:   F F F F F F F F F F
```

Everything starts as `false`.

---

## Add()

Suppose:

```
_filter.Add(3);
```

Our hashes might produce:

```
hash1 = 3
hash2 = 3
```

So:

```
Index:  0 1 2 3 4 5 6 7 8 9
Bits:   F F F T F F F F F F
```

For another value:

```
_filter.Add(7);
```

the relevant bits become `true`.

The important idea is:

> `Add()` does not store the actual value. It only marks positions in the bit array.

---

## MightContain()

When we ask:

```
_filter.MightContain(3);
```

we calculate the same hashes.

Then:

```
return _bits[hash1] && _bits[hash2];
```

If both positions are `true`:

```
Possibly exists
```

If even one position is `false`:

```
Definitely does not exist
```

That's the core Bloom Filter algorithm.

---

## Why "Might Contain"?

A Bloom Filter has an important property:

- If it says **"Definitely does not exist"**, the value was not added.
- If it says **"Possibly exists"**, the value may or may not have been added.

In other words:

```
                MightContain()
                       │
              ┌────────┴────────┐
              │                 │
        Any bit false       Both bits true
              │                 │
              ↓                 ↓
     Definitely absent     Possibly exists
```

This means Bloom Filters can have:

> **False positives**

But, under normal correct usage, they do not have:

> **False negatives**

---

## Why Can False Positives Happen?

Suppose we have:

```
Index:  0 1 2 3 4 5 6 7 8 9
Bits:   F T F T F F F F F F
```

A value might have produced:

```
hash1 = 1
hash2 = 3
```

So bits `1` and `3` were set to `true`.

Later, another value might produce the same hash positions:

```
hash1 = 1
hash2 = 3
```

Both bits are already `true`.

The Bloom Filter therefore returns:

```
Possibly exists
```

But that does **not** guarantee that the value was actually added.

This is called a **false positive**.

The Bloom Filter only knows that the required bits are set. It does not know which value caused those bits to become `true`.

---

## Our Hash Functions

We currently use two very simple hash functions.

### Hash 1

```
private int GetHash1(int value)
{
    return Math.Abs(value) % _size;
}
```

Conceptually:

```
value
  ↓
% size
  ↓
bit position
```

### Hash 2

```
private int GetHash2(int value)
{
    return Math.Abs(value * 31) % _size;
}
```

Conceptually:

```
value
  ↓
value × 31
  ↓
% size
  ↓
bit position
```

The `% _size` operation makes sure the result falls within the valid array range:

```
0 ... _size - 1
```

---

## One Important Correction From Our Earlier Simplified Explanation

This implementation is **educational**, not a production-quality Bloom Filter.

Our hash functions:

```
value
value × 31
```

are deliberately simple so you can clearly see the mechanics.

A production Bloom Filter would need to consider things such as:

- Better hash functions
- Hash distribution
- Number of hash functions
- Bit-array size
- Expected number of elements
- Desired false-positive probability
- Integer overflow and edge cases
- Memory efficiency
- Thread safety, if required

For now, the goal is not optimization.

The goal is to understand **how a Bloom Filter works internally**.

---

## Core Mental Model

Think of the Bloom Filter like this:

```
                 value
                   │
          ┌────────┴────────┐
          ↓                 ↓
       Hash 1             Hash 2
          │                 │
          ↓                 ↓
       Position           Position
          │                 │
          └────────┬────────┘
                   ↓
               Bit Array
```

### Adding a Value

```
value
  ↓
2 hash functions
  ↓
2 positions
  ↓
set both bits to TRUE
```

### Checking a Value

```
value
  ↓
2 hash functions
  ↓
2 positions
  ↓
check both bits
  ↓
┌───────────────────────┐
│                       │
Both TRUE            Any FALSE
│                       │
↓                       ↓
Possibly exists      Definitely absent
```

That's the core Bloom Filter algorithm.

## Key Takeaways

1. A Bloom Filter uses a **bit array** to represent membership.
2. `Add()` calculates hash positions and sets those bits to `true`.
3. `MightContain()` calculates the same positions and checks them.
4. If **any required bit is false**, the value is definitely absent.
5. If **all required bits are true**, the value might exist.
6. Bloom Filters can produce **false positives**.
7. Bloom Filters do not produce **false negatives** when implemented and used correctly.
8. Our current hash functions are intentionally simple for learning purposes.
9. A production implementation would need better hashing and careful sizing.