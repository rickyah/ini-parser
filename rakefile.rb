require "albacore"
require "rake/clean"

CLOBBER.include("build", "pack")

task :default => [:build, :test]

Albacore.configure do |config|
  config.log_level = :verbose
end

desc "Build the solution in Release mode"
msbuild :build do |cmd|
  cmd.solution = "src/INIFileParser.sln"
  cmd.targets = [:clean, :build]
  cmd.properties = {configuration: "Release"}
  cmd.verbosity = :minimal
  cmd.nologo
end

desc "Run all unit tests"
nunit :test => [:build] do |cmd|
  cmd.command = "tools/nunit-console.exe"
  cmd.assemblies = FileList["src/**/bin/Release/*.Tests.dll"]
  cmd.parameters = ["/nologo", "/noresult"]
end

output :output do |out|
  out.from "."
  out.to "pack"
  out.dir "build", as: "lib"
end

desc "Create the NuGet package"
nugetpack :pack => [:clobber, :build, :output] do |cmd|
  cmd.command = "nuget"
  cmd.nuspec = "ini-parser.nuspec"
  cmd.base_folder = "pack"
end
