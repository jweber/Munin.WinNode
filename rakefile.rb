require 'albacore'
require 'fileutils'
require 'semver'

CLR_TOOLS_VERSION = 'v4.0.30319'
ARTIFACTS_PATH = "build"

$config = ENV['config'] || 'Debug'

task :default => :compile

desc 'Generate the VersionInfo.cs class'
assemblyinfo :version => [:versioning] do |asm|
  git_data = commit_data()
  revision_hash = git_data[0]
  revision_date = git_data[1]
    
  asm.version = FORMAL_VERSION
  asm.file_version = FORMAL_VERSION
  asm.product_name = "Munin.WinNode"
  asm.description = "Git commit hash: #{revision_hash} - #{revision_date}"
  asm.custom_attributes :AssemblyInformationalVersion => "#{BUILD_VERSION}"
  asm.output_file = "source/VersionInfo.cs"
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
  include FileUtils
  mkpath ARTIFACTS_PATH unless Dir.exists? ARTIFACTS_PATH
  
  nunit.command = nunit_path
  nunit.assemblies "source/Munin.WinNode.Tests/bin/#{$config}/Munin.WinNode.Tests.dll"
  #nunit.options '/xml=nunit-console-output.xml'
  
  nunit.options = "/framework=#{CLR_TOOLS_VERSION}", '/noshadow', '/nologo', '/labels', "\"/xml=#{File.join(ARTIFACTS_PATH, "nunit-test-results.xml")}\""
end

desc 'Builds release package'
task :package => :compile do
  include FileUtils
  
  assemble_path = File.join(ARTIFACTS_PATH, "assemble")
  
  mkpath ARTIFACTS_PATH unless Dir.exists? ARTIFACTS_PATH
  rm_rf Dir.glob(File.join(ARTIFACTS_PATH, "**/*.zip"))
  rm_rf assemble_path if Dir.exists? assemble_path
    
  mkpath assemble_path unless Dir.exists? assemble_path 
  rm_rf Dir.glob(File.join(assemble_path, "**/*"))
    
  cp_r Dir.glob("source/Munin.WinNode/bin/#{$config}/**"), assemble_path, :verbose => true
  rm Dir.glob("#{assemble_path}/log.*")
     
  zip_directory(assemble_path, File.join(ARTIFACTS_PATH, "Munin.WinNode-#{BUILD_VERSION}.zip"))
  rm_rf assemble_path if Dir.exists? assemble_path
end

desc 'Builds version environment variables'
task :versioning do
  ver = SemVer.find
  revision = (ENV['BUILD_NUMBER'] || ver.patch).to_i
  var = SemVer.new(ver.major, ver.minor, revision, ver.special)
  
  ENV['BUILD_VERSION'] = BUILD_VERSION = ver.format("%M.%m.%p%s") + ".#{commit_data()[0]}"
  ENV['NUGET_VERSION'] = NUGET_VERSION = ver.format("%M.%m.%p%s")
  ENV['FORMAL_VERSION'] = FORMAL_VERSION = "#{ SemVer.new(ver.major, ver.minor, revision).format "%M.%m.%p"}"
  puts "##teamcity[buildNumber '#{BUILD_VERSION}']"  
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

def commit_data
  begin
    commit = `git rev-parse --short HEAD`.chomp()
    git_date = `git log -1 --date=iso --pretty=format:%ad`
	commit_date = DateTime.parse(git_date).strftime("%Y-%m-%d %H%M%S")
  rescue Exception => e
    puts e.inspect
	commit = (ENV['BUILD_VCS_NUMBER'] || "000000")
	commit_date = Time.new.strftime("%Y-%m-%d %H%M%S")
  end
  [commit, commit_date]
 end
 