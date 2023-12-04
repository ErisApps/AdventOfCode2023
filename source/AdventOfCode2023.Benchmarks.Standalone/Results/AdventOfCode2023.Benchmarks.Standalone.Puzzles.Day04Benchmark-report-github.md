```

BenchmarkDotNet v0.13.10, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean     | Error    | StdDev   | StdErr   | Min      | Q1       | Median   | Q3       | Max      | Op/s     | Baseline | Allocated |
|------- |----------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|--------- |----------:|
| Part1  | Part 1     | 32.20 μs | 0.142 μs | 0.126 μs | 0.034 μs | 31.96 μs | 32.11 μs | 32.23 μs | 32.30 μs | 32.38 μs | 31,054.5 | No       |         - |
|        |            |          |          |          |          |          |          |          |          |          |          |          |           |
| Part2  | Part 2     | 31.22 μs | 0.121 μs | 0.113 μs | 0.029 μs | 31.04 μs | 31.15 μs | 31.24 μs | 31.27 μs | 31.42 μs | 32,027.0 | No       |         - |
