```

BenchmarkDotNet v0.13.11, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean      | Error    | StdDev   | StdErr   | Min       | Q1        | Median    | Q3        | Max       | Op/s     | Baseline | Allocated |
|------- |----------- |----------:|---------:|---------:|---------:|----------:|----------:|----------:|----------:|----------:|---------:|--------- |----------:|
| Part1  | Part 1     |  34.48 μs | 0.102 μs | 0.095 μs | 0.025 μs |  34.31 μs |  34.44 μs |  34.50 μs |  34.56 μs |  34.58 μs | 29,002.0 | No       |         - |
|        |            |           |          |          |          |           |           |           |           |           |          |          |           |
| Part2  | Part 2     | 321.85 μs | 0.375 μs | 0.351 μs | 0.091 μs | 321.36 μs | 321.63 μs | 321.75 μs | 322.12 μs | 322.56 μs |  3,107.0 | No       |         - |
