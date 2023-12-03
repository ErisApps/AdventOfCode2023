```

BenchmarkDotNet v0.13.10, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean     | Error    | StdDev   | StdErr   | Min      | Q1       | Median   | Q3       | Max      | Op/s     | Baseline | Allocated |
|------- |----------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|--------- |----------:|
| Part1  | Part 1     | 22.30 μs | 0.417 μs | 0.697 μs | 0.116 μs | 21.08 μs | 21.78 μs | 22.40 μs | 22.77 μs | 23.92 μs | 44,845.1 | No       |         - |
|        |            |          |          |          |          |          |          |          |          |          |          |          |           |
| Part2  | Part 2     | 19.82 μs | 0.392 μs | 0.420 μs | 0.099 μs | 19.10 μs | 19.46 μs | 19.87 μs | 20.05 μs | 20.60 μs | 50,451.9 | No       |         - |
