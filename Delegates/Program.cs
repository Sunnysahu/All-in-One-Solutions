// What is Delegates => A delegate is a type that can store a reference to a method
void PrintMessage()
{
    Console.WriteLine("Hello World");
}

// ================================
// 1. Normal Method
// ================================
Console.WriteLine("1. Normal Method");

PrintMessage();

Console.WriteLine();

// ================================
// 2. Action (Method with no return value)
// ================================
Console.WriteLine("2. Action");

Action action = PrintMessage;
action();

Console.WriteLine();

//---------------------------------------
void PrintSquare(int number)
{
    Console.WriteLine($"Square = {number * number}");
}

// ================================
// 3. Action<int>
// ================================
Console.WriteLine("3. Action<int>");

Action<int> printSquare = PrintSquare;
printSquare(5);

Console.WriteLine();

// ------------------------------------------

int Square(int number)
{
    return number * number;
}

// ================================
// 4. Func<int, int>
// ================================
Console.WriteLine("4. Func<int, int>");

Func<int, int> square = Square;

int result = square(6);

Console.WriteLine($"Square = {result}");

Console.WriteLine();


// -----------------------------------------------

int Add(int a, int b)
{
    return a + b;
}


// ================================
// 5. Func<int, int, int>
// ================================
Console.WriteLine("5. Func<int, int, int>");

Func<int, int, int> add = Add;

Console.WriteLine($"Sum = {add(10, 20)}");

Console.WriteLine();

// ================================
// 6. Lambda with Action
// ================================
Console.WriteLine("6. Lambda Action");

Action hello = () =>
{
    Console.WriteLine("Hello from Lambda Action");
};

hello();

Console.WriteLine();

// ================================
// 7. Lambda with Func
// ================================
Console.WriteLine("7. Lambda Func");

Func<int, int> cube = x => x * x * x;

Console.WriteLine($"Cube = {cube(3)}");

Console.WriteLine();