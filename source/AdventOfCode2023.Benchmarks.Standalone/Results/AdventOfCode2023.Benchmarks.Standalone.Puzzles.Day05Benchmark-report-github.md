```

BenchmarkDotNet v0.13.10, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean      | Error     | StdDev    | StdErr    | Min       | Q1        | Median    | Q3        | Max       | Op/s      | Baseline | Allocated |
|--------|----------- |----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|--------- |----------:|
| Part1  | Part 1     |  4.407 μs | 0.0116 μs | 0.0103 μs | 0.0028 μs |  4.395 μs |  4.399 μs |  4.406 μs |  4.411 μs |  4.431 μs | 226,910.0 | No       |         - |
|        |            |           |           |           |           |           |           |           |           |           |           |          |           |
| Part2  | Part 2     | 14.297 μs | 0.0591 μs | 0.0553 μs | 0.0143 μs | 14.206 μs | 14.256 μs | 14.282 μs | 14.338 μs | 14.400 μs |  69,942.8 | No       |         - |
