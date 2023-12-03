```

BenchmarkDotNet v0.13.10, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean     | Error     | StdDev    | StdErr    | Min      | Q1       | Median   | Q3       | Max      | Op/s      | Baseline | Allocated |
|------- |----------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|----------:|--------- |----------:|
| Part1  | Part 1     | 2.817 μs | 0.0053 μs | 0.0049 μs | 0.0013 μs | 2.810 μs | 2.814 μs | 2.816 μs | 2.821 μs | 2.826 μs | 354,950.3 | No       |         - |
|        |            |          |           |           |           |          |          |          |          |          |           |          |           |
| Part2  | Part 2     | 3.049 μs | 0.0034 μs | 0.0030 μs | 0.0008 μs | 3.044 μs | 3.047 μs | 3.050 μs | 3.051 μs | 3.054 μs | 327,955.3 | No       |         - |
