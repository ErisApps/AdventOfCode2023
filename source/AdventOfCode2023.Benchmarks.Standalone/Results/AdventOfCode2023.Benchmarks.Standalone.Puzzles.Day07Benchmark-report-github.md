```

BenchmarkDotNet v0.13.10, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean     | Error    | StdDev   | StdErr   | Min      | Q1       | Median   | Q3       | Max      | Op/s     | Baseline | Allocated |
|------- |----------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|--------- |----------:|
| Part1  | Part 1     | 73.45 μs | 1.467 μs | 3.708 μs | 0.428 μs | 65.99 μs | 70.93 μs | 73.41 μs | 75.68 μs | 82.61 μs | 13,614.5 | No       |         - |
|        |            |          |          |          |          |          |          |          |          |          |          |          |           |
| Part2  | Part 2     | 71.26 μs | 1.410 μs | 3.065 μs | 0.406 μs | 65.32 μs | 69.08 μs | 71.22 μs | 73.86 μs | 77.92 μs | 14,033.3 | No       |         - |
