require 'albacore'
require 'fileutils'

VERSION = '1.0.0'
BUILD_NUMBER = "#{VERSION}.#{Date.today.strftime('%y%j')}"

$config = ENV['config'] || 'Debug'

task :default => :compile

desc 'Generate the VersionInfo.cs class'
assemblyinfo :version do |asm|
  commit_data = get_commit_hash_and_date
  commit = commit_data[0]
  commit_date = commit_data[1]
    
  asm.version = VERSION + ".0"
  asm.file_version = BUILD_NUMBER
  asm.output_file = "source/VersionInfo.cs"
  asm.product_name = "Munin.WinNode"
  asm.description = "Git commit hash: #{commit} - #{commit_date}"
  asm.custom_attributes :AssemblyInformationalVersion => "#{asm.version}"
  asm.namespaces 'System', 'System.Reflection'  
end

desc 'Compile the project'
msbuild :compile => :version do |msb|
  msb.properties :configuration => $config
  msb.targets [:clean, :build]
  msb.solution = 'source/Munin.WinNode.sln'
end

desc 'Run tests'
nunit :test => :compile do |nunit|
  nunit.command = nunit_path
  nunit.assemblies "source/Munin.WinNode.Tests/bin/#{$config}/Munin.WinNode.Tests.dll"
  nunit.options '/xml=nunit-console-output.xml'
end

desc 'Builds release package'
task :package => :compile do
  include FileUtils
  
  build_path = "build"
  mkpath build_path unless Dir.exists? build_path
  rm_rf Dir.glob(File.join(build_path, "**/*"))
  
  assemble_path = "build/assemble"
  mkpath assemble_path unless Dir.exists? assemble_path 
  rm_rf Dir.glob(File.join(assemble_path, "**/*"))
    
  cp_r Dir.glob("source/Munin.WinNode/bin/#{$config}/**"), assemble_path, :verbose => true
  rm Dir.glob("#{assemble_path}/log.*")
  
  #ilmerge
  exec = Exec.new
  exec.command = "tools/ILMerge/ILMerge.exe"
  exec.parameters ["/target:exe", "/targetplatform:v4", "/ndebug", "/internalize", "/lib:#{assemble_path}", "/out:#{assemble_path}/Munin.WinNode.Merged.exe", "Munin.WinNode.exe", "log4net.dll"]
  exec.execute  
  
  rm Dir.glob(File.join(assemble_path, "log4net*"))
  rm File.join(assemble_path, "Munin.WinNode.exe")
  rm File.join(assemble_path, "Munin.WinNode.pdb")
  mv File.join(assemble_path, "Munin.WinNode.Merged.exe"), File.join(assemble_path, "Munin.WinNode.exe")
    
  zip_directory(assemble_path, File.join(build_path, "Munin.WinNode-#{BUILD_NUMBER}.zip"))
  rm_rf assemble_path
end

def nunit_path()
  File.join(Dir.glob(File.join('lib', "nunit.*")).sort.last, "tools", "nunit-console.exe")
end

def zip_directory(assemble_path, output_path)
  zip = ZipDirectory.new
  zip.directories_to_zip assemble_path
  zip.output_path = File.dirname(output_path)
  zip.output_file = File.basename(output_path)
  zip.execute  
end

def get_commit_hash_and_date
	begin
		commit = `git log -1 --pretty=format:%H`
		git_date = `git log -1 --date=iso --pretty=format:%ad`
		commit_date = DateTime.parse( git_date ).strftime("%Y-%m-%d %H%M%S")
	rescue
		commit = "git unavailable"
	end

	[commit, commit_date]
end