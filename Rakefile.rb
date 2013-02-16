# this is not a build script, just an utlity for now

require 'albacore'

task :predistclean do
  FileUtils.rm_rf Dir.glob("*.user*")
  FileUtils.rm_rf Dir.glob("*.nupkg")
  FileUtils.rm_rf Dir.glob("*.suo")
  FileUtils.rm_rf Dir.glob("*\x7E")
  FileUtils.rm_rf Dir.glob("doc/*\x7E")
  FileUtils.rm_rf Dir.glob("src/HttpHelpers/*.user")
  FileUtils.rm_rf Dir.glob("src/HttpHelpers.Demo/*.user")
  FileUtils.rm_rf Dir.glob("src/HttpHelpers.Tests/*.user")
  FileUtils.rm_rf "src/HttpHelpers/obj"
  FileUtils.rm_rf "src/HttpHelpers.Demo/obj"
  FileUtils.rm_rf "src/HttpHelpers.Tests/obj"
  FileUtils.rm_rf Dir.glob("src/HttpHelpers/bin/Release/*?db")
  FileUtils.rm_rf Dir.glob("src/HttpHelpers/bin/Debug/*.mdb")  
  FileUtils.rm_rf "src/HttpHelpers.Demo/bin/Debug"
  FileUtils.rm_rf Dir.glob("src/HttpHelpers.Demo/bin/Release/*vshost*")
  FileUtils.rm_rf "src/HttpHelpers.Tests/bin/Release"
  FileUtils.rm_rf Dir.glob("src/HttpHelpers.Tests/bin/Debug/*.mdb")
  FileUtils.rm_rf Dir.glob("src/HttpHelpers.Tests/bin/Debug/*.xml")
end
