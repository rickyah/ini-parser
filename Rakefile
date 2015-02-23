require 'bundler/setup'
require 'albacore'
require 'albacore/tasks/versionizer'
require 'pry'
require 'pry-debugger'

solution_path = "src/INIFileParser.sln"
nuget_exe_path = "tools/NuGet.exe"
#Albacore::Tasks::Versionizer.new :versioning


desc 'restore all nugets as per the packages.config files'
nugets_restore :restore do |p|
  p.out = 'src/packages'
  p.exe = nuget_exe_path
end

desc 'Perform full build'
build :build => [:restore] do |b|
  b.prop 'Configuration', 'Release'
  b.sln = solution_path
  b.nologo
  b.logging = :normal
end


desc "Create the nuget package"
nugets_pack :pack => [:build] do |cmd|
  cmd.files   = FileList['src/INIFileParser/INIFileParser.nuspec']
  cmd.out     = 'build'
  cmd.exe     = nuget_exe_path
  cmd.no_project_dependencies
  cmd.leave_nuspec
end

task :default => :pack