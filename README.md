# NorthSouthSystems.Text

This .NET library contains string utilities including fluent APIs for splitting and joining quoted delimited text (e.g.
CSV used by Microsoft Excel) and fixed-width positional text.

## Quoted Delimited Text (e.g. CSV used by Microsoft Excel) Splitting and Joining

**Simple Split**

```csharp
string row = "a,b,c";
string[]() fields = row.SplitQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180);

foreach(string field in fields);
    Console.WriteLine(field);
```

```console
Console Output:
a
b
c
```

**Simple Join**

```csharp
string[]()() fields = new string[]()() { "a", "b", "c" };
string result = fields.JoinQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180);
Console.WriteLine(result);
```

```console
Console Output:
a,b,c
```

**Quoted Split**

```csharp
var signals = new StringQuotedSignalsBuilder()
    .Delimiter(",")
    .NewRowTolerantEnvironmentPrimary()
    .Quote("'")
    .ToSignals();
string row = "'a,a',b,c";
string[]() fields = row.SplitQuotedRow(signals);

foreach(string field in fields);
    Console.WriteLine(field);
```

```console
Console Output:
a,a
b
c
```

**Quoted Join**

```csharp
string[]()() fields = new string[]()() { "a,a", "b", "c" };
string result = fields.JoinQuotedRow(StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180);
Console.WriteLine(result);
```

```console
Console Output:
"a,a",b,c
```

## Fixed-Width (a.k.a. Positional) Text Splitting and Joining

**Split**

```csharp
string row = "A-B-C";
string[]()() fields = row.SplitFixedRow(new []()() { 2, 2, 1 });

foreach(string field in fields)
    Console.WriteLine(field);

fields = row.SplitFixedRow(new []() { 2, 2, 1 }, '-');

foreach(string field in fields)
    Console.WriteLine(field);
```

```console
Console Output:
A-
B-
C
A
B
C
```

**Join**

```csharp
string fields = new []() { "A", "B", "C" };
string row = fields.JoinFixedRow(new []() { 2, 2, 1 });
Console.WriteLine(row);

row = fields.JoinFixedRow(new []() { 2, 2, 1 }, '-', false);
Console.WriteLine(row);
```

```console
Console Output:
A B C
A-B-C
```

## Schema Fixed-Width (a.k.a. Positional) Text Splitting and Joining

**Split**

```csharp
var schema = new StringSchema();
schema.AddEntry(new StringSchemaEntry("A", new[]() { 1, 1, 1 }));
schema.AddEntry(new StringSchemaEntry("B", new[]() { 2, 2, 2 }));
schema.AddEntry(new StringSchemaEntry("CD", new[]() { 3, 3, 3 }));

var split = "A123".SplitSchemaRow(schema);
Console.WriteLine(split.Entry.Header);

foreach(StringFieldWrapper field in split.Result.Fields)
    Console.WriteLine(field);

split = "B123456".SplitSchemaRow(schema);
Console.WriteLine(split.Entry.Header);

foreach(StringFieldWrapper field in split.Result.Fields)
    Console.WriteLine(field);

split = "CD123456789".SplitSchemaRow(schema);
Console.WriteLine(split.Entry.Header);

foreach(StringFieldWrapper field in split.Result.Fields)
    Console.WriteLine(field);
```

```console
Console Output:
A
1
2
3
B
12
34
56
CD
123
456
789
```

**Join**

```csharp
var a = new StringSchemaEntry("A", new[]() { 1, 1, 1 });
var b = new StringSchemaEntry("B", new[]() { 2, 2, 2 });
var c = new StringSchemaEntry("C", new[]() { 2, 2, 2 }, '-');

string[]()() fields = new[]()() { "1", "2", "3" };
string join = fields.JoinSchemaRow(a);
Console.WriteLine(join);

fields = new[]() { "12", "34", "56" };
join = fields.JoinSchemaRow(b);
Console.WriteLine(join);

fields = new[]() { "1", "2", "3" };
join = fields.JoinSchemaRow(c);
Console.WriteLine(join);
```

```console
Console Output:
A123
B123456
C1-2-3-
```

## Other String Utilities

**NormalizeWhiteSpace**

```csharp
Console.WriteLine(" A  B C   D   ".NormalizeWhiteSpace());
Console.WriteLine(("Lots\tOf" + Environment.NewLine + "Changes").NormalizeWhiteSpace());
```

```console
Console Output:
A B C D
Lots Of
Changes
```

**SpaceCamelCase**

```csharp
Console.WriteLine("FooBarFoo FooBarFoo".SpaceCamelCase());
Console.WriteLine("123A".SpaceCamelCase());
Console.WriteLine("123a".SpaceCamelCase());
Console.WriteLine("A123".SpaceCamelCase());
Console.WriteLine("A123A".SpaceCamelCase());
```

```console
Console Output:
Foo Bar Foo Foo Bar Foo
123 A
123 a
A 123
A 123 A
```

**ToLowerCamelCase**

```csharp
Console.WriteLine("FooBar".ToLowerCamelCase());
```

```console
Console Output:
fooBar
```

**ToUpperCamelCase**

```csharp
Console.WriteLine("fooBar".ToUpperCamelCase());
```

```console
Console Output:
FooBar
```

**WhereIsInAnyCategory**

```csharp
Console.WriteLine("a1b2c3d".WhereIsInAnyCategory(CharCategories.All));
Console.WriteLine("a1b2c3d".WhereIsInAnyCategory(CharCategories.Digit));
Console.WriteLine("a1b2c3d".WhereIsInAnyCategory(CharCategories.Punctuation | CharCategories.WhiteSpace));
Console.WriteLine("a1b2-c3d".WhereIsInAnyCategory(CharCategories.Digit | CharCategories.Letter));
Console.WriteLine("a1b2-c3d".WhereIsInAnyCategory(CharCategories.Punctuation));
```

```console
Console Output:
a1b2c3d
123

a1b2c3d
-
```
