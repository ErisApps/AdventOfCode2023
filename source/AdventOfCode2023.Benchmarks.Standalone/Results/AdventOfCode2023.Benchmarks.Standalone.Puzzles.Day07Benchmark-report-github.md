```

BenchmarkDotNet v0.13.10, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean     | Error    | StdDev   | StdErr   | Min      | Q1       | Median   | Q3       | Max      | Op/s     | Baseline | Allocated |
|------- |----------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|--------- |----------:|
| Part1  | Part 1     | 71.87 μs | 1.416 μs | 1.631 μs | 0.365 μs | 69.77 μs | 70.68 μs | 71.25 μs | 72.98 μs | 75.38 μs | 13,913.1 | No       |         - |
|        |            |          |          |          |          |          |          |          |          |          |          |          |           |
| Part2  | Part 2     | 69.61 μs | 1.358 μs | 1.270 μs | 0.328 μs | 68.03 μs | 68.57 μs | 69.55 μs | 70.25 μs | 72.81 μs | 14,366.5 | No       |         - |
