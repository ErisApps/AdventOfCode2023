```

BenchmarkDotNet v0.13.11, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean     | Error    | StdDev   | StdErr   | Min      | Q1       | Median   | Q3       | Max      | Op/s     | Baseline | Allocated |
|------- |----------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|--------- |----------:|
| Part1  | Part 1     | 42.97 μs | 0.066 μs | 0.059 μs | 0.016 μs | 42.89 μs | 42.93 μs | 42.98 μs | 43.00 μs | 43.08 μs | 23,270.3 | No       |         - |
|        |            |          |          |          |          |          |          |          |          |          |          |          |           |
| Part2  | Part 2     | 44.10 μs | 0.067 μs | 0.059 μs | 0.016 μs | 44.01 μs | 44.05 μs | 44.09 μs | 44.12 μs | 44.22 μs | 22,677.8 | No       |         - |
