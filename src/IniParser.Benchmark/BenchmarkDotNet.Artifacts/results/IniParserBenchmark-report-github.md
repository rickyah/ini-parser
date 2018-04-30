``` ini

BenchmarkDotNet=v0.10.13, OS=macOS 10.13.3 (17D102) [Darwin 17.4.0]
Intel Core i5-5287U CPU 2.90GHz (Broadwell), 1 CPU, 4 logical cores and 2 physical cores
.NET Core SDK=2.1.101
  [Host]     : .NET Core 2.0.6 (CoreCLR 4.6.0.0, CoreFX 4.6.26212.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.0.6 (CoreCLR 4.6.0.0, CoreFX 4.6.26212.01), 64bit RyuJIT


```
|               Method |         Mean |         Error |        StdDev |   Gen 0 | Allocated |
|--------------------- |-------------:|--------------:|--------------:|--------:|----------:|
|   TestCreatingParser | 16,560.74 ns |   113.5699 ns |   100.6768 ns |  8.9111 |   14056 B |
|  TestCreatingParser2 | 27,545.14 ns |   385.3468 ns |   360.4536 ns | 13.5498 |   21352 B |
|  TestCreatingIniData | 16,119.08 ns |   224.7746 ns |   187.6969 ns |  8.6365 |   13600 B |
|  TestParsingIniSmall |     39.95 ns |     0.4269 ns |     0.3993 ns |  0.0407 |      64 B |
|  TestParserIniMedium |  3,991.01 ns |    52.5670 ns |    46.5993 ns |  3.6163 |    5688 B |
|     TestParserIniBig |  4,035.95 ns |    54.2015 ns |    50.7001 ns |  3.6163 |    5688 B |
| TestCreatingIniData2 |    375.87 ns |     5.5491 ns |     5.1906 ns |  0.2999 |     472 B |
| TestParsingIniSmall2 |  2,191.88 ns |    29.1592 ns |    27.2755 ns |  1.3924 |    2192 B |
| TestParserIniMedium2 | 25,833.16 ns |   353.8138 ns |   330.9576 ns | 12.7258 |   20056 B |
|    TestParserIniBig2 | 86,953.28 ns | 1,487.3349 ns | 1,241.9919 ns | 36.6211 |   57776 B |
