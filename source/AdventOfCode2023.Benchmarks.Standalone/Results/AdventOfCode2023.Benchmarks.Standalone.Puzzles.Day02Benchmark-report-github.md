```

BenchmarkDotNet v0.13.10, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean     | Error     | StdDev    | StdErr    | Min      | Q1       | Median   | Q3       | Max      | Op/s      | Baseline | Allocated |
|------- |----------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|----------:|--------- |----------:|
| Part1  | Part 1     | 3.326 μs | 0.0049 μs | 0.0044 μs | 0.0012 μs | 3.319 μs | 3.323 μs | 3.325 μs | 3.329 μs | 3.333 μs | 300,647.4 | No       |         - |
|        |            |          |           |           |           |          |          |          |          |          |           |          |           |
| Part2  | Part 2     | 3.051 μs | 0.0061 μs | 0.0057 μs | 0.0015 μs | 3.046 μs | 3.047 μs | 3.050 μs | 3.055 μs | 3.064 μs | 327,709.4 | No       |         - |
