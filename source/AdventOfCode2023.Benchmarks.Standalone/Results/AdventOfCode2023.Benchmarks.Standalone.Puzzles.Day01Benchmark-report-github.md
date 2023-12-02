```

BenchmarkDotNet v0.13.10, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean      | Error     | StdDev    | StdErr    | Min       | Q1        | Median    | Q3        | Max       | Op/s      | Baseline | Allocated |
|------- |----------- |----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|----------:|--------- |----------:|
| Part1  | Part 1     |  8.859 μs | 0.0744 μs | 0.0659 μs | 0.0176 μs |  8.771 μs |  8.821 μs |  8.853 μs |  8.894 μs |  9.013 μs | 112,880.4 | No       |      32 B |
|        |            |           |           |           |           |           |           |           |           |           |           |          |           |
| Part2  | Part 2     | 20.834 μs | 0.2310 μs | 0.2048 μs | 0.0547 μs | 20.507 μs | 20.658 μs | 20.864 μs | 21.009 μs | 21.114 μs |  47,997.6 | No       |         - |
