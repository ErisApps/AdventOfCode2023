```

BenchmarkDotNet v0.13.10, macOS Ventura 13.5.2 (22G91) [Darwin 22.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method | Categories | Mean     | Error    | StdDev   | StdErr   | Min      | Q1       | Median   | Q3       | Max      | Op/s         | Baseline | Allocated |
|------- |----------- |---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|---------:|-------------:|--------- |----------:|
| Part1  | Part 1     | 43.74 ns | 0.041 ns | 0.032 ns | 0.009 ns | 43.70 ns | 43.72 ns | 43.74 ns | 43.77 ns | 43.80 ns | 22,861,294.0 | No       |         - |
|        |            |          |          |          |          |          |          |          |          |          |              |          |           |
| Part2  | Part 2     | 50.50 ns | 0.220 ns | 0.206 ns | 0.053 ns | 50.09 ns | 50.35 ns | 50.53 ns | 50.66 ns | 50.79 ns | 19,803,525.5 | No       |         - |
