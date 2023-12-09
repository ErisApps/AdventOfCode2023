```

BenchmarkDotNet v0.13.11, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean      | Error    | StdDev   | StdErr   | Min       | Q1        | Median    | Q3        | Max       | Op/s     | Baseline | Allocated |
|------- |----------- |----------:|---------:|---------:|---------:|----------:|----------:|----------:|----------:|----------:|---------:|--------- |----------:|
| Part1  | Part 1     |  34.47 μs | 0.043 μs | 0.039 μs | 0.010 μs |  34.42 μs |  34.45 μs |  34.47 μs |  34.49 μs |  34.56 μs | 29,007.3 | No       |         - |
|        |            |           |          |          |          |           |           |           |           |           |          |          |           |
| Part2  | Part 2     | 297.09 μs | 0.252 μs | 0.235 μs | 0.061 μs | 296.75 μs | 296.92 μs | 297.04 μs | 297.25 μs | 297.49 μs |  3,366.0 | No       |         - |
